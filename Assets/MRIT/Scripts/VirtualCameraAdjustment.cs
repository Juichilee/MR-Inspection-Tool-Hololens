using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraAdjustment : MonoBehaviour
{
    public Camera RScamera;

    public void incrementX()
    {
        RScamera.transform.position += new Vector3(1, 0, 0);
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
