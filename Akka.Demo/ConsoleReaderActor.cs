using System;
using System.Threading.Tasks;

using Akka.Actor;

namespace Akka.Demo.HelloWorld
{
    public class ConsoleReaderActor : UntypedActor
    {
        private readonly IActorRef consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            this.consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message is MonitorConsole)
            {
                var line = Console.ReadLine();

                if (line == "bye")
                {
                    this.consoleWriterActor.Tell(new Bye());
                }
                else if (line == "count")
                {
                    var self = Self;
                    
                    Task<CountMessageResult> countTask = this.consoleWriterActor.Ask<CountMessageResult>(new CountMessage());
                    
                    countTask.PipeTo(self);
                }
                else
                {
                    this.consoleWriterActor.Tell(line);
                }

                this.Self.Tell(new MonitorConsole());
            }
            else if (message is CountMessageResult)
            {
                this.consoleWriterActor.Tell("Count is: " + message);
            }
        }
    }
}