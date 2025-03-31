using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class SC_PlayerStateMachine : MonoBehaviour
{
    PlayerInput _playerInput;
    [SerializeField] CharacterController _charController;
    [SerializeField] Animator _animator;
    [SerializeField] float _maxHookDistace;
    public CharacterController CharController 
    { get { return _charController; } }

    public PlayerBaseState currentState;
    SC_PlayerStateFactory _states;

    Transform _cameraTransform;
    Vector2 _input;
    public int maxHP;
    public int currentHP;
    public Vector3 currentMovementInput;
    public float baseSpeed = 8;
    public Vector3 velocity;
    public Vector3 hookPoint;
    public float currentSpeedMultiplier = 1;
    public float maxSpeedMultiplier = 5f;
    public float minSpeedMultiplier = 0.3f;
    public int animationState;
    public LayerMask wallLayer;
    public LayerMask grabLayer;
    public bool bumpingIntoWall;
    public LineRenderer hookLine;
    public bool isHooked;

    public bool movementPressed;
    public bool slidePressed;
    public bool dashPressed;
    public bool attackPressed;
    public bool grabPressed;
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
        hookLine = GetComponent<LineRenderer>();
        hookLine.enabled = false;
        _states = new SC_PlayerStateFactory(this);
        currentState = _states.Grounded();
        currentState.EnterState();

        //vincula la función que se ejecuta al moverse
        _playerInput.PlayerControls.Move.started += onMovementInput;
        _playerInput.PlayerControls.Move.performed += onMovementInput;
        _playerInput.PlayerControls.Move.canceled += onMovementInput;

        //vincula la función que se ejecuta al deslizar
        _playerInput.PlayerControls.Slide.performed += onSlide;
        _playerInput.PlayerControls.Slide.canceled += onSlide;

        //vincula la función que se ejecuta al saltar
        _playerInput.PlayerControls.Jump.started += onJump;
        _playerInput.PlayerControls.Jump.canceled += onJump;

        //vincula la función que se ejecuta al dashear
        _playerInput.PlayerControls.Dash.started += onDash;
        _playerInput.PlayerControls.Dash.canceled += onDash;

        //vincula la función que se ejecuta al atacar
        _playerInput.PlayerControls.Attack.started += onAttack;
        _playerInput.PlayerControls.Attack.canceled += onAttack;

        //vincula la función que se ejecuta al lanzar el gancho
        _playerInput.PlayerControls.Grab.started += onGrab;
        _playerInput.PlayerControls.Grab.canceled += onGrab;

        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update()
    {
        bumpingIntoWall = WallDetect();
        HandleMoveInput();
        currentState.UpdateState();
        HandleRotation();
        _animator.SetInteger("state", animationState);
        Vector3 e = new Vector3(velocity.x, 0, velocity.z);
        _animator.SetFloat("moveVelocity", e.magnitude);
        _animator.SetFloat("verticalVelocity", velocity.y);
        _animator.SetBool("movementPressed", movementPressed);
        _animator.SetBool("bumpingIntoWall", bumpingIntoWall);
        _charController.Move(velocity * Time.deltaTime);

        MoveToHookPoint();

        Debug.DrawRay(transform.position, transform.position + currentMovementInput, Color.cyan);
    }

    void MoveToHookPoint()
    {
        // Actualizar la línea del rayo
        hookLine.SetPosition(0, transform.position);
        hookLine.SetPosition(1, hookPoint);
    }

    private bool WallDetect()
    {
        RaycastHit hit;
        // Detecta una pared frente al personaje
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, wallLayer))
        {
            return true;
        }
        else { return false; }
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = velocity.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = velocity.z;

        Quaternion currentRotation = transform.rotation;

        if (movementPressed || isHooked)
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

    void FireRay()
    {
        if(grabPressed)
        {
            RaycastHit hit;
            Vector3 direction = _cameraTransform.forward;
            if (Physics.Raycast(_cameraTransform.position, direction, out hit, _maxHookDistace, grabLayer))
            {
                hookPoint = hit.point;  // Guardar el punto de enganche
                if (Vector3.Distance(transform.position, hookPoint) > 1.2f && !slidePressed)
                { isHooked = true; }
            }
        }
        else
        {
            isHooked = false;
        }
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

    void onDash(InputAction.CallbackContext context)
    {
        dashPressed = context.ReadValueAsButton();
    }

    void onAttack(InputAction.CallbackContext context)
    {
        attackPressed = context.ReadValueAsButton();
    }

    void onGrab(InputAction.CallbackContext context)
    {
        grabPressed = context.ReadValueAsButton();
        FireRay();
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
