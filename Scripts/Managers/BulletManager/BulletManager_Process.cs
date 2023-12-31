namespace Game.Managers;

public sealed partial class BulletManager
{
    public void Spawn(Vector2 position, Vector2 dir)
    {
        if (!_bullets.CanAppend())
            return;

        Sprite2D sprite = _spritePool.Get(out int spriteIndex);

        BulletData data = new() {
            Age = 1.0f,
            Position = position,
            Direction = dir,

            SpriteIndex = spriteIndex
        };

        _bullets.Append(sprite, data);
        sprite.Position = data.Position;
    }

    private void OnUpdate(
        float delta,
        ReadOnlySpan<Sprite2D> sprites,
        ReadOnlySpan<BulletData> bullets)
    {
        // To prevent needless mass-intersection tests
        // The way this implementation works is that each bullet
        // Will register their position into the enemy manager's
        // 'hurt spot' zones, which is reset per frame.

        for (int i = sprites.Length; i --> 0;) {
            if (bullets[i].Age < 0.1f) {
                _spritePool.Invalidate(bullets[i].SpriteIndex);
                _bullets.Remove(i);

                continue;
            }

            BulletData nextData = bullets[i];

            nextData.Age -= delta;

            nextData.Position =
                bullets[i].Position + (bullets[i].Direction * 100.0f * delta);

            sprites[i].Position = nextData.Position;
            _enemyManager.AppendHurtSpot(nextData.Position);

            _bullets.Update(nextData, i);
        }
    }

    void OnSpriteInit(Sprite2D sprite)
    {
        sprite.Texture = _bulletSprite;
    }
}