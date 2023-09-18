namespace Game.Core;

using System.Reflection;

public static class NodeExtensions
{
    /// <summary>
    /// Grab node references configured through the 'GetNode' attribute.
    /// </summary>
    /// <param name="node"></param>
    public static void GetNodes(this Node node)
    {
        Type type = node.GetType();

        ReadOnlySpan<FieldInfo> fields =
            type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .AsSpan();

        for (int i = 0; i < fields.Length; ++i)
        {
            GetNodeAttribute getterAttribute =
                fields[i].GetCustomAttribute<GetNodeAttribute>();

            if (getterAttribute is null)
                continue;

            node.Set(
                property: fields[i].Name,
                value: node.GetNodeOrNull(getterAttribute.Path)
            );
        }
    }
}