using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class LabelManagerScript : MonoBehaviour
{
    public static LabelManagerScript SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int AmountToPool;

    public Transform RSTransform;
    public Camera RSCameraView;

    //public GameObject targetObject;
    public float hitMarkerSize = 1.0f;

    public Vector2 textureSize = new Vector2(512, 512);

    // Queue for holding LabelInstanceInfos for a single frame
    Queue<LabelInstanceInfo> frameQueue;

    private void Awake()
    {
        SharedInstance = this;
        RSCameraView.aspect = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        frameQueue = new Queue<LabelInstanceInfo>();
        textureSize = InputManager._InputManagerInstance.streamingSize;
        pooledObjects = new List<GameObject>();

        GameObject tmp;
        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(RSTransform);
            //tmp.GetComponent<Canvas>().worldCamera = RSCameraView;
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject(List<GameObject> list)
    {
        for (int i = 0; i < AmountToPool; i++)
        {
            if (!list[i].activeInHierarchy)
            {
                return list[i];
            }
        }
        return null;
    }

    public struct LabelInstanceInfo
    {
        public Vector2 startPos;
        public Vector2 endPos;

        public Vector3 raycastPoint;
        public Vector2 size;

        public string className;

        public LabelInstanceInfo(Vector2 s, Vector2 e, Vector3 r, Vector2 si, string cl)
        {
            startPos = s;
            endPos = e;
            raycastPoint = r;
            size = si;
            className = cl;
        }
    }

    bool stopProcess = false;
    public bool receiveFrameReady = true;

    // When ready, onDetection sends a single frame to this function
    public void GetAndProcessLabelJSON(string content)
    {
        if (!stopProcess && receiveFrameReady)
        {
            // Wait until the current frame is done processing before receiving any more frames
            receiveFrameReady = false;

            var frame = JsonConvert.DeserializeObject<LabelInfo>(content);
            string[] pred_classes = frame.pred_classes;
            float[] scores = frame.scores;
            float[][] boxes = frame.boxes;

            //Debug.Log($"JSON String: {content}");

            for (int i = 0; i < pred_classes.Length; i++)
            {
                // Create new labelInstanceInfo
                LabelInstanceInfo labelInstanceInfo = new LabelInstanceInfo(new Vector2(), new Vector2(), new Vector3(), new Vector2(), "");

                labelInstanceInfo.className = pred_classes[i];
                float[] boxPositions = boxes[i];

                //Debug.Log($"BEFORE PredClass: {labelInstanceInfo.className}, BoxPos1:{boxPositions[0]} , BoxPos2:{boxPositions[1]}, BoxPos3:{boxPositions[2]}, BoxPos4:{boxPositions[3]}");

                float startPosX = boxPositions[0];
                float startPosY = textureSize.y - boxPositions[3];

                float endPosX = boxPositions[2];
                float endPosY = textureSize.y - boxPositions[1];

                //Debug.Log($"AFTER PredClass: {labelInstanceInfo.className}, BoxPos1:{startPosX} , BoxPos2:{startPosY}, BoxPos3:{endPosX}, BoxPos4:{endPosY}");

                labelInstanceInfo.startPos.x = startPosX;
                labelInstanceInfo.startPos.y = startPosY;

                labelInstanceInfo.endPos.x = endPosX;
                labelInstanceInfo.endPos.y = endPosY;

                labelInstanceInfo.size.x = endPosX - startPosX;
                labelInstanceInfo.size.y = endPosY - startPosY;

                int xMidpoint = (int)CalculateMidPoint(startPosX, endPosX);
                int yMidpoint = (int)CalculateMidPoint(startPosY, endPosY);

                //Debug.Log($"MidPointX: {xMidpoint}, MidPointY:{yMidpoint}");

                labelInstanceInfo.raycastPoint.x = xMidpoint;
                labelInstanceInfo.raycastPoint.y = yMidpoint;

                frameQueue.Enqueue(labelInstanceInfo);
            }
        }
    }

    public float CalculateMidPoint(float startPos, float endPos)
    {
        return startPos + ((endPos - startPos) / 2);
    }

    public void TextureToWorldRaycast(LabelInstanceInfo labelInstanceInfo)
    {
        RaycastHit hit;
        Ray labelRay = RSCameraView.ScreenPointToRay(labelInstanceInfo.raycastPoint);

        Vector3 start = RSTransform.position;

        Vector3 end = labelRay.GetPoint(1000f);

        Debug.DrawRay(start, end, Color.red);
        if (Physics.Raycast(labelRay, out hit, Physics.IgnoreRaycastLayer))
        {
            GameObject labelInstance = GetPooledObject(pooledObjects);

            if (labelInstance == null)
            {
                Debug.Log("NULL INSTANCE");
            }
            else
            {
                labelInstance.SetActive(true);
                Canvas LabelCanvas = labelInstance.GetComponent<Canvas>();
                Transform RawImage = labelInstance.transform.GetChild(0);
                Transform Text = labelInstance.transform.GetChild(1);

                RectTransform RawImageRect = RawImage.GetComponent<RectTransform>();
                RectTransform TextRect = Text.GetComponent<RectTransform>();
                TMP_Text TextMesh = Text.GetComponent<TMP_Text>();

                LabelCanvas.renderMode = RenderMode.WorldSpace;

                float distz = RSTransform.InverseTransformPoint(hit.point).z;

                Vector2 startPos = labelInstanceInfo.startPos;
                Vector2 endPos = labelInstanceInfo.endPos;
                Vector2 size = labelInstanceInfo.size;
                string className = labelInstanceInfo.className;
                
                SetRectTransform(RawImageRect, startPos.x, startPos.y, endPos.x, endPos.y);
                SetRectTransform(TextRect, startPos.x, startPos.y + size.y - 25, endPos.x, endPos.y);
                SetLabelName(TextMesh, className);

                // Get Hitmaker gameobject in LabelInstance
                GameObject hitMarker = labelInstance.transform.GetChild(0).GetChild(0).gameObject;

                float xVal = (endPos.x - startPos.x) * (hitMarkerSize * 0.09765625f); // Default (25/256)
                float yVal = (endPos.y - startPos.y) * (hitMarkerSize * 0.09765625f);
                float boxScale = (xVal + yVal)/2.0f;

                hitMarker.transform.localScale = new Vector3(boxScale, boxScale, boxScale);

                float fov = RSCameraView.fieldOfView;
                LabelCanvas.transform.localPosition = new Vector3(0, 0, distz);
                LabelCanvas.transform.localScale = new Vector3(0.000037209f * fov * distz, 0.000037209f * fov * distz, 0.000037209f * fov * distz);

                //LabelCanvas.transform.localScale = new Vector3(0.002120921f * distz, 0.002120921f * distz, 1);

                //GameObject targetObjectInstance = Instantiate(targetObject, hit.point, transform.rotation, RSTransform);
                //targetObjectInstance.transform.localScale = new Vector3(targetObjectInstance.transform.localScale.x * boxScale * fov * distz, targetObjectInstance.transform.localScale.y * boxScale * fov * distz, targetObjectInstance.transform.localScale.z * boxScale * fov * distz); ;

                //tmp.GetComponent<Canvas>().worldCamera = RSCameraView;
                StartCoroutine(DeactivateObjectTimer(labelInstance, LabelCanvas));
            }
        }
    }

    public void SetRectTransform(RectTransform RawImageRect, float startPosX, float startPosY, float endPosX, float endPosY)
    {

        RawImageRect.offsetMin = new Vector2(startPosX, startPosY);
        RawImageRect.offsetMax = new Vector2(endPosX - textureSize.x, endPosY - textureSize.y);
    }

    public void SetLabelName(TMP_Text TextMesh, string className)
    {
        TextMesh.text = className;
    }

    IEnumerator DeactivateObjectTimer(GameObject obj, Canvas LabelCanvas)
    {
        //LabelCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        obj.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        //LabelCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        obj.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!receiveFrameReady)
        {
            int frameSize = frameQueue.Count;
            for(int i = 0; i < frameSize; i++)
            {
                TextureToWorldRaycast(frameQueue.Dequeue());
            }
            StartCoroutine(ReadyTimer());
            
        }
    }

    IEnumerator ReadyTimer()
    {
        yield return new WaitForSeconds(0.5f);
        receiveFrameReady = true;
    }
}
