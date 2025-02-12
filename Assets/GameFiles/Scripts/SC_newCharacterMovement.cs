/**
 * Copyright (c) code written by Germán López Gutiérrez
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class SC_newCharacterMovement : MonoBehaviour
{
    public bool isSliding { get; private set; }

    #region Variables Serializables
    [Header("Movement Parameters")]
    [SerializeField] private float _walkSpeed = 6f;
    [SerializeField] private float _slideSpeed = 12f;
    [SerializeField] private float _jumpPower = 10f;
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _movementSmoothness = 12f;
    [SerializeField] private int _airJumps = 1;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _airSmoothness = 4;
    [SerializeField] private float _wallDetection = 1;
    [SerializeField] private float _maxSpeedMultiplier = 2.0f;
    [SerializeField] private float _minSpeedMultiplier = 0.5f;
    [SerializeField] private float _slideDrag = 0.5f;
    #endregion

    #region Variables Privadas
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _CharacterController;
    private bool canMove = true;
    private float _currentMovementSpeed = 0;
    private Vector3 _smoothMovement = Vector3.zero;
    private int _jumpCount;
    private Vector3 _airMove;
    private GameObject _jumpedWall;
    private float _currentSpeedMultiplier = 1;
    #endregion

    void Start()
    {
        // Buscamos el character controller dentro de este game object
        _CharacterController = GetComponent<CharacterController>();

        // Configuramos el ratón para que se haga invisible y no se salga de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currentSpeedMultiplier = 1;
    }

    void Update()
    {
        #region Cálculo de la dirección del movimiento
        Transform _CameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Vector3 _right = Camera.main.transform.right.normalized;

        Vector3 _viewDir = (transform.position - new Vector3(_CameraTransform.position.x, transform.position.y, _CameraTransform.position.z)).normalized;

        isSliding = Input.GetKey(KeyCode.LeftShift);

        _currentMovementSpeed = isSliding ? _slideSpeed : _walkSpeed;
        // _currentControllerVelocity = Mathf.Lerp(_currentMovementSpeed, _CharacterController.velocity.magnitude, Time.deltaTime * 10);

        float _inputY = canMove ? Input.GetAxisRaw("Vertical") : 0;

        float _inputX = canMove ? Input.GetAxisRaw("Horizontal") : 0;

        float _movementDirectionY = _moveDirection.y;

        _moveDirection = (_viewDir * _inputY + _right * _inputX).normalized;

        // Alteramos la velocidad según el ángulo de la rampa si se está deslizando
        if (isSliding && _moveDirection.magnitude > 0)
        {
            RaycastHit _groundHit;
            if (Physics.Raycast(transform.position, Vector3.down, out _groundHit, 1.5f))
            {
                Vector3 groundNormal = _groundHit.normal;
                float angle = Vector3.Angle(transform.forward, groundNormal) - 90f;

                if (angle > 0)
                {
                    _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, _minSpeedMultiplier, angle / 45.0f * 2f * Time.deltaTime);
                }
                else if (angle < 0)
                {
                    _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, _maxSpeedMultiplier, -angle / 45.0f * 2f * Time.deltaTime);
                }
                else
                {
                    _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, 1.0f, _slideDrag * Time.deltaTime);
                }

                //Debug.Log($"Slope: {angle} / Speed Multiplier: {_currentSpeedMultiplier}");
            }
        }
        else if (_moveDirection.magnitude > 0)
        {
            _currentSpeedMultiplier = Mathf.Lerp(_currentSpeedMultiplier, 1.0f, _slideDrag * 2f * Time.deltaTime);
        }
        else
        {
            _currentSpeedMultiplier = 1;
        }

        _currentMovementSpeed *= _currentSpeedMultiplier;
        #endregion

        // Comprobamos si el jugador ha pulsado la tecla de salto y aplicamos la fuerza correspondiente en caso de que sea verdadero
        if (canMove && Input.GetButtonDown("Jump") && !isSliding)
        {
            RaycastHit _wallHit;
            if (_CharacterController.isGrounded)
            {
                _airMove = _moveDirection;
                _moveDirection.y = _jumpPower;
                _jumpCount = 0;
            }
            else if (Physics.Raycast(transform.position, transform.forward, out _wallHit, _wallDetection, _groundLayer))
            {
                _airMove = new Vector3(-transform.forward.x, _moveDirection.y, -transform.forward.z).normalized * 1.2f;
                _moveDirection.y = _jumpPower;
                transform.LookAt(transform.position - transform.forward);
            }
            else if (_jumpCount < _airJumps)
            {
                _airMove = _moveDirection;
                _moveDirection.y = _jumpPower;
                _jumpCount++;
            }
            else
            {
                _moveDirection.y = _movementDirectionY;
            }
        }
        else
        {
            _moveDirection.y = _movementDirectionY;
        }

        if (!_CharacterController.isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }

        Vector3 _finalGroundedMove = new Vector3(_moveDirection.x, _moveDirection.y, _moveDirection.z) * _currentMovementSpeed;
        Vector3 _finalAirMove = Vector3.Lerp(_airMove, _moveDirection, _airSmoothness * Time.deltaTime);
        _airMove = _finalAirMove;
        _finalAirMove *= _currentMovementSpeed;
        _finalAirMove.y = _moveDirection.y * _walkSpeed;

        Vector3 finalMove = _CharacterController.isGrounded ? _finalGroundedMove : _finalAirMove;
        _smoothMovement = Vector3.Lerp(_smoothMovement, finalMove, _movementSmoothness * Time.deltaTime);

        _CharacterController.Move(_smoothMovement * Time.deltaTime);

        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z)), Time.deltaTime * 15);
        }

        Debug.DrawRay(transform.position, _CharacterController.velocity / 8, isSliding ? Color.yellow : Color.cyan);
        Debug.DrawRay(transform.position + (_CharacterController.velocity / 8), (-transform.forward + transform.right) / 12, isSliding ? Color.yellow : Color.cyan);
        Debug.DrawRay(transform.position + (_CharacterController.velocity / 8), (-transform.forward - transform.right) / 12, isSliding ? Color.yellow : Color.cyan);
        Debug.DrawRay(transform.position, transform.forward * _wallDetection, Color.red);
    }

}
