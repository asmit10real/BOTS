using Fusion;
using UnityEngine;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    private CharacterController _controller;
    private Animator _animator;
    public Camera Camera;

    public float PlayerSpeed = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    public GameObject AttackHitbox; // Assigned in Inspector

    private bool _isAttacking;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // Get Animator
    }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            Camera = Camera.main;
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                TopDownCamera cameraScript = mainCamera.GetComponent<TopDownCamera>();
                if (cameraScript != null)
                {
                    cameraScript.Target = transform; // Assign camera target
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.V)) // "V" for attack
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                RPC_PerformAttack();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        // Keep movement aligned with the camera's perspective
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        _animator.SetFloat("Speed", move.magnitude);

        _jumpPressed = false;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_PerformAttack()
    {
        Debug.Log("[PlayerController] Attack triggered!");

        _animator.SetTrigger("Attack");

        StartCoroutine(EnableHitboxTemporarily());
    }

    private IEnumerator EnableHitboxTemporarily()
    {
        AttackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f); // Adjust based on animation timing
        AttackHitbox.SetActive(false);
        _isAttacking = false;
    }
}
