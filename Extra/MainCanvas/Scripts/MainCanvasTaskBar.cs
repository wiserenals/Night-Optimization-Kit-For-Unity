using System;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasTaskBar : Singleton<MainCanvasTaskBar>
{
    public MainCanvasTaskComponent taskPrefab;
    private List<MainCanvasTask> tasks = new List<MainCanvasTask>();

    public void AddTask(MainCanvasTask task)
    {
        task._instance = Instantiate(taskPrefab, transform);
        task._instance.task = task;
        
        tasks.Add(task);
    }

    public void RemoveTask(MainCanvasTask task)
    {
        Destroy(task._instance.gameObject);
        tasks.Remove(task);
    }


}
public class MainCanvasTask : IDisposable
{
    public readonly string name;
    public string details;
    public float completedAmount01;
    public MainCanvasTaskComponent _instance;

    public MainCanvasTask(string name, bool show = false)
    {
        this.name = name;
        if(show) Show();
    }

    public void Show()
    {
        MainCanvasTaskBar.Instance.AddTask(this);
    }
    
    public void Dispose()
    {
        Debug.Log("Waouw");
        MainCanvasTaskBar.Instance.RemoveTask(this);
    }
}


