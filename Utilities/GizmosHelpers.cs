using UnityEngine;

public static class GizmosHelpers
{
    public static void DrawArrowDown(Vector3 pos, float length, Color? color = null)
    {
        Gizmos.color = color ?? Color.white;

        length = Mathf.CeilToInt(length / 2);

        for (int i = 0; i < length; i++)
        {
            Gizmos.DrawCube(pos + Vector3.down * (2 * i), Vector3.one * ((length - i) / length));
        }
    }
    
    public static void SetAlpha(float val)
    {
        var c = Gizmos.color;
        c.a = val;
        Gizmos.color = c;
    }
    
    /// <summary>
    /// Inverts the color and blends it with the original color based on the blend amount.
    /// </summary>
    /// <param name="color">The original color to be inverted.</param>
    /// <param name="blendAmount">A value between 0 and 1 representing the blend ratio.</param>
    /// <returns>The blended color after inverting.</returns>
    public static Color InvertColor(Color color, float blendAmount)
    {
        // Ensure blendAmount is clamped between 0 and 1
        blendAmount = Mathf.Clamp01(blendAmount);
        
        // Invert the color
        Color invertedColor = new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
        
        // Blend the original color and the inverted color
        return Color.Lerp(color, invertedColor, blendAmount);
    }
    
    /// <summary>
    /// Generates a highlight color that is distinct from both the main color and the inverted color.
    /// </summary>
    /// <param name="mainColor">The main color to be used.</param>
    /// <param name="blendAmount">Blend amount for generating the inverted color.</param>
    /// <returns>A highlight color that contrasts with both the main and inverted colors.</returns>
    public static Color GenerateHighlightColor(Color mainColor, float blendAmount)
    {
        // Invert the main color
        Color invertedColor = InvertColor(mainColor, blendAmount);
    
        // Calculate the average of the main and inverted colors
        Color averageColor = (mainColor + invertedColor) / 2f;
    
        // Lightly adjust the average color to create a distinct highlight color
        float highlightR = Mathf.Clamp01(averageColor.r + 0.3f); // Shift red slightly
        float highlightG = Mathf.Clamp01(averageColor.g - 0.2f); // Shift green slightly
        float highlightB = Mathf.Clamp01(averageColor.b + 0.15f); // Shift blue slightly
    
        // Return the final highlight color
        return new Color(highlightR, highlightG, highlightB, 1f); // Fully opaque color
    }

}