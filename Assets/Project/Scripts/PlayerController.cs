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
    public float RunningSpeed = 3f; // 1.5x speed
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    private bool _isRunning = false;
    private bool _isAttacking = false;
    private bool _isBlocking = false;
    private bool _isSlapAttacking = false; // Track running attack state

    private float lastTapTime = 0f;
    private KeyCode lastTappedKey;
    private const float doubleTapWindow = 0.25f; // Time allowed for double tap

    public GameObject AttackHitbox; // Assigned in Inspector

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
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
                    cameraScript.Target = transform;
                }
            }
        }
    }

    void Update()
    {
        if (!HasInputAuthority) return;
        if (_isAttacking || _isSlapAttacking) return; // Prevent movement input during attack

        HandleMovementInput();
        HandleCombatInput();
    }

    private void HandleMovementInput()
    {
        bool isMoving = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
                        Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);

        if (CheckForDoubleTap(KeyCode.LeftArrow) || CheckForDoubleTap(KeyCode.RightArrow) ||
            CheckForDoubleTap(KeyCode.UpArrow) || CheckForDoubleTap(KeyCode.DownArrow))
        {
            _isRunning = true;
        }

        if (!isMoving)
        {
            _isRunning = false;
        }

        _animator.SetBool("IsRunning", _isRunning);
    }

    private bool CheckForDoubleTap(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastTappedKey == key && (Time.time - lastTapTime) < doubleTapWindow)
            {
                return true; // Double tap detected!
            }
            lastTappedKey = key;
            lastTapTime = Time.time;
        }
        return false;
    }

    private void HandleCombatInput()
    {
        if (Input.GetKeyDown(KeyCode.C)) { _jumpPressed = true; }
        if (Input.GetKeyDown(KeyCode.X)) { _isBlocking = true; }
        if (Input.GetKeyUp(KeyCode.X)) { _isBlocking = false; }

        if (Input.GetKeyDown(KeyCode.Z)) { ActivateTransformation(); }
        if (Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.V)) { ShootGun(); return; }

        if (Input.GetKeyDown(KeyCode.V) && !_isAttacking)
        {
            Debug.Log($"[PlayerController] Attack Input Received. _isRunning: {_isRunning}");

            if (_isRunning)
            {
                Debug.Log("[PlayerController] Calling PerformRunningAttack()!");
                PerformRunningAttack();
            }
            else
            {
                Debug.Log("[PlayerController] Calling PerformNormalAttack()!");
                PerformNormalAttack();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_controller.isGrounded) { _velocity = new Vector3(0, -1, 0); }
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        float moveSpeed = _isRunning ? RunningSpeed : PlayerSpeed;

        // Prevent movement during slap attack but allow during normal attacks
        if (!_isSlapAttacking)
        { 
            Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * moveSpeed;
            _velocity.y += GravityValue * Runner.DeltaTime;
            if (_jumpPressed && _controller.isGrounded) { _velocity.y += JumpForce; }

            _controller.Move(move + _velocity * Runner.DeltaTime);
            if (move != Vector3.zero) { transform.forward = move; }

            _animator.SetFloat("Speed", move.magnitude);
        }

        _jumpPressed = false;
    }

    private void PerformNormalAttack()
    {
        _isAttacking = true;
        _animator.SetTrigger("Attack");
    }

    private void PerformRunningAttack()
    {
        _isAttacking = true;
        _isSlapAttacking = true;
        _isRunning = false; // Running attack stops running

        Debug.Log("[PlayerController] Running attack triggered!");
        _animator.SetTrigger("RunningAttack");
    }

    // 🎯 Called by Animation Event to Enable Hitbox
    public void EnableHitbox()
    {
        if (!Runner.IsServer) return;

        Debug.Log("[PlayerController] Hitbox activated!");
        AttackHitbox.SetActive(true);
    }

    // 🎯 Called at the end of **Normal Attack animation** via Animation Event
    public void EndAttackAnimation()
    {
        Debug.Log("[PlayerController] Normal attack ended.");
        _isAttacking = false;
    }

    // 🎯 Called at the end of **Running Attack animation** via Animation Event
    public void EndRunningAttack()
    {
        Debug.Log("[PlayerController] Running attack ended.");
        _isSlapAttacking = false;
        _isAttacking = false;
    }

    // 🎯 Called by Animation Event to Disable Hitbox
    public void DisableHitbox()
    {
        Debug.Log("[PlayerController] Hitbox disabled!");
        AttackHitbox.SetActive(false);
    }

    private void ActivateTransformation() { Debug.Log("Transformation Activated! (Mock function)"); }
    private void ShootGun() { Debug.Log("Gun Attack! (Mock function)"); }
}
