using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;
using Unity.RenderStreaming;

public class VirtualCameraStreamer : MonoBehaviour
{
    private Vector2Int streamingSize = new Vector2Int(512, 512);
    public Camera cameraStreamer;
    private VideoStreamTrack track;
    public Renderer image;

    void OnEnable()
    {
        // Create a track from the RenderTexture
        //rt = new RenderTexture(streamingSize.x, streamingSize.y, 0, format);
        //RenderTexture.active = rt;
        //Graphics.Blit(ri.mainTexture, rt);

        //image.material = RS_Mat;

        //track = new VideoStreamTrack("video", rt);
        //cameraStreamer.targetTexture = rt;

        //track = cameraStreamer.CaptureStreamTrack(streamingSize.x, streamingSize.y, 1000000);
        //image.material.mainTexture = track.Texture;
        //image.material.mainTexture = ri.texture;
        //RenderStreaming.Instance.AddVideoStreamTrack(track);
    }
}
