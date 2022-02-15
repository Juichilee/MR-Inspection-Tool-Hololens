using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.WebRTC;
using Unity.RenderStreaming.Signaling;

namespace Unity.RenderStreaming
{
    using DataChannelDictionary = Dictionary<int, RTCDataChannel>;

    public class RenderStreaming : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField, Tooltip("Signaling server url")]
        private string urlSignaling = "http://localhost";

        [SerializeField, Tooltip("Type of signaling server")]
        private string signalingType = typeof(HttpSignaling).FullName;

        [SerializeField, Tooltip("Array to set your own STUN/TURN servers")]
        private RTCIceServer[] iceServers = new RTCIceServer[]
        {
            new RTCIceServer()
            {
                urls = new string[] { "stun:stun.l.google.com:19302" }
            }
        };

        [SerializeField, Tooltip("Time interval for polling from signaling server")]
        private float interval = 5.0f;

        [SerializeField, Tooltip("Enable or disable hardware encoder")]
        private bool hardwareEncoderSupport = true;

#pragma warning restore 0649

        private SynchronizationContext m_mainThreadContext;
        private ISignaling signalingService;

        private readonly Dictionary<string, RTCPeerConnection> mapConnectionIdAndPeer = new Dictionary<string, RTCPeerConnection>();
        private readonly Dictionary<RTCPeerConnection, DataChannelDictionary> m_mapPeerAndChannelDictionary = new Dictionary<RTCPeerConnection, DataChannelDictionary>();
        private readonly List<VideoStreamTrack> listVideoStreamTrack = new List<VideoStreamTrack>();
        private readonly Dictionary<MediaStreamTrack, List<RTCRtpSender>> mapTrackAndSenderList = new Dictionary<MediaStreamTrack, List<RTCRtpSender>>();
        private readonly List<MediaStream> listVideoReceiveStream = new List<MediaStream>();
        private MediaStream audioStream;
        private RTCConfiguration rtcConfig;
        private string connectionId;

        public static RenderStreaming Instance { get; private set; }

        enum UnityEventType
        {
            SwitchVideo = 0
        }

        public void Awake()
        {
            Instance = this;
            var encoderType = hardwareEncoderSupport ? EncoderType.Hardware : EncoderType.Software;
            WebRTC.WebRTC.Initialize(encoderType);
            m_mainThreadContext = SynchronizationContext.Current;
        }

        public void OnDestroy()
        {
            Instance = null;
            WebRTC.WebRTC.Dispose();
            WebRTC.Audio.Stop();
            m_mainThreadContext = null;
        }

        public void Start()
        {
            audioStream = WebRTC.Audio.CaptureStream();
            rtcConfig = default;
            rtcConfig.iceServers = iceServers;
            StartCoroutine(WebRTC.WebRTC.Update());
        }

        VideoStreamTrack track;


        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (signalingService == null)
                {
                    Type t = Type.GetType(signalingType);
                    object[] args = { urlSignaling, interval, m_mainThreadContext };
                    signalingService = (ISignaling)Activator.CreateInstance(t, args);
                    signalingService.OnStart += signaling => signaling.CreateConnection();
                    signalingService.OnCreateConnection += (signaling, id) =>
                    {
                        connectionId = id;
                        CreatePeerConnection(signaling, connectionId, true);
                    };
                    signalingService.OnOffer += (signaling, data) => StartCoroutine(OnOffer(signaling, data));
                    signalingService.OnAnswer += (signaling, data) => StartCoroutine(OnAnswer(signaling, data));
                    signalingService.OnIceCandidate += OnIceCandidate;
                }
                signalingService.Start();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (string.IsNullOrEmpty(connectionId) ||
                    !mapConnectionIdAndPeer.TryGetValue(connectionId, out var pc))
                {
                    return;
                }

                RTCRtpTransceiver transceiver = pc.AddTransceiver(track);
                //transceiver.Direction = RTCRtpTransceiverDirection.SendRecv;
               // pc.AddTrack(track);
                // ToDO: need webrtc package version 2.3
            }


            if (Input.GetKeyDown(KeyCode.A))
            {
                if (string.IsNullOrEmpty(connectionId) ||
                !mapConnectionIdAndPeer.TryGetValue(connectionId, out var pc))
                {
                    return;
                }

                pc.AddTrack(track);
            }


        }


        void OnEnable()
        {
        }

        public void AddVideoStreamTrack(VideoStreamTrack track)
        {
            this.track = track;
            Debug.Log("AddVideoStreamTrack" + track.Id);
            listVideoStreamTrack.Add(track);
        }

        public void RemoveVideoStreamTrack(VideoStreamTrack track)
        {
            Debug.Log("RemoveVideoStreamTrack" + track.Id);
            listVideoStreamTrack.Remove(track);
        }

        public void AddVideoReceiveStream(MediaStream stream)
        {
            listVideoReceiveStream.Add(stream);
        }

        public void RemoveVideoReceiveStream(MediaStream stream)
        {
            listVideoReceiveStream.Remove(stream);
        }

        public void AddTransceiver()
        {
            if (string.IsNullOrEmpty(connectionId) ||
                !mapConnectionIdAndPeer.TryGetValue(connectionId, out var pc))
            {
                return;
            }

            RTCRtpTransceiver transceiver = pc.AddTransceiver(TrackKind.Video);
            //transceiver.Direction = RTCRtpTransceiverDirection.SendRecv;

            // ToDO: need webrtc package version 2.3
            //transceiver2.Direction = RTCRtpTransceiverDirection.SendOnly;
        }

        public void ChangeVideoParameters(VideoStreamTrack track, ulong? bitrate, uint? framerate)
        {
            foreach (var sender in mapTrackAndSenderList[track])
            {
                RTCRtpSendParameters parameters = sender.GetParameters();
                foreach (var encoding in parameters.encodings)
                {
                    if(bitrate != null) encoding.maxBitrate = bitrate;
                    if (framerate != null) encoding.maxFramerate = framerate;
                }
                sender.SetParameters(parameters);
            }
        }

        void OnDisable()
        {
            if (signalingService != null)
            {
                signalingService.Stop();
                signalingService = null;
            }
        }

        IEnumerator OnOffer(ISignaling signaling, DescData e)
        {
            Debug.Log($"OnOffer");
            var connectionId = e.connectionId;
            if (mapConnectionIdAndPeer.ContainsKey(connectionId))
            {
                Debug.LogError($"connection:{connectionId} peerConnection already exist");
                yield break;
            }

            var pc = CreatePeerConnection(signaling, connectionId, false);

            RTCSessionDescription rtcDesc;
            rtcDesc.type = RTCSdpType.Offer;
            rtcDesc.sdp = e.sdp;

            var opRemoteDesc = pc.SetRemoteDescription(ref rtcDesc);
            yield return opRemoteDesc;

            if (opRemoteDesc.IsError)
            {
                Debug.LogError($"Network Error: {opRemoteDesc.Error.message}");
                yield break;
            }

            foreach (var track in listVideoStreamTrack)
            {
                RTCRtpSender sender = pc.AddTrack(track);
                if (!mapTrackAndSenderList.TryGetValue(track, out List<RTCRtpSender> list))
                {
                    list = new List<RTCRtpSender>();
                    mapTrackAndSenderList.Add(track, list);
                }
                list.Add(sender);
            }

            foreach (var track in audioStream.GetTracks())
            {
                RTCRtpSender sender = pc.AddTrack(track);
                if (!mapTrackAndSenderList.TryGetValue(track, out List<RTCRtpSender> list))
                {
                    list = new List<RTCRtpSender>();
                    mapTrackAndSenderList.Add(track, list);
                }
                list.Add(sender);
            }

            RTCOfferAnswerOptions options = default;
            var op = pc.CreateAnswer(ref options);
            yield return op;

            if (op.IsError)
            {
                Debug.LogError($"Network Error: {op.Error.message}");
                yield break;
            }

            var desc = op.Desc;
            var opLocalDesc = pc.SetLocalDescription(ref desc);
            yield return opLocalDesc;

            if (opLocalDesc.IsError)
            {
                Debug.LogError($"Network Error: {opLocalDesc.Error.message}");
                yield break;
            }

            signaling.SendAnswer(connectionId, desc);
        }

        RTCPeerConnection CreatePeerConnection(ISignaling signaling, string connectionId, bool isOffer)
        {
            Debug.Log($"CreatePeerConnection: {connectionId}");
            if (mapConnectionIdAndPeer.TryGetValue(connectionId, out var peer))
            {
                peer.Close();
            }

            var pc = new RTCPeerConnection();
            mapConnectionIdAndPeer[connectionId] = pc;

            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(pc, channel); });
            pc.SetConfiguration(ref rtcConfig);
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                Debug.Log($"SendCandidate: {candidate}");
                signaling.SendCandidate(connectionId, candidate);
            });
            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if(state == RTCIceConnectionState.Disconnected)
                {
                    pc.Close();
                    mapConnectionIdAndPeer.Remove(connectionId);
                }
            });
            pc.OnTrack = trackEvent =>
            {
                foreach (var receiveStream in listVideoReceiveStream)
                {
                    receiveStream.AddTrack(trackEvent.Track);
                }
            };

            pc.OnNegotiationNeeded = () => StartCoroutine(OnNegotiationNeeded(signaling, connectionId, isOffer));
            return pc;
        }

        IEnumerator OnNegotiationNeeded(ISignaling signaling, string connectionId, bool isOffer)
        {
            Debug.Log($"OnNegotiationNeeded: {connectionId}");
            if (!isOffer)
            {
                yield break;
            }

            if (!mapConnectionIdAndPeer.TryGetValue(connectionId, out var pc))
            {
                Debug.LogError($"connectionId: {connectionId}, did not created peerConnection");
                yield break;
            }

            var offerOp = pc.CreateOffer();
            yield return offerOp;

            if (offerOp.IsError)
            {
                Debug.LogError($"Network Error: {offerOp.Error.message}");
                yield break;
            }

            if (pc.SignalingState != RTCSignalingState.Stable)
            {
                Debug.LogError($"peerConnection's signaling state is not stable.");
                yield break;
            }

            var desc = offerOp.Desc;
            var setLocalSdp = pc.SetLocalDescription(ref desc);
            yield return setLocalSdp;

            if (setLocalSdp.IsError)
            {
                Debug.LogError($"Network Error: {setLocalSdp.Error.message}");
                yield break;
            }

            Debug.LogError($"SendOffer: {connectionId}");
            signaling.SendOffer(connectionId, desc);
        }

        IEnumerator OnAnswer(ISignaling signaling, DescData e)
        {
            Debug.Log($"OnAnswer");
            if (!mapConnectionIdAndPeer.TryGetValue(e.connectionId, out var pc))
            {
                Debug.Log($"connectiondId:{e.connectionId}, peerConnection not exist");
                yield break;
            }

            var desc = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = e.sdp
            };
            var opRemoteSdp = pc.SetRemoteDescription(ref desc);
            yield return opRemoteSdp;

            if (opRemoteSdp.IsError)
            {
                Debug.LogError($"Network Error: {opRemoteSdp.Error.message}");
                yield break;
            }
        }

        void OnIceCandidate(ISignaling signaling, CandidateData e)
        {
            Debug.Log($"OnIceCandidate");
            if (!mapConnectionIdAndPeer.TryGetValue(e.connectionId, out var pc))
            {
                return;
            }

            RTCIceCandidateInit rtcInit = new RTCIceCandidateInit
            {
                candidate = e.candidate,
                sdpMLineIndex = e.sdpMLineIndex,
                sdpMid = e.sdpMid
            };

            RTCIceCandidate _candidate = new RTCIceCandidate(rtcInit);
            pc.AddIceCandidate(_candidate);
        }

        void OnDataChannel(RTCPeerConnection pc, RTCDataChannel channel)
        {
            Debug.Log($"OnDataChannel");
        }

        void OnCloseChannel(RTCDataChannel channel)
        {
        }

    }
}
