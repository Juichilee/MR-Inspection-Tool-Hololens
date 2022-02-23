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

    [Range(0.0f, 1.0f)]
    public float thresholdVal = 0.5f;

    [Range(0.0f, 1.0f)]
    public float focalVal = 0.5f;
    
    public void rsTFsetMax(SliderEventData eventData) 
    {
        //rsTF.SetMaxDistance(eventData.NewValue * 4);
        rsTF.SetMaxDistance(thresholdVal * 4);

    }

    public void canvasSetFocal(SliderEventData eventData)
    {
        //canvas.planeDistance = eventData.NewValue * 3 + 0.5f;
        canvas.planeDistance = focalVal * 3 + 0.5f;

    }

    private void Awake()
    {
        
    }


    private void Start()
    {
        //WebCamDevice[] devices = WebCamTexture.devices;
        //// for debugging purposes, prints available devices to the console
        //for (int i = 0; i < devices.Length; i++)
        //{
        //    print("Webcam available: " + devices[i].name);
        //}

        //// assuming the first available WebCam is desired
        //WebCamTexture tex = new WebCamTexture(devices[0].name);
        //rend.material.mainTexture = tex;
        
        //tex.Play();
    }

    private void Update()
    {
        rsTF.SetMaxDistance(thresholdVal * 4);
        canvas.planeDistance = focalVal * 3 + 0.5f;
        rend.material.mainTexture = rgbMat.mainTexture;
    }
}
