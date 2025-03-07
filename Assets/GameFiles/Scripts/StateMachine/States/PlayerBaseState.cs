using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
// Start is called before the first frame update
{
    protected bool isRootState = false;

    protected SC_PlayerStateMachine _context;
    protected SC_PlayerStateFactory _factory;

    protected PlayerBaseState _subState;
    protected PlayerBaseState _superstate;

    public PlayerBaseState(SC_PlayerStateMachine context, SC_PlayerStateFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();

        if (_subState != null)
        {
            _subState.UpdateState();
        }
    }
    protected void SwitchState(PlayerBaseState newState) 
    {
        ExitState();

        newState.EnterState();

        if (isRootState)
        {
            _context.CurrentState = newState;
        }
        else if (_superstate != null)
        {
            _superstate.SetSubState(newState);
        }
    }
    protected void SetSuperState(PlayerBaseState newSuperState) 
    {
        _superstate = newSuperState;
    }
    protected void SetSubState(PlayerBaseState newSubState) 
    {
        _subState = newSubState;
        newSubState.SetSuperState(this);
    }
}
