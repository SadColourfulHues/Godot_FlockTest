namespace Game.Managers;

public sealed partial class EnemyManager
{
    public void TakeDamage(int idx, float damage)
    {
        EnemyData data = _enemies.GetDataAt(idx);
        TakeDamage(ref data, damage);

        VisualSyncFlash(_enemies.GetNodeAt(idx));

        _enemies.Update(data, idx);
    }

    private async void DeathEffect(AnimatedSprite2D sprite, int index)
    {
        Tween outTween = sprite.CreateTween();

        outTween.TweenProperty(
            @object: sprite,
            property: Node2D.PropertyName.Scale.ToString(),
            finalVal: Vector2.Zero,
            duration: 0.33f
        );

        await ToSignal(outTween, Tween.SignalName.Finished);
        _spritePool.Invalidate(index);
    }

    private static void TakeDamage(ref EnemyData data, float damage)
    {
        data.Health -= damage;
    }

    private static void Knockback(
        ref EnemyData data,
        Vector2 awayPosition,
        float amount)
    {
        Vector2 dirAway = (data.Position - awayPosition) * amount;
        data.Forces += dirAway;
    }
}