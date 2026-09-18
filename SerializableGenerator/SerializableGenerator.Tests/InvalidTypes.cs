using SerializableGenerator;

// Compile with the corresponding DefineConstants to verify generator diagnostics.
#if SERDE_INVALID_CONTAINER
[SerdeType]
public class UnsupportedContainer
{
    public System.Collections.Generic.Dictionary<string, Leaf> Items = new();
}
#endif

#if SERDE_INVALID_CYCLE
[SerdeType]
public class CycleA
{
    public CycleB? Next;
}

[SerdeType]
public class CycleB
{
    public CycleA? Next;
}
#endif
