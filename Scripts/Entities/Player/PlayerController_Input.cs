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
}