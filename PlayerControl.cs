using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private Animator animations;
    private PlayerInput accessPlayerInputUnity;
    private CharacterController characterController;
    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentJump;

    public Transform CameraAnglePlayer;
    public Transform TargetPlayer;
    public float speedNormal = 0.25f;

    public float turnSmoothTime = 0.1f;
    public float turnSmootVelocity = 1.0f;
    public float rotateSmoothSlerp = 20.0f;

    public float runSpeed = 5.0f;

    public float jumpGravity = 9.81f;
    public float jumpSpeed = 3.0f;
    public float jumpUpMax = 8.0f;
    public float jumpUpMin = 8.0f;
    private float moveVelocityFront;
    private float moveVelocityUp;

    private bool walkPressed;
    private bool runPressed;
    private bool jumpPressed;
    private bool jumpPressedLookOff = false;

    private int isWalkingHash;
    private int isRunningHash;
    private int isJumpHash;

    private void Awake()
    {
        accessPlayerInputUnity = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animations = GetComponent<Animator>();

        accessPlayerInputUnity.PlayerController.PlayerMovementController.started += onMovementInput;
        accessPlayerInputUnity.PlayerController.PlayerMovementController.canceled += onMovementInput;
        accessPlayerInputUnity.PlayerController.PlayerMovementController.performed += onMovementInput;

        accessPlayerInputUnity.PlayerController.PlayerJumpController.started += onJumpInput;
        accessPlayerInputUnity.PlayerController.PlayerJumpController.canceled += onJumpInput;
        accessPlayerInputUnity.PlayerController.PlayerJumpController.performed += onJumpInput;

        accessPlayerInputUnity.PlayerController.PlayerRunController.started += onRunInput;
        accessPlayerInputUnity.PlayerController.PlayerRunController.canceled += onRunInput;
        accessPlayerInputUnity.PlayerController.PlayerRunController.performed += onRunInput;

        isWalkingHash = Animator.StringToHash("WalkBool");
        isRunningHash = Animator.StringToHash("RunBool");
        isJumpHash = Animator.StringToHash("JumpBool");
    }
    private void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        walkPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    private void onRunInput(InputAction.CallbackContext context)
    {
        runPressed = context.ReadValueAsButton();
    }
    private void onJumpInput(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }
    private void Update()
    {
        controlrotationcharacter();
        controlmovementcharacter();
        controljumpcharacter();
        // handleRun();
    }
    private void controlrotationcharacter()
    {
        Vector3 direct;
        direct.x = currentMovement.x;
        direct.y = 0f;
        direct.z = currentMovement.z;
        Quaternion currentRotation = TargetPlayer.rotation;

        if (direct.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direct.x, direct.z) * Mathf.Rad2Deg + CameraAnglePlayer.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(targetAngle, transform.eulerAngles.y, ref turnSmootVelocity, turnSmoothTime);
            Quaternion EulerEz = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speedNormal * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(currentRotation, EulerEz, rotateSmoothSlerp * Time.deltaTime);
        }
    }
    private void controlmovementcharacter()
    {
        bool WalkBool = animations.GetBool(isWalkingHash);
        bool RunBool = animations.GetBool(isRunningHash);
        bool JumpIdleBool = animations.GetBool(isJumpHash);

        if (walkPressed && !WalkBool)
        {
            animations.SetBool(isWalkingHash, true);
        }
        else if (!walkPressed && WalkBool)
        {
            animations.SetBool(isWalkingHash, false);
        }

        if ((runPressed && walkPressed) && !RunBool)
        {
            animations.SetBool(isRunningHash, true);
        }
        else if ((!runPressed && !walkPressed) && RunBool)
        {
            animations.SetBool(isRunningHash, false);
        }
        else if (runPressed && !RunBool)
        {
            animations.SetBool(isRunningHash, true);
        }
        else if ((!runPressed && !walkPressed) && RunBool)
        {
            animations.SetBool(isRunningHash, false);
        }
    }
    private void controljumpcharacter()
    {
        Vector3 moveUp = new Vector3(currentJump.x, currentJump.y, currentJump.z);
        moveUp = Vector3.ClampMagnitude(moveUp, 0f);
        moveUp.z = currentJump.z;
        moveUp.x = currentJump.x;

        bool WalkBool = animations.GetBool(isWalkingHash);
        bool RunBool = animations.GetBool(isRunningHash);
        bool JumpIdleBool = animations.GetBool(isJumpHash);

        if (characterController.isGrounded)
        {
            moveVelocityFront = -jumpGravity * Time.deltaTime;
            moveVelocityUp = +jumpGravity * Time.deltaTime;

            if (jumpPressed && !JumpIdleBool)
            {
                animations.SetBool(isJumpHash, true);
                moveVelocityFront = jumpUpMin;
            }
            
            else
            {
                animations.SetBool(isJumpHash, false);
            }
            
            if (jumpPressed && walkPressed && !JumpIdleBool)
            {
                moveVelocityFront = jumpUpMax;
                moveVelocityUp = jumpSpeed;
                animations.SetBool(isJumpHash, true);
            }

            if (runPressed && jumpPressed && !JumpIdleBool)
            {
                moveVelocityFront = jumpUpMax;
                moveVelocityUp = jumpSpeed;
                animations.SetBool(isJumpHash, true);
            }
        }
        else
        {
            moveVelocityFront -= jumpGravity * Time.deltaTime;
            if (moveVelocityFront >= 0.1f)
            {
                moveUp = moveVelocityUp * Vector3.forward;
            }
        }
        moveUp.y = moveVelocityFront + moveVelocityUp * Time.deltaTime;
        moveUp = transform.TransformDirection(moveUp);

        characterController.Move(moveUp * Time.deltaTime);
    }
    private void handleRun()
    {
        Vector3 moveDir = transform.forward;
        if (runPressed)
        {
            characterController.Move(moveDir.normalized * runSpeed * Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        accessPlayerInputUnity.PlayerController.Enable();
    }
    private void OnDisable()
    {
        accessPlayerInputUnity.PlayerController.Disable();
    }
}