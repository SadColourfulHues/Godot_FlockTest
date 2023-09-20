namespace Game.Entities;

public sealed partial class PlayerCamera : Node2D
{
    private static event Action<float> OnRequestShakeEvent;

    [Export]
    public Node2D _subject;

    [Export]
    private float _followFac = 0.2f;

    [Export]
    private FastNoiseLite _shakeNoise;

    private Vector2 _offset;

    private float _shakeFac;

    public override void _Ready()
    {
        _shakeFac = 0.0f;
        UpdateOffset();

        OnRequestShakeEvent += PerformShake;
    }

    public override void _Notification(int what)
    {
        if (what != GodotObject.NotificationPredelete)
            return;

        OnRequestShakeEvent -= PerformShake;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleShake();
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

    #region Shake

    public static void Shake(float intensity = 0.45f)
    {
        OnRequestShakeEvent?.Invoke(intensity);
    }

    private void HandleShake()
    {
        const float TargetAngle = 8f * Mathf.Pi / 180f;

        float noise = _shakeNoise.GetNoise1D(Time.GetTicksMsec() * 0.1f);

        float shakeTarget = noise * Mathf.Lerp(-1f, 1f, noise);
        shakeTarget = Mathf.LerpAngle(shakeTarget, 0.0f, _shakeFac);

        Rotation = Mathf.Lerp(Rotation, shakeTarget * TargetAngle, _shakeFac);
        _shakeFac = Mathf.Lerp(_shakeFac, 0.0f, 0.1f);
    }

    private void PerformShake(float intensity)
    {
        _shakeFac = Mathf.Min(1.0f, _shakeFac + intensity);
    }

    #endregion
}
