using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner cameraConfiner;
    private CinemachineFramingTransposer cameraBodySettings;
    [SerializeField] private BoxCollider2D triggerCollider;

    [SerializeField][Range(0f, 25f)] private float smallOrthographicSize = 7.5f;
    [SerializeField][Range(0f, 25f)] private float normalOrthographicSize = 9.4f;

    private CustomCameraSettings smallCameraBodySettings;
    private CustomCameraSettings normalCameraBodySettings;

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
            cameraBodySettings = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            
            normalCameraBodySettings = new CustomCameraSettings(
                cameraBodySettings.m_ScreenY,
                cameraBodySettings.m_DeadZoneHeight,
                cameraBodySettings.m_DeadZoneWidth,
                cameraBodySettings.m_SoftZoneHeight,
                cameraBodySettings.m_SoftZoneWidth,
                normalOrthographicSize
            );

            smallCameraBodySettings = new CustomCameraSettings(
                0.75f,
                0,
                0,
                0.21f,
                0.21f,
                smallOrthographicSize
            );
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Camera Boundary"))
        {
            //Debug.Log("Triggered!");
            cameraConfiner.m_BoundingShape2D = collision;

            Vector3 cameraTrackerOffsetObject = cameraBodySettings.m_TrackedObjectOffset;


            if (collision.gameObject.GetComponent<SmallCameraBounder>())
            {
                //Debug.Log("Small Bounding Box");
                //virtualCamera.m_Lens.OrthographicSize = smallOrthographicSize;
                virtualCamera.m_Lens.OrthographicSize = collision.gameObject.GetComponent<SmallCameraBounder>().getPreferredCameraSize();
                SetCameraSettings(smallCameraBodySettings);
                cameraBodySettings.m_TrackedObjectOffset = Vector3.up;
            }
            else
            {
                //Debug.Log("Normal Bounding Box");
                virtualCamera.m_Lens.OrthographicSize = normalOrthographicSize;
                SetCameraSettings(normalCameraBodySettings);
                cameraBodySettings.m_TrackedObjectOffset = cameraTrackerOffsetObject;
            }
        }
    }

    private void SetCameraSettings(CustomCameraSettings settings)
    {
        cameraBodySettings.m_ScreenY = settings.ScreenY;
        cameraBodySettings.m_DeadZoneHeight = settings.DeadZoneHeight;
        cameraBodySettings.m_DeadZoneWidth = settings.DeadZoneWidth;
        cameraBodySettings.m_SoftZoneHeight = settings.SoftZoneHeight;
        cameraBodySettings.m_SoftZoneWidth = settings.SoftZoneWidth;
        virtualCamera.m_Lens.OrthographicSize = settings.OrthographicSize;
    }
}

public class CustomCameraSettings
{
    float screenY;
    float deadZoneHeight;
    float deadZoneWidth;
    float softZoneHeight;
    float softZoneWidth;
    float orthographicSize;

    public CustomCameraSettings(float screenY, float deadZoneHeight, float deadZoneWidth, float softZoneHeight, float softZoneWidth, float orthographicSize)
    {
        ScreenY = screenY;
        DeadZoneHeight = deadZoneHeight;
        DeadZoneWidth = deadZoneWidth;
        SoftZoneHeight = softZoneHeight;
        SoftZoneWidth = softZoneWidth;
        OrthographicSize = orthographicSize;
    }

    public float ScreenY { get => screenY; set => screenY = value; }
    public float DeadZoneHeight { get => deadZoneHeight; set => deadZoneHeight = value; }
    public float DeadZoneWidth { get => deadZoneWidth; set => deadZoneWidth = value; }
    public float SoftZoneHeight { get => softZoneHeight; set => softZoneHeight = value; }
    public float SoftZoneWidth { get => softZoneWidth; set => softZoneWidth = value; }
    public float OrthographicSize { get => orthographicSize; set => orthographicSize = value; }
}
