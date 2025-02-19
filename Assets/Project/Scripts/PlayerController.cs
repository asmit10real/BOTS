using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour {
    private Vector3 _velocity;
    private bool _jumpPressed;
    private CharacterController _controller;
    public Camera Camera;

    public float PlayerSpeed = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    private void Awake() {
        _controller = GetComponent<CharacterController>();
    }

public override void Spawned() {
    Debug.Log($"PlayerController Spawned. HasInputAuthority: {Object.HasInputAuthority}");

    if (HasInputAuthority) {
        Debug.Log("This player has Input Authority! Setting up camera...");
        Camera = Camera.main;
        Camera mainCamera = Camera.main;
        if (mainCamera != null) {
            TopDownCamera cameraScript = mainCamera.GetComponent<TopDownCamera>();
            if (cameraScript != null) {
                cameraScript.Target = transform; // Assign camera target
            }
        }
    }
}

    void Update() {
        if (Input.GetButtonDown("Jump")) {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork() {
        if (_controller.isGrounded) {
            _velocity = new Vector3(0, -1, 0);
        }

        // Keep movement aligned with the camera's perspective
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded) {
            _velocity.y += JumpForce;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero) {
            transform.forward = move;
        }

        _jumpPressed = false;
    }
}
