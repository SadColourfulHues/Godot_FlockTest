using System.Diagnostics;

namespace Game.Core;

public abstract class BasePool<T>
    where T: class
{
    public readonly int Capacity;
    private readonly T[] _objects;

    protected BasePool(int capacity)
    {
        Capacity = capacity;
        _objects = new T[capacity];
    }

    protected abstract T PoolNewObject();
    protected abstract void PoolDestroyObject(T @object);

    protected abstract bool PoolIsObjectReusable(T @object);

    protected abstract void PoolWakeObject(T @object);
    protected abstract void PoolInvalidateObject(T @object);

    public T Get(out int objectIndex)
    {
        ReadOnlySpan<T> objects = _objects.AsSpan();
        int? lastOpenIndex = null;

        for (int i = 0; i < Capacity; ++i) {
            if (objects[i] is null) {
                lastOpenIndex ??= i;
                continue;
            }

            if (!PoolIsObjectReusable(objects[i]))
                continue;

            PoolWakeObject(objects[i]);

            objectIndex = i;
            return objects[i];
        }

        // Failure: Can't alloc new objects
        if (lastOpenIndex == null) {
            objectIndex = -1;
            return null;
        }

        objectIndex = lastOpenIndex.Value;

        T @object = PoolNewObject();
        _objects[objectIndex] = @object;

        return @object;
    }

    public T Get()
    {
        return Get(out _);
    }

    public void Invalidate(int index)
    {
        Debug.Assert(
            condition: index >= 0 && index < Capacity,
            message: "BasePool: attempted to invalidate an invalid index... how ironic."
        );

        if (_objects[index] is null)
            return;

        PoolInvalidateObject(_objects[index]);
    }

    public int GetIndexFor(T @object)
    {
        for (int i = 0; i < Capacity; ++i) {
            if (_objects[i] != @object)
                continue;

            return i;
        }

        return -1;
    }

    public void Reset()
    {
        for (int i = 0; i < Capacity; ++i) {
            PoolDestroyObject(_objects[i]);
            _objects[i] = null;
        }
    }
}