namespace Game.Core;

[AttributeUsage(AttributeTargets.Field)]
public sealed class GetNodeAttribute : Attribute
{
    public readonly string Path;

    public GetNodeAttribute(string nodePath)
    {
        Path = nodePath;
    }
}