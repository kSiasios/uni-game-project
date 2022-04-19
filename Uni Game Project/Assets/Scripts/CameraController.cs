using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner cameraConfiner;
    [SerializeField] private BoxCollider2D triggerCollider;
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
        }
    }
}
