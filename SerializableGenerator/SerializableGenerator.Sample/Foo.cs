using HelloGenerator;

namespace SerializableGenerator.Sample;

[GenerateHello]
[SerdeType]
public partial class Foo
{
    private float m_Value;

    internal float GetValue()
    {
        return m_Value;
    }

    internal void SetValue(float value)
    {
        m_Value = value;
    }
}
