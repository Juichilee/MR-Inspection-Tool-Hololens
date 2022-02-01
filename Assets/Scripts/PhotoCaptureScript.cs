using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class PhotoCaptureScript : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    Texture2D targetTexture = null;

    public GameObject displayScreen;
    public int screenSize = 1;

    Renderer quadRenderer;

    // Use this for initialization
    void Start()
    {
        // Create a gameobject that we can apply our texture to
        //displayScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = displayScreen.GetComponent<Renderer>() as Renderer;
        //quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        Vector3 scale = new Vector3((screenSize * ((float)16))/(float)144, (screenSize * ((float)9))/(float)144, 1);
        displayScreen.transform.localScale = scale;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    private void Update()
    {
        // Take a picture
        if (photoCaptureObject != null)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }

    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into our target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate our camera
        //photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}