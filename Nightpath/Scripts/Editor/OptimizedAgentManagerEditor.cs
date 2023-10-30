using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OptimizedAgentManager))]
public class OptimizedAgentManagerEditor : Editor
{
    private GUIStyle grayBoxStyle;
    private OptimizedAgentManager script;
    private void OnEnable()
    {
        if (grayBoxStyle == null) return;
        grayBoxStyle.normal.background = MakeTex(2, 2, new Color(0.7f, 0.7f, 0.7f, 1.0f));
    }

    private void OnDisable()
    {
        ((OptimizedAgentManager)target).drawGizmosTrigger = false;
    }

    public override void OnInspectorGUI()
    {
        script = (OptimizedAgentManager)target;

        DrawDefaultInspector();
        
        script.drawGizmosTrigger = true;

        grayBoxStyle = new GUIStyle(GUI.skin.box);
        
        GUILayout.BeginVertical(grayBoxStyle);

        var agentCount = script.agents.Count;

        var newAgentCount = EditorGUILayout.IntField("Agents count:", agentCount);
        if (newAgentCount < 0) newAgentCount = 0;
        if (newAgentCount > agentCount)
        {
            var plus = newAgentCount - agentCount;
            for (int i = 0; i < plus; i++)
            {
                script.agents.Add(Vector2.zero);
            }
        }
        else if(agentCount > newAgentCount)
        {
            var minus = agentCount - newAgentCount;
            if (EditorUtility.DisplayDialog("Confirmation", 
                    "Are you sure you want to remove last " + minus + " agents from list?", "Yes", "No"))
            {
                for (int i = 0; i < minus; i++)
                {
                    script.agents.RemoveAt(agentCount - 1 - i);
                }
            }
        }

        if (script.agents.Count > 0)
        {
            script.selectedIndex = EditorGUILayout.IntField("Selected index:", script.selectedIndex);

            if (script.selectedIndex < 0)
            {
                script.selectedIndex = 0;
            }
            else if (script.selectedIndex >= script.agents.Count)
            {
                script.selectedIndex = script.agents.Count - 1;
            }

            script.agents[script.selectedIndex] = EditorGUILayout.Vector2Field("Position:", script.agents[script.selectedIndex]);
            SceneView.RepaintAll();

            if (GUILayout.Button("Remove Selected Agent"))
            {
                script.agents.RemoveAt(script.selectedIndex);
            }
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Add Agent", GUILayout.Height(30)))
        {
            script.agents.Add(new Vector2());
        }

        GUIStyle redButtonStyle = new GUIStyle(GUI.skin.button);
        redButtonStyle.normal.textColor = Color.red;

        if (script.agents.Count > 0 && GUILayout.Button("Reset List", redButtonStyle))
        {
            if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to reset the agent list?", "Yes", "No"))
            {
                script.agents.Clear();
            }
        }

        GUILayout.EndVertical();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
