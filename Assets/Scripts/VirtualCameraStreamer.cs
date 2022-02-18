using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class VirtualCameraStreamer : MonoBehaviour
{
    [SerializeField, Tooltip("Streaming size should match display aspect ratio")]
    private Vector2Int streamingSize = new Vector2Int(512, 512);

    private Camera cameraStreamer;
    private VideoStreamTrack track;
    public Renderer image;

    void Awake()
    {
        cameraStreamer = GetComponent<Camera>();
    }

    public float rotSpeed = 250;
    public float damping = 10;
    private float desiredRot;


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x > Screen.width / 2) desiredRot -= rotSpeed * Time.deltaTime;
            else desiredRot += rotSpeed * Time.deltaTime;
        }

        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);

    }
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

    public RenderTexture rt;

    void OnEnable()
    {
        // Get a valid RendertextureFormat
        var gfxType = SystemInfo.graphicsDeviceType;
        var format = GetSupportedRenderTextureFormat(gfxType);

        // Create a track from the RenderTexture
        rt = new RenderTexture(streamingSize.x, streamingSize.y, 0, format);
        track = new VideoStreamTrack("video", rt);
        cameraStreamer.targetTexture = rt;

        //track = cameraStreamer.CaptureStreamTrack(streamingSize.x, streamingSize.y, 1000000);
        image.material.mainTexture = track.Texture;
        //RenderStreaming.Instance.AddVideoStreamTrack(track);

        desiredRot = transform.eulerAngles.z;
    }

    void OnDisable()
    {
        //RenderStreaming.Instance.RemoveVideoStreamTrack(track);
    }
}
