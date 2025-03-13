using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{

    float _jumpPower = 11f;
    float _gravity = -20f;
    float _airSmoothness = 4f;

    public PlayerJumpState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
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

        if (_context.jumpPressed)
        {
            _context.velocity.y = _jumpPower;
        }
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
        _context.airMove = _context.velocity / (_context.baseSpeed * _context.currentSpeedMultiplier);

        _context.airMove = Vector3.Lerp(_context.airMove, _context.currentMovementInput, _airSmoothness * Time.deltaTime);
        _context.airMove *= (_context.baseSpeed * _context.currentSpeedMultiplier);
        _context.velocity = new Vector3(_context.airMove.x, _context.velocity.y, _context.airMove.z);
        _context.velocity.y += (_gravity * Time.deltaTime);

        CheckSwitchStates();
    }

}
