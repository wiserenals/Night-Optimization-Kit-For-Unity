using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Debugger
{
    private static readonly string LogDirectory = Application.streamingAssetsPath + "/Debugger";
    private static readonly string LogPath = LogDirectory + "/logs.txt";
    
    static Debugger()
    {
        if (Directory.Exists(LogDirectory)) return;
        Directory.CreateDirectory(LogDirectory);
    }
    
    public static void WriteLine(string header, string line)
    {
        #if !UNITY_EDITOR
            File.AppendAllText(LogPath, 
            $"[{SceneManager.GetActiveScene().name}] [{DateTime.Now.ToShortTimeString()}] [{header}] \n " + 
            line + "\n\n");        
        #endif
    }
    
    public static void WriteLineC(string line)
    {
        #if UNITY_EDITOR
        Debug.Log(line);  
        #endif
        WriteLine("", line);
    }
    
    public static void WriteLineCError(string line)
    {
        #if UNITY_EDITOR
        Debug.LogError(line);  
        #endif
        WriteLine("ERROR", line);
    }
    
    public static void WriteLineCWarning(string line)
    {
        #if UNITY_EDITOR
        Debug.LogWarning(line);  
        #endif
        WriteLine("WARNING", line);
    }
}
