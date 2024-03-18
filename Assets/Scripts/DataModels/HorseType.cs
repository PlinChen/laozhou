using ProtoBuf;

namespace DataModels
{
    [ProtoContract]
    public enum HorseType
    {
        // [ProtoEnum(Name = @"None", Value = 0)]
        [ProtoEnum]
        None = 0, // 0b0000
        // [ProtoEnum(Name = @"Stone", Value = 1)]
        [ProtoEnum]
        Stone = 1, // 0b0001
        // [ProtoEnum(Name = @"Stick", Value = 2)]
        [ProtoEnum]
        Stick = 3, // 0b0011
        // [ProtoEnum(Name = @"StoneCover", Value = 3)]
        [ProtoEnum]
        StoneCover = 5, // 0b0101
        // [ProtoEnum(Name = @"StickCover", Value = 4)]
        [ProtoEnum]
        StickCover = 7 // 0b0111
    }
    
    public static class HorseTypeExtension
    {
        public const int Stick = 1 << 1;
        public const int Cover = 1 << 2;
        public static bool IsCover(this HorseType horseType)
        {
            return ((int) horseType & Cover) != 0;
        }
        
        public static bool IsStick(this HorseType horseType)
        {
            return ((int) horseType & Stick) != 0;
        }
        
        public static bool IsStone(this HorseType horseType)
        {
            return horseType != HorseType.None && !IsStick(horseType);
        }
        
        public static HorseType ToCover(this HorseType horseType)
        {
            return (HorseType) ((int) horseType | Cover);
        }
    }
}