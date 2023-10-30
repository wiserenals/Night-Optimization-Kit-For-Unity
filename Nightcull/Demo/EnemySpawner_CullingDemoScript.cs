using UnityEngine;

public class EnemySpawner_CullingDemoScript : ExpandedBehaviour
{
    public NightCullCamera nightcull;
    public GameObject enemyPrefab;
    private void Start()
    {
        nightcull.cullingPool.Instantiate(enemyPrefab, 300);
    }
}
