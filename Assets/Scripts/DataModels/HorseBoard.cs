using System;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;
using HorseType = System.Int32;

namespace DataModels
{
    [ProtoContract]
    public class HorseBoard
    {
        [ProtoContract]
        public class BoardLine
        {
            [ProtoIgnore]
            public HorseType[] Line;

            [ProtoMember(1)]
            public int[] LineAsIntArray
            {
                set;
                get;
            }
            
            public BoardLine()
            {
                Line = new HorseType[16];
            }

            public HorseType this[int i]
            {
                get => Line[i];
                set => Line[i] = value;
            }
            
            public int Count() => Line.Length;
            
            public override string ToString()
            {
                return $"[{string.Join(", ", Line.Select(ht => ht.ToString()))}]";
            }
            
            [OnDeserialized]
            public void OnDeserialized()
            {
                Line = LineAsIntArray.Select(ht => (HorseType) ht).ToArray();
            }
            [OnSerializing]
            public void OnSerializing()
            {
                LineAsIntArray = Line.Select(ht => (int) ht).ToArray();
            }
        }
        
        [ProtoIgnore]
        public BoardLine[] Lines;
        
        [ProtoMember(1)]
        public int[] HorsesAsIntArray
        {
            set;
            get;
        }
        
        public HorseBoard()
        {
            Lines = new BoardLine[16];
            for (var i = 0; i < 16; i++)
            {
                Lines[i] = new BoardLine();
            }
        }

        public BoardLine this[int i]
        {
            get
            {
                if (i > Lines.Length || i < 0)
                {
                    throw new IndexOutOfRangeException("Index out of range");
                }

                return Lines[i] ?? (Lines[i] = new BoardLine());
            }
        }

        public HorseType this[int i, int j]
        {
            get => this[i][j];
            set => this[i][j] = value;   
        }
        public int Count() => Lines.Length;

        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("[\n");
            for (var i = 0; i < Lines.Length; i++)
            {
                stringBuilder.Append("    ");
                stringBuilder.Append(this[i]);
                if (i < Lines.Length - 1)
                {
                    stringBuilder.Append(",\n");
                }
            }
            stringBuilder.Append("\n]");
            return stringBuilder.ToString();
        }
        
        [OnDeserialized]
        public void OnDeserialized()
        {
            var lineCount = (int)Mathf.Sqrt(HorsesAsIntArray.Length);
            Lines = new BoardLine[lineCount];
            for (var i = 0; i < lineCount; i++)
            {
                Lines[i] = new BoardLine();
                for (var j = 0; j < lineCount; j++)
                {
                    Lines[i][j] = (HorseType) HorsesAsIntArray[i * lineCount + j];
                }
            }
        }
        [OnSerializing]
        public void OnSerializing()
        {
            HorsesAsIntArray = Lines.SelectMany(line =>
            {
                line.OnSerializing();
                return line.LineAsIntArray;
            }).ToArray();
        }
    }
}