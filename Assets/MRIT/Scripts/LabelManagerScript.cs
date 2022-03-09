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
        GameObject tmp;
        for(int i = 0; i < AmountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            tmp.transform.parent = RSTransform;
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

    Vector3 raycastPoint = new Vector3(0,0,0);
    Vector2 startingPos = new Vector2(0, 0);
    Vector2 endingPos = new Vector2(0, 0);
    Vector2 size = new Vector2(0, 0);
    string currClass;

    public struct FrameLabel
    {
        public Vector3 raycastPoint;
        public string[] predClasses;
        public float[][] boxPositions;
    }

    public FrameLabel currFrame;

    public void ProcessLabelJSON(string content)
    {
        //LabelInfo frame = JsonUtility.FromJson<LabelInfo>(content);


        var frame = JsonConvert.DeserializeObject<LabelInfo>(content);
        string[] pred_classes = frame.pred_classes;
        float[] scores = frame.scores;
        float[][] boxes = frame.boxes;

        
        //raycastPoint = new Vector3(0, 0, 0);

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

            Debug.Log($"MidPointX: {xMidpoint}, MidPointY:{yMidpoint}");

            raycastPoint.x = xMidpoint;
            raycastPoint.y = yMidpoint;
            raycastPoint.z = 0;

            size.x = endingPos.x - startingPos.x;
            size.y = endingPos.y - startingPos.y;

            frameArrived = true;
        }

        
        //Debug.Log($"First Pred: {classes[0]}");
    }

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
        if(Physics.Raycast(labelRay, out hit))
        {
            GameObject labelCanvasInstance = GetPooledObject(pooledObjects);
            if(labelCanvasInstance == null)
            {
                Debug.Log("NULL INSTANCE");
            }
            else
            {
                labelCanvasInstance.SetActive(true);
                labelCanvasInstance.transform.GetChild(0).GetComponent<RectTransform>().position = new Vector3(startingPos.x, startingPos.y);
                labelCanvasInstance.GetComponent<Canvas>().planeDistance = hit.point.z;
                labelCanvasInstance.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
                labelCanvasInstance.transform.GetChild(1).GetComponent<TextMeshPro>().text = currClass;
                StartCoroutine(DeactivateObjectTimer(labelCanvasInstance));
            }
        }

        //Debug.Log("Finished Drawing Ray");
    }

    IEnumerator DeactivateObjectTimer(GameObject targetObj)
    {
        yield return new WaitForSeconds(1.0f);
        targetObj.SetActive(false);
    }


    bool frameArrived = false;
    // Update is called once per frame
    void Update()
    {
        if (frameArrived)
        {
            TextureToWorldRaycast(raycastPoint);
            frameArrived = false;
        }
        
        
    }
}


