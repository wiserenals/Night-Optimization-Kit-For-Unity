using UnityEngine;

[DisallowMultipleComponent]
public class Singleton<T> : ExpandedBehaviour where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<T>(true);
            return _instance;
        }
    }
}

public class SingletonDontDestroy<T> : DontDestroy where T : ExpandedBehaviour
{
    private static T _instance;

    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<T>(true);
            return _instance;
        }
    }
}

[DisallowMultipleComponent]
public class ForceSingleton<T> : ExpandedBehaviour where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null) _instance = new GameObject().AddComponent<T>();
            }
            return _instance;
        }
    }
}

[DisallowMultipleComponent]
public class ForceSingletonDontDestroy<T> : DontDestroy where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null) _instance = new GameObject().AddComponent<T>();
            }
            return _instance;
        }
    }
}

[DisallowMultipleComponent]
public class ProtectedSingleton<T> : ExpandedBehaviour where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    protected static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<T>(true);
            return _instance;
        }
    }
}

[DisallowMultipleComponent]
public class ProtectedSingletonDontDestroy<T> : DontDestroy where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    protected static T Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<T>(true);
            return _instance;
        }
    }
}

[DisallowMultipleComponent]
public class ForceProtectedSingleton<T> : ExpandedBehaviour where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    protected static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null) _instance = new GameObject(typeof(T).Name + " (Force created)").AddComponent<T>();
            }
            return _instance;
        }
        set => _instance = value;
    }
}

[DisallowMultipleComponent]
public class ForceProtectedSingletonDontDestroy<T> : DontDestroy where T : ExpandedBehaviour
{
    private static T _instance;
    
    public static bool HasInstance()
    {
        if (_instance == null && FindObjectOfType<T>(true)) return true;
        return _instance != null;
    }

    protected static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null) _instance = new GameObject(typeof(T).Name + " (Force created)").AddComponent<T>();
            }
            return _instance;
        }
        set => _instance = value;
    }
}