using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Events;
using HorseType = System.Int32;

namespace DataModels
{
    [ProtoContract]
    public class HorseBoard
    {
        [ProtoIgnore]
        public HorseType[][] Horses;
        
        [ProtoMember(1)]
        public byte[] HorsesAsIntArray
        {
            set;
            get;
        }

        public HorseBoard()
        {
            Horses = new HorseType[16][];
            for (var i = 0; i < 16; i++)
            {
                Horses[i] = new HorseType[16];
            }
        }

        public HorseType this[int i, int j]
        {
            get
            {
                if (i > Horses.Length || i < 0 || j > Horses[i].Length || j < 0)
                {
                    throw new IndexOutOfRangeException("Index out of range");
                }

                return Horses[i][j];
            }
            set
            {
                if (i > Horses.Length || i < 0 || j > Horses[i].Length || j < 0)
                {
                    throw new IndexOutOfRangeException("Index out of range");
                }
                Horses[i][j] = value;
            }
        }

        [ProtoIgnore]
        public int RowCount => Horses.Length;
        [ProtoIgnore]
        public int ColumnCount => Horses == null ? 0 : Horses[0].Length;
        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("[\n");
            for (var i = 0; i < Horses.Length; i++)
            {
                stringBuilder.Append("    ");
                stringBuilder.Append(string.Join(", ", Horses[i]));
                if (i < Horses[i].Length - 1)
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
            if (Horses == null || Horses.Length != lineCount)
            {
                Horses = new HorseType[lineCount][];
            }
            
            for (var i = 0; i < lineCount; i++)
            {
                if (Horses[i] == null || Horses[i].Length != lineCount)
                {
                    Horses[i] = new HorseType[lineCount];
                }
                for (var j = 0; j < lineCount; j++)
                {
                    Horses[i][j] = (HorseType) HorsesAsIntArray[i * lineCount + j];
                }
            }
        }
        [OnSerializing]
        public void OnSerializing()
        {
            HorsesAsIntArray = Horses.SelectMany(ht => ht).Select(ht => (byte) ht).ToArray();
        }
    }


    public static class HorseBoardExtensions
    {
        public static void CheckStatus(this HorseBoard horseBoard, HorsePosition position, UnityAction<GottyType> callback)
        {
            var gotyType = GottyType.None;
            gotyType = horseBoard.CheckLaoZhou(position);
            if (gotyType == GottyType.Laozhou)
            {
                callback?.Invoke(GottyType.Laozhou);
                return;
            }
            gotyType = horseBoard.CheckStraightHorses(position);
            if (gotyType != GottyType.None)
            {
                if (gotyType == GottyType.FiveStraightHorses)
                {
                    // check existing gotty of the same type at the same position
                }
                if (gotyType == GottyType.QuadStraightHorses)
                {
                    // check existing gotty of the same type at the same position
                }
                callback?.Invoke( gotyType);
            }
            gotyType = horseBoard.CheckRect(position);
            callback?.Invoke(gotyType);
        }
        
        /// <summary>
        /// Check if the horse at the position make it a LaoZhou
        /// </summary>
        /// <param name="horseBoard"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static GottyType CheckLaoZhou(this HorseBoard horseBoard, HorsePosition position)
        {
            var currentHorse = horseBoard[position.X, position.Y];
            if (currentHorse == HorseType.None || currentHorse.IsCover())
            {
                return GottyType.None;
            }

            var directions = new[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(1, 1),
            };
            
            var laozhouList = new List<List<HorsePosition>>();

            foreach (var direction in directions)
            {
                var linkedPositions = new List<HorsePosition> {new(position)};
                linkedPositions.AddRange(horseBoard.GetLinkedPositions(position, direction));
                linkedPositions.AddRange(horseBoard.GetLinkedPositions(position, -direction));
                if (linkedPositions.Count == 6)
                {
                    laozhouList.Add(linkedPositions);
                }
            }
            
            if (!laozhouList.Any())
            {
                return GottyType.None;
            }

            var hasSideLaozhou = false;
            foreach (var laozhou in laozhouList)
            {
                laozhou.Sort(HorsePositionComparison);
                var first = laozhou.First();
                var last = laozhou.Last();
                if (first.X == last.X)
                {
                    if (first.X == 0 || first.X == horseBoard.ColumnCount - 1)
                    {
                        hasSideLaozhou = true;
                    }
                }
                else if (first.Y == last.Y)
                {
                    if (first.Y == 0 || first.Y == horseBoard.RowCount - 1)
                    {
                        hasSideLaozhou = true;
                    }
                }
            }
            if (!hasSideLaozhou || laozhouList.Count > 1)
            {
                return GottyType.Laozhou;
            }
            return GottyType.SideLaozhou;
        }
        
        private static GottyType CheckStraightHorses(this HorseBoard horseBoard, HorsePosition position)
        {
            var currentHorse = horseBoard[position.X, position.Y];
            if (currentHorse == HorseType.None || currentHorse.IsCover())
            {
                return GottyType.None;
            }

            var directions = new[]
            {
                new Vector2Int(1, -1),
                new Vector2Int(1, 1),
            };
            foreach (var direction in directions)
            {
                var linkedPositions = new List<HorsePosition> {new(position)};
                linkedPositions.AddRange(horseBoard.GetLinkedPositions(position, direction));
                linkedPositions.AddRange(horseBoard.GetLinkedPositions(position, -direction));
                if (linkedPositions.Count > 2)
                {
                    linkedPositions.Sort(HorsePositionComparison);
                    var first = linkedPositions.First();
                    var last = linkedPositions.Last();
                    if (first.IsAtBoarder() && last.IsAtBoarder())
                    {
                        if (linkedPositions.Count == 5)
                        {
                            return GottyType.FiveStraightHorses;
                        }
                        if (linkedPositions.Count == 4)
                        {
                            return GottyType.QuadStraightHorses;
                        }
                        if (linkedPositions.Count == 3)
                        {
                            return GottyType.TriStraightHorses;
                        }
                    }
                }
            }

            return GottyType.None;
        }
        
        private static GottyType CheckRect(this HorseBoard horseBoard, HorsePosition position)
        {
            var currentHorse = horseBoard[position.X, position.Y];
            if (currentHorse == HorseType.None || currentHorse.IsCover())
            {
                return GottyType.None;
            }

            var directions = new[]
            {
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1),
            };
            foreach (var direction in directions)
            {
                var xSidePosition = position.GetNextPosition(new Vector2Int(direction.x, 0));
                if (!xSidePosition.IsInBoard()) continue;
                if (horseBoard[xSidePosition.X, xSidePosition.Y] != currentHorse) continue;
                
                var ySidePosition = position.GetNextPosition(new Vector2Int(0, direction.y));
                if (!ySidePosition.IsInBoard()) continue;
                if (horseBoard[ySidePosition.X, ySidePosition.Y] != currentHorse) continue;
                
                var xySidePosition = position.GetNextPosition(direction);
                if (!xySidePosition.IsInBoard()) continue;
                if (horseBoard[xySidePosition.X, xySidePosition.Y] != currentHorse) continue;

                return GottyType.Rect;
            }
            return GottyType.None;
        }

        private static List<HorsePosition> GetLinkedPositions(this HorseBoard horseBoard, HorsePosition position, Vector2Int direction)
        {
            var currentHorse = horseBoard[position.X, position.Y];
            var linkedPositions = new List<HorsePosition>();
            var nextPosition = new HorsePosition(position);
            while (true)
            {
                nextPosition = nextPosition.GetNextPosition(direction);
                if (!nextPosition.IsInBoard(horseBoard.RowCount, horseBoard.ColumnCount))
                {
                    break;
                }

                if (horseBoard[nextPosition.X, nextPosition.Y] != currentHorse)
                {
                    break;
                }
                linkedPositions.Add(nextPosition);
            }
            return linkedPositions;
        }
        
        
        public static Comparison<HorsePosition> HorsePositionComparison = (a, b) =>
        {
            if (a.X == b.X)
            {
                return a.Y - b.Y;
            }
            return a.X - b.X;
        };
    }
}