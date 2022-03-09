using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraAdjustment : MonoBehaviour
{
    public Camera RScamera;
    public int val;

    Vector3 translation = new Vector3();
    Vector3 rotation = new Vector3();

    public void updateValue()
    {
        RScamera.transform.Translate(translation);
        RScamera.transform.Rotate(rotation, Space.Self);
    }

    public void changeTranslation(string axis)
    {
        switch (axis)
        {
            case "x":
                translation.x += val;
                break;
            case "y":
                translation.y += val;
                break;
            case "z":
                translation.z += val;
                break;

            case "-x":
                translation.x -= val;
                break;
            case "-y":
                translation.y -= val;
                break;
            case "-z":
                translation.z -= val;
                break;
        }
    }

    public void changeRotation(string axis)
    {
        switch (axis)
        {
            case "z":
                rotation.z += val;
                break;
            case "x":
                rotation.x += val;
                break;
            case "y":
                rotation.y += val;
                break;

            case "-z":
                rotation.z -= val;
                break;
            case "-x":
                rotation.x -= val;
                break;
            case "-y":
                rotation.y -= val;
                break;
        }
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
