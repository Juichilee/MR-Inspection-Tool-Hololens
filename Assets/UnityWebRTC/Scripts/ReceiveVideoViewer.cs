using System;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.RenderStreaming
{
    public class ReceiveVideoViewer : MonoBehaviour
    {
        [SerializeField] private Vector2Int streamingSize = new Vector2Int(512, 512);

        private MediaStream receiveStream;
        public RawImage receiveImage;

        void OnEnable()
        {
            receiveStream = new MediaStream();
            RenderStreaming.Instance.AddVideoReceiveStream(receiveStream);
            receiveStream.OnAddTrack = e =>
            {
                if (receiveImage != null && e.Track.Kind == TrackKind.Video)
                {
                    var videoTrack = (VideoStreamTrack)e.Track;
                    receiveImage.texture = videoTrack.InitializeReceiver(streamingSize.x, streamingSize.y);
                }
            };
        }

        void OnDisable()
        {
            //RenderStreaming.Instance.RemoveVideoReceiveStream(receiveStream);
            receiveStream.OnAddTrack = null;
            receiveStream.Dispose();
            receiveStream = null;
        }
    }
}
