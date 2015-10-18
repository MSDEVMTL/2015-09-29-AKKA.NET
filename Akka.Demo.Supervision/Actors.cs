using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Akka.Actor;

namespace Akka.Demo.Supervision
{
    public class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor()
        {
            this.Receive<ByeMessage>(x => this.PrintBye());
            this.Receive<SomeValueMessage>(x => this.PrintMessage(x.ToString()));            
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

    public class ConsoleReaderActor : UntypedActor
    {
        private readonly IActorRef _consoleWriter;
        private readonly IActorRef _supervisor;

        public ConsoleReaderActor(IActorRef consoleWriter, IActorRef supervisor)
        {
            this._consoleWriter = consoleWriter;
            this._supervisor = supervisor;
        }

        protected override void OnReceive(object message)
        {
            if (message is StartMessage)
            {
                var line = Console.ReadLine();

                if (line == "bye")
                {
                    _consoleWriter.Tell(new ByeMessage());
                }
                else if (line == "fail")
                {
                    _supervisor.Tell(new GetSomeValueMessage("fail"));
                }
                else if (line == "fail-fail-fail")
                {
                    _supervisor.Tell(new GetSomeValueMessage("fail"));
                    _supervisor.Tell(new GetSomeValueMessage("fail"));
                    _supervisor.Tell(new GetSomeValueMessage("fail"));
                    _supervisor.Tell(new GetSomeValueMessage("fail"));
                }
                else if (line == "halt")
                {
                    _supervisor.Tell(new GetSomeValueMessage("halt"));
                }
                else
                {
                    _supervisor.Tell(new GetSomeValueMessage(line));
                }

                this.Self.Tell(new StartMessage());
            }
        }
    }

    public class SupervisorActor :  ReceiveActor
    {
        private readonly IActorRef _consoleWriter;
        private IActorRef _characterActor;

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(3, TimeSpan.FromSeconds(5), e =>
            {
                if (e is FailingException)
                {
                    return Directive.Restart;
                }

                return Directive.Escalate;
            });
        }

        public SupervisorActor(IActorRef consoleWriter)
        {
            _characterActor = Context.ActorOf(Props.Create(() => new CharacterActor(consoleWriter)));

            Receive<GetSomeValueMessage>(x => GetSomeValue(x));
        }

        private void GetSomeValue(GetSomeValueMessage msg)
        {
            _characterActor.Tell(msg, _consoleWriter);
        }
    }

    /// <summary>
    /// Character actors are used to do specialized tasks that might fail such as network calls
    /// </summary>
    public class CharacterActor: UntypedActor
    {
        private readonly IActorRef _writer;

        private int _restartCount;

        public CharacterActor(IActorRef writer)
        {
            this._writer = writer;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as GetSomeValueMessage;
            if (msg != null)
            {
                if (msg.Value == "fail")
                {
                    throw new FailingException();
                }

                if (msg.Value == "halt")
                {
                    throw new HaltAndCatchFireException();
                }

                _writer.Tell(new SomeValueMessage(msg.Value, _restartCount));

                return;
            }

            var restartCountMessage = message as RestartCountMessage;
            if (restartCountMessage != null)
            {
                this._restartCount = restartCountMessage.Count;
            }
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(new RestartCountMessage(_restartCount + 1));
        }
    }

    public class HaltAndCatchFireException : Exception
    {
    }

    public class FailingException : Exception
    {
    }
}