using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public float scale = 1f;
    public float position = 0f;

    public RectTransform rt;
    public Canvas canvas;

    private void Update()
    {
        if (Input.GetKey("h"))
        {
            scale -= 0.001f;
        }
        if (Input.GetKey("j"))
        {
            scale += 0.001f;
        }
        if (Input.GetKey("k"))
        {
            position -= 1f;
        }
        if (Input.GetKey("l"))
        {
            position += 1f;
        }
        //rt.sizeDelta = new Vector2(1280 * (1+scale), 800 * (1+scale));
        canvas.planeDistance = scale;
        rt.localPosition = new Vector3(0, position, 0);
    }


}
