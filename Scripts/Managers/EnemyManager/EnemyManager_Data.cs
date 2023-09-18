namespace Game.Managers;

public sealed partial class EnemyManager
{
    private struct EnemyData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Health;
    }

    private struct AgentInfo
    {
        public Vector2 Position;
        public Vector2 Velocity;
    }
}