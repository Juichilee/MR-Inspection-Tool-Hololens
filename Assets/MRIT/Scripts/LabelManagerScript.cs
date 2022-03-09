using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class LabelManagerScript : MonoBehaviour
{
    public static LabelManagerScript SharedInstance;
    public List<GameObject> pooledObjects;
    public List<GameObject> pooledHitMarkers;
    public GameObject hitMarker;
    public GameObject objectToPool;
    public int AmountToPool;

    public Transform RSTransform;
    public Camera RSCameraView;

    public Vector2 textureSize;

    private void Awake()
    {
        SharedInstance = this;
        RSCameraView.aspect = 1.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        textureSize = InputManager._InputManagerInstance.streamingSize;
        pooledObjects = new List<GameObject>();
        //pooledHitMarkers = new List<GameObject>();

        GameObject tmp;
        for(int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            tmp.transform.parent = RSTransform;
            //tmp.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            tmp.GetComponent<Canvas>().worldCamera = RSCameraView;
            pooledObjects.Add(tmp);

            //tmp = Instantiate(hitMarker);
            //tmp.SetActive(false);
            //tmp.transform.parent = RSTransform;
            //pooledHitMarkers.Add(tmp);
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


    Vector2 startingPos = new Vector2(0, 0);
    Vector2 endingPos = new Vector2(0, 0);
    Vector3 raycastPoint = new Vector3(0, 0, 0);
    Vector2 size = new Vector2(0, 0);
    string currClass;

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

            startingPos.x = boxPositions[0];
            startingPos.y = InvertYPos(boxPositions[1]);
            endingPos.x = boxPositions[2];
            endingPos.y = InvertYPos(boxPositions[3]);

            int xMidpoint = (int)CalculateMidPoint(startingPos.x, endingPos.x);
            int yMidpoint = (int)CalculateMidPoint(startingPos.y, endingPos.y);

            size.x = endingPos.x - startingPos.y;
            size.y = endingPos.y - startingPos.x;

            Debug.Log($"MidPointX: {xMidpoint}, MidPointY:{yMidpoint}");

            raycastPoint.x = xMidpoint;
            raycastPoint.y = yMidpoint;
            raycastPoint.z = 0;

            Debug.Log("Raycasting");
            raycastTrue = true;
            //TextureToWorldRaycast(xMidpoint, yMidpoint);
        }
        

        //Debug.Log($"First Pred: {classes[0]}");
    }

    //public void AddLabel()
    //{
    //    GameObject virtualLabelInstance = GetPooledObject();

    //}

    public float InvertYPos(float pos)
    {
        return textureSize.y - pos;
    }

    public float CalculateMidPoint(float startingPos, float endingPos)
    {
        return startingPos + ((endingPos-startingPos)/2);
    }

    
    public void TextureToWorldRaycast(Vector3 texturePos)
    {

        RaycastHit hit;
        Ray labelRay = RSCameraView.ScreenPointToRay(texturePos);

        Vector3 start = RSCameraView.transform.position;

        Vector3 end = labelRay.GetPoint(10000f);
        // OR
        // Vector3 end = start + ray.direction * 10000;
        Debug.DrawRay(start, end, Color.red);
        if(Physics.Raycast(labelRay, out hit, Physics.IgnoreRaycastLayer))
        {
            GameObject labelInstance = GetPooledObject(pooledObjects);
            //GameObject hitMarkerInstance = GetPooledObject(pooledHitMarkers);

            //if(hitMarkerInstance == null)
            //{
            //    Debug.Log("NULL HITMARKER");
            //}
            //else
            //{
            //    hitMarkerInstance.transform.position = hit.point;
            //    hitMarkerInstance.SetActive(true);
            //    StartCoroutine(DeactivateObjectTimer(hitMarkerInstance));
            //}

            if(labelInstance == null)
            {
                Debug.Log("NULL INSTANCE");
            }
            else
            {
                labelInstance.SetActive(true);
                Canvas LabelCanvas = labelInstance.GetComponent<Canvas>();
                RectTransform RawImageRect = labelInstance.transform.GetChild(0).GetComponent<RectTransform>();
                Debug.Log("BREAK 1");
                //TextMeshPro TextMeshPro = labelInstance.transform.GetChild(1).GetComponent<TextMeshPro>();
                Debug.Log("BREAK 2");

                LabelCanvas.planeDistance = Mathf.Abs(hit.point.z - RSCameraView.transform.position.z);
                Debug.Log("BREAK 3");
                
                Debug.Log("BREAK 5");
                //TextMeshPro.SetText(currClass);

                //labelInstance.GetComponent<Canvas>().planeDistance = hit.point.z;
                //labelInstance.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
                //labelInstance.transform.GetChild(1).GetComponent<TextMeshPro>().text = currClass;
                
                StartCoroutine(DeactivateObjectTimer(labelInstance));
            }
            
            
            ///labelInstance.transform.position = hit.point;
            
        }

        //Debug.Log("Finished Drawing Ray");
    }


    public RectTransform testRect;
    Vector2 startPos = new Vector2(256, 256);
    Vector2 endPos = new Vector2(512, 512);

    public void SetRectTransform(RectTransform RawImageRect, Vector2 startingPos, Vector2 endingPos)
    {


        RawImageRect.offsetMin = startingPos;
        RawImageRect.offsetMax = endingPos;
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

        SetRectTransform(testRect, startPos, endPos);
    }
}
