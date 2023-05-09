using System;
using System.Reflection.Metadata.Ecma335;

Say("Console program started");
Task<long> MyCpuTask = StartAsyncCpuBoundTask();
Say("After StartAsyncCpuBoundTask()");
MyCpuTask.Wait();
Say(string.Format("My CPU task number at top level: {0}", MyCpuTask.Result));


async Task<long> StartAsyncCpuBoundTask()
{
    Task<long> MyTask = new Task<long>(() => { return LongRunningCpuBoundFunction(); } );
    MyTask.Start();
    TimeSpan MyTimeout = new TimeSpan((long)(10000000) * 100000); // (10,000,000 tick/sec)*(seconds)
    return await MyTask.WaitAsync(MyTimeout);
}

long LongRunningCpuBoundFunction()
{
    int OuterLoopCount = 100000;
    int InnerLoopCount = 100000;
    long MyNum = 0;
    for (int i=0; i<OuterLoopCount; i++)
    {
        for (int j = 0; j < InnerLoopCount; j++)
        {
            MyNum++;
        }
    }
    Say("LongRunningCpuBoundFunction(): Done");
    return MyNum;
}

void Say(string MsgIn)
{
    Console.WriteLine("{0}: {1}", CurrentDateTimeString(), MsgIn);
}

string CurrentDateTimeString()
{
    return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
}