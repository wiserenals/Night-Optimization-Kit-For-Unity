using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavigationWorld))]
public class NavigationWorldEditor : Editor
{
    private List<GameObject> objectsWithNightPathBuilder;
    private Vector2 scrollPosition;
    public override void OnInspectorGUI()
    {
        UpdateObjectsWithNightPathBuilder();
        
        EditorGUILayout.HelpBox("You can edit this navigation world data in a NightPath Builder.", MessageType.Info);
        if (GUILayout.Button("Create New Builder", GUILayout.Height(30)))
        {
            var newObject = new GameObject("NightPath Builder");
            var builder = newObject.AddComponent<NightPathBuilder>();
            builder.navigationWorld = (NavigationWorld)target;

            Selection.activeGameObject = newObject;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        if (objectsWithNightPathBuilder.Count == 0) return;
        
        EditorGUILayout.LabelField("GameObjects with NightPathBuilder Component", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));

        for (int i = 0; i < objectsWithNightPathBuilder.Count; i++)
        {
            GameObject obj = objectsWithNightPathBuilder[i];
            if (GUILayout.Button(obj.name))
            {
                obj.GetComponent<NightPathBuilder>().navigationWorld = (NavigationWorld)target;
                Selection.activeGameObject = obj;
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    
    private void UpdateObjectsWithNightPathBuilder()
    {
        objectsWithNightPathBuilder = new List<GameObject>();
        NightPathBuilder[] nightPathBuilders = GameObject.FindObjectsOfType<NightPathBuilder>();

        foreach (NightPathBuilder npb in nightPathBuilders)
        {
            objectsWithNightPathBuilder.Add(npb.gameObject);
        }
    }
}