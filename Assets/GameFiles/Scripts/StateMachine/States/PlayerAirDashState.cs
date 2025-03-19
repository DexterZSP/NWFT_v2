using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirDashState : PlayerBaseState
{

    public PlayerAirDashState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    public override void EnterState()
    {
        _context.animationState = 2;
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
        HandleGravity(4f, -20f);

        CheckSwitchStates();
    }

}
