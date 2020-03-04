namespace DotPacket
{
    public class NetContext
    {
        public int State { get; set; }

        public NetContext(int state)
        {
            State = state;
        }
    }
}