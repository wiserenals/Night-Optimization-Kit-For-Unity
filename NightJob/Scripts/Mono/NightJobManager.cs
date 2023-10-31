using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NightJob;
using UnityEngine;

[ExecuteAlways]
public class NightJobManager : ForceProtectedSingleton<NightJobManager>
{
    public readonly List<NightJobQueue> AllQueue = new List<NightJobQueue>();
    
    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    public static bool gamePlaying = true;

    public static void Dispose()
    {
        Instance = null;
    }
    
    public static NightJobQueue NewQueue()
    {
        return Instance._NewQueue();
    }
    
    private void Update()
    {
        #if UNITY_EDITOR
        gamePlaying = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
        #endif
        while (_actions.TryDequeue(out var action))
        {
            action.Invoke();
        }
    }

    public static void EnqueueTask(Action action)
    {
        Instance._actions.Enqueue(action);
    }

    private void _EnqueueTask(Action action)
    {
        _actions.Enqueue(action);
    }

    public static Task<T> WaitForMainThread<T>(Func<T> function)
    {
        var tcs = new TaskCompletionSource<T>();
        Instance._EnqueueTask(() => tcs.SetResult(function()));
        return tcs.Task;
    }
    
    public static Task WaitForMainThread(Action action)
    {
        var tcs = new TaskCompletionSource<object>();
        Instance._EnqueueTask(() =>
        {
            action();
            tcs.SetResult(null);
        });
        return tcs.Task;
    }
    
    public static Task WaitForMainThread(Func<Task> function)
    {
        var tcs = new TaskCompletionSource<object>();
        Instance._EnqueueTask(async () =>
        {
            await function();
            tcs.SetResult(null);
        });
        return tcs.Task;
    }

    private static float yieldTemp;

    public static async Task Yield(float times)
    {
        yieldTemp %= 1;
        yieldTemp += times;
        while (yieldTemp >= 1)
        {
            yieldTemp--;
            await Task.Yield();
        }
    }
    
    private NightJobQueue _NewQueue()
    {
        var queue = new NightJobQueue(this);
        AllQueue.Add(queue);
        return queue;
    }

    private void _DisposeAll()
    {
        for (int i = 0; i < AllQueue.Count; i++)
        {
            AllQueue[i].Dispose();
        }
    }

    private void OnDestroy()
    {
        _DisposeAll();
    }
}
