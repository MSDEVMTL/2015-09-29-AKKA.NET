namespace Akka.Demo.Stashing
{
    public class StartMessage
    {
    }

    public class ByeMessage
    {
    }

    public class PasswordMessage
    {
        public PasswordMessage(string password)
        {
            this.Password = password;
        }

        public string Password { get; }
    }

    internal class AuthenticatedMessage
    {
    }

    internal class PushCommand
    {
        public PushCommand(string repo)
        {
            this.Repo = repo;
        }

        public string Repo { get; }
    }

    public class PullCommand
    {
        public PullCommand(string repo)
        {
            this.Repo = repo;
        }

        public string Repo { get; }
    }
}