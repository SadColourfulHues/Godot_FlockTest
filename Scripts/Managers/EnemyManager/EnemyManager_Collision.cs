namespace Game.Managers;

public sealed partial class EnemyManager
{
    public bool AppendHurtSpot(Vector2 hurtspot)
    {
        if (_hurtSpotIdx >= MaxHurtSpots)
            return false;

        _hurtSpots[_hurtSpotIdx] = hurtspot;
        _hurtSpotIdx ++;

        return true;
    }

    private void ClearHurtSpots()
    {
        for (int i = 0; i < MaxHurtSpots; ++i) {
            _hurtSpots[i] = default;
        }

        _hurtSpotIdx = 0;
    }

    private static Box GetBoxForEntity(Vector2 position)
    {
        return new(position.X, position.Y, 16, 14);
    }

    /// <summary>
    /// Returns 'true' if at least one entity is touching the target position.
    /// </summary>
    /// <returns></returns>
    public bool IsTouching(Vector2 position, out int firstHitIndex, float w = 16, float h = 14)
    {
        ReadOnlySpan<EnemyData> enemies = _enemies.GetData();
        Box targetBox = new(position.X, position.Y, w, h);

        for (int i = 0; i < enemies.Length; ++i) {
            Box enemyBox = GetBoxForEntity(enemies[i].Position);

            if (!targetBox.IsTouching(enemyBox))
                continue;

            firstHitIndex = i;
            return true;
        }

        firstHitIndex = -1;
        return false;
    }
}