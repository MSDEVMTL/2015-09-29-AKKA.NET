using System;
using System.Threading.Tasks;

using Akka.Actor;

namespace Akka.Demo.HelloWorld
{
    public class ConsoleWriterActor : ReceiveActor
    {
        private int _count;

        public ConsoleWriterActor()
        {
            this.Receive<Bye>(x => this.PrintBye());
            this.Receive<string>(x => this.PrintMessage(x));
            this.Receive<CountMessage>(x => this.ReplyWithCount());
        }

        private void ReplyWithCount()
        {
            this.Sender.Tell(new CountMessageResult( this._count));
        }

        private void PrintMessage(string s)
        {
            this._count++;
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ForegroundColor = consoleColor;
        }

        private void PrintBye()
        {
            Console.WriteLine("See you later :)");
            Context.System.Shutdown();
        }
    }
}