namespace Game.Managers;

[GlobalClass]
public sealed partial class EnemyManager : Node2D
{
    [Signal]
    public delegate void OnEntityDeathEventHandler();
    public event Action<int> OnHurtzoneTouched;

    private const int Capacity = 512;
    private const int MaxHurtSpots = 24;

    [Export]
    private SpriteFrames _enemySprite;

    [Export]
    private SpriteFrames _enemySpriteAlt;

    [Export]
    private PackedScene _pkgShadowSprite;

    [Export]
    private Resource _visualSyncDelegate;

    [Export]
    private Node2D _focalPoint;

    private readonly Vector2[] _hurtSpots;
    private int _hurtSpotIdx;

    private readonly RandomNumberGenerator _randomGen;
    private readonly MultiNodeManager<AnimatedSprite2D, EnemyData> _enemies;
    private readonly AnimatedSpritePool _spritePool;

    public EnemyManager()
    {
        _randomGen = new();
        _enemies = new(Capacity);
        _spritePool = new(this, Capacity);

        _hurtSpots = new Vector2[MaxHurtSpots];
        _hurtSpotIdx = 0;

        _spritePool.SpriteConfigurator = OnSpriteInit;
    }

    public override void _PhysicsProcess(double delta)
    {
        ReadOnlySpan<AnimatedSprite2D> sprites = _enemies.GetNodes();
        ReadOnlySpan<EnemyData> enemies = _enemies.GetData();

        OnUpdate((float) delta, sprites, enemies);
    }
}