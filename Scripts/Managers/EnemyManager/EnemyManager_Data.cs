namespace Game.Managers;

public sealed partial class EnemyManager
{
    private struct EnemyData
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Health;

        public SteerType SteerType;
        public Vector2 Forces;

        public int SpriteIndex;
    }

    private struct AgentInfo
    {
        public Vector2 Position;
        public Vector2 Velocity;
    }

    private enum SteerType
    {
        Boring,
        Bizarre
    }
}