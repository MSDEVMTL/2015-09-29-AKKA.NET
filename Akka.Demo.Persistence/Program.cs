using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Dispatch.SysMsg;

namespace Akka.Demo.Persistence
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n--- BASIC EXAMPLE ---\n");

            var system = ActorSystem.Create("system");

            // create a persistent actor, using LocalSnapshotStore and MemoryJournal
            var aref = system.ActorOf(Props.Create<ExamplePersistentActor>(), "basic-actor");

            // all commands are stacked in internal actor's state as a list
            aref.Tell(new Command("foo"));
            aref.Tell(new Command("baz"));
            aref.Tell(new Command("bar"));

            // save current actor state using LocalSnapshotStore (it will be serialized and stored inside file on example bin/snapshots folder)
            aref.Tell("snap");

            // add one more message, this one is not snapshoted and won't be persisted (because of MemoryJournal characteristics)
            aref.Tell(new Command("buzz"));

            // print current actor state
            aref.Tell("print");

            Console.ReadLine();

            aref.Tell(PoisonPill.Instance);

            aref = system.ActorOf(Props.Create<ExamplePersistentActor>(), "basic-actor-2");

            aref.Tell("print");

            Console.ReadLine();
        }
    }
}
