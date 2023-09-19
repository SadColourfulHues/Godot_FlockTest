namespace Game.Managers;

public sealed partial class EnemyManager
{
    public void TakeDamage(int idx, float damage)
    {
        EnemyData data = _enemies.GetDataAt(idx);
        TakeDamage(ref data, damage);

        Flash(_enemies.GetNodeAt(idx));

        _enemies.Update(data, idx);
    }

    private static void Flash(AnimatedSprite2D sprite)
    {
        Tween tween = sprite.CreateTween();
        sprite.Modulate = Colors.White;

        tween.TweenProperty(
            @object: sprite,
            property: CanvasItem.PropertyName.Modulate.ToString(),
            finalVal: Colors.Red,
            0.1f
        );

        tween.TweenProperty(
            @object: sprite,
            property: CanvasItem.PropertyName.Modulate.ToString(),
            finalVal: Colors.White,
            0.15f
        );
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