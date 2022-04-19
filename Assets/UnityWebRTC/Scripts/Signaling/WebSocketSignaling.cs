using System;
using System.Threading;
using Unity.WebRTC;
using UnityEngine;
using SocketIOClient;
using Newtonsoft.Json;

namespace Unity.RenderStreaming.Signaling
{
    public class WebSocketSignaling : ISignaling
    {
        private readonly string url;
        private readonly float timeout;
        private readonly AutoResetEvent wsCloseEvent;
        private readonly SynchronizationContext mainThreadContext;
        private Thread signalingThread;
        private SocketIO webSocket;
        private bool running;

        public event OnStartHandler OnStart;
        public event OnConnectHandler OnCreateConnection;
        public event OnOfferHandler OnOffer;

#pragma warning disable 0067
        // this event is never used in this class
        public event OnAnswerHandler OnAnswer;
#pragma warning restore 0067
        public event OnIceCandidateHandler OnIceCandidate;

        public WebSocketSignaling(string url, float timeout, SynchronizationContext mainThreadContext)
        {
            this.url = url;
            this.timeout = timeout;
            this.mainThreadContext = mainThreadContext;
            wsCloseEvent = new AutoResetEvent(false);
        }

        public void Start()
        {
            running = true;
            signalingThread = new Thread(WSManage);
            signalingThread.Start();
        }

        public void Stop()
        {
            if (running)
            {
                running = false;
                webSocket.DisconnectAsync();
                signalingThread.Abort();
            }
        }

        private void WSManage()
        {
            while (running)
            {
                WSCreate();
                wsCloseEvent.WaitOne();
                Thread.Sleep((int)(timeout * 1000));
            }

            Debug.Log("Signaling: WS managing thread ended");
        }

        private void OnDetection(SocketIOResponse e)
        {
            if (LabelManagerScript.SharedInstance.receiveFrameReady)
            {
                string json = e.ToString();
                var content = json.Substring(1, json.Length - 2);
                LabelManagerScript.SharedInstance.GetAndProcessLabelJSON(content);
            }
        }

        private void WSCreate()
        {
            webSocket = new SocketIO(url, new SocketIOOptions{ EIO = 4 });

            webSocket.OnConnected += OnConnected;
            webSocket.OnError += OnError;
            webSocket.OnDisconnected += OnClosed;
            webSocket.On("message", OnMessage);
            webSocket.On("detection", OnDetection);
            Monitor.Enter(webSocket);
            Debug.Log($"Signaling: Connecting WS {url}");
            webSocket.ConnectAsync();
        }

        public void CreateConnection()
        {
            webSocket.EmitAsync("message", new object[] { JsonConvert.DeserializeObject("{\"type\":\"connect\"}") });
        }

        private void WSSend<T>(RoutedMessage<T> data)
        {
            if (webSocket == null)
            {
                Debug.LogError("Signaling: WS is not connected. Unable to send message");
                return;
            }

            if (data is string s)
            {
                Debug.Log("Signaling: Sending WS data " + data.type + " : " + s);
                webSocket.EmitAsync("message", new object[] { JsonConvert.DeserializeObject(s) });
            }
            else
            {
                string str = JsonUtility.ToJson(data);
                Debug.Log("Signaling: Sending WS data " + data.type + " : " + str);
                webSocket.EmitAsync(data.type, new object[] { data });
            }
        }

        public void SendOffer(string connectionId, RTCSessionDescription offer)
        {
            DescData data = new DescData();
            data.connectionId = connectionId;
            data.sdp = offer.sdp.Replace("msid-semantic: WMS\r", 
                "msid-semantic: WMS local_av_stream\r").Replace("msid:-", "msid:-local_av_stream");
            data.type = "offer";

            RoutedMessage<DescData> routedMessage = new RoutedMessage<DescData>();
            routedMessage.from = connectionId;
            routedMessage.data = data;
            routedMessage.type = "offer";

            WSSend(routedMessage);
        }

        public void SendAnswer(string connectionId, RTCSessionDescription answer)
        {
            DescData data = new DescData();
            data.connectionId = connectionId;
            data.sdp = answer.sdp.Replace("msid-semantic: WMS\r", 
                "msid-semantic: WMS local_av_stream\r").Replace("msid:-", "msid:-local_av_stream");
            data.type = "answer";

            RoutedMessage<DescData> routedMessage = new RoutedMessage<DescData>();
            routedMessage.from = connectionId;
            routedMessage.data = data;
            routedMessage.type = "answer";

            WSSend(routedMessage);
        }

        public void SendCandidate(string connectionId, RTCIceCandidate candidate)
        {
            CandidateData data = new CandidateData();
            data.connectionId = connectionId;
            data.candidate = candidate.Candidate;
            data.sdpMLineIndex = candidate.SdpMLineIndex.Value;
            data.sdpMid = candidate.SdpMid;

            RoutedMessage<CandidateData> routedMessage = new RoutedMessage<CandidateData>();
            routedMessage.from = connectionId;
            routedMessage.data = data;
            routedMessage.type = "candidate";

            WSSend(routedMessage);
        }

        private void OnMessage(SocketIOResponse e)
        {
            try
            {
                string json = e.ToString();
                var content = json.Substring(1, json.Length - 2);
                Debug.Log($"Signaling: Receiving message: {content}");
                var routedMessage = JsonUtility.FromJson<RoutedMessage<SignalingMessage>>(content);

                SignalingMessage msg;
                if (!string.IsNullOrEmpty(routedMessage.type))
                {
                    msg = routedMessage.data;
                }
                else
                {
                    msg = JsonUtility.FromJson<SignalingMessage>(content);
                }

                if (!string.IsNullOrEmpty(routedMessage.type))
                {
                    if (routedMessage.type == "connect")
                    {
                        Debug.Log($"Signaling: OnCreateConnection");
                        string connectionId = JsonUtility.FromJson<SignalingMessage>(content).connectionId;
                        mainThreadContext.Post(d => OnCreateConnection?.Invoke(this, connectionId), null);
                    }
                    else if (routedMessage.type == "offer")
                    {
                        DescData offer = new DescData();
                        offer.connectionId = routedMessage.from;
                        offer.sdp = msg.sdp;
                        mainThreadContext.Post(d => OnOffer?.Invoke(this, offer), null);
                    }
                    else if (routedMessage.type == "answer")
                    {
                        DescData answer = new DescData
                        {
                            connectionId = routedMessage.from,
                            sdp = msg.sdp
                        };
                        mainThreadContext.Post(d => OnAnswer?.Invoke(this, answer), null);
                    }
                    else if (routedMessage.type == "candidate")
                    {
                        CandidateData candidate = new CandidateData
                        {
                            connectionId = routedMessage.@from,
                            candidate = msg.candidate,
                            sdpMLineIndex = msg.sdpMLineIndex,
                            sdpMid = msg.sdpMid
                        };
                        mainThreadContext.Post(d => OnIceCandidate?.Invoke(this, candidate), null);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Signaling: Failed to parse message: " + ex);
            }
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Debug.Log("Signaling: WS connected.");
            mainThreadContext.Post(d => OnStart?.Invoke(this), null);
        }

        private void OnError(object sender, string e)
        {
            Debug.LogError($"Signaling: WS connection error: {e}");
        }

        private void OnClosed(object sender, string e)
        {
            Debug.Log($"Signaling: WS connection closed, code: {e}");
            wsCloseEvent.Set();
            webSocket = null;
        }
    }
}
