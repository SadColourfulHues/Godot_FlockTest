namespace Game.Managers;

[GlobalClass]
public sealed partial class EnemyManager: Node2D
{
    [Signal]
    public delegate void OnEntityDeathEventHandler();

    private const int Capacity = 200;

    [Export]
    private SpriteFrames _enemySprite;

    [Export]
    private SpriteFrames _enemySpriteAlt;

    [Export]
    private PackedScene _pkgShadowSprite;

    [Export]
    private Node2D _focalPoint;

    private readonly RandomNumberGenerator _randomGen;
    private readonly MultiNodeManager<AnimatedSprite2D, EnemyData> _enemies;

    public EnemyManager()
    {
        _randomGen = new();
        _enemies = new(Capacity);
    }

    public override void _Process(double delta)
    {
        Vector2 playerPosition = _focalPoint.GlobalPosition;

        if (!IsTouching(playerPosition, out int touchIdx))
            return;

        // Damage and push back entities upon colliding with the player

        if (touchIdx == -1)
            return;

        EnemyData data = _enemies.GetDataAt(touchIdx);

        TakeDamage(ref data, 1f);
        Knockback(ref data, playerPosition, 500f);

        Flash(_enemies.GetNodeAt(touchIdx));
        _enemies.Update(data, touchIdx);
    }

    public override void _PhysicsProcess(double delta)
    {
        ReadOnlySpan<AnimatedSprite2D> sprites = _enemies.GetNodes();
        ReadOnlySpan<EnemyData> enemies = _enemies.GetData();

        OnUpdate((float) delta, sprites, enemies);
    }
}