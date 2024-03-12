using ProtoBuf;

namespace DataModels
{
    public enum HorseType
    {
        [ProtoEnum(Name = @"None", Value = 0)]
        None,
        [ProtoEnum(Name = @"Stone", Value = 1)]
        Stone,
        [ProtoEnum(Name = @"Stick", Value = 2)]
        Stick,
        [ProtoEnum(Name = @"StoneCover", Value = 3)]
        StoneCover,
        [ProtoEnum(Name = @"StickCover", Value = 4)]
        StickCover,
    }
}