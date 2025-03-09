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
        Debug.Log($"Switch State: {this} to {newState}");
        ExitState();

        newState.EnterState();

        _context.currentState = newState;
    }

}
