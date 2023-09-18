namespace Game.Entities;

public sealed partial class PlayerCamera : Node2D
{
    [Export]
    public Node2D _subject;

    [Export]
    private float _followFac = 0.2f;

    private Vector2 _offset;

    public override void _Ready()
    {
        UpdateOffset();
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdatePosition(_followFac);
    }

    #region Smooth Follow

    public void UpdateOffset()
    {
        _offset = _subject.Position - Position;
    }

    public void UpdatePosition(float fac)
    {
        Vector2 targetPosition = _subject.Position + _offset;
        Position += (targetPosition - Position) * fac;
    }

    #endregion
}
