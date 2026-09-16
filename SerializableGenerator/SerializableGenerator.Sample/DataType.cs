using System.Collections.Generic;

namespace SerializableGenerator.Sample
{
    [SerdeType]
    public readonly record struct DataType(int Value, List<float> Vector);
}