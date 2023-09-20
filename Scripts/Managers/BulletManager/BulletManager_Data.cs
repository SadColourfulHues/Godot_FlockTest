namespace Game.Managers;

public sealed partial class BulletManager
{
    private struct BulletData
    {
        public float Age;
        public Vector2 Position;
        public Vector2 Direction;

        public int SpriteIndex;
    }
}