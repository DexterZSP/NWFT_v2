using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) 
    { 
        isRootState = true; 
        InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
       if(_context.IsJumpPressed == true)
        {
            SwitchState(_factory.Jump());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Grounded");
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

}
