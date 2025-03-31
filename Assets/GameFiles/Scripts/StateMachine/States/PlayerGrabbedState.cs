using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabbedState : PlayerBaseState
{
    private Vector3 prevPos;

    public PlayerGrabbedState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if(Vector3.Distance(_context.transform.position, _context.hookPoint) < 1f || _context.grabPressed == false)
        {
            if(_context.CharController.isGrounded)
            {
                SwitchState(_factory.Grounded());
            }
            else
            {
                SwitchState(_factory.Jump());
            }
        }
    }

    public override void EnterState()
    {
        _context.animationState = 6;
        _context.hookLine.enabled = true;
        prevPos = _context.transform.position;
    }

    public override void ExitState()
    {
        _context.isHooked = false;
        _context.hookLine.enabled = false;
    }

    public override void UpdateState()
    {
        Vector3 direction = (_context.hookPoint - _context.transform.position).normalized;
        Vector3 movement = direction * _context.currentSpeedMultiplier * 20f;

        float e = _context.transform.position.y - prevPos.y;
        _context.currentSpeedMultiplier += e * Time.deltaTime;

        _context.velocity = movement;

        prevPos = _context.transform.position;
        CheckSwitchStates();
    }

}
