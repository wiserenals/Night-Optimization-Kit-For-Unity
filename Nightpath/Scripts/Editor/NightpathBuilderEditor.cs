using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NightPathBuilder))]
public class NightpathBuilderEditor : Editor
{
    private const string BuildingLabel = "Building...";

    private NightPathBuilder _nightPathBuilder;

    private void OnDisable()
    {
        if (!_nightPathBuilder) return;
        _nightPathBuilder.drawGizmosTrigger = false;
    }

    public override void OnInspectorGUI()
    {
        _nightPathBuilder = (NightPathBuilder)target;
        _nightPathBuilder.drawGizmosTrigger = true;

        serializedObject.Update();

        DrawDefaultInspector();

        var navigationWorld = _nightPathBuilder.navigationWorld;

        if (navigationWorld is null) goto end;

        var radiusOk = navigationWorld.tileRadius > 0;
        var maskOk = navigationWorld.wallMask != 0;

        GUI.enabled = radiusOk && maskOk;

        EditorGUILayout.LabelField("Navigation World Settings", EditorStyles.boldLabel);

        if (EditorApplication.isPlayingOrWillChangePlaymode) GUI.enabled = false;

        navigationWorld.obstacleBaked = EditorGUILayout.Toggle("Obstacle Baked", navigationWorld.obstacleBaked);

        navigationWorld.generateLoop = EditorGUILayout.Toggle("Generate Loop", navigationWorld.generateLoop);

        if (navigationWorld.generateLoop)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Generate Delay");
            navigationWorld.generateDelay = EditorGUILayout.FloatField(navigationWorld.generateDelay);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
        }

        GUI.enabled = (radiusOk && maskOk) || !maskOk;

        navigationWorld.wallMask = LayerMaskField("Wall Mask", navigationWorld.wallMask);
        if (!maskOk)
        {
            EditorGUILayout.HelpBox(
                "'Wall Mask' is set to Nothing. In this case, creating a 'Navigation World' is unnecessary. Please select at least one layer.",
                MessageType.Error);
            EditorGUILayout.Space(15);
        }

        GUI.enabled = (radiusOk && maskOk) || !radiusOk;
        navigationWorld.tileRadius = EditorGUILayout.FloatField("Tile Radius", navigationWorld.tileRadius);

        if (navigationWorld.tileRadius <= 0.1f)
        {
            EditorGUILayout.HelpBox(
                "Possible performance problem detected. Tile radius may need to be higher than 0.1f.",
                MessageType.Warning);
        }

        if (!radiusOk)
        {
            EditorGUILayout.HelpBox(
                "Possible stack overflow exception. Tile radius have to be higher than zero.", MessageType.Error);
        }

        GUI.enabled = radiusOk && maskOk;

        navigationWorld.delayedObstacleBuild =
            EditorGUILayout.Toggle("Delayed Obstacle Build", navigationWorld.delayedObstacleBuild);
        if (navigationWorld.delayedObstacleBuild)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.PrefixLabel("Obstacle Build Delay");
            navigationWorld.obstacleBuildDelay = EditorGUILayout.FloatField(navigationWorld.obstacleBuildDelay);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);
        }

        navigationWorld.navigationGridSize =
            EditorGUILayout.Vector2Field("Navigation Grid Size", navigationWorld.navigationGridSize);

        var tileDiameter = navigationWorld.tileRadius * 2;

        var tileCount = Mathf.RoundToInt(navigationWorld.navigationGridSize.x / tileDiameter)
                        * Mathf.RoundToInt(navigationWorld.navigationGridSize.y / tileDiameter);

        GUILayout.Label("Tile count: " + tileCount);

        if (navigationWorld.navigationGridSize.x < 1) navigationWorld.navigationGridSize.x = 1;
        if (navigationWorld.navigationGridSize.y < 1) navigationWorld.navigationGridSize.y = 1;

        EditorGUILayout.Space(15);

        var isBuilding = _nightPathBuilder.IsBuilding;

        GUI.enabled = navigationWorld.wallMask != 0
                      && !isBuilding
                      && radiusOk;

        if (GUILayout.Button(isBuilding ? BuildingLabel : "Build", GUILayout.Height(30)))
        {
            navigationWorld.Init(_nightPathBuilder);
            navigationWorld
                .Build(_nightPathBuilder.transform.position, _nightPathBuilder.targetTransform.position,
                    navigationWorld is not { navInstance: not null } || !navigationWorld.obstacleBaked);
        }

        GUI.enabled = radiusOk && maskOk;

        Color originalColor = GUI.backgroundColor; // Store the original background color

        GUI.backgroundColor = Color.red;

        if (isBuilding && GUILayout.Button("Stop Building (Experimental)"))
        {
            navigationWorld.StopBuilding();
        }

        GUI.backgroundColor = originalColor;

        if (isBuilding && navigationWorld.Current is { bake: true } && navigationWorld.navInstance != null)
        {
            var val = navigationWorld.Current;
            float bt = navigationWorld.navInstance.obstacleBuildTime;
            GUILayout.Label("Obstacle Generation Process: "
                            + bt + "/" + tileCount
                            + $" ({Mathf.FloorToInt(bt / tileCount * 100)}%)");
        }

        int total = 0;

        if (navigationWorld.tileCreationTime != null)
        {
            var ms = navigationWorld.tileCreationTime.Value.Milliseconds;
            GUILayout.Label("Latest tiles is generated in "
                            + ms + " milliseconds.");
            total += ms;
        }


        if (navigationWorld.heatmapCreationTime != null)
        {
            var ms = navigationWorld.heatmapCreationTime.Value.Milliseconds;
            GUILayout.Label("Latest heatmap is generated in "
                            + ms + " milliseconds.");
            total += ms;
        }


        if (navigationWorld.flowFieldCreationTime != null)
        {
            var ms = navigationWorld.flowFieldCreationTime.Value.Milliseconds;
            GUILayout.Label("Latest flow field is generated in "
                            + ms + " milliseconds.");
            total += ms;
        }

        if (total > 0)
        {
            GUILayout.Label("Total: " + total + " milliseconds.");
        }
        
        end:

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    private static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<string> layerNames = new List<string>();
        List<int> layerValues = new List<int>();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layerNames.Add(layerName);
                layerValues.Add(1 << i);
            }
        }

        int selectedValue = 0;
        for (int i = 0; i < layerValues.Count; i++)
        {
            if ((layerMask.value & layerValues[i]) != 0)
            {
                selectedValue |= 1 << i;
            }
        }

        selectedValue = EditorGUILayout.MaskField(label, selectedValue, layerNames.ToArray());
        layerMask.value = 0;
        for (int i = 0; i < layerValues.Count; i++)
        {
            if ((selectedValue & (1 << i)) != 0)
            {
                layerMask.value |= layerValues[i];
            }
        }

        return layerMask;
    }
}