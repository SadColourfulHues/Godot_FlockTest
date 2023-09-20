namespace Game.Managers;

public sealed partial class EnemyManager
{
    private static Vector2 GetSteeringForce(
        Vector2 current,
        Vector2 target,
        float acceleration,
        float maxVelocity)
    {
        Vector2 steer = (target - current).Normalized() * acceleration;
		return (target + steer).LimitLength(maxVelocity);
    }

	private static void ProcessFlocking(
		ref Vector2 velocity,
		Vector2 position,
        Vector2 targetPosition,
		ReadOnlySpan<AgentInfo> neighbours,
        int neighbourCount,
        SteerType steerType,
		float acceleration,
		float maxVelocity)
	{
        // If it has no neighbours, move towards the player, regardless of their movement type
		if (neighbourCount < 1) {
            velocity = GetSteeringForce(
                current: velocity,
                target: (targetPosition - position).Normalized() * maxVelocity,
                acceleration: acceleration,
                maxVelocity: maxVelocity
            );

			return;
        }

        Vector2 averageVelocity = Vector2.Zero;
		Vector2 averagePosition = Vector2.Zero;
        Vector2 separation = Vector2.Zero;

        // Collect averaged agent info from its neighbours
		for (int i = 0; i < neighbourCount; ++ i) {
            float distanceMod = position.DistanceTo(neighbours[i].Position);
            distanceMod = 1.0f - Mathf.Clamp(distanceMod / 64.0f, 0.1f, 1.225f);

            separation += (position - neighbours[i].Position).Normalized() * 8.0f * distanceMod;
			averagePosition += neighbours[i].Position;
            averageVelocity += neighbours[i].Velocity;
		}

        float normalise = 1.0f / neighbourCount;

        averageVelocity *= normalise;
		averagePosition *= normalise;
        separation *= normalise;

        // Calculate updated direction
        Vector2 flockingDir = (averagePosition - position).Normalized();
        flockingDir += averageVelocity *= 0.2f;
        flockingDir *= velocity.Length();

        targetPosition += (position - targetPosition).Normalized() * 8.0f;
        Vector2 targetDir = (targetPosition - position).Normalized();

        if (steerType == SteerType.Bizarre) {
            targetDir = targetDir.Rotated(45f);
            targetDir *= maxVelocity * 2.0f;
        }
        else {
            targetDir *= maxVelocity;
        }

        targetDir += separation;

        // Gradually move towards the updated direction
		velocity = GetSteeringForce(
			current: flockingDir,
			target: targetDir,
			acceleration: acceleration,
			maxVelocity: maxVelocity
		);
	}

    /// <summary>
    /// (buffer) is going to be shared per physics_frame, and is expected to be pre-allocated
    /// </summary>
    private static ReadOnlySpan<AgentInfo> GetNeighbours(
        Vector2 position,
        ReadOnlySpan<EnemyData> enemies,
        Span<AgentInfo> buffer,
        int excludeIndex,
        out int count)
    {
        const int MaxNeighbours = 32;

        const float DistanceThreshold = 40.0f;
        const float MinDistance = DistanceThreshold * DistanceThreshold;

        int neighbourIdx = 0;
        int neighbourUpperLimit = Math.Min(buffer.Length, MaxNeighbours);

        for (int i = 0; i < buffer.Length; ++i) {
            if (neighbourIdx >= neighbourUpperLimit)
                break;

            if (i == excludeIndex)
                continue;

            Vector2 otherPosition = enemies[i].Position;

            if (position.DistanceSquaredTo(otherPosition) > MinDistance)
                continue;

            buffer[neighbourIdx].Position = otherPosition;
            buffer[neighbourIdx].Velocity = enemies[i].Velocity;

            neighbourIdx ++;
        }

        count = neighbourIdx;
        return buffer[..neighbourIdx];
    }
}