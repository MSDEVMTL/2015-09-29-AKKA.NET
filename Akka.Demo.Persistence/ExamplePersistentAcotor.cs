//-----------------------------------------------------------------------
// <copyright file="ExamplePersistentActor.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Akka.Persistence;

namespace Akka.Demo.Persistence
{
    public class Command
    {
        public Command(string data)
        {
            this.Data = data;
        }

        public string Data { get; private set; }

        public override string ToString()
        {
            return this.Data;
        }
    }

    public class Event
    {
        public Event(string data)
        {
            this.Data = data;
        }

        public string Data { get; private set; }

        public override string ToString()
        {
            return this.Data;
        }
    }

    public class ExampleState
    {
        public ExampleState(List<string> events = null)
        {
            this.Events = events ?? new List<string>();
        }

        public IEnumerable<string> Events { get; private set; }

        public ExampleState Update(Event evt)
        {
            var list = new List<string> { evt.Data };
            list.AddRange(this.Events);
            return new ExampleState(list);
        }

        public override string ToString()
        {
            return string.Join(", ", this.Events);
        }
    }

    public class ExamplePersistentActor : PersistentActor
    {
        public ExamplePersistentActor()
        {
            this.State = new ExampleState();
        }

        public override string PersistenceId { get { return "sample-id-1"; } }

        public ExampleState State { get; set; }
        public int EventsCount { get { return this.State.Events.Count(); } }

        public void UpdateState(Event evt)
        {
            this.State = this.State.Update(evt);
        }

        protected override bool ReceiveRecover(object message)
        {
            ExampleState state;
            if (message is Event)
                this.UpdateState(message as Event);
            else if (message is SnapshotOffer && (state = ((SnapshotOffer)message).Snapshot as ExampleState) != null)
                this.State = state;
            else return false;
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message is Command)
            {
                var cmd = message as Command;
                this.Persist(new Event(cmd.Data + "-" + this.EventsCount), this.UpdateState);
            }
            else if (message as string == "snap")
                this.SaveSnapshot(this.State);
            else if (message as string == "print")
                Console.WriteLine(this.State);
            else return false;
            return true;
        }
    }
}

