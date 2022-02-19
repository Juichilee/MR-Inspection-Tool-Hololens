using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Intel.RealSense;
using Microsoft.MixedReality.Toolkit.UI;

public class RealSenseCameraStreamer : MonoBehaviour
{
    public RsThresholdFilter rsTF;
    public Canvas canvas;

    public void rsTFsetMax(SliderEventData eventData) 
    {
        rsTF.SetMaxDistance(eventData.NewValue * 4);
    }

    public void canvasSetFocal(SliderEventData eventData)
    {
        Debug.Log("Hello World");
        canvas.planeDistance = eventData.NewValue * 3 + 0.5f;
    }


}
