using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{

    public PlayerGroundedState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded == false || _context.jumpPressed && _context.requireNewJumpPress == false)
        {
            _context.velocity.y = 0;
            SwitchState(_factory.Jump());
        }
        else if (_context.slidePressed) 
        {
            SwitchState(_factory.Slide());
        }
    }

    public override void EnterState()
    {
        _context.animationState = 0;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
        Vector3 i = new Vector3(_context.currentMovementInput.x, -3, _context.currentMovementInput.z);

        float e = _context.movementPressed ? 0 : 2;
        updateSpeedMultiplayer(3f, e, 0.4f, 0.05f);

        if (_context.movementPressed)
        {
            _context.velocity = i * _context.baseSpeed * _context.currentSpeedMultiplier;
        }
        else
        {

            if (Vector3.Distance(_context.velocity, i) > 0.1f)
            {
                _context.velocity = Vector3.Lerp(_context.velocity, i, Time.deltaTime * 3f);
            }
            else
            {
                _context.velocity = i;
            }
        }


        CheckSwitchStates();
    }

}
