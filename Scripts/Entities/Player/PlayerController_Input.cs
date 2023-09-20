namespace Game.Entities;

public sealed partial class PlayerController
{
    private void HandleMotionInput()
    {
        Vector2 motionInput = Input.GetVector(
            negativeX: "left",
            positiveX: "right",
            negativeY: "up",
            positiveY: "down"
        );

        _smoothVelocity.X = Mathf.Lerp(_smoothVelocity.X, motionInput.X, 0.2f);
        _smoothVelocity.Y = Mathf.Lerp(_smoothVelocity.Y, motionInput.Y, 0.2f);

        // Update Visuals //
        if (_smoothVelocity.LengthSquared() <= 0.1f) {
            _sprite.Play("idle");
            return;
        }

        _sprite.Play("walk");
        _sprite.FlipH = _smoothVelocity.X < 0;
    }

    private void HandleActions()
    {
        if (Input.IsActionJustPressed("shoot")) {
            SetProcess(true);
        }
        else if (Input.IsActionJustReleased("shoot")) {
            SetProcess(false);
        }
    }

    private void FireProjectiles(float delta)
    {
        _shootCooldown = Mathf.Max(0.0f, _shootCooldown - delta);

        if (_shootCooldown > 0.0f)
            return;

        Vector2 position = GlobalPosition;

        _bulletManager.Spawn(position: position, Vector2.Left.Rotated(_shootAngle));
        _bulletManager.Spawn(position, Vector2.Right.Rotated(_shootAngle));
        _bulletManager.Spawn(position, Vector2.Up.Rotated(_shootAngle));
        _bulletManager.Spawn(position, Vector2.Down.Rotated(_shootAngle));

        _shootCooldown = ShootCooldown;
        _shootAngle = (_shootAngle + 25.0f) % 360f;
    }
}