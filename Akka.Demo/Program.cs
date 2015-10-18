using System.Diagnostics;

using Akka.Actor;
using Akka.Demo.HelloWorld;
using Akka.Event;

namespace Akka.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("actor-system");

            var consoleWriterActor =
                actorSystem.ActorOf(Props.Create<ConsoleWriterActor>(() => new ConsoleWriterActor()));

            var consoleReaderActor =
                actorSystem.ActorOf(Props.Create<ConsoleReaderActor>(() => new ConsoleReaderActor(consoleWriterActor)));

            consoleReaderActor.Tell(new MonitorConsole());

            actorSystem.AwaitTermination();
        }
    }
}
