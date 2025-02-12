/**
 * Copyright (c) code written by Germán López Gutiérrez
 */

using UnityEngine;

/// <summary>
/// Controlador del movimiento del personaje.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SC_CharacterMovement : MonoBehaviour
{
        #region Variables Serializables
        [Header("Movement Parameters")]
        [SerializeField] private float _walkSpeed = 6f;
        [SerializeField] private float _runSpeed = 12f;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private float _gravity = 20f;
        [SerializeField] private float _movementSmoothness = 12f;
        #endregion

        #region Variables Privadas
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _CharacterController;
        private bool canMove = true;
        private float _currentMovementSpeed = 0;
        private Vector3 _smoothMovement = Vector3.zero;
        #endregion

        void Start()
        {
            // Buscamos el character controller dentro de este game object
            _CharacterController = GetComponent<CharacterController>();

            // Configuramos el ratón para que se haga invisible y no se salga de la pantalla
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()

        {
            #region Cálculo de la dirección del movimiento
            Transform _CameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            Vector3 _right = Camera.main.transform.right.normalized;

            Vector3 _viewDir = (transform.position - new Vector3(_CameraTransform.position.x, transform.position.y, _CameraTransform.position.z)).normalized;

            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            _currentMovementSpeed = isRunning ? _runSpeed : _walkSpeed;
            // _currentControllerVelocity = Mathf.Lerp(_currentMovementSpeed, _CharacterController.velocity.magnitude, Time.deltaTime * 10);

            float _inputY = canMove ? Input.GetAxisRaw("Vertical") : 0;

            float _inputX = canMove ? Input.GetAxisRaw("Horizontal") : 0;

            float _movementDirectionY = _moveDirection.y;

            _moveDirection = (_viewDir * _inputY + _right * _inputX).normalized;
            #endregion

            // Comprobamos si el jugador ha pulsado la tecla de salto y aplicamos la fuerza correspondiente en caso de que sea verdadero
            if (Input.GetButton("Jump") && canMove && _CharacterController.isGrounded)
                _moveDirection.y = _jumpPower;
            else
                _moveDirection.y = _movementDirectionY;

            // Aplicamos gravedad si el personaje no está tocando el suelo
            if (!_CharacterController.isGrounded)
                _moveDirection.y -= _gravity * Time.deltaTime;

            // Movimiento del character controller
            _smoothMovement = Vector3.Lerp(_smoothMovement, _moveDirection * _currentMovementSpeed, _movementSmoothness * Time.deltaTime);
            _CharacterController.Move(_smoothMovement * Time.deltaTime);

            // Rotación del personaje respecto al movimiento
            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z)), Time.deltaTime * 15);
            }

            // Debug para ver la dirección del movimiento
            Debug.DrawRay(transform.position, _CharacterController.velocity / 8, isRunning ? Color.yellow : Color.cyan);
            Debug.DrawRay(transform.position + (_CharacterController.velocity / 8), (-transform.forward + transform.right) / 12, isRunning ? Color.yellow : Color.cyan);
            Debug.DrawRay(transform.position + (_CharacterController.velocity / 8), (-transform.forward - transform.right) / 12, isRunning ? Color.yellow : Color.cyan);
        }

    }
