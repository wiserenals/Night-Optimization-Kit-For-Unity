using UnityEngine;

[System.Serializable]
public class ConditionalObject<F, T> 
    where T : ScriptableObject 
    where F : MonoBehaviour
{
    public ObjectType objectType; // Enum ile tür seçimi
    public F firstType;           // MonoBehaviour türü
    public T secondType;          // ScriptableObject türü

    public Object GetSelectedObject()
    {
        return objectType == ObjectType.MonoBehaviour ? firstType : secondType;
    }
}


public enum ObjectType
{
    MonoBehaviour,
    ScriptableObject
}
