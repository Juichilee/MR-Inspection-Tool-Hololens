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

    private void Awake()
    {
        SharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
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
            string currClass = pred_classes[i];
            float[] boxPositions = boxes[i];
        }
        
        //Debug.Log($"First Pred: {classes[0]}");
    }

    public void AddLabel()
    {
        GameObject virtualLabelInstance = GetPooledObject();

    }

    Vector3 testPos = new Vector3(256, 256, 0);
    // Update is called once per frame
    void Update()
    {
        Ray labelRay = RSCameraView.ScreenPointToRay(testPos);
        Vector3 start = RSCameraView.transform.position;
        Vector3 end = labelRay.GetPoint(10000f);
        // OR
        // Vector3 end = start + ray.direction * 10000;
        Debug.DrawRay(start, end, Color.red);
    }
}
