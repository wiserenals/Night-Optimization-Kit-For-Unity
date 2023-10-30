using System;
using Nightpath;
using UnityEditor;

[CustomEditor(typeof(OptimizedAgentListEditor))]
public class OptimizedAgentListEditor_Editor : Editor
{
    private OptimizedAgentListEditor script;
    private void OnDisable()
    {
        ((OptimizedAgentListEditor)target).drawGizmosTrigger = false;
    }

    public override void OnInspectorGUI()
    { 
        DrawDefaultInspector();
        
        script = (OptimizedAgentListEditor)target;
        if(script.agentManager == null) script.agentManager = script.GetComponent<OptimizedAgentManager>();

        if (script.agentManager != null)
        { 
            script.drawGizmosTrigger = true;
            script.gridFormationStyle = 
                (GridFormationStyle) EditorGUILayout.EnumPopup("Grid Formation Style", script.gridFormationStyle);
            var tileDiameter = EditorGUILayout.FloatField("Tile Diameter", script.tileDiameter);

            if (tileDiameter < 0.01f) tileDiameter = 0.01f;

            var newGridSize = EditorGUILayout.Vector2IntField("Grid Size", script.gridSize);

            EditorGUILayout.LabelField($"Each line is equivalent to {script.equivalent} agents.");

            if (newGridSize != script.gridSize || script.tileDiameter != tileDiameter)
            {
                if (newGridSize.x < 1) newGridSize.x = 1;
                if (newGridSize.y < 1) newGridSize.y = 1;
                script.gridSize = newGridSize;
                script.tileDiameter = tileDiameter;
                script.agentManager.agents = script.CreateGrids();
            }

            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.HelpBox("OptimizedAgentManager component not found on the same GameObject.", MessageType.Error);
        }
    }
}