using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intel.RealSense;
using Microsoft.MixedReality.Toolkit.UI;

public class RealSenseCameraStreamer : MonoBehaviour
{
    public RsThresholdFilter rsTF;
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

    public void OnUpdate()
    {
        rsTF.SetMaxDistance(thresholdVal * 4);
    }

    public void setThresholdVal(SliderEventData eventData)
    {
        ThresholdVal = eventData.NewValue;
    }

    private void Update()
    {
        // Debug Purpose
        ThresholdVal = thresholdVal;
        rend.material.mainTexture = rgbMat.mainTexture;
    }
}
