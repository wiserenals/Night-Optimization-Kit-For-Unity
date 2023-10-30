using UnityEngine;

namespace NightJob.Demo
{
    public class JobTestMono : MonoBehaviour
    {
        private void Start()
        {
            var q = NightJobManager.NewQueue();
            q.AddJobToQueue(new TestJob());
        
        }
    }

}

