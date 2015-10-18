namespace Akka.Demo
{
    public class CountMessage
    {
    }

    public class CountMessageResult
    {
        public CountMessageResult(int count)
        {
            this.Count = count;
        }

        public int Count { get; }
    }


    public class MonitorConsole
    {
    }

    public class Bye
    {
    }
}