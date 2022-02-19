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

    public void rsTFsetMax(SliderEventData eventData) 
    {
       rsTF.SetMaxDistance(eventData.NewValue * 4);
    }

    public void canvasSetFocal(SliderEventData eventData)
    {
        Debug.Log("Hello World");
        canvas.planeDistance = eventData.NewValue * 3 + 0.5f;
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
        rend.material.mainTexture = rgbMat.mainTexture;
    }
}
