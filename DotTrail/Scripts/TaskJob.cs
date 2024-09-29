using System;
using System.Collections;
using System.Threading.Tasks;
using NightJob;

namespace DotTrail
{
    public class TaskJob : INightJob
    {
        private readonly Func<Task> secondaryThreadTask;
        public TaskJob(Func<Task> secondaryThreadTask)
        {
            this.secondaryThreadTask = secondaryThreadTask;
        }

        public bool JobCompleted { get; set; }
        public IEnumerator OnMainThread()
        {
            yield break;
        }

        public Task OnSecondaryThread()
        {
            return secondaryThreadTask();
        }
    }
}