using System;
using System.Collections;
using System.IO;
using DataModels;
using NUnit.Framework;
using ProtoBuf;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

public class DataModelTest
{
    private const string TempFileFolder = "Temp/DataModelTest";
    [SetUp]
    public void Setup()
    {
        if (!Directory.Exists(TempFileFolder))
        {
            Directory.CreateDirectory(TempFileFolder);
        }
    }
        
    [Test]
    public void HorseTypeTest()
    {
        Assert.IsFalse(HorseType.None.IsCover());
        Assert.IsFalse(HorseType.None.IsStick());
        Assert.IsFalse(HorseType.None.IsStone());
        
        // Assert.AreEqual(1, (int)HorseType.Stone);
        Assert.IsFalse(HorseType.Stone.IsCover());
        Assert.IsFalse(HorseType.Stone.IsStick());
        Assert.IsTrue(HorseType.Stone.IsStone());
        
        // Assert.AreEqual(2, (int)HorseType.Stick);
        Assert.IsFalse(HorseType.Stick.IsCover());
        Assert.IsTrue(HorseType.Stick.IsStick());
        Assert.IsFalse(HorseType.Stick.IsStone());
        
        // Assert.AreEqual(3, (int)HorseType.StoneCover);
        Assert.IsTrue(HorseType.StoneCover.IsCover());
        Assert.IsFalse(HorseType.StoneCover.IsStick());
        Assert.IsTrue(HorseType.StoneCover.IsStone());
        
        // Assert.AreEqual(4, (int)HorseType.StickCover);
        Assert.IsTrue(HorseType.StickCover.IsCover());
        Assert.IsTrue(HorseType.StickCover.IsStick());
        Assert.IsFalse(HorseType.StickCover.IsStone());
    }

    [Test]
    public void SerializeHorseTest()
    {
        var horse = new Horse
        {
            Type = HorseType.Stick,
            Position = new HorsePosition(1, 2)
        };
        var fileStream = ReadFromSerialized<Horse>(horse);
        var deserialized = Serializer.Deserialize<Horse>(fileStream);
            
        Assert.AreEqual(horse.Type, deserialized.Type);
        Assert.AreEqual(horse.Position.X, deserialized.Position.X);
        Assert.AreEqual(horse.Position.Y, deserialized.Position.Y);
    }
    
    [Test]
    public void SerializeHorseBoardTest()
    {
        var horseBoard = new HorseBoard();
        
        var values = Enum.GetValues(typeof(HorseType));
        var random = new Random();
        for (var i = 0; i < 16; i++)
        {
            for (var j = 0; j < 16; j++)
            {
                horseBoard[i, j] = (HorseType) values.GetValue(random.Next(1, values.Length));
            }
        }
        
        Debug.Log($"horseBoard = {horseBoard}");
        var fileStream = ReadFromSerialized<HorseBoard>(horseBoard);
        var deserialized = Serializer.Deserialize<HorseBoard>(fileStream);
        Debug.Log($"deserialized = {deserialized}");

        var commonLinesCount = Math.Min(horseBoard.RowCount, deserialized.RowCount);
        for (var i = 0; i < commonLinesCount; i++)
        {
            var commonHorseCount = Math.Min(horseBoard.ColumnCount, deserialized.ColumnCount);
            for (var j = 0; j < commonHorseCount; j++)
            {
                Assert.AreEqual(horseBoard[i, j], deserialized[i, j]);
            }
        }
    }
    
    [Test]
    public void SerializeHorsePositionTest()
    {
        var horsePosition = new HorsePosition(1, 2);
        var fileStream = ReadFromSerialized<HorsePosition>(horsePosition);
        var deserialized = Serializer.Deserialize<HorsePosition>(fileStream);
        Assert.AreEqual(horsePosition.X, deserialized.X);
        Assert.AreEqual(horsePosition.Y, deserialized.Y);
    }
    
    [Test]
    public void SerializeHorsePositionArrayTest()
    {
        var horsePositionArray = new HorsePositionArray()
        {
            Positions = new[]
            {
                new HorsePosition(1, 2),
                new HorsePosition(3, 4),
                new HorsePosition(5, 4),
                new HorsePosition(3, 2),
                new HorsePosition(1, 0),
            }
        };
        var fileStream = ReadFromSerialized<HorsePositionArray>(horsePositionArray);
        var deserialized = Serializer.Deserialize<HorsePositionArray>(fileStream);
        Assert.AreEqual(horsePositionArray.Positions.Length, deserialized.Positions.Length);
        for (var i = 0; i < horsePositionArray.Positions.Length; i++)
        {
            Assert.AreEqual(horsePositionArray.Positions[i].X, deserialized.Positions[i].X);
            Assert.AreEqual(horsePositionArray.Positions[i].Y, deserialized.Positions[i].Y);
        }
    }

    private FileStream ReadFromSerialized<T>(object obj)
    {
        var objType = typeof(T);
        // serialize
        using MemoryStream memory = new MemoryStream();
        Serializer.Serialize<T>(memory, (T) obj);
        // write to file
        var fileName = $"{TempFileFolder}/{objType}.bin";
        FileStream file = new FileStream(fileName, FileMode.Create);
        memory.WriteTo(file);
        memory.Close();
        file.Close();
        // read from file
        var fileStream = new FileStream(fileName, FileMode.Open);
        return fileStream;
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
