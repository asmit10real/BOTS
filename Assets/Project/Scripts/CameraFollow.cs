using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 5, -5);

    void LateUpdate() {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target);
    }

    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }
}
