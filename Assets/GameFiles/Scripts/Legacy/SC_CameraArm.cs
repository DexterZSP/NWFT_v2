/**
 * Copyright (c) code written by Germán López Gutiérrez
 */

using UnityEngine;

/// <summary>
/// Controlador del brazo de la cámara.
/// </summary>
public class SC_CameraArm : MonoBehaviour
{
    #region Variables Serializables
    [Header("Base Settings")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField, Range(0.0f, 10)] private float _armLength = 3f;
    [SerializeField] private Vector2 _cameraOffset;
    [SerializeField] private bool _enableCollisionTest = true;
    [SerializeField] private float _collisionRepositioningSpeed = 40;
    [SerializeField] private bool _showCollisionDebug = false;
    [SerializeField] private bool _enableCameraLag = true;
    [SerializeField] private Vector2 _cameraLag;

    [Header("Movement")]
    [SerializeField] private Vector2 _sensitivity = new Vector2(1, 1);
    [SerializeField] private Vector2 _cameraMaxVerticalRotation = new Vector2(-90, 90);
    #endregion

    #region Variables Privadas
    private float _rotationX = 0;
    private float _rotationY = 0;
    private float _currentArmLength;
    private float _cameraArmPositionX;
    private float _cameraArmPositionY;
    private float _cameraArmPositionZ;

    private Transform _CameraTransform;
    private Transform _CameraInitialTransform;
    private Transform _CharacterTransform;
    #endregion


    void Start()
    {
        // Inicializamos los parámetros que se actualizarán en cada update
        _currentArmLength = _armLength;

        // Desanclamos la cámara del player para poder configurar su posición con mayor libertad
        _CharacterTransform = transform.parent;
        transform.parent = null;
    }

    void LateUpdate()
    {

        // Buscamos la cámara si nos falta la referencia del transform
        if (!_CameraTransform)
        {
            FindCameraTransform(ref _CameraTransform);

            if (_CameraTransform != null)
            {
                UpdateCameraArmLength(ref _CameraTransform);
                UpdateCameraOffset(ref _CameraTransform);

                GameObject _InitialCamera = new GameObject("Camera Initial Transform");
                _InitialCamera.transform.parent = transform;
                _InitialCamera.transform.position = _CameraTransform.position;
                _CameraInitialTransform = _InitialCamera.transform;       
            }

            return;
        }

        // Actualizamos la posición de la cámara
        if (_enableCameraLag)
        {
            _cameraArmPositionX = Mathf.Lerp(_cameraArmPositionX, _CharacterTransform.position.x, Time.deltaTime * _cameraLag.x);
            _cameraArmPositionY = Mathf.Lerp(_cameraArmPositionY, _CharacterTransform.position.y, Time.deltaTime * _cameraLag.y);
            _cameraArmPositionZ = Mathf.Lerp(_cameraArmPositionZ, _CharacterTransform.position.z, Time.deltaTime * _cameraLag.x);
            transform.position = new Vector3(_cameraArmPositionX, _cameraArmPositionY, _cameraArmPositionZ);
        }
        else
            transform.position = _CharacterTransform.position;


        // Comprobamos si hay un obstáculo frente a la cámara
        if (_enableCollisionTest)
        {
            RaycastHit _hit;
            Vector3 _cameraDirection = (_CameraInitialTransform.position - transform.position).normalized;


            // Does the ray intersect any objects excluding the player layer
            if (Physics.CapsuleCast(transform.position + (transform.up * 0.2f), transform.position + (transform.up * -0.2f), 0.05f, _cameraDirection, out _hit, _armLength, ~_playerLayer /* Ignoramos la colisión con el character */))

            {
                if(_showCollisionDebug)
                    Debug.DrawRay(transform.position, _cameraDirection * _armLength, Color.yellow);
                _currentArmLength = Mathf.Lerp(_currentArmLength, Vector3.Distance(transform.position, _hit.point), Time.deltaTime * 30);
            }
            else
            {
                if(_showCollisionDebug)
                    Debug.DrawRay(transform.position, _cameraDirection * _armLength, Color.white);
                _currentArmLength = Mathf.Lerp(_currentArmLength, _armLength, Time.deltaTime * _collisionRepositioningSpeed);
            }

            UpdateCameraArmLength(ref _CameraTransform);
        }

        // Actualizamos la rotación de la cámara
        _rotationX += -Input.GetAxis("Mouse Y") * _sensitivity.y *Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, _cameraMaxVerticalRotation.x, _cameraMaxVerticalRotation.y);
        _rotationY += Input.GetAxis("Mouse X") * _sensitivity.x * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
    }

    /// <summary>
    /// Actualiza la longitud del brazo de la cámara.
    /// </summary>
    /// <param name="CameraTransform">Referencia al transform de la cámara</param>
    public void UpdateCameraArmLength(ref Transform CameraTransform)
    {
        CameraTransform.localPosition = new Vector3(CameraTransform.localPosition.x, CameraTransform.localPosition.y, -_currentArmLength);
    }

    /// <summary>
    /// Actualiza el offset de la cámara.
    /// </summary>
    /// <param name="CameraTransform">Referencia al transform de la cámara</param>
    public void UpdateCameraOffset(ref Transform CameraTransform)
    {
        CameraTransform.localPosition = new Vector3(_cameraOffset.x, _cameraOffset.y, CameraTransform.localPosition.z);
    }

    /// <summary>
    /// Busca el transform de la cámara entre los hijos del brazo.
    /// </summary>
    /// <param name="CameraTransform">Referencia a la variable que contendrá el transform de la cámara</param>
    protected void FindCameraTransform(ref Transform CameraTransform)
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Camera>() != null)
            {
                _CameraTransform = child;
            }
        }
    }

    /// <summary>
    /// Esto es solo para debug. Permite visualizar una traza entre la posición del character y la cámara, es decir, 
    /// lo que sería el brazo.
    /// </summary>
    private void OnDrawGizmos()
    {
        FindCameraTransform(ref _CameraTransform);

        if (_CameraTransform == null)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            _currentArmLength = _armLength;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _CameraTransform.position);
            Gizmos.DrawSphere(transform.position, 0.05f);
            Gizmos.DrawSphere(_CameraTransform.position, 0.05f);
            UpdateCameraArmLength(ref _CameraTransform);
            UpdateCameraOffset(ref _CameraTransform);
        }
    }
}
