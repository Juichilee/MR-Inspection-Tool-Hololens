using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraAdjustment : MonoBehaviour
{
    public GameObject RSModel;
    public float posVal;
    public float rotVal;

    Vector3 translation = new Vector3();
    Vector3 rotation = new Vector3();

    public void updateValue()
    {
        RSModel.transform.Translate(translation);
        RSModel.transform.Rotate(rotation, Space.Self);

        translation.x = 0;
        translation.y = 0;
        translation.z = 0;

        rotation.x = 0;
        rotation.y = 0;
        rotation.z = 0;
    }

    public void changeTranslation(string axis)
    {
        switch (axis)
        {
            case "x":
                translation.x = posVal;
                break;
            case "y":
                translation.y = posVal;
                break;
            case "z":
                translation.z = posVal;
                break;

            case "-x":
                translation.x = -posVal;
                break;
            case "-y":
                translation.y = -posVal;
                break;
            case "-z":
                translation.z = -posVal;
                break;
        }
        updateValue();
    }

    public void changeRotation(string axis)
    {
        switch (axis)
        {
            case "z":
                rotation.z = rotVal;
                break;
            case "x":
                rotation.x = rotVal;
                break;
            case "y":
                rotation.y = rotVal;
                break;

            case "-z":
                rotation.z = -rotVal;
                break;
            case "-x":
                rotation.x = -rotVal;
                break;
            case "-y":
                rotation.y = -rotVal;
                break;
            case "flat":
                RSModel.transform.eulerAngles = rotation;
                break;
        }
        updateValue();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
