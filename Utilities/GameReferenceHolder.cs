namespace Karayel
{
    public class GameReferenceHolder : ExpandedBehaviour
    {
        private void Awake()
        {
            if(holder == null) holder = this;
            else Destroy(gameObject);
        }
        
        
    }
}
