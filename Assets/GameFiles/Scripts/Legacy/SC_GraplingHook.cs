using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class SC_GraplingHook : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _grabLayer;
    [SerializeField] private float _grabForce = 5f;

    private bool _isGrabing;
    private RaycastHit _hit;
    private Vector3 _grabMovement;


    void Start()
    {
        if (_lineRenderer == null)
        { _lineRenderer = gameObject.GetComponent<LineRenderer>(); }
        
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, 80f, _grabLayer))
            {
                _lineRenderer.enabled = true;
                _isGrabing = true;
                _lineRenderer.SetPosition(1, _hit.point);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _lineRenderer.enabled = false;
            _isGrabing = false;
            _grabMovement = Vector3.Lerp(_grabMovement, Vector3.zero, Time.deltaTime * 5f);
        }

        if (_isGrabing)
        {
            _lineRenderer.SetPosition(0, transform.position);
            _grabMovement = (_hit.point - transform.position).normalized * _grabForce;
        }
        else
        {
            _grabMovement = Vector3.zero;
        }
    }
}
