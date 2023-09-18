namespace Game.Entities;

public sealed partial class PlayerController : CharacterBody2D
{
    [Export]
    private float _moveSpeed = 8.0f;

    [GetNode("Body")]
    AnimatedSprite2D _sprite;

    private Vector2 _smoothVelocity;

    public override void _Ready()
    {
        this.GetNodes();
        _sprite.Animation = "idle";
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMotionInput();

        // Adapt to isometric's 2:1 ratio
        Vector2 nextVelocity = _smoothVelocity;
        nextVelocity *= _moveSpeed;
        nextVelocity *= _moveSpeed * 0.5f;

        Velocity = nextVelocity;
        MoveAndSlide();
    }
}