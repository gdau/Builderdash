using System;

namespace Builderdash
{
    public class CallbackImpl : IDataOutputCallback
    {
        public void SendDataPacket(string data)
        {
            Console.WriteLine("Server sent: {0}", data);
        }

    }
}