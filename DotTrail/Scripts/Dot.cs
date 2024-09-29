using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Karayel;
using NightJob;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DotTrail
{
    public class Dot
    {
        private static Dot _instance;
        public static Dot Trail => _instance ??= new Dot();

        private NightJobQueue _currentQueue;

        public Dot()
        {
            RenewQueue();
        }

        private void RenewQueue()
        {
            _currentQueue = NightJobManager.NewQueue();
        }

        public Dot After(Action mainThreadAction)
        {
            _currentQueue.AddJobToQueue(new ActionJob(mainThreadAction, null, null, null));
            return this;
        }

        public Dot After(Action<List<object>> threadActionWithParams, bool secondaryThread = false)
        {
            _currentQueue.AddJobToQueue(
                new ActionJob(null, 
                    secondaryThread ? null : threadActionWithParams, 
                    null, secondaryThread ? threadActionWithParams : null));
            return this;
        }

        public Dot After(AsyncDelegate secondaryThreadAsyncDelegate)
        {
            _currentQueue.AddJobToQueue(
                new ActionJob(null, null, 
                    secondaryThreadAsyncDelegate, null));
            return this;
        }
        
        public Dot After(Func<Task> secondaryThreadTask)
        {
            _currentQueue.AddJobToQueue(
                new TaskJob(secondaryThreadTask));
            return this;
        }
        
        public Dot After(Action<List<object>> mainThreadActionWithContainer, Action<List<object>> secondaryThreadActionWithContainer)
        {
            _currentQueue.AddJobToQueue(
                new ActionJob(null, mainThreadActionWithContainer, 
                    null, secondaryThreadActionWithContainer));
            return this;
        }

        public Dot Wait(float seconds)
        {
            var time = (int)(seconds * 1000);
            After(async _ => await Task.Delay(time));
            return this;
        }

        public Dot Parallel()
        {
            RenewQueue();
            return this;
        }

        public Dot Start(Action action)
        {
            NightJobManager.EnqueueTask(action);
            return this;
        }
        
        public Dot Loop(Func<bool> loopFunc)
        {
            After(async () =>
            {
                while (NightJobManager.gamePlaying && loopFunc()) await Task.Yield();
            });
            return this;
        }
        
        /// <summary>
        /// IMPORTANT: You must use .Parallel() if you want to continue execution if you are executing this forever.
        /// </summary>
        /// <param name="loopFunc"></param>
        /// <param name="seconds"></param>
        /// <param name="secondThread"></param>
        /// <returns></returns>
        public Dot Loop(Func<bool> loopFunc, float seconds, bool secondThread = false)
        {
            if (secondThread) After(async _ =>
            {
                while (NightJobManager.gamePlaying && loopFunc()) await Task.Delay((int)(seconds * 1000));
            });
            else After(async () =>
            {
                while (NightJobManager.gamePlaying && loopFunc()) await Task.Delay((int)(seconds * 1000));
            });
            return this;
        }
        
        public Dot Loop(Func<List<object>, List<object>> secondThread, Action<List<object>> mainThread, int milliseconds)
        {
            After(async _ =>
            {
                while (NightJobManager.gamePlaying)
                {
                    var result = secondThread(new List<object>());
                    await NightJobManager.WaitForMainThread(() =>
                    {
                        mainThread(result);
                    });
                    await Task.Delay(milliseconds);
                }
            });
            Parallel();
            return this;
        }

        private readonly List<LoopData> _loopHolder = new List<LoopData>();

        public Dot Loop(string name, DeltaTimeDelegate secondThread, Action mainThread, CancellationToken cancellationToken)
        {
            Loop(name, secondThread, mainThread);
            cancellationToken.Register(() =>
            {
                var possible = _loopHolder.Find(x => x.name == name);
                if (possible == null) return; 
                possible.secondThread -= secondThread;
                possible.mainThread -= mainThread;
            });
            return this;
        }

        public Dot Loop(string name, DeltaTimeDelegate secondThread, Action mainThread)
        {
            var timer = new Stopwatch();
            float deltaTime;
            var possible = _loopHolder.Find(x => x.name == name);
            if (possible is not { started: true })
            {
                if (possible is not { started: false })
                {
                    possible = new LoopData(name, secondThread, mainThread);
                    _loopHolder.Add(possible);
                }

                possible.started = true;
                After(async _ =>
                {
                    timer.Start();
                    while (NightJobManager.gamePlaying)
                    {
                        // Calculate deltaTime in seconds
                        deltaTime = (float)timer.Elapsed.TotalSeconds;

                        timer.Restart();
                        possible.secondThread(deltaTime);

                        try
                        {
                            await NightJobManager.WaitForMainThread(possible.mainThread);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Cannot create instance in secondary thread. " +
                                      "Resetting all data due error." +
                                      "\nAdditional error data: " + e);
                            ClearEverything();
                            return;
                        }
                    }
                });
                Parallel();
            }
            else
            {
                possible.secondThread += secondThread;
                possible.mainThread += mainThread;
            }

            return this;
        }
        
        /// <summary>
        /// NOTE: This method cannot start a new thread. Instead this will be started when the thread is started.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="secondThread"></param>
        /// <returns></returns>
        public Dot Loop(string name, DeltaTimeDelegate secondThread)
        {
            var possible = _loopHolder.Find(x => x.name == name) 
                           ?? new LoopData(name, secondThread, default);
            possible.secondThread += secondThread;
            return this;
        }
        

        /// <summary>
        /// It reset whole trail and whole jobs in game.
        /// Most commonly used when changing between scenes.
        /// </summary>
        public void ClearEverything()
        {
            _loopHolder.Clear();
            _instance = null;
            NightJobManager.Dispose();
        }
        
        public static Task DummyTask(Action secondaryThread)
        {
            var task = Task.Run(secondaryThread);
            task.ContinueWith((task1) =>
            {
                if (task1.Exception == null) return;
                foreach (var ex in task1.Exception.InnerExceptions)
                {
                    Debug.LogException(ex);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
            
            return task;
        }
    }

    public class LoopData
    {
        public string name;
        public bool started;
        public DeltaTimeDelegate secondThread;
        public Action mainThread;

        public LoopData(string name, DeltaTimeDelegate secondThread, Action mainThread)
        {
            this.name = name;
            this.secondThread = secondThread;
            this.mainThread = mainThread;
        }
    }
}
