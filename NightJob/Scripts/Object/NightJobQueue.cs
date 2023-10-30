using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NightJob
{
    public class NightJobQueue
    {
        private readonly NightJobManager _parent;
        public readonly Queue<INightJob> NotStartedJobQueue = new Queue<INightJob>();
        public INightJob Current { get; private set; }
        
        public event Action OnCompleted;
    
        public int AliveJobCount { get; private set; }
    
        private bool _isStarted;
        private bool _isDisposed;

        private Task secondaryThreadTask;
        private Coroutine _coroutine;
    
        public NightJobQueue(NightJobManager nightJobManager)
        {
            _parent = nightJobManager;
        }
    
        public void AddJobToQueue(INightJob job)
        {
            AliveJobCount++;
            NotStartedJobQueue.Enqueue(job);
            
            if (_isStarted) return;
    
            StartExecute();
        }
    
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _parent.AllQueue.Remove(this);
            if(_coroutine != null) _parent.StopCoroutine(_coroutine);
            NotStartedJobQueue.Clear();
            AliveJobCount = 0;
        }
    
        public void Reactivate()
        {
            if (!_isDisposed) return;
            
            _parent.AllQueue.Add(this);
            StartExecute();
        }
    
        private void StartExecute()
        {
            _coroutine = _parent.StartCoroutine(ExecuteLoop());
        }
    
        private IEnumerator ExecuteLoop()
        {
            _isStarted = true;
    
            while (true)
            {
                if(NotStartedJobQueue.Count == 0) yield return new WaitUntil(() => NotStartedJobQueue.Count > 0);
                
                Current = NotStartedJobQueue.Dequeue();
                
                Current.JobCompleted = false;
                
                secondaryThreadTask = Task.Run(Current.OnSecondaryThread).ContinueWith((task) =>
                {
                    if (task.Exception == null) return;
                    foreach (var ex in task.Exception.InnerExceptions)
                    {
                        Debug.LogException(ex);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
                
                if (_isDisposed) yield break;

                yield return new WaitUntil(() => secondaryThreadTask.IsCompleted);
                
                Current.JobCompleted = true;
    
                yield return Current.OnMainThread();

                AliveJobCount--;

                if (AliveJobCount < 0) AliveJobCount = 0;

                if (AliveJobCount == 0) OnCompleted?.Invoke();
            }
        }
    }
}

