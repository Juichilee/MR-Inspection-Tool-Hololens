using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTestScript : MonoBehaviour
{
    public RectTransform TextRect;
    public TMP_Text TextMesh;
    public Vector2 textureSize = new Vector2(512, 512);
    public GameObject LabelCanvas;

    // Start is called before the first frame update
    void Start()
    {
        CreateAndSetText();
    }
    public void SetRectTransform(RectTransform RawImageRect, float startPosX, float startPosY, float endPosX, float endPosY)
    {
        RawImageRect.offsetMin = new Vector2(startPosX, startPosY);
        RawImageRect.offsetMax = new Vector2(endPosX - textureSize.x, endPosY - textureSize.y);
    }
    void CreateAndSetText()
    {
        GameObject LabelCanvasInstance = Instantiate(LabelCanvas, this.transform);
        TextRect = LabelCanvasInstance.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
        TextMesh = LabelCanvasInstance.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        SetRectTransform(TextRect, 0, 231, 256, 256);
        LabelCanvasInstance.transform.localScale = new Vector3(0.002120921f * 1, 0.002120921f * 1, 1);
        TextMesh.text = "Obese";

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
