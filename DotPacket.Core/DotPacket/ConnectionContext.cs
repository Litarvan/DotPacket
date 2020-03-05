namespace DotPacket
{
    public class ConnectionContext
    {
        public NetContext GlobalContext { get; }
        public NetworkConnection Connection { get; }
        public int State { get; set; }

        public ConnectionContext(NetContext globalContext, NetworkConnection connection, int defaultState)
        {
            GlobalContext = globalContext;
            Connection = connection;
            State = defaultState;
        }
    }
}