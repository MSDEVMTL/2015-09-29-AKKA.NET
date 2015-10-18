using System;

using Akka.Actor;

namespace Akka.Demo.Stashing
{
    public class GytActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly IActorRef _consoleWriterActor;

        public GytActor(IActorRef consoleWriterActor)
        {
            this._consoleWriterActor = consoleWriterActor;

            // Start by being unuthenticated
            this.Unauthenticated();
        }

        public IStash Stash { get; set; }

        private void Unauthenticated()
        {
            this._consoleWriterActor.Tell("Not authenticated please send command password.");
            this.Receive<AuthenticatedMessage>(
                x =>
                    {
                        // Once we have been authenticated, we can start handling all the stashed messages
                        this.Become(this.Authenticated);
                    });

            this.Receive<object>(
                x =>
                    {
                        // Any message other then Authenticated should be stashed
                        this.Stash.Stash();
                    });
        }

        private void Authenticated()
        {
            //Now that we are authenticated we can start handlind commands
            this.Receive<PullCommand>(
                x => this._consoleWriterActor.Tell("pulling " + x.Repo));

            this.Receive<PushCommand>(
                x => this._consoleWriterActor.Tell("pushing " + x.Repo));
        }
    }

    public class Authenticator : ReceiveActor
    {
        private readonly IActorRef securedResource;

        public Authenticator(IActorRef securedResource)
        {
            this.securedResource = securedResource;

            this.Receive<PasswordMessage>(
                x =>
                    {
                        if (x.Password == "monkey")
                        {
                            this.securedResource.Tell(new AuthenticatedMessage());
                        }
                    });
        }
    }

    public class ConsoleReaderActor : UntypedActor
    {
        private readonly IActorRef _consoleWriter;

        private readonly IActorRef _authenticator;

        private readonly IActorRef _git;

        public ConsoleReaderActor(IActorRef consoleWriter, IActorRef git, IActorRef authenticator)
        {
            this._consoleWriter = consoleWriter;
            this._git = git;
            this._authenticator = authenticator;
        }

        protected override void OnReceive(object message)
        {
            if (message is StartMessage)
            {
                var line = Console.ReadLine();

                var token = line.Split();
                var command = token[0];
                var arg = token[1];

                if (command == "bye")
                {
                    this._consoleWriter.Tell(new ByeMessage());
                }
                else if (command == "pull")
                {
                    this._git.Tell(new PullCommand(arg));
                }
                else if (command == "push")
                {
                    this._git.Tell(new PushCommand(arg));
                }
                else if (command == "password")
                {
                    this._authenticator.Tell(new PasswordMessage(arg));
                }

                // Listen to next console message
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
}