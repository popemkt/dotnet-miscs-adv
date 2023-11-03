namespace ReferenceCounting;

public sealed class KeyedLock<T> where T : notnull
{
    private sealed class RefCounted<TRefCounted>
    {
        public RefCounted(TRefCounted value)
        {
            RefCount = 1;
            Value = value;
        }

        public int RefCount { get; set; }
        public TRefCounted Value { get; private set; }
    }

    private static readonly Dictionary<T, RefCounted<SemaphoreSlim>> SemaphoreSlims
        = new();

    private SemaphoreSlim GetOrCreate(T key)
    {
        RefCounted<SemaphoreSlim> item;
        lock (SemaphoreSlims)
        {
            if (SemaphoreSlims.TryGetValue(key, out item))
            {
                ++item.RefCount;
            }
            else
            {
                item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                SemaphoreSlims[key] = item;
            }
        }

        return item.Value;
    }

    public IDisposable Lock(T key)
    {
        GetOrCreate(key).Wait();
        return new Releaser<T>(key, SemaphoreSlims);
    }

    public async Task<IDisposable> LockAsync(T key)
    {
        await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
        return new Releaser<T>(key, SemaphoreSlims);
    }

    private sealed class Releaser<TReleaser> : IDisposable where TReleaser : notnull
    {
        private readonly TReleaser _key;
        private readonly Dictionary<TReleaser, RefCounted<SemaphoreSlim>> _semaphoreSlims;

        public Releaser(TReleaser key, Dictionary<TReleaser, RefCounted<SemaphoreSlim>> semaphoreSlims)
        {
            _key = key;
            _semaphoreSlims = semaphoreSlims;
        }


        public void Dispose()
        {
            RefCounted<SemaphoreSlim> item;
            lock (_semaphoreSlims)
            {
                item = _semaphoreSlims[_key];
                --item.RefCount;
                if (item.RefCount == 0)
                    _semaphoreSlims.Remove(_key);
            }

            item.Value.Release();
        }
    }
}