namespace Akka.Demo.Supervision
{
    public class SomeValueMessage {
        
        public string Value { get; private set; }
        public int RestartCount { get; private set; }

        public SomeValueMessage(string value, int restartCount)
        {
            RestartCount = restartCount;
            Value = value;
        }

        public override string ToString()
        {
            return "Value: " + Value + " Restarted: " + RestartCount;
        }
    }

    public class RestartCountMessage
    {
        public int Count { get; private set; }

        public RestartCountMessage(int count)
        {
            Count = count;
        }
    }

    public class ByeMessage
    {
    }

    public class StartMessage
    {
    }

    public class GetSomeValueMessage {
        public GetSomeValueMessage(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}