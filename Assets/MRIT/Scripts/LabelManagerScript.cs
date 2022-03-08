using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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
            pooledObjects.Add(tmp);
        }
        
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < AmountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    Vector3 raycastPoint;

    public void ProcessLabelJSON(string content)
    {
        //LabelInfo frame = JsonUtility.FromJson<LabelInfo>(content);
        var frame = JsonConvert.DeserializeObject<LabelInfo>(content);
        string[] pred_classes = frame.pred_classes;
        float[] scores = frame.scores;
        float[][] boxes = frame.boxes;

        Vector2 startingPos = new Vector2(0,0);
        Vector2 endingPos = new Vector2(0,0);
        raycastPoint = new Vector3(0, 0, 0);

        Debug.Log($"JSON String: {content}");
        
        for(int i = 0; i < pred_classes.Length; i++)
        {
            string currClass = pred_classes[i];
            float[] boxPositions = boxes[i];

            float x = 0.00005f;
            Debug.Log($"PredClass: {currClass}, box1: {boxPositions[0].GetType()}, box2: {x.GetType()}");

            startingPos.x = boxPositions[0];
            //Debug.Log("Setting Pos Success1");
            startingPos.y = InvertYPos(boxPositions[1]);
            //Debug.Log("Setting Pos Success2");
            endingPos.x = boxPositions[2];
            //Debug.Log("Setting Pos Success3");
            endingPos.y = InvertYPos(boxPositions[3]);
            //Debug.Log("Setting Pos Success4");



            int xMidpoint = (int)CalculateMidPoint(startingPos.x, endingPos.x);
            int yMidpoint = (int)CalculateMidPoint(startingPos.y, endingPos.y);

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
        if(Physics.Raycast(labelRay, out hit))
        {
            GameObject labelInstance = GetPooledObject();
            if(labelInstance == null)
            {
                Debug.Log("NULL INSTANCE");
            }
            labelInstance.SetActive(true);
            labelInstance.transform.position = hit.point;
        }

        //Debug.Log("Finished Drawing Ray");
    }

    bool raycastTrue = false;
    // Update is called once per frame
    void Update()
    {
        if (raycastTrue)
        {
            TextureToWorldRaycast(raycastPoint);
            raycastTrue = false;
        }
        
        
    }
}
