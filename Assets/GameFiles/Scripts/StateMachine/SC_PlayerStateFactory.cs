public class SC_PlayerStateFactory
{
    SC_PlayerStateMachine _context;

    public SC_PlayerStateFactory(SC_PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Grounded() 
    { return new PlayerGroundedState(_context, this); }
    public PlayerBaseState Idle() 
    { return new PlayerIdleState(_context, this); }
    public PlayerBaseState Jump() 
    { return new PlayerJumpState(_context, this); }
    public PlayerBaseState Run() 
    { return new PlayerRunState(_context, this); }
    public PlayerBaseState Slide() 
    { return new PlayerSlideState(_context, this); }
}
