using System.Diagnostics;

namespace Game.Core;

public sealed partial class MultiNodeManager<NodeType, DataType>
    where NodeType: Node2D
    where DataType: struct
{
    public readonly int Capacity;

    private readonly NodeType[] _nodes;
    private readonly DataType[] _data;

    public MultiNodeManager(int capacity)
    {
        Capacity = capacity;
        _count = 0;

        _nodes = new NodeType[capacity];
        _data = new DataType[capacity];
    }

    #region Main Functions

    /// <summary>
    /// Try to append an object into the manager.
    /// </summary>
    /// <returns></returns>
    public bool Append(NodeType node, DataType data)
    {
        Debug.Assert(
            condition: node?.IsInsideTree() ?? false,
            message: "MultiNodeManager: attempted to append an invalid node."
        );

        if (!GetOpenIndex(out int index))
            return false;

        _nodes[index] = node;
        _data[index] = data;

        Organise();
        return true;
    }

    public void Remove(int index)
    {
        Debug.Assert(
            condition: index >= 0 && index < Capacity,
            message: "MultiNodeManager: attempted to remove an invalid index!"
        );

        _nodes[index] = null;
        _data[index] = default;

        Organise();
    }

    public void Update(DataType data, int index)
    {
        Debug.Assert(
            condition: index >= 0 && index < Capacity,
            message: "MultiNodeManager: attempted to update an invalid index!"
        );

        _data[index] = data;
    }

    #endregion

    #region Getters

    public ReadOnlySpan<NodeType> GetNodes()
    {
        return _nodes.AsSpan()[.._count];
    }

    public ReadOnlySpan<DataType> GetData()
    {
        return _data.AsSpan()[.._count];
    }

    #endregion
}
