using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCameraBounder : MonoBehaviour
{
    [SerializeField][Range(0f, 25f)] private float cameraScale = 5.4f;

    public float getPreferredCameraSize()
    {
        return cameraScale;
    }
}
