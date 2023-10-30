using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NOK_CharacterController))]
public class NOK_CharacterControllerEditor : Editor
{
    Texture2D errorIcon;
    private SerializedProperty rigidbodyProperty, crouchTargetProperty;
    private SerializedProperty movementSpeedProperty;

    private void OnEnable()
    {
        movementSpeedProperty = serializedObject.FindProperty("movementSpeed");
        rigidbodyProperty = serializedObject.FindProperty("rigidbody");
        crouchTargetProperty = serializedObject.FindProperty("crouchTarget");
        errorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;
    }
    
    public override void OnInspectorGUI()
    {
        NOK_CharacterController script = (NOK_CharacterController)target;
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
        var stylef = new GUIStyle(GUI.skin.textArea) {alignment = TextAnchor.MiddleCenter};

        serializedObject.Update();
        
        EditorGUILayout.PropertyField(rigidbodyProperty);

        if (rigidbodyProperty.objectReferenceValue == null)
        {
            Rigidbody foundRigidbody = script.GetComponent<Rigidbody>();

            if (foundRigidbody == null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(errorIcon);
                EditorGUILayout.LabelField("Rigidbody is not assigned! Do you want to create?", 
                    EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(33));

                if (GUILayout.Button("Fix", GUILayout.Width(60), GUILayout.Height(33)))
                {
                    rigidbodyProperty.objectReferenceValue = script.gameObject.AddComponent<Rigidbody>();
                    goto end;
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(errorIcon);
                EditorGUILayout.LabelField("Rigidbody is not assigned but found on the same GameObject.", 
                    EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(33));

                if (GUILayout.Button("Fix", GUILayout.Width(60), GUILayout.Height(33)))
                {
                    rigidbodyProperty.objectReferenceValue = foundRigidbody;
                    goto end;
                }
                EditorGUILayout.EndHorizontal();

            }
            goto end;
        }

        EditorGUILayout.PropertyField(crouchTargetProperty);
        serializedObject.ApplyModifiedProperties();

        if (crouchTargetProperty.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("Crouch target is not assigned!", MessageType.Error);
            return;
        }

        var defaultVector4 = movementSpeedProperty.vector4Value;
        
        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var nVecZ = defaultVector4;
        nVecZ.z = EditorGUILayout.FloatField("", script.movementSpeed.z, stylef, 
            GUILayout.Width(50), GUILayout.Height(30));
        movementSpeedProperty.vector4Value = nVecZ;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        nVecZ.y = EditorGUILayout.FloatField("", script.movementSpeed.y, stylef, 
            GUILayout.Width(50), GUILayout.Height(30));
        movementSpeedProperty.vector4Value = nVecZ;
        EditorGUILayout.LabelField("Movement\nSpeed", style, GUILayout.Width(100), 
            GUILayout.Height(30));
        nVecZ.x = EditorGUILayout.FloatField("", script.movementSpeed.x, stylef, 
            GUILayout.Width(50), GUILayout.Height(30));
        movementSpeedProperty.vector4Value = nVecZ;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        nVecZ.w = EditorGUILayout.FloatField("", script.movementSpeed.w, stylef, 
            GUILayout.Width(50), GUILayout.Height(30));
        movementSpeedProperty.vector4Value = nVecZ;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        SerializedProperty property = serializedObject.GetIterator();

        if (property.NextVisible(true))
        {
            do
            {
                if (property.name != "m_Script")
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            } while (property.NextVisible(false));
        }
        end:
        serializedObject.ApplyModifiedProperties();
    }
}