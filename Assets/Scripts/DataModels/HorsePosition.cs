using ProtoBuf;
using UnityEngine;

namespace DataModels
{
    [ProtoContract]
    public class HorsePosition
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;
        
        public HorsePosition(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public HorsePosition()
        {
            X = 0;
            Y = 0;
        }
        
        public HorsePosition(Vector2Int position)
        {
            X = position.x;
            Y = position.y;
        }
    }
}