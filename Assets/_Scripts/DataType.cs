
using System.Collections.Generic;
using SerializableGenerator;

namespace _Scripts
{
    [SerdeType]
    public record struct DataType(bool Boolean, int Integer);
    [SerdeType]
    public record struct NestedDataType(List<DataType> DataList, int Integer);
}