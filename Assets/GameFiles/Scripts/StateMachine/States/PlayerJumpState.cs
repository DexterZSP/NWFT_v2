using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    float _jumpPower = 11f;
    bool checkJump = false;
    float airSmoothness = 6f;

    public PlayerJumpState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_context.CharController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }

        if (checkJump == false && _context.jumpPressed == true)
        {
            RaycastHit hit;
            // Detecta una pared frente al personaje
            if (Physics.Raycast(_context.transform.position, _context.transform.forward, out hit, 1f, _context.wallLayer))
            {
                Vector3 jumpDirection = Vector3.up * 10 - _context.transform.forward * 6;
                _context.velocity = jumpDirection;
                airSmoothness = 0.8f;
            }
            else 
            { SwitchState(_factory.DJump()); }
            
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
        _context.animationState = 2;

        if (_context.jumpPressed)
        {
            _context.velocity.y = _jumpPower;
        }
        checkJump = _context.jumpPressed;
        airSmoothness = 6f;
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
        HandleGravity(airSmoothness, -20f);

        CheckSwitchStates();
    }

}
