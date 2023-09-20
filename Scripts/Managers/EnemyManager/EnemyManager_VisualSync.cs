namespace Game.Managers;

public sealed partial class EnemyManager
{
    private void VisualSyncUpdateData(
        AnimatedSprite2D sprite,
        Vector2 position,
        bool flipped)
    {
        _visualSyncDelegate.Call(
            method: "update_data",
            sprite, position, flipped
        );
    }

    private void VisualSyncSpawn(
        AnimatedSprite2D sprite,
        SpriteFrames frames,
        Vector2 position)
    {
        _visualSyncDelegate.Call(
            method: "spawn",
            sprite, frames, position
        );
    }

    private void VisualSyncFlash(AnimatedSprite2D sprite)
    {
        _visualSyncDelegate.Call(
            method: "flash",
            sprite
        );
    }
}