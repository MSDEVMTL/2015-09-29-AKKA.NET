using System;

using Akka.Actor;

namespace Akka.Demo.Fsm
{
    public class ConsoleReaderActor : UntypedActor
    {
        private readonly IActorRef _consoleWriter;
        private readonly IActorRef _vcr;

        public ConsoleReaderActor(IActorRef consoleWriter, IActorRef vcr)
        {
            this._consoleWriter = consoleWriter;
            this._vcr = vcr;
        }

        protected override void OnReceive(object message)
        {
            if (message is StartMessage)
            {
                var line = Console.ReadLine();

                if (line == "bye")
                {
                    this._consoleWriter.Tell(new ByeMessage());
                }
                else if (line == "play")
                {
                    this._vcr.Tell(new PlayCommand());
                }

                else if (line == "stop")
                {
                    this._vcr.Tell(new StopCommand());
                }

                else if (line == "pause")
                {
                    this._vcr.Tell(new PauseCommand());
                }
                this.Self.Tell(new StartMessage());
            }
        }
    }

    public class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor()
        {
            this.Receive<ByeMessage>(x => this.PrintBye());
            this.Receive<string>(x => this.PrintMessage(x));
        }

        private void PrintMessage(string s)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ForegroundColor = consoleColor;
        }

        private void PrintBye()
        {
            Console.WriteLine("See you later");
            Context.System.Shutdown();
        }
    }

    internal class VcrActor : ReceiveActor
    {
        private readonly IActorRef _consoleWriterActor;

        public VcrActor(IActorRef consoleWriterActor)
        {
            this._consoleWriterActor = consoleWriterActor;
            this.Stopped();
            
        }

        private void Stopped()
        {
            this._consoleWriterActor.Tell("stopped");
            
            this.Receive<PlayCommand>(x =>
                {
                    this.Become(this.Playing);
                });
        }

        private void Playing()
        {
            this._consoleWriterActor.Tell("playing");
            
            this.Receive<StopCommand>(x =>
                {
                    this.Become(this.Stopped);
                });

            this.Receive<PauseCommand>(x =>
                {
                    this.Become(this.Paused);
                });
        }

        private void Paused()
        {
            this._consoleWriterActor.Tell("paused");

            this.Receive<PauseCommand>(x =>
                {
                    this.Become(this.Playing);
                });
            
            this.Receive<StopCommand>(x =>
                {
                    this.Become(this.Stopped);
                });

            this.Receive<PlayCommand>(x =>
                {
                    this.Become(this.Paused);
                });
        }
    }
}