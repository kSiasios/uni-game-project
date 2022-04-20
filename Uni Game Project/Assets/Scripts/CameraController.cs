using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner cameraConfiner;
    [SerializeField] private BoxCollider2D triggerCollider;

    [SerializeField][Range(0f, 25f)] private float smallOrthographicSize = 7.5f;
    [SerializeField][Range(0f, 25f)] private float normalOrthographicSize = 9.4f;
    //private bool inBoundary = false;

    private void Awake()
    {
        if (!triggerCollider)
        {
            triggerCollider = transform.GetComponent<BoxCollider2D>();
        }
        if (!virtualCamera)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            cameraConfiner = virtualCamera.GetComponent<CinemachineConfiner>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Camera Boundary"))
        {
            //Debug.Log("Triggered!");
            cameraConfiner.m_BoundingShape2D = collision;

            if (collision.gameObject.GetComponent<SmallCameraBounder>())
            {
                //Debug.Log("Small Bounding Box");
                virtualCamera.m_Lens.OrthographicSize = smallOrthographicSize;
            }
            else
            {
                //Debug.Log("Normal Bounding Box");
                virtualCamera.m_Lens.OrthographicSize = normalOrthographicSize;
            }
        }
    }
}
