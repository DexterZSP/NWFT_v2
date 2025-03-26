using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirDashState : PlayerBaseState
{
    private float DashPower = 42f;

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
        _context.animationState = 4;

        if (_context.dashPressed)
        {
            _context.velocity = _context.currentMovementInput * DashPower;
            _context.velocity.y = 6f;

            if (_context.currentSpeedMultiplier < 1.2f)
            { _context.currentSpeedMultiplier = 1.2f; }
        }
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        HandleGravity(4f, -20f);

        CheckSwitchStates();
    }

}
