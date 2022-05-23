using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFollowCamera : MonoBehaviour
{
    GameObject MainCamera;
    public Vector3 Offset;

    private void Awake()
    {
        MainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = MainCamera.transform.position + Offset;

    }
}
