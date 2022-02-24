using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using Unity.RenderStreaming;

public class InputManager : MonoBehaviour
{
    public static InputManager _InputManagerInstance;
    private VirtualCameraStreamer VirtualCameraStreamer;
    private RealSenseCameraStreamer RealCameraStreamer;

    public bool objectRecognitionModeOn = false;
    public bool virtualObjectRecognitionModeOn = false;
    public bool realSensePointCloudOverlayOn = false;

    public Vector2Int streamingSize = new Vector2Int(512, 512);
    public float depthThreshold;

    private VideoStreamTrack track;

    private RenderTexture finalRenderTexture;

    private void Awake()  //Used to initialize any variables or gams state before the game starts. Only called once
    {
        _InputManagerInstance = this;
        VirtualCameraStreamer = this.GetComponent<VirtualCameraStreamer>();
        RealCameraStreamer = this.GetComponent<RealSenseCameraStreamer>();

    }
    // Start is called before the first frame update. Used to pass information between referenced scripts
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        track = new VideoStreamTrack("video", VirtualCameraStreamer.rt);
        RenderStreaming.Instance.AddVideoStreamTrack(track);
    }

    void SendRenderTextureToWebRTCServer()
    {

    }
}
