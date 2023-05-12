using System;
//using System.Reflection.Metadata.Ecma335;
using System.Net.Http;

Say("Top: Console program started");
Task<long> MyCpuTask = StartAsyncCpuBoundTask();
Say("Top: After StartAsyncCpuBoundTask()");
MyCpuTask.Wait();
Say(string.Format("Top: My CPU task result at top level: {0}", MyCpuTask.Result));

Say("Top: Before I/O test");
Task<string> MyIoTask = StartIoBoundTask();
Say("Top: After StartIoBoundTask()");
MyIoTask.Wait();
Say(string.Format("Top: My I/O result: {0}", MyIoTask.Result));
Say("Top: Console program ending");


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

async Task<string> StartIoBoundTask()
{
    Task<string> MyTask = new Task<string>(() => { return IoBoundFunction(); });
    MyTask.Start();
    TimeSpan MyTimeout = new TimeSpan((long)(10000000) * 100000); // (10,000,000 tick/sec)*(seconds)
    return await MyTask.WaitAsync(MyTimeout);
}

string IoBoundFunction()
{
    var client = new HttpClient();

    var request = new HttpRequestMessage(
        method: HttpMethod.Get,
        requestUri: "https://api.collection.cooperhewitt.org/rest/?method=cooperhewitt.search.objects&query=clock%20radio&page=1&per_page=1&access_token=876b3d9db8a40349a648501dbfa1bbe1"
    );

    HttpResponseMessage response = client.Send(request);

    if (response.IsSuccessStatusCode)
    {
        string responseContent = StreamAsString(response.Content.ReadAsStream());

        Say("IoBoundFunction(): Done reading.");
        return responseContent;
    }

    return "";
}

string StreamAsString(Stream StreamIn)
{
    List<byte> MyAccumBytes = new();
    byte[] MyInputBuffer = new byte[] { 0 };
    int NumBytesRead = StreamIn.Read(MyInputBuffer, 0, 1);
    while (NumBytesRead > 0)
    {
        byte CurByte = MyInputBuffer[0];
        MyAccumBytes.Add(CurByte);
        NumBytesRead = StreamIn.Read(MyInputBuffer, 0, 1);
    }
    return System.Text.Encoding.UTF8.GetString(MyAccumBytes.ToArray());
}

void Say(string MsgIn)
{
    Console.WriteLine("{0}: {1}", CurrentDateTimeString(), MsgIn);
}

string CurrentDateTimeString()
{
    return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
}