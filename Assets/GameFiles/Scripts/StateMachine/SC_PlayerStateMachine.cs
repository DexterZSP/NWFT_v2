using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class SC_PlayerStateMachine : MonoBehaviour
{
    PlayerInput _playerInput;
    [SerializeField] CharacterController _charController;
    [SerializeField] Animator _animator;
    public CharacterController CharController 
    { get { return _charController; } }

    public PlayerBaseState currentState;
    SC_PlayerStateFactory _states;

    Transform _cameraTransform;
    Vector2 _input;
    public Vector3 currentMovementInput;
    public float baseSpeed = 8;
    public Vector3 velocity;
    public float currentSpeedMultiplier = 1;
    public float maxSpeedMultiplier = 5f;
    public float minSpeedMultiplier = 0.3f;
    public int animationState;

    public bool movementPressed;
    public bool slidePressed;
    public bool jumpPressed = false;
    public bool requireNewJumpPress;
    public Vector3 airMove;

    void Awake()
    {
        // Configuramos el ratón para que se haga invisible y no se salga de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _playerInput = new PlayerInput();
        _charController = GetComponent<CharacterController>();
        _states = new SC_PlayerStateFactory(this);
        currentState = _states.Grounded();
        currentState.EnterState();

        //vincula la función que se ejecuta al moverse
        _playerInput.PlayerControls.Move.started += onMovementInput;
        _playerInput.PlayerControls.Move.performed += onMovementInput;
        _playerInput.PlayerControls.Move.canceled += onMovementInput;

        //vincula a función que se ejecuta al deslizar
        _playerInput.PlayerControls.Slide.performed += onSlide;
        _playerInput.PlayerControls.Slide.canceled += onSlide;

        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled += onJump;
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        HandleMoveInput();
        currentState.UpdateState();
        HandleRotation();
        _animator.SetInteger("state", animationState);
        _animator.SetFloat("velocity", velocity.magnitude);
        _charController.Move(velocity * Time.deltaTime);

        Debug.DrawRay(transform.position, transform.position + currentMovementInput, Color.cyan);
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = velocity.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = velocity.z;

        Quaternion currentRotation = transform.rotation;

        if (movementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, 20f * Time.deltaTime);
        }
    }

    void HandleMoveInput()
    {
        Vector3 _right = Camera.main.transform.right.normalized;
        Vector3 _viewDir = (transform.position - new Vector3(_cameraTransform.position.x, transform.position.y, _cameraTransform.position.z)).normalized;

        currentMovementInput = (_viewDir * _input.y + _right * _input.x).normalized;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        movementPressed = _input.x != 0 || _input.y != 0;
    }

    void onSlide(InputAction.CallbackContext context)
    {
        slidePressed = context.ReadValueAsButton();
    }

    void onJump(InputAction.CallbackContext context)
    {
        jumpPressed = context.ReadValueAsButton(); 
        requireNewJumpPress = false;
    }

    //Activar y desactivar el input system
    void OnEnable()
    { _playerInput.PlayerControls.Enable(); }

    void OnDisable()
    { _playerInput.PlayerControls.Disable(); }
}
