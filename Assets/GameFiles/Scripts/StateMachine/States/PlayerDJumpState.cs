using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDJumpState : PlayerBaseState
{
    float _jumpPower = 10.1f;
    bool checkJump = false;

    public PlayerDJumpState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }

        if (_context.isHooked)
        {
            SwitchState(_factory.Grabbed());
        }

        if (checkJump == false && _context.jumpPressed == true)
        {
            checkJump = true;
        }
        else if (checkJump == true && _context.jumpPressed == false)
        {
            checkJump = false; 
        }

        if (_context.dashPressed)
        {
            SwitchState(_factory.AirDash());
        }
    }

    public override void EnterState()
    {
        _context.animationState = 3;

        if (_context.jumpPressed)
        {
            _context.velocity.y = _jumpPower;
        }
        checkJump = _context.jumpPressed;
    }

    public override void ExitState()
    {
        if (_context.jumpPressed)
        {
            _context.requireNewJumpPress = true;
        }
    }

    public override void UpdateState()
    {
        HandleGravity(5f, -20f);

        CheckSwitchStates();
    }

}
