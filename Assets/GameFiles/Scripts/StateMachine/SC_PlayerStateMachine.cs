using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class SC_PlayerStateMachine : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField] CharacterController characterController;

    [SerializeField] Animator animator;
    int isWalkingHash;
    int isSlidingHash;
    int isJumpingHash;

    PlayerBaseState currentState;

    Transform _CameraTransform;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 appliedMovement;
    bool movementPressed;
    bool slidePressed;
    bool jumpPressed = false;

    public float jumpPower;
    public float gravity = -9.81f;

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
        _CameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
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

    void Update()
    {
        HandleMoveInput();
        HandleRotation();
        characterController.Move(appliedMovement * speed * Time.deltaTime);
    }

    void HandleMoveInput()
    {
        Vector3 _right = Camera.main.transform.right.normalized;
        Vector3 _viewDir = (transform.position - new Vector3(_CameraTransform.position.x, transform.position.y, _CameraTransform.position.z)).normalized;

        float _movementDirectionY = currentMovement.y;
        currentMovement = (_viewDir * currentMovementInput.y + _right * currentMovementInput.x).normalized;
        currentMovement.y = _movementDirectionY;

    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        movementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void onSlide(InputAction.CallbackContext context)
    {
        slidePressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton();
    }

    //Activar y desactivar el input system
    void OnEnable()
    { playerInput.PlayerControls.Enable(); }

    void OnDisable()
    { playerInput.PlayerControls.Disable(); }
}
