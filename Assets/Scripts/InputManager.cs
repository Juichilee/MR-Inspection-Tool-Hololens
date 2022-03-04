using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using Unity.RenderStreaming;

public class InputManager : MonoBehaviour
{
    public static InputManager _InputManagerInstance;
    //private VirtualCameraStreamer VirtualCameraStreamer;
    //private RealSenseCameraStreamer RealCameraStreamer;

    //public bool objectRecognitionModeOn = false;
    //public bool virtualObjectRecognitionModeOn = false;
    //public bool realSensePointCloudOverlayOn = false;

    public Vector2Int streamingSize = new Vector2Int(512, 512);
    public Material inputMat;
    public Material cropMat;
    public Renderer image;
    public Renderer cropImage;
    public Renderer processedImage;
    //public float depthThreshold;

    private VideoStreamTrack track;

    //private RenderTexture finalRenderTexture;

    public Camera CropCamera;

    bool trackAdded = false;

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

    private void Awake()  //Used to initialize any variables or gams state before the game starts. Only called once
    {
        _InputManagerInstance = this;

        // Get a valid RendertextureFormat
        var gfxType = SystemInfo.graphicsDeviceType;
        RenderTextureFormat format = GetSupportedRenderTextureFormat(gfxType);
        Debug.Log("RenderTextureFormat: " + format);

        //RenderTexture rend = (RenderTexture)inputMat.mainTexture;
        //RenderTexture rend = new RenderTexture(streamingSize.x, streamingSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
        RenderTexture rend = new RenderTexture(streamingSize.x, streamingSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.B8G8R8A8_UNorm);

        inputMat.mainTexture = rend;

        Debug.Log("RenderTextureFormat: " + format);
        Debug.Log("InputRTFormat: " + rend.graphicsFormat);

        //Color[] c = 
        //RenderTexture processedRend = new RenderTexture(streamingSize.x, streamingSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.B8G8R8A8_UNorm);
        image.material = inputMat;
        cropImage.material = inputMat;

        RenderTexture cropRend = new RenderTexture(streamingSize.x, streamingSize.y, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.B8G8R8A8_UNorm);
        CropCamera.targetTexture = cropRend;
        cropMat.mainTexture = cropRend;

        processedImage.material = cropMat;

        //VirtualCameraStreamer = this.GetComponent<VirtualCameraStreamer>();
        //RealCameraStreamer = this.GetComponent<RealSenseCameraStreamer>();

    }

    // Start is called before the first frame update. Used to pass information between referenced scripts
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!trackAdded)
        {
            track = new VideoStreamTrack("video", cropMat.mainTexture);
            RenderStreaming.Instance.AddVideoStreamTrack(track);
            trackAdded = true;
        }
        
    }

    void SendRenderTextureToWebRTCServer()
    {

    }
}
