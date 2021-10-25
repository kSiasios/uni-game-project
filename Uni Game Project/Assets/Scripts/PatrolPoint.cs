using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [Tooltip("The position of the PatrolPoint")]
    public Vector2 point = Vector2.zero;

    private void Awake()
    {
        point = transform.localPosition;
    }

    // Function that sends a Ray downwards to check if the point is floating in the air or not,
    // so that the Walker type of enemy do not go off their platform.
    public void SetPatrolPoint(float distance)
    {
        // Offset the patrol point from the parent by distance
        transform.localPosition = new Vector3(transform.position.x + distance, transform.position.y, 0);
    }

    // Function that sends a Ray to ensure that the point is not "inside" any other object
    // and above the ground layer. For Flyer type enemies.
    public void SetPatrolPointFlyer(Vector2 point)
    {
        // Offset the patrol point from the parent by point.x on the X-Axis and point.y on the Y-Axis
        transform.localPosition = new Vector3(transform.position.x + point.x, transform.position.y + point.y, 0);
    }
}
