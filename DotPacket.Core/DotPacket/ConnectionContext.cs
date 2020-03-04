namespace DotPacket
{
    public class ConnectionContext
    {
        public NetContext GlobalContext { get; }
        public NetworkConnection Connection { get; }

        public int State
        {
            get => GlobalContext.State;
            set => GlobalContext.State = value;
        }

        public ConnectionContext(NetContext globalContext, NetworkConnection connection)
        {
            GlobalContext = globalContext;
            Connection = connection;
        }
    }
}