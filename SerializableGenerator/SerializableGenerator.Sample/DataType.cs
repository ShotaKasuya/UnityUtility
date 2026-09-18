using System.Collections.Generic;

namespace SerializableGenerator.Sample
{
    [SerdeType]
    public readonly record struct DataType(int Value, List<float> Vector);
    [SerdeType]
    public readonly record struct NestedDataType(DataType MyDataType, List<float> Vector);
}