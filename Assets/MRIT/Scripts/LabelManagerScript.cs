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
        for(int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(RSTransform);
            //tmp.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            tmp.GetComponent<Canvas>().worldCamera = RSCameraView;
            pooledObjects.Add(tmp);
        }
    }

    public GameObject GetPooledObject(List<GameObject> list)
    {
        for(int i = 0; i < AmountToPool; i++)
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
        Vector2 startPos;
        Vector2 endPos;

        Vector3 raycastPoint;
        Vector2 size;

        string className;

        public LabelInstanceInfo(Vector2 s, Vector2 e, Vector3 r, Vector2 si, string cl)
        {
            startPos = s;
            endPos = e;
            raycastPoint = r;
            size = si;
            className = cl;
        }
    }

    public void ProcessLabelJSON(string content)
    {
        //LabelInfo frame = JsonUtility.FromJson<LabelInfo>(content);
        var frame = JsonConvert.DeserializeObject<LabelInfo>(content);
        string[] pred_classes = frame.pred_classes;
        float[] scores = frame.scores;
        float[][] boxes = frame.boxes;

        Debug.Log($"JSON String: {content}");
        
        for(int i = 0; i < pred_classes.Length; i++)
        {
            currClass = pred_classes[i];
            float[] boxPositions = boxes[i];

            Debug.Log($"PredClass: {currClass}, box1: {boxPositions[0]}, box2: {boxPositions[1]}");

            startPos.x = boxPositions[0];
            startPos.y = InvertYPos(boxPositions[1]);
            endPos.x = boxPositions[2];
            endPos.y = InvertYPos(boxPositions[3]);

            int xMidpoint = (int)CalculateMidPoint(startPos.x, endPos.x);
            int yMidpoint = (int)CalculateMidPoint(startPos.y, endPos.y);

            size.x = endPos.x - startPos.y;
            size.y = endPos.y - startPos.x;

            Debug.Log($"MidPointX: {xMidpoint}, MidPointY:{yMidpoint}");

            raycastPoint.x = xMidpoint;
            raycastPoint.y = yMidpoint;
            raycastPoint.z = 0;

            Debug.Log("Raycasting");
            raycastTrue = true;
        }
    }

    public float InvertYPos(float pos)
    {
        return textureSize.y - pos;
    }

    public float CalculateMidPoint(float startPos, float endPos)
    {
        return startPos + ((endPos-startPos)/2);
    }

    public void TextureToWorldRaycast(Vector3 texturePos)
    {

        RaycastHit hit;
        Ray labelRay = RSCameraView.ScreenPointToRay(texturePos);

        //Vector3 start = RSCameraView.transform.position;
        Vector3 start = RSTransform.position;

        Vector3 end = labelRay.GetPoint(10000f);

        Debug.DrawRay(start, end, Color.red);
        if(Physics.Raycast(labelRay, out hit, Physics.IgnoreRaycastLayer))
        {
            GameObject labelInstance = GetPooledObject(pooledObjects);

            if(labelInstance == null)
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
                LabelCanvas.planeDistance = Mathf.Abs(hit.point.z - RSTransform.position.z);

                StartCoroutine(DeactivateObjectTimer(labelInstance));
            }
        }
    }


    //public RectTransform testRect;
    //public RectTransform labelRect;
    //public TMP_Text TextMesh;
    //Vector2 startPos = new Vector2(256, 256);
    //Vector2 endPos = new Vector2(512, 512);

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
        yield return new WaitForSeconds(1f);
        obj.SetActive(false);
    }

    bool raycastTrue = false;
    // Update is called once per frame
    void Update()
    {
        //if (raycastTrue)
        //{
        //    TextureToWorldRaycast(raycastPoint);
        //    raycastTrue = false;
        //}
        //SetRectTransform(testRect, startPos.x, startPos.y, endPos.x, endPos.y);
        //Vector2 size = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
        //Vector2 midPoint = startPos + (size/2);
        //SetRectTransform(labelRect, startPos.x, startPos.y + size.y - 25, endPos.x, endPos.y);
        //SetLabelName(TextMesh, "Baby Harp Seal");
    }
}
