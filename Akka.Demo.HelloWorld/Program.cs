//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

namespace Akka.Demo.HelloWorld
{

    using System;
    using Akka.Actor;

    namespace HelloAkka
    {
        class Program
        {
            static void Main(string[] args)
            {
                // create a new actor system (a container for actors)
                var system = ActorSystem.Create("MySystem");

                // create actor and get a reference to it.
                // this will be an "ActorRef", which is not a 
                // reference to the actual actor instance
                // but rather a client or proxy to it
                IActorRef greeter = system.ActorOf<GreetingActor>("greeter");

                // send a message to the actor
                greeter.Tell(new Greet("World"));

                // prevent the application from exiting before message is handled
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Immutable message type that actor will respond to
        /// </summary>
        public class Greet
        {
            public string Who { get; }

            public Greet(string who)
            {
                Who = who;
            }
        }

        /// <summary>
        /// The actor class
        /// </summary>
        public class GreetingActor : ReceiveActor
        {
            public GreetingActor()
            {
                // Tell the actor to respond to the Greet message
                Receive<Greet>(greet => Console.WriteLine("Hello {0}", greet.Who));
            }
        }

    }


}
