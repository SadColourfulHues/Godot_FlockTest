namespace Game.Managers;

[GlobalClass]
public sealed partial class BulletManager : Node2D
{
    private const int Capacity = 32;

    [Export]
    private Texture2D _bulletSprite;

    [Export]
    private EnemyManager _enemyManager;

    private readonly MultiNodeManager<Sprite2D, BulletData> _bullets;
    private readonly SpritePool _spritePool;

    public BulletManager()
    {
        _bullets = new(Capacity);
        _spritePool = new(this, Capacity);

        _spritePool.SpriteConfigurator = OnSpriteInit;
    }

    public override void _Ready()
    {
        _enemyManager.OnHurtzoneTouched += OnBulletHit;
    }

    public override void _PhysicsProcess(double delta)
    {
        ReadOnlySpan<Sprite2D> sprites = _bullets.GetNodes();
        ReadOnlySpan<BulletData> bullets = _bullets.GetData();

        OnUpdate((float) delta, sprites, bullets);
    }

    private void OnBulletHit(int idx)
    {
        if (idx < 0 || idx >= _bullets.GetCount())
            return;

        // Kill the bullet in the next cycle
        BulletData nextData = _bullets.GetDataAt(idx);
        nextData.Age -= 0.1f;

        _bullets.Update(nextData, idx);
    }
}