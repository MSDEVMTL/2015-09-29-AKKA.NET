using Akka.Actor;
using Akka.Configuration;

namespace Akka.Demo.Fsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString("akka.loglevel = DEBUG \n akka.actor.debug.lifecycle = on");

            var actorSystem = ActorSystem.Create("actor-system", config);

            var consoleWriterActor =
                actorSystem.ActorOf(Props.Create<ConsoleWriterActor>(() => new ConsoleWriterActor()));

            var supervisorActor = actorSystem.ActorOf(Props.Create(() => new VcrActor(consoleWriterActor)));

            var consoleReaderActor =
                   actorSystem.ActorOf(Props.Create<ConsoleReaderActor>(() => new ConsoleReaderActor(consoleWriterActor, supervisorActor)));

            consoleReaderActor.Tell(new StartMessage());

            actorSystem.AwaitTermination();
        }
    }
}
