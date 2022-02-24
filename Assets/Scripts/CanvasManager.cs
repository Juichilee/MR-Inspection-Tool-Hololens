using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public float scale = 0f;
    public float position = 0f;

    public RectTransform rt;

    private void Update()
    {
        if (Input.GetKey("q"))
        {
            scale -= 0.001f;
        }
        if (Input.GetKey("w"))
        {
            scale += 0.001f;
        }
        if (Input.GetKey("e"))
        {
            position -= 1f;
        }
        if (Input.GetKey("r"))
        {
            position += 1f;
        }
        rt.sizeDelta = new Vector2(1280 * (1+scale), 800 * (1+scale));
        rt.localPosition = new Vector3(0, position, 0);
    }


}
