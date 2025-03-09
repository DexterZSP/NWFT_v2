using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    float acceletation = 3f;

    public PlayerGroundedState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded == false || _context.jumpPressed && _context.requireNewJumpPress == false)
        {
            SwitchState(_factory.Jump());
        }
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {
        _context.velocity.y = 0;
    }

    public override void UpdateState()
    {
        Vector3 i = new Vector3(_context.currentMovementInput.x, -3, _context.currentMovementInput.z);

        RaycastHit _groundHit;
        if (Physics.Raycast(_context.transform.position, Vector3.down, out _groundHit, 1.5f))
        {
            Vector3 groundNormal = _groundHit.normal;
            float angle = Vector3.Angle(_context.transform.forward, groundNormal) - 90f;

            if (angle > 0)
            {
                _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, _context.minSpeedMultiplier, angle / 45.0f * Time.deltaTime);
            }
            else
            {
                _context.currentSpeedMultiplier = Mathf.Lerp(_context.currentSpeedMultiplier, 1.0f, acceletation * Time.deltaTime);
            }
        }

        if (_context.movementPressed)
        {
            _context.velocity = i * _context.baseSpeed * _context.currentSpeedMultiplier;
        }
        else
        {

            if (Vector3.Distance(_context.velocity, i) > 0.1f)
            {
                _context.velocity = Vector3.Lerp(_context.velocity, i, Time.deltaTime * acceletation);
            }
            else
            {
                _context.velocity = i;
            }
        }


        CheckSwitchStates();
    }

}
