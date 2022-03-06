using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class LabelInfo
{
    public float[] scores;
    public string[] pred_classes;
    public float[][] boxes;
}

//public class BoxPositions
//{
//    public float[] boxPositions;
//}