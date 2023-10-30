using UnityEditor;

[CustomEditor(typeof(NightJobManager))]
public class NightJobManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NightJobManager nightJobManager = (NightJobManager)target;

        EditorGUILayout.LabelField("All Queue:");

        if (nightJobManager.AllQueue.Count == 0)
        {
            EditorGUILayout.LabelField("There is no queue.");
        }
        else
        {
            EditorGUI.indentLevel++;
            int queueIndex = 1;

            foreach (var jobQueue in nightJobManager.AllQueue)
            {
                EditorGUILayout.LabelField($"Queue {queueIndex}: {jobQueue.AliveJobCount} jobs executing.");
                queueIndex++;
            }

            EditorGUI.indentLevel--;
        }
    }
}
