using System;
using System.Collections;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.RenderStreaming
{
    public class WebCamStreamer : MonoBehaviour
    {
        [SerializeField, Tooltip("Streaming size should match display aspect ratio")]
        private Vector2Int streamingSize = new Vector2Int(1280, 720);

        [SerializeField, Tooltip("Device index of web camera")]
        private int deviceIndex = 0;

        private VideoStreamTrack track;
        private WebCamTexture webCamTexture;
        public Renderer image;

        public void ChangeBitrate(int bitrate)
        {
            RenderStreaming.Instance.ChangeVideoParameters(track, Convert.ToUInt64(bitrate), null);
        }

        public void ChangeFramerate(int framerate)
        {
            RenderStreaming.Instance.ChangeVideoParameters(track, null, Convert.ToUInt32(framerate));
        }

        IEnumerator Start()
        {
            if (WebCamTexture.devices.Length == 0)
            {
                Debug.LogFormat("WebCam device not found");
                yield break;
            }

            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogFormat("authorization for using the device is denied");
                yield break;
            }

            WebCamDevice userCameraDevice = WebCamTexture.devices[deviceIndex];

            webCamTexture = new WebCamTexture(userCameraDevice.name, streamingSize.x, streamingSize.y);
            image.material.mainTexture = webCamTexture;
            webCamTexture.Play();
            yield return new WaitUntil(() => webCamTexture.didUpdateThisFrame);

            track = new VideoStreamTrack(gameObject.name, webCamTexture);
            RenderStreaming.Instance.AddVideoStreamTrack(track);
        }

        void OnDisable()
        {
            RenderStreaming.Instance.RemoveVideoStreamTrack(track);
        }
    }
}
