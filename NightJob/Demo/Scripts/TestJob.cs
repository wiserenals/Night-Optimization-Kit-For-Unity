using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NightJob;
using UnityEngine;

namespace NightJob.Demo
{
    public struct TestJob : INightJob
    {
        public bool JobCompleted { get; set; }

        public IEnumerator OnMainThread()
        {
            yield return null;
        }

        public async Task OnSecondaryThread()
        {
            var arr = new int[5];
            var a = arr[6];
            
            await Task.Delay(10);
        }
    }
}

