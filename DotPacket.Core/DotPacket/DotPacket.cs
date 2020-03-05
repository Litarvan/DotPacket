namespace DotPacket
{
    public class DotPacket
    {
        public const string Version = "1.0.0";
        public const uint DefaultBufferSize = 1024;

        public static ConnectionContext DefaultContextFactory(NetworkConnection connection, NetContext global)
        {
            return new ConnectionContext(global, connection, 0);
        }
    }

    public delegate ConnectionContext ContextFactory(NetworkConnection connection, NetContext global);
}