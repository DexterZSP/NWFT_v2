using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{

    public PlayerSlideState(SC_PlayerStateMachine currentContext, SC_PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!_context.slidePressed || !_context.movementPressed)
        {
            SwitchState(_factory.Grounded());
        }
        else if (_context.CharController.isGrounded == false)
        {
            _context.velocity.y = 0;
            SwitchState(_factory.Jump());
        }
    }

    public override void EnterState()
    {
        _context.animationState = 1;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        Vector3 i = new Vector3(_context.currentMovementInput.x, -3, _context.currentMovementInput.z);

        RaycastHit _groundHit;
        if (Physics.Raycast(_context.transform.position, Vector3.down, out _groundHit, 1.5f))
        {
            Vector3 groundNormal = _groundHit.normal;
            float angle = Vector3.Angle(_context.transform.forward, groundNormal) - 90f;

            updateSpeedMultiplayer(3f, 0f, 1f, 0.5f);
        }

        _context.velocity = i * _context.baseSpeed * _context.currentSpeedMultiplier;
        
        CheckSwitchStates();
    }

}
