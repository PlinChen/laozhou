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
        
        [ProtoIgnore]
        public int Row => Y;
        [ProtoIgnore]
        public int Column => X;
        
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
        
        public HorsePosition(HorsePosition other)
        {
            X = other.X;
            Y = other.Y;
        }
        
        public HorsePosition GetNextPosition(Vector2Int direction)
        {
            return new HorsePosition(X + direction.x, Y + direction.y);
        }
        
        public bool IsInBoard(int rowCount = 6, int columnCount = 6)
        {
            return X >= 0 && X < columnCount && Y >= 0 && Y < rowCount;
        }
        
        public bool IsAtBoarder(int rowCount = 6, int columnCount = 6)
        {
            return X == 0 || X == columnCount - 1 || Y == 0 || Y == rowCount - 1;
        }
    }
}