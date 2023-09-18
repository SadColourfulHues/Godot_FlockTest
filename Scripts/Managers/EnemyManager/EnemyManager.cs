namespace Game.Managers;

[GlobalClass]
public sealed partial class EnemyManager: Node2D
{
    private const int Capacity = 200;

    [Export]
    private SpriteFrames _enemySprite;

    [Export]
    private PackedScene _pkgShadowSprite;

    [Export]
    private Node2D _focalPoint;

    private readonly MultiNodeManager<AnimatedSprite2D, EnemyData> _enemies;

    public EnemyManager()
    {
        _enemies = new(Capacity);
    }

    public override void _PhysicsProcess(double delta)
    {
        ReadOnlySpan<AnimatedSprite2D> sprites = _enemies.GetNodes();
        ReadOnlySpan<EnemyData> enemies = _enemies.GetData();

        OnUpdate((float) delta, sprites, enemies);
    }
}