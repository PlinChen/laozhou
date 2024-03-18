using ProtoBuf;

namespace DataModels
{
    [ProtoContract]
    public class HorsePositionArray
    {
        [ProtoMember(1)]
        public HorsePosition[] Positions;
    }
}