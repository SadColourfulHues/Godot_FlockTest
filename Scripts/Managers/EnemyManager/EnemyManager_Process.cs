using Game.Entities;

namespace Game.Managers;

public sealed partial class EnemyManager
{
    public void Spawn(Vector2 position)
    {
        if (!_enemies.CanAppend())
            return;

        AnimatedSprite2D sprite = _spritePool.Get(out int spriteIndex);

        if (sprite == null)
            return;

        // On a more serious project, I'll probably move these to a separate method
        // to make initialisation cleaner

        EnemyData data = new() {
            Position = position,
            Velocity = Vector2.Zero,
            SteerType = _randomGen.Randf() > 0.33f ? SteerType.Boring : SteerType.Bizarre,
            Health = 10.0f,

            SpriteIndex = spriteIndex
        };

        VisualSyncSpawn(
            sprite: sprite,
            frames: data.SteerType == SteerType.Boring ? _enemySprite : _enemySpriteAlt,
            position: position
        );

        _enemies.Append(sprite, data);
    }

    private void OnUpdate(
        float delta,
        ReadOnlySpan<AnimatedSprite2D> sprites,
        ReadOnlySpan<EnemyData> enemies)
    {
        ReadOnlySpan<Vector2> hurtSpots = _hurtSpots;
        Vector2 playerPosition = _focalPoint.Position;
        Box playerBox = new(playerPosition.X, playerPosition.Y, 14, 8);

        Span<AgentInfo> neighbourBuffer = stackalloc AgentInfo[sprites.Length];

        for (int i = sprites.Length; i --> 0;) {
            if (enemies[i].Health < 0.1f) {
                DeathEffect(sprites[i], enemies[i].SpriteIndex);
                _enemies.Remove(i);

                EmitSignal(SignalName.OnEntityDeath);
                continue;
            }

            // Main entity 'tick' //

            EnemyData nextData = enemies[i];
            Vector2 nextVelocity = enemies[i].Velocity;

            // Steering
            ReadOnlySpan<AgentInfo> neighbours = GetNeighbours(
                position: nextData.Position,
                enemies: enemies,
                buffer: neighbourBuffer,
                excludeIndex: i,
                count: out int neighbourCount
            );

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

            // Forces
            nextVelocity += nextData.Forces * delta;

            nextData.Position.X += nextVelocity.X * delta;
            nextData.Position.Y += nextVelocity.Y * 0.5f * delta;

            nextData.Forces = nextData.Forces.Lerp(Vector2.Zero, 0.12f);

            // Finalisation
            Box box = GetBoxForEntity(nextData.Position);

            VisualSyncUpdateData(
                sprite: sprites[i],
                position: nextData.Position,
                flipped: (playerPosition.X - enemies[i].Position.X) < 0.0f
            );

            // Interactions //

            ApplyDamageOnPlayerContact(
                entityBox: box,
                playerBox: playerBox,
                sprite: sprites[i],
                data: ref nextData
            );

            // Bullet collision
            for (int j = 0; j < _hurtSpotIdx; ++j) {
                Vector2 hurtSpotPosition = hurtSpots[j];
                Box hurtbox = new(hurtSpotPosition.X + 4, hurtSpotPosition.Y + 4, 8, 8);

                if (!hurtbox.IsTouching(box))
                    continue;

                Knockback(ref nextData, hurtSpotPosition, 80f);
                TakeDamage(ref nextData, 1f);
                VisualSyncFlash(sprites[i]);

                OnHurtzoneTouched?.Invoke(j);
                break;
            }

            _enemies.Update(nextData, i);
        }

        ClearHurtSpots();
    }

    private void OnSpriteInit(AnimatedSprite2D sprite)
    {
        Sprite2D shadow = _pkgShadowSprite.Instantiate<Sprite2D>();

        shadow.Position = new(0, 6);
        shadow.ShowBehindParent = true;

        sprite.AddChild(shadow);
    }

    private void ApplyDamageOnPlayerContact(
        Box entityBox,
        Box playerBox,
        AnimatedSprite2D sprite,
        ref EnemyData data)
    {
        if (!playerBox.IsTouching(entityBox))
            return;

        TakeDamage(ref data, 0.33f);
        Knockback(ref data, playerBox.GetPosition(), 300f);

        VisualSyncFlash(sprite);
        PlayerCamera.Shake(0.1f);
    }
}