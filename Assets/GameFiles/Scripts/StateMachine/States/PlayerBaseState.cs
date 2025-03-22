using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
// Start is called before the first frame update
{

    protected SC_PlayerStateMachine _context;
    protected SC_PlayerStateFactory _factory;

    public PlayerBaseState(SC_PlayerStateMachine context, SC_PlayerStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();


    protected void SwitchState(PlayerBaseState newState) 
    {
        //Debug.Log($"Switch State: {this} to {newState}");
        ExitState();

        newState.EnterState();

        _context.currentState = newState;
    }

    protected void updateSpeedMultiplayer(float acceleration, float deceleration, float uphillFactor, float downhillFactor)
    {
        RaycastHit _groundHit;

        if (_context.movementPressed)
        {
            if (Physics.Raycast(_context.transform.position, Vector3.down, out _groundHit, 1.5f))
            {
                Vector3 groundNormal = _groundHit.normal;
                float angle = Vector3.Angle(_context.transform.forward, groundNormal) - 90f;

                if (angle > 0)
                {
                    _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, _context.minSpeedMultiplier, angle / 45.0f * uphillFactor * Time.deltaTime);
                }
                else if (angle < 0)
                {
                    _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, _context.maxSpeedMultiplier, -angle / 45.0f * downhillFactor * Time.deltaTime);
                }
                else
                {
                    float e = _context.currentSpeedMultiplier > 1f ? deceleration : acceleration;
                    _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, 1f, e * Time.deltaTime);
                }
            }
        }
        else
        { _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, 1f, deceleration * 5 * Time.deltaTime); }
    }

    protected void HandleGravity(float airSmoothness, float gravity)
    {
        _context.airMove = _context.velocity / (_context.baseSpeed * _context.currentSpeedMultiplier);

        _context.airMove = Vector3.Lerp(_context.airMove, _context.currentMovementInput, airSmoothness * Time.deltaTime);
        _context.airMove *= (_context.baseSpeed * _context.currentSpeedMultiplier);
        _context.velocity = new Vector3(_context.airMove.x, _context.velocity.y, _context.airMove.z);
        _context.velocity.y += (gravity * Time.deltaTime);

        if(_context.currentMovementInput.magnitude <= 0.1)
        { _context.currentSpeedMultiplier = 1; }
    }

}
