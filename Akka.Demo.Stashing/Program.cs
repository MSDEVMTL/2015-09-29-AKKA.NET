using Akka.Actor;
using Akka.Configuration;

namespace Akka.Demo.Stashing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString("akka.loglevel = DEBUG \n akka.actor.debug.lifecycle = off");

            var actorSystem = ActorSystem.Create("actor-system", config);

            var consoleWriterActor = actorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));

            var git = actorSystem.ActorOf(Props.Create(() => new GytActor(consoleWriterActor)));

            var authenticator = actorSystem.ActorOf(Props.Create(() => new Authenticator(git)));

            var consoleReaderActor =
                actorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor, git, authenticator)));

            consoleReaderActor.Tell(new StartMessage());

            actorSystem.AwaitTermination();
        }
    }
}