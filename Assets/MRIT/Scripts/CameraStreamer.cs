using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class CameraStreamer : MonoBehaviour
{
    public Camera RSCameraView;

    [Range(0, 1)]
    [SerializeField]
    private float fovVal = 0.31843575f;
    public float FovVal
    {
        get { return fovVal; }
        set
        {
            fovVal = value;
            OnUpdate();
        }
    }

    public void OnUpdate()
    {
        RSCameraView.fieldOfView = fovVal * 179;
    }

    public void setFovVal(SliderEventData eventData)
    {
        FovVal = eventData.NewValue;
    }

    private void Start()
    {

    }

    private void Update()
    {
        FovVal = fovVal;
    }
}
