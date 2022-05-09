using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using Unity.RenderStreaming;

public class InputManager : MonoBehaviour
{
    [SerializeField, Tooltip("Streaming size should match display aspect ratio")]
    private Vector2Int streamingSize; 
    private VideoStreamTrack track;
    public Renderer image;
    public Renderer crop;

    public static RenderTextureFormat GetSupportedRenderTextureFormat(UnityEngine.Rendering.GraphicsDeviceType type)
    {
        switch (type)
        {
            case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
            case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
            case UnityEngine.Rendering.GraphicsDeviceType.Vulkan:
                return RenderTextureFormat.BGRA32;
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore:
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2:
            case UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3:
                return RenderTextureFormat.ARGB32;
            case UnityEngine.Rendering.GraphicsDeviceType.Metal:
                return RenderTextureFormat.BGRA32;
        }
        return RenderTextureFormat.Default;
    }

    void Awake()
    {
        // Get a valid RendertextureFormat
        var gfxType = SystemInfo.graphicsDeviceType;
        var format = GetSupportedRenderTextureFormat(gfxType);

        // Create a track from the RenderTexture
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Webcam available: " + devices[0].name);
        WebCamTexture camTex = new WebCamTexture(devices[0].name);

        track = new VideoStreamTrack("video", camTex);
        image.material.mainTexture = camTex;
        camTex.Play();

        crop.material.mainTexture = camTex;
        // cameraStreamer.targetTexture = rt;

        //track = cameraStreamer.CaptureStreamTrack(streamingSize.x, streamingSize.y, 1000000);
        //image.material.mainTexture = track.Texture;
        RenderStreaming.Instance.AddVideoStreamTrack(track);
    }

    void OnDisable()
    {
        //RenderStreaming.Instance.RemoveVideoStreamTrack(track);
    }
}
