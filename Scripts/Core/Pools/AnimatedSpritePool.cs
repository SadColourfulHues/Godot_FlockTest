namespace Game.Core;

public sealed class AnimatedSpritePool : BasePool<AnimatedSprite2D>
{
    public delegate void ConfigurationDelegate(AnimatedSprite2D sprite);

    public ConfigurationDelegate SpriteConfigurator;

    private readonly Node _owner;

    public AnimatedSpritePool(Node owner, int capacity)
        : base(capacity)
    {
        _owner = owner;
    }

    protected override AnimatedSprite2D PoolNewObject()
    {
        AnimatedSprite2D sprite = new() {
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };

        SpriteConfigurator?.Invoke(sprite);

        _owner.AddChild(sprite);
        return sprite;
    }

    protected override void PoolDestroyObject(AnimatedSprite2D sprite)
    {
        sprite.QueueFree();
    }

    protected override bool PoolIsObjectReusable(AnimatedSprite2D sprite)
    {
        return !sprite.Visible;
    }

    protected override void PoolWakeObject(AnimatedSprite2D sprite)
    {
        sprite.Visible = true;
        sprite.Scale = Vector2.One;
    }

    protected override void PoolInvalidateObject(AnimatedSprite2D sprite)
    {
        sprite.Visible = false;
    }
}