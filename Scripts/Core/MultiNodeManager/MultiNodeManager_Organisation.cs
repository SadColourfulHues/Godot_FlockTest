using System.Diagnostics;

namespace Game.Core;

public sealed partial class MultiNodeManager<NodeType, DataType>
{
    private int _count;

    /// <summary>
    /// Moves null entries to the back.
    /// </summary>
    public void Organise()
    {
        _count = Capacity;

        for (int i = 0; i < _nodes.Length; ++i) {
            if (IsContinuous<NodeType>(_nodes, out int count)) {
                _count = count;
                return;
            }

            for (int j = _nodes.Length; j --> i;) {
                if (_nodes[j] is null)
                    continue;

                NodeType tmpName = _nodes[j];
                DataType tmpKey = _data[j];

                _nodes[j] = _nodes[i];
                _nodes[i] = tmpName;

                _data[j] = _data[i];
                _data[i] = tmpKey;
            }
        }
    }

    /// <summary>
    /// Frees references and reverts data to its default state
    /// </summary>
    public void Clear()
    {
        _count = 0;

        for (int i = 0; i < Capacity; ++i) {
            _nodes[i]?.QueueFree();

            _nodes[i] = null;
            _data[i] = default;
        }
    }

    private bool GetOpenIndex(out int index)
    {
        ReadOnlySpan<NodeType> nodes = _nodes.AsSpan();

        for (int i = 0; i < nodes.Length; ++i) {
            if (nodes[i] is not null)
                continue;

            index = i;
            return true;
        }

        index = -1;
        return false;
    }

    private bool IsContinuous<T>(
        ReadOnlySpan<T> array,
        out int switchPoint)
    {
        switchPoint = Capacity;

        bool expected = false;
        bool enforcePattern = false;

        for (int i = 0; i < array.Length; ++i) {
            bool isNull = array[i] is null;

            if (isNull == expected)
                continue;

            if (enforcePattern)
                return false;

            expected = isNull;
            enforcePattern = true;

            switchPoint = i;
        }

        return true;
    }
}