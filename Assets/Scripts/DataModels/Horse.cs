using ProtoBuf;

namespace DataModels
{
    [ProtoContract]
    public class Horse
    {
        [ProtoMember(1)]
        public HorseType Type;
        [ProtoMember(2)]
        public HorsePosition Position;
    }
}