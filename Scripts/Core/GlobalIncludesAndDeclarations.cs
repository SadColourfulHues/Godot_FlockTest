global using Godot;
global using System;

global using Game.Core;
global using Game.Core.Types;

namespace Game.Core.Types;

public ref struct Box
{
    public float x;
    public float y;

    public float w;
    public float h;

    public Box(float x, float y, float w, float h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    public readonly Vector2 GetPosition()
        => new(x, y);

    public readonly float GetRight()
        => this.x + this.w;

    public readonly float GetBottom()
        => this.y + this.h;

    public readonly bool IsTouching(Box other)
    {
        return !(
            other.x > GetRight() ||
            other.y > GetBottom() ||
            other.GetRight() < x ||
            other.GetBottom() < y
        );
    }
}