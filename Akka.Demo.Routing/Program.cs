using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Akka.Util.Internal;

namespace Akka.Demo.Routing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
 //           var config = ConfigurationFactory.ParseString(@"
 //               akka.loglevel = DEBUG
 //               akka.actor.debug.lifecycle = off");

            var config = ConfigurationFactory.ParseString(@"
                akka.loglevel = DEBUG 
                akka.actor.debug.lifecycle = off
                akka.actor.deployment 
                {
                    /router1 
                    {
                        router = smallest-mailbox-pool
                        nr-of-instances = 3
                    }
                }
");

//            var config = ConfigurationFactory.ParseString(@"
//                akka.loglevel = DEBUG 
//                akka.actor.debug.lifecycle = off
//                akka.actor.deployment 
//                {
//                    /router1 
//                    {
//                        router = smallest-mailbox-pool
//                        nr-of-instances = 3
//                    }
//                }
//");

            var actorSystem = ActorSystem.Create("routing-demo", config);

            var counter = new AtomicCounter();
            counter.IncrementAndGet();

            var router =
                actorSystem.ActorOf(
                    Props.Create(
                        () => new RouteeActor(() => counter.IncrementAndGet()))
                        .WithRouter(FromConfig.Instance), "router1");

            //            var router =
            //                actorSystem.ActorOf(
            //                    Props.Create<RouteeActor>(
            //                        () => new RouteeActor(() => counter.IncrementAndGet()))
            //                        .WithRouter(FromConfig.Instance), "router1");

            for (int i = 0; i < 10; i++)
            {
                router.Tell(i);
            }

            Console.ReadLine();
        }
    }

    public class RouteeActor : UntypedActor
    {
        private readonly int _instance;

        private int _counter;

        public RouteeActor(Func<int> wait)
        {
            this._instance = wait();
        }

        protected override void OnReceive(object message)
        {
            if (message is int)
            {
                Thread.Sleep(TimeSpan.FromSeconds(this._instance));
                _counter++;
                Console.ForegroundColor = (ConsoleColor)this._instance + 5 ;
                Console.WriteLine("routee " + this._instance + " message:  " + message + " handled: " + _counter);
            }
        }
    }
}
