public abstract class DontDestroy : ExpandedBehaviour
{
    protected virtual void Awake()
    {
        if(transform.parent) transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}
