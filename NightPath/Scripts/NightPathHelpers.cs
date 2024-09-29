using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class NightPathHelpers
{

    public static void Run(this IEnumerator enumerator)
    {
        while (enumerator.MoveNext()) {  }
    }
    
    public static NightPathNode Heaviest(this IEnumerable<NightPathNode> source)
    {
        return source.OrderByDescending(x => x.weight).First();
    }
    
    public static NightPathNode Lightest(this IEnumerable<NightPathNode> source)
    {
        return source.OrderBy(x => x.weight).First();
    }
    
    public static float GetValueFromNormal(Vector3 normal, float angleThreshold, float maxAngle)
    {
        // Normalin yukarı yönle (Vector3.up) olan açısını hesapla
        float angle = Vector3.Angle(normal, Vector3.up);

        // Açının koşullarına göre değeri döndür
        if (angle < angleThreshold)
        {
            return 1f; // Normal Vector3.up'a yakınsa
        }
        else if (angle >= maxAngle)
        {
            return 0f; // Normal 90 derece veya daha fazla açıya sahipse
        }
        else
        {
            // Normal ile Vector3.up arasındaki açıyı 0 ile 1 arasında bir değere dönüştür
            // 1f (normal yukarı) ve 0f (normal maxAngle derece) arasında lineer bir değer hesapla
            return Mathf.Lerp(1f, 0f, (angle - angleThreshold) / (maxAngle - angleThreshold));
        }
    }
}