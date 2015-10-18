using System;

using Akka.Actor;

using NUnit.Framework;

using Xunit;

namespace Akka.Demo.Fsm
{
    public class VcrTests : TestKit.Xunit2.TestKit
    {
        private readonly IActorRef vcr;

        public VcrTests()
            : base()
        {
            var testActor = this.TestActor;
            this.vcr = this.Sys.ActorOf(Props.Create(() => new VcrActor(testActor)), "vcr");
        }

        [Fact]
        public void Given_stopped_When_play_tells_playing()
        {
            vcr.Tell(new PlayCommand());

            ExpectMsg<string>("stopped");
            ExpectMsg<string>("playing");
        }

        [Fact]
        public void Expect_a_message()
        {
            TestActor.Tell("Test");
            ExpectMsg("stopped");
            ExpectMsg("Test");
        }
    }
}