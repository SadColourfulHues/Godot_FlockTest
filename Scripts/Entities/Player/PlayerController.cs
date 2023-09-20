using Game.Managers;

namespace Game.Entities;

public sealed partial class PlayerController : CharacterBody2D
{
    private const float ShootCooldown = 0.1f;

    [Export]
    private float _moveSpeed = 8.0f;

    [Export]
    private BulletManager _bulletManager;

    [GetNode("Body")]
    AnimatedSprite2D _sprite;

    private Vector2 _smoothVelocity;
    private float _shootCooldown;
    private float _shootAngle;

    public override void _Ready()
    {
        this.GetNodes();
        SetProcess(false);

        _shootCooldown = 0.0f;
        _shootAngle = 0.0f;

        _sprite.Animation = "idle";
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey &&
            @event is not InputEventJoypadButton &&
            @event is not InputEventMouseButton)
        {
            return;
        }

        HandleActions();
    }

    public override void _Process(double delta)
    {
        FireProjectiles((float) delta);
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