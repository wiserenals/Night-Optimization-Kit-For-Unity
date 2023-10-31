using System;
using Karayel;
using UnityEngine;

public class ExpandedBehaviour : MonoBehaviour
{
    public DebuggingSettings debuggingSettings;
    protected static GameReferenceHolder holder;
    
    protected void Log(string message)
    {
        if(debuggingSettings.messageDebugging) Debugger.WriteLineC(message);
    }
    
    protected void LogWarning(string message)
    {
        if (debuggingSettings.warningDebugging) Debugger.WriteLineCWarning(message);
    }
    
    protected void LogError(string message)
    {
        if (debuggingSettings.warningDebugging) Debugger.WriteLineCError(message);
    }
}

[Serializable]
public class DebuggingSettings
{
    public bool messageDebugging = true;
    public bool warningDebugging = true;
    public bool errorDebugging = true;
    [Space(5)] public bool drawDebugging = true;
}