using System.Threading.Channels;
//In depth discussion https://chat.openai.com/share/ae64de11-0311-4249-a758-46c91dffd1fe

YieldForProgressOfOtherMethods();

void YieldForProgressOfOtherMethods()
{
    var ch = Channel.CreateUnbounded<int>();

    var producer = Task.Run(() =>
    {
        for (int i = 0; i < 500_000; i++)
            ch.Writer.WriteAsync(i);

        ch.Writer.Complete();
    });

    Task.WaitAll(
        Read(1),
        Read(2),
        Read(3),
        producer);

    async Task Read(int id)
    {
        Console.WriteLine(id);

        //Try to comment/uncomment this
        await Task.Yield(); // to prevent blocking when the producer has already written elements
        // await Task.Delay(1); has the same effect
	
        await foreach (var x in ch.Reader.ReadAllAsync())
            Console.WriteLine($"{id}: {x}");
    }
}