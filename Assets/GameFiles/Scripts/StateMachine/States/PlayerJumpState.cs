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

        if (_context.isHooked)
        {
            SwitchState(_factory.Grabbed());
        }

        if (checkJump == false && _context.jumpPressed == true)
        {
            
            if (_context.bumpingIntoWall)
            {
                _context.audioPlayer.PlaySound(SoundEffect.WallJump);
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
            _context.audioPlayer.PlaySound(SoundEffect.Jump);
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
        float g =  _context.bumpingIntoWall ? -10f : -20f;

        HandleGravity(airSmoothness, g);

        CheckSwitchStates();
    }

}
