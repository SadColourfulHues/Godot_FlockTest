namespace Game.Objects;

using Game.Managers;

[GlobalClass]
public sealed partial class EnemySpawner : Marker2D
{
    [Export]
    private EnemyManager _manager;

    private Timer _spawnTimer;

    public override void _Ready()
    {
        CallDeferred(MethodName.Configure);
    }

    private void Configure()
    {
        _spawnTimer = new() {
            WaitTime = GD.RandRange(1.0f, 2.0f),
            OneShot = false
        };

        AddChild(_spawnTimer);

        _spawnTimer.Timeout += OnTimeout;
        _spawnTimer.Start();
    }

    private void OnTimeout()
    {
        _manager.Spawn(GlobalPosition);
    }
}