namespace Game.Managers;

public sealed partial class EnemyManager
{
    public void Spawn(Vector2 position)
    {
        if (!_enemies.CanAppend())
            return;

        // On a more serious project, I'll probably move these to a separate method
        // to make initialisation cleaner

        EnemyData data = new() {
            Position = position,
            Velocity = Vector2.Zero,
            SteerType = _randomGen.Randf() > 0.33f ? SteerType.Boring : SteerType.Bizarre,
            Health = 10.0f
        };

        AnimatedSprite2D sprite = new() {
            SpriteFrames = data.SteerType == SteerType.Boring ? _enemySprite : _enemySpriteAlt,
            TextureFilter = CanvasItem.TextureFilterEnum.NearestWithMipmaps
        };

        Sprite2D shadow = _pkgShadowSprite.Instantiate<Sprite2D>();
        shadow.TextureFilter = CanvasItem.TextureFilterEnum.NearestWithMipmaps;

        sprite.AddChild(shadow);

        shadow.ShowBehindParent = true;
        shadow.Position = new(0, 6);

        AddChild(sprite);
        _enemies.Append(sprite, data);

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

                EmitSignal(SignalName.OnEntityDeath);
                return;
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
                steerType: nextData.SteerType,
                acceleration: 8.5f,
                maxVelocity: 20.0f
            );

            nextData.Velocity = nextVelocity;

            // Apply forces
            nextVelocity += nextData.Forces * delta;

            nextData.Position.X += nextVelocity.X * delta;
            nextData.Position.Y += nextVelocity.Y * 0.5f * delta;

            nextData.Forces = nextData.Forces.Lerp(Vector2.Zero, 0.12f);

            // Finalise update
            sprites[i].FlipH = (playerPosition.X - enemies[i].Position.X) < 0.0f;
            sprites[i].GlobalPosition = nextData.Position;

            _enemies.Update(nextData, i);
        }
    }
}