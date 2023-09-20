namespace Game.Core;

public sealed class SpritePool : BasePool<Sprite2D>
{
    public delegate void ConfigurationDelegate(Sprite2D sprite);

    public ConfigurationDelegate SpriteConfigurator;

    private readonly Node _owner;

    public SpritePool(Node owner, int capacity)
        : base(capacity)
    {
        _owner = owner;
    }

    protected override Sprite2D PoolNewObject()
    {
        Sprite2D sprite = new() {
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };

        SpriteConfigurator?.Invoke(sprite);

        _owner.AddChild(sprite);
        return sprite;
    }

    protected override void PoolDestroyObject(Sprite2D sprite)
    {
        sprite.QueueFree();
    }

    protected override bool PoolIsObjectReusable(Sprite2D sprite)
    {
        return !sprite.Visible;
    }

    protected override void PoolWakeObject(Sprite2D sprite)
    {
        sprite.Visible = true;
    }

    protected override void PoolInvalidateObject(Sprite2D sprite)
    {
        sprite.Visible = false;
    }
}