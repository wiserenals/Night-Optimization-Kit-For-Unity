using System;

public class SliderMinMaxAttribute : Attribute
{
    public float min, max;
    public SliderMinMaxAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
