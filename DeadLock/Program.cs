// See https://aka.ms/new-console-template for more information

using ReferenceCounting;

KeyedLock<string> @lock = new();
Parallel.Invoke(
    () =>
        Parallel.Invoke(Thread1, Thread2),
    () =>
        Parallel.Invoke(() => UsingKeyedLock("first", @lock, "first", "second"),
            () => UsingKeyedLock("second", @lock, "second", "first"))
);

void UsingKeyedLock(string threadName, KeyedLock<string> keyedLock, string firstKey, string secondKey)
{
    using (keyedLock.Lock(firstKey))
    {
        Console.WriteLine($"thread {threadName} got {firstKey} lock");
        Thread.Sleep(1000);
        using (keyedLock.Lock(secondKey))
        {
            Console.WriteLine($"thread {threadName} got both locks");
        }
    }
}

void Thread1()
{
    lock (typeof(int))
    {
        Console.WriteLine("thread 1 got int lock");
        Thread.Sleep(1000);
        lock (typeof(long))
            Console.WriteLine("thread 1 got both locks");
        // do something
    }
}

void Thread2()
{
    lock (typeof(long))
    {
        Console.WriteLine("thread 2 got long lock");
        Thread.Sleep(1000);
        lock (typeof(int))
            Console.WriteLine("thread 2 got both locks"); // do something
    }
}