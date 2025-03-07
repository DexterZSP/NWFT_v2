using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        isRootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
    }

    public override void EnterState()
    {
        HandleJump();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    void HandleJump()
    {

    }
}
