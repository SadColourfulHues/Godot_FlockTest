namespace Game.Managers;

public sealed partial class EnemyManager
{
    public void Spawn(Vector2 position)
    {
        EnemyData data = new() {
            Position = position,
            Velocity = Vector2.Zero,
            Health = 10.0f
        };

        AnimatedSprite2D sprite = new() {
            SpriteFrames = _enemySprite
        };

        Sprite2D shadow = _pkgShadowSprite.Instantiate<Sprite2D>();
        sprite.AddChild(shadow);

        shadow.ShowBehindParent = true;
        shadow.Position = new(0, 6);

        AddChild(sprite);

        if (!_enemies.Append(sprite, data)) {
            sprite.QueueFree();
            return;
        }

        sprite.GlobalPosition = position;
        sprite.Play();
    }

    private void OnUpdate(
        float delta,
        ReadOnlySpan<AnimatedSprite2D> sprites,
        ReadOnlySpan<EnemyData> enemies)
    {
        Vector2 playerPosition = _focalPoint.GlobalPosition;
        Span<AgentInfo> neighbourBuffer = stackalloc AgentInfo[sprites.Length];

        for (int i = 0; i < sprites.Length; ++i) {
            if (enemies[i].Health <= 1f) {
                sprites[i].QueueFree();
                _enemies.Remove(i);

                continue;
            }

            // Main process
            EnemyData nextData = enemies[i];

            ReadOnlySpan<AgentInfo> neighbours = GetNeighbours(
                position: nextData.Position,
                enemies: enemies,
                buffer: neighbourBuffer,
                excludeIndex: i,
                count: out int neighbourCount
            );

            Vector2 nextVelocity = enemies[i].Velocity;

            ProcessFlocking(
                velocity: ref nextVelocity,
                position: nextData.Position,
                targetPosition: playerPosition,
                neighbours: neighbours,
                neighbourCount: neighbourCount,
                acceleration: 8.5f,
                maxVelocity: 20.0f
            );

            nextData.Position.X += nextVelocity.X * delta;
            nextData.Position.Y += nextVelocity.Y * 0.5f * delta;

            nextData.Velocity = nextVelocity;

            // Finalise update
            sprites[i].FlipH = (playerPosition.X - enemies[i].Position.X) < 0.0f;
            sprites[i].GlobalPosition = nextData.Position;

            _enemies.Update(nextData, i);
        }
    }
}