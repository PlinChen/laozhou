using System;
using ProtoBuf;

namespace DataModels
{
    [ProtoContract]
    public class HorseBoard
    {
        [ProtoContract]
        public class BoardLine
        {
            [ProtoMember(1)]
            public HorseType[] Line = new HorseType[16];
            
            public BoardLine()
            {
                for (var i = 0; i < 16; i++)
                {
                    Line[i] = HorseType.None;
                }
            }
        }
        
        [ProtoMember(1)]
        public BoardLine[] Board = new BoardLine[16];
        
        public HorseBoard()
        {
            for (var i = 0; i < 16; i++)
            {
                Board[i] = new BoardLine();
            }
        }
    }
}