using System;
using System.Collections.Generic;
using SerializableGenerator;

var vector = new List<float> { 1, 2 };
var record = new S_RecordData { Value = 42, Vector = vector }.ToOriginal();
Check(record.Value == 42 && ReferenceEquals(record.Vector, vector), "readonly record struct");

var privateData = new S_PrivateData { m_Value = 12 }.ToOriginal();
Check(privateData.Value() == 12, "private field in partial class");

var mutable = new S_MutableData { Number = 7, Text = "test", @event = 3 }.ToOriginal();
Check(mutable.Number == 7 && mutable.Text == "test" && mutable.@event == 3,
    "fields, properties and escaped identifiers");

var immutable = new S_ImmutableData { Value = 9, Text = "constructor" }.ToOriginal();
Check(immutable.Value == 9 && immutable.Text == "constructor", "constructor and initializer");

var privateStruct = new S_PrivateStruct { m_Value = 5 }.ToOriginal();
Check(privateStruct.Value() == 5, "private field in partial struct");

var partialRecord = new S_PartialRecord { Value = 8 }.ToOriginal();
Check(partialRecord.Value == 8, "partial readonly record struct");

Console.WriteLine("All 6 ToOriginal conversion checks passed.");

static void Check(bool condition, string description)
{
    if (!condition) throw new Exception($"Conversion failed: {description}");
}

[SerdeType]
public readonly record struct RecordData(int Value, List<float> Vector);

[SerdeType]
public partial class PrivateData
{
    private int m_Value;
    public int Value() => m_Value;
}

[SerdeType]
public class MutableData
{
    public int Number;
    public string? Text { get; set; }
    public int @event;
}

[SerdeType]
public class ImmutableData
{
    public readonly int Value;
    public string? Text { get; init; }
    public ImmutableData(int value) => Value = value;
}

[SerdeType]
public partial struct PrivateStruct
{
    private int m_Value;
    public int Value() => m_Value;
}

[SerdeType]
public readonly partial record struct PartialRecord(int Value);
