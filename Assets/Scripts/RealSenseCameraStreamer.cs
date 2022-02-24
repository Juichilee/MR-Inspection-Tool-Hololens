using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intel.RealSense;
using Microsoft.MixedReality.Toolkit.UI;

public class RealSenseCameraStreamer : MonoBehaviour
{
    public RsThresholdFilter rsTF;
    public Canvas canvas;
    public Renderer rend;
    public Material rgbMat;

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

    [Range(0, 1)]
    [SerializeField]
    private float focalVal = 0.5f;
    public float FocalVal
    {
        get { return focalVal; }
        set
        {
            focalVal = value;
            OnUpdate();
        }
    }

    public void OnUpdate()
    {
        rsTF.SetMaxDistance(thresholdVal * 4);
        canvas.planeDistance = focalVal * 3 + 0.5f;
    }

    public void setThresholdVal(SliderEventData eventData)
    {
        ThresholdVal = eventData.NewValue;
    }
    public void setFocalVal(SliderEventData eventData)
    {
        FocalVal = eventData.NewValue;
    }

    private void Update()
    {
        // Debug Purpose
        ThresholdVal = thresholdVal;
        FocalVal = focalVal;
        rend.material.mainTexture = rgbMat.mainTexture;
    }
}
