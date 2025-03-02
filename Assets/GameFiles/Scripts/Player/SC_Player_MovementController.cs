using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class SC_Player_MovementController : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] CharacterController characterController;

    [SerializeField] Animator animator;
    int isWalkingHash;
    int isSlidingHash;
    int isJumpingHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool movementPressed;
    bool slidePressed;
    bool jumpPressed = false;

    public float jumpPower;
    public float gravity = -9.81f; 
    bool isJumping = false;

    [SerializeField] float speed = 5f;
    float rotationFactorPerFrame = 20f;

    void Awake()
    {
        // Configuramos el ratón para que se haga invisible y no se salga de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isSlidingHash = Animator.StringToHash("isSliding");
        isJumpingHash = Animator.StringToHash("isJumping");

        //vincula la función que se ejecuta al moverse
        playerInput.PlayerControls.Move.started += onMovementInput;
        playerInput.PlayerControls.Move.performed += onMovementInput;
        playerInput.PlayerControls.Move.canceled += onMovementInput;

        //vincula a función que se ejecuta al deslizar
        playerInput.PlayerControls.Slide.performed += onSlide;
        playerInput.PlayerControls.Slide.canceled += onSlide;

        playerInput.PlayerControls.Jump.started += onJump;
        playerInput.PlayerControls.Jump.canceled += onJump;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        Transform _CameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Vector3 _right = Camera.main.transform.right.normalized;

        Vector3 _viewDir = (transform.position - new Vector3(_CameraTransform.position.x, transform.position.y, _CameraTransform.position.z)).normalized;

        currentMovementInput = context.ReadValue<Vector2>();

        float _movementDirectionY = currentMovement.y;
        currentMovement = (_viewDir * currentMovementInput.y + _right * currentMovementInput.x).normalized;
        currentMovement.y = _movementDirectionY;


        movementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void onSlide (InputAction.CallbackContext context)
    {
        slidePressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }

    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && jumpPressed && !slidePressed)
        {
            isJumping = true;
            currentMovement.y = jumpPower;
        }
        else if (!jumpPressed && isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0f || !jumpPressed;
        float fallMultiplier = 1.2f;

        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
        }
        else 
        {
            if (isFalling)
            { currentMovement.y = Mathf.Max(currentMovement.y + (gravity * Time.deltaTime), -20f); }
            else
            { currentMovement.y = Mathf.Max(currentMovement.y + (gravity * Time.deltaTime * fallMultiplier), -20f); }
            
        }
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (movementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void HandleAnimation()
    {
        bool _isWalking = animator.GetBool(isWalkingHash);
        bool _isSliding = animator.GetBool(isSlidingHash);
        bool _isJumping = animator.GetBool(isJumpingHash);

        if (movementPressed && !_isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!movementPressed && _isWalking) {
            {
                animator.SetBool(isWalkingHash, false);
            }
        }

        if (slidePressed && !_isSliding)
        {
            animator.SetBool(isSlidingHash, true);
        }
        else if (!slidePressed && _isSliding)
        {
            {
                animator.SetBool(isSlidingHash, false);
            }
        }

        if (!characterController.isGrounded && !_isJumping)
        {
            animator.SetBool(isJumpingHash, true);
        }
        else if (characterController.isGrounded && _isJumping)
        {
            {
                animator.SetBool(isJumpingHash, false);
            }
        }
    }

    void Update()
    {
        HandleAnimation();
        HandleRotation();

        characterController.Move(currentMovement * speed * Time.deltaTime);

        HandleGravity();
        HandleJump();
    }

    //Activar y desactivar el input system
    void OnEnable()
    { playerInput.PlayerControls.Enable(); }

    void OnDisable()
    { playerInput.PlayerControls.Disable(); }

}
