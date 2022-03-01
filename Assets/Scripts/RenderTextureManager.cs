using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;
using Unity.RenderStreaming;

public class RenderTextureManager : MonoBehaviour
{
    public RenderTexture renderTexture;
    private VideoStreamTrack track;


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

    void OnEnable()
    {
        var gfxType = SystemInfo.graphicsDeviceType;
        var format = GetSupportedRenderTextureFormat(gfxType);

        //track = new VideoStreamTrack("video", rt);
        //RenderStreaming.Instance.AddVideoStreamTrack(track);

    }

    void OnDisable()
    {
        //RenderStreaming.Instance.RemoveVideoStreamTrack(track);
    }
}
