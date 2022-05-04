using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intel.RealSense;
using Microsoft.MixedReality.Toolkit.UI;

public class RealSenseCameraStreamer : MonoBehaviour
{
    public RsThresholdFilter rsTF;
    public Camera RSCameraView;
    public Renderer rend;
    public Material rsMat;

    [Range(0, 1)]
    [SerializeField]
    private float thresholdVal = 0.5f;
    public float ThresholdVal
    {
        get { return thresholdVal; }
        set
        {
            thresholdVal = value;
            OnUpdate();
        }
    }
    //
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
        rsTF.SetMaxDistance(thresholdVal * 4);
        RSCameraView.fieldOfView = fovVal * 179;
    }

    public void setThresholdVal(SliderEventData eventData)
    {
        ThresholdVal = eventData.NewValue;
    }

    public void setFovVal(SliderEventData eventData)
    {
        FovVal = eventData.NewValue;
    }

    private void Start()
    {
        rend.material = rsMat;
    }

    private void Update()
    {
        // Debug Purpose
        ThresholdVal = thresholdVal;
        FovVal = fovVal;
        //rend.material.mainTexture = rgbMat.mainTexture;
    }
}
