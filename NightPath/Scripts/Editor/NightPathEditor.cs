using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NightPath))]
public class NightPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Varsayılan Inspector çizimi
        DrawDefaultInspector();

        // NightPath bileşenine referans
        NightPath nightPath = (NightPath)target;

        // Buton ekle
        if (GUILayout.Button("Build Path"))
        {
            // Build metodunu çağır
            nightPath.Build();
            
            // Sahneyi güncelle
            EditorUtility.SetDirty(nightPath);
        }
    }
}