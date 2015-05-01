namespace PacketIn
{
    public abstract class NetSnapshot
    {
        NetSnapshot PreviousSnapshot { get; set; }

        int SnapshotId { get; set; }
        
    }
}
