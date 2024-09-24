using UnityEngine;

public static class MathHelpers
{
    public static float Expand(this float originalValue, float min, float max)
    {
        return min + (originalValue * (max - min));
    }

    public static float Compress01(this float originalValue, float min, float max)
    {
        originalValue = Mathf.Clamp(originalValue, min, max);
        return (originalValue - min) / (max - min);
    }

    public static float ChangeRange(this float originalValue, float previousMin, float previousMax, float newMin, float newMax)
    {
        return originalValue.Compress01(previousMin, previousMax).Expand(newMin, newMax);
    }
    
    public static float Wrap(this float value, float min, float max)
    {
        float range = max - min;
        return min + ((value - min) % range + range) % range;
    }
    
    public static float Bounce(this float value, float min, float max)
    {
        if (value < min) return min + (min - value);
        if (value > max) return max - (value - max);
        return value;
    }

    public static Vector2 ScaleToFit(this Vector2 size, Vector2 targetSize)
    {
        float scale = Mathf.Min(targetSize.x / size.x, targetSize.y / size.y);
        return size * scale;
    }
    
    public static float SineWave(this float frequency, float amplitude, float time)
    {
        return amplitude * Mathf.Sin(2 * Mathf.PI * frequency * time);
    }

    public static float Oscillate(this float time, float frequency, float amplitude, float phaseShift = 0)
    {
        return amplitude * Mathf.Sin(2 * Mathf.PI * frequency * time + phaseShift);
    }

    public static float ExpDecay(this float initialValue, float decayRate, float time)
    {
        return initialValue * Mathf.Exp(-decayRate * time);
    }

    public static float LogisticGrowth(this float time, float carryingCapacity, float growthRate, float midpoint)
    {
        return carryingCapacity / (1f + Mathf.Exp(-growthRate * (time - midpoint)));
    }
}