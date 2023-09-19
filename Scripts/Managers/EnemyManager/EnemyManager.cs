namespace Game.Managers;

[GlobalClass]
public sealed partial class EnemyManager: Node2D
{
    [Signal]
    public delegate void OnEntityDeathEventHandler();
    public event Action<int> OnHurtzoneTouched;

    private const int Capacity = 200;
    private const int MaxHurtSpots = 32;

    [Export]
    private SpriteFrames _enemySprite;

    [Export]
    private SpriteFrames _enemySpriteAlt;

    [Export]
    private PackedScene _pkgShadowSprite;

    [Export]
    private Node2D _focalPoint;

    private readonly Vector2[] _hurtSpots;
    private int _hurtSpotIdx;

    private readonly RandomNumberGenerator _randomGen;
    private readonly MultiNodeManager<AnimatedSprite2D, EnemyData> _enemies;

    public EnemyManager()
    {
        _randomGen = new();
        _enemies = new(Capacity);

        _hurtSpots = new Vector2[MaxHurtSpots];
        _hurtSpotIdx = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        ReadOnlySpan<AnimatedSprite2D> sprites = _enemies.GetNodes();
        ReadOnlySpan<EnemyData> enemies = _enemies.GetData();

        OnUpdate((float) delta, sprites, enemies);
    }
}