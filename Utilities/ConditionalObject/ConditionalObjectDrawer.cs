using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalObject<,>), true)]
public class ConditionalObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty objectTypeProperty = property.FindPropertyRelative("objectType");
        SerializedProperty firstTypeProperty = property.FindPropertyRelative("firstType");
        SerializedProperty secondTypeProperty = property.FindPropertyRelative("secondType");

        EditorGUI.BeginProperty(position, label, property);

        // Enum seçimi
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, objectTypeProperty);

        position.y += EditorGUIUtility.singleLineHeight; // Move down for the next field

        // Seçilen nesne türüne göre ObjectField'i çiz
        if (objectTypeProperty.enumValueIndex == (int)ObjectType.MonoBehaviour)
        {
            EditorGUI.PropertyField(position, firstTypeProperty, new GUIContent("MonoBehaviour"));
        }
        else
        {
            EditorGUI.PropertyField(position, secondTypeProperty, new GUIContent("ScriptableObject"));
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3; // Üç satır yüksekliği
    }
}