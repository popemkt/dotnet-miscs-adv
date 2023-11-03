// See https://aka.ms/new-console-template for more information
Parallel.Invoke(Thread1, Thread2);

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