using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;

public class InputManager : MonoBehaviour
{
    private static InputManager _InputManagerInstance;
    public static InputManager InputManagerInstance { get { return _InputManagerInstance; } }
    public bool ObjectRecognitionModeOn = false;
    public bool RealSensePointCloudOverlayOn = false;
    public bool VirtualObjectRecognitionModeOn = false;

    public Vector2Int StreamingSize = new Vector2Int(512, 512);

    public float DepthThreshold;
    VideoStreamTrack track;

    private void Awake()  //Used to initialize any variables or gams state before the game starts. Only called once
    {
        if(_InputManagerInstance != null && _InputManagerInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _InputManagerInstance = this;
        }
    }
    // Start is called before the first frame update. Used to pass information between referenced scripts
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //RenderTexture OverlayRenderTextures(RenderTexture r1, RenderTexture r2)
    //{

    //}
}
