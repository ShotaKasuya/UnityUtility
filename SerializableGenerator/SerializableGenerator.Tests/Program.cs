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

var nested = new S_Envelope
{
    Child = new S_Branch { Child = new S_Leaf { Value = 21 } },
    Leaves = new[] { new S_Leaf { Value = 22 } },
    Groups = new List<S_Leaf?[]> { new S_Leaf?[] { new S_Leaf { Value = 23 }, null } },
    Optional = new S_Leaf { Value = 24 },
    Reference = new S_ReferenceLeaf { Value = 25 },
    References = new List<S_ReferenceLeaf?> { new S_ReferenceLeaf { Value = 26 }, null }
}.ToOriginal();
Check(nested.Child.Child.Value == 21, "recursive member types and constructor arguments");
Check(nested.Leaves![0].Value == 22 && nested.Groups![0][0]?.Value == 23 &&
      nested.Groups[0][1] == null, "arrays and nested generic containers");
Check(nested.Optional?.Value == 24 && nested.Reference?.Value == 25 &&
      nested.References![0]?.Value == 26 && nested.References[1] == null,
    "nullable value types and nullable reference types");
var empty = default(S_Envelope).ToOriginal();
Check(empty.Leaves == null && empty.Groups == null && empty.Optional == null &&
      empty.Reference == null && empty.References == null, "null preservation");

var privateNested = new S_PrivateNested { m_Child = new S_Leaf { Value = 27 } }.ToOriginal();
Check(privateNested.Value() == 27, "recursive conversion inside partial helper");
var tree = new S_Tree
{
    Children = new List<S_Tree> { new S_Tree { Children = new List<S_Tree>() } }
}.ToOriginal();
Check(tree.Children[0].Children.Count == 0, "self reference through a collection");

var namespaced = new S_NamespacedContainer
{
    First = new First.S_Item { Value = 28 },
    Second = new Second.S_Item { Value = 29 }
}.ToOriginal();
Check(namespaced.First.Value == 28 && namespaced.Second.Value == 29,
    "fully qualified type names and unique source hint names");

Console.WriteLine("All 13 ToOriginal conversion checks passed.");

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

[SerdeType]
public readonly record struct Leaf(int Value);

[SerdeType]
public readonly record struct Branch(Leaf Child);

[SerdeType]
public class ReferenceLeaf
{
    public int Value;
}

[SerdeType]
public class Envelope
{
    public Branch Child;
    public Leaf[]? Leaves;
    public List<Leaf?[]>? Groups;
    public Leaf? Optional;
    public ReferenceLeaf? Reference;
    public List<ReferenceLeaf?>? References;
}

[SerdeType]
public partial class PrivateNested
{
    private Leaf m_Child;
    public int Value() => m_Child.Value;
}

[SerdeType]
public class Tree
{
    public List<Tree> Children = new();
}

[SerdeType]
public readonly record struct NamespacedContainer(First.Item First, Second.Item Second);

namespace First
{
    [SerdeType]
    public readonly record struct Item(int Value);
}

namespace Second
{
    [SerdeType]
    public readonly record struct Item(int Value);
}
