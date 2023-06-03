using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Reference to the character's transform
    public float smoothSpeed = 0.125f;   // Smoothness factor for camera movement
    public Vector3 offset;          // Offset between the character and camera

    private void LateUpdate()
    {
        // Calculate the desired position for the camera
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position to the smoothed position
        transform.position = smoothedPosition;
    }
}
