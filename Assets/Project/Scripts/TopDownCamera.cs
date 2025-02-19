using UnityEngine;

public class TopDownCamera : MonoBehaviour {
    public Transform Target;
    public float SmoothSpeed = 10f;
    public Vector3 Offset = new Vector3(0, 10, -10); // Set for 45-degree view

    private Quaternion fixedRotation;

    void Start() {
        // Set the camera to a fixed 45-degree angle
        fixedRotation = Quaternion.Euler(45, 0, 0);
        transform.rotation = fixedRotation;
    }

    void LateUpdate() {
        if (Target == null) return;

        // Move the camera towards the player's position while keeping a fixed rotation
        Vector3 targetPosition = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, SmoothSpeed * Time.deltaTime);
        transform.rotation = fixedRotation; // Keep rotation fixed
    }
}
