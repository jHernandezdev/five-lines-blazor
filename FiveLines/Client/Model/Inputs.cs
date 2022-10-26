using System;
namespace FiveLines.Client.Model
{
	public interface Input
	{
        void Handle();
	}

    public class Right : Input
    {
        public Right(Action<int> action)
        {
            this.action = action;
        }
        private readonly Action<int> action;


        public void Handle()
        {
            action(1);
        }
    }

    public class Left : Input
    {
        public Left(Action<int> action)
        {
            this.action = action;
        }
        private readonly Action<int> action;


        public void Handle()
        {
            action(-1);
        }
    }

    public class Up : Input
    {
        public Up(Action<int> action)
        {
            this.action = action;
        }
        private readonly Action<int> action;


        public void Handle()
        {
            action(-1);
        }
    }

    public class Down : Input
    {
        public Down(Action<int> action)
        {
            this.action = action;
        }
        private readonly Action<int> action;


        public void Handle()
        {
            action(1);
        }
    }
}

