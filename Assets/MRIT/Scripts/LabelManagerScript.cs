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

    public Vector2 textureSize = new Vector2(512, 512);

    private void Awake()
    {
        SharedInstance = this;
        RSCameraView.aspect = 1.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        labelInstanceInfo = new LabelInstanceInfo(new Vector2(), new Vector2(), new Vector3(), new Vector2(), "");

        textureSize = InputManager._InputManagerInstance.streamingSize;
        pooledObjects = new List<GameObject>();

        GameObject tmp;
        for (int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(RSTransform);
            tmp.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            tmp.GetComponent<Canvas>().worldCamera = RSCameraView;
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

    LabelInstanceInfo labelInstanceInfo;
    struct LabelInstanceInfo
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
    int labelCount = 0;
    public void ProcessLabelJSON(string content)
    {
        if (!stopProcess)
        {
            //LabelInfo frame = JsonUtility.FromJson<LabelInfo>(content);
            var frame = JsonConvert.DeserializeObject<LabelInfo>(content);
            string[] pred_classes = frame.pred_classes;
            float[] scores = frame.scores;
            float[][] boxes = frame.boxes;

            Debug.Log($"JSON String: {content}");

            for (int i = 0; i < pred_classes.Length; i++)
            {
                labelInstanceInfo.className = pred_classes[i];
                float[] boxPositions = boxes[i];

                Debug.Log($"BEFORE PredClass: {labelInstanceInfo.className}, BoxPos1:{boxPositions[0]} , BoxPos2:{boxPositions[1]}, BoxPos3:{boxPositions[2]}, BoxPos4:{boxPositions[3]}");

                float startPosX = boxPositions[0];
                float startPosY = textureSize.y - boxPositions[3];

                float endPosX = boxPositions[2];
                float endPosY = textureSize.y - boxPositions[1];

                Debug.Log($"AFTER PredClass: {labelInstanceInfo.className}, BoxPos1:{startPosX} , BoxPos2:{startPosY}, BoxPos3:{endPosX}, BoxPos4:{endPosY}");

                labelInstanceInfo.startPos.x = startPosX;
                labelInstanceInfo.startPos.y = startPosY;

                labelInstanceInfo.endPos.x = endPosX;
                labelInstanceInfo.endPos.y = endPosY;

                labelInstanceInfo.size.x = endPosX - startPosX;
                labelInstanceInfo.size.y = endPosY - startPosY;

                int xMidpoint = (int)CalculateMidPoint(startPosX, endPosX);
                int yMidpoint = (int)CalculateMidPoint(startPosY, endPosY);

                Debug.Log($"MidPointX: {xMidpoint}, MidPointY:{yMidpoint}");

                labelInstanceInfo.raycastPoint.x = xMidpoint;
                labelInstanceInfo.raycastPoint.y = yMidpoint;
                //raycastPoint.x = xMidpoint;
                //raycastPoint.y = yMidpoint;
                //raycastPoint.z = 0;

                Debug.Log("Raycasting");
                raycastTrue = true;
                labelCount++;
                //stopProcess = true;

                //SetRectTransform(testRect, startPosX, startPosY, endPosX, endPosY);
                ////Vector2 size = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
                //Vector2 size = new Vector2();
                //size.x = endPosX - startPosX;
                //size.y = endPosY - startPosY;

                ////Vector2 midPoint = startPos + (size / 2);
                //SetRectTransform(labelRect, startPosX, startPosY + size.y - 25, endPosX, endPosY);
                //SetLabelName(TextMesh, "TestLabel");
            }
            if(labelCount == 1000)
            {
                stopProcess = true;
            }
        }

    }

    //public float InvertYPos(float pos)
    //{
    //    return textureSize.y - pos;
    //}

    public float CalculateMidPoint(float startPos, float endPos)
    {
        return startPos + ((endPos - startPos) / 2);
    }

    public void TextureToWorldRaycast(Vector3 texturePos)
    {
        RaycastHit hit;
        Ray labelRay = RSCameraView.ScreenPointToRay(texturePos);

        //Vector3 start = RSCameraView.transform.position;
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

                //LabelCanvas.planeDistance = Mathf.Abs(hit.point.z - RSCameraView.transform.position.z);
                Debug.Log($"HitPoint.z: {hit.point.z}");

                //LabelCanvas.planeDistance = Mathf.Abs(hit.point.z - RSTransform.position.z);
                //LabelCanvas.planeDistance = hit.distance;
                float distz = hit.point.z - RSTransform.position.z;

                Vector2 startPos = labelInstanceInfo.startPos;
                Vector2 endPos = labelInstanceInfo.endPos;
                Vector2 size = labelInstanceInfo.size;
                string className = labelInstanceInfo.className;

                SetRectTransform(RawImageRect, startPos.x, startPos.y, endPos.x, endPos.y);
                //Vector2 size = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
                //Vector2 midPoint = startPos + (size / 2);
                SetRectTransform(TextRect, startPos.x, startPos.y + size.y - 25, endPos.x, endPos.y);
                SetLabelName(TextMesh, className);

                LabelCanvas.renderMode = RenderMode.WorldSpace;
                LabelCanvas.transform.localScale = new Vector3(0.002120921f * distz, 0.002120921f * distz, 0);
                LabelCanvas.transform.localPosition = new Vector3(0, 0, distz);

                //StartCoroutine(DeactivateObjectTimer(labelInstance));
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

    IEnumerator DeactivateObjectTimer(GameObject obj)
    {
        yield return new WaitForSeconds(0.01f);
        //obj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        obj.SetActive(false);
    }

    bool raycastTrue = false;

    //public RectTransform testRect;
    //public RectTransform labelRect;
    //public TMP_Text TextMesh;
    //Vector2 startPos = new Vector2(0, 256);
    //Vector2 endPos = new Vector2(256, 512);

    // Update is called once per frame
    void Update()
    {
        if (raycastTrue)
        {
            raycastTrue = false;
            TextureToWorldRaycast(labelInstanceInfo.raycastPoint);
        }

        //Vector2 startPos = labelInstanceInfo.startPos;
        //Vector2 endPos = labelInstanceInfo.endPos;

        //SetRectTransform(testRect, startPos.x, startPos.y, endPos.x, endPos.y);
        ////Vector2 size = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        //Vector2 size = new Vector2();
        //size.x = endPos.x - startPos.x;
        //size.y = endPos.y - startPos.y;

        ////Vector2 midPoint = startPos + (size / 2);
        //SetRectTransform(labelRect, startPos.x, startPos.y + size.y - 25, endPos.x, endPos.y);
        //SetLabelName(TextMesh, "TestLabel");
    }
}
