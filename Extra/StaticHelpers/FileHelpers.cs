using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileHelpers
{
    /// <summary>
    /// Saves a file to the StreamingAssets folder, creating the directory if it doesn't exist.
    /// </summary>
    /// <param name="fileName">The name of the file to save (e.g., "data.json").</param>
    /// <param name="data">The string data to save to the file.</param>
    /// <param name="subFolder">Optional subfolder within StreamingAssets where the file will be saved.</param>
    public static void Save(this string data, string fileName, string subFolder = "")
    {
        // Construct the full path to the StreamingAssets folder
        string path = Application.streamingAssetsPath;

        // If a subfolder is specified, append it to the path
        if (!string.IsNullOrEmpty(subFolder))
        {
            path = Path.Combine(path, subFolder);
        }

        // Ensure the directory exists
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Combine the path with the file name
        string filePath = Path.Combine(path, fileName);

        // Write the data to the file
        File.WriteAllText(filePath, data);

        Debug.Log($"File saved to: {filePath}");
    }

    public static void RemoveFile(string path)
    {
        // Ensure the path is not null or empty
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is null or empty.");
            return;
        }

        try
        {
            // Check if the file exists
            if (File.Exists(path))
            {
                // Delete the file
                File.Delete(path);
                Debug.Log($"File deleted: {path}");
            }
            else
            {
                Debug.LogWarning("File not found at the specified path.");
            }
        }
        catch (IOException ex)
        {
            // Log any IO exceptions that occur
            Debug.LogWarning($"An error occurred while deleting the file: {ex.Message}");
        }
    }
    
    public static bool ChangeFileName(string path, string newFileName)
    {
        // Ensure the path is not null or empty
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(newFileName))
        {
            Debug.LogWarning("Path or new file name is null or empty.");
            return false;
        }

        // Get the directory and current file name
        string directory = Path.GetDirectoryName(path);
        string currentFileName = Path.GetFileName(path);
        string fileExtension = Path.GetExtension(path);

        // Combine the directory and new file name to get the new file path
        string newFilePath = Path.Combine(directory, newFileName + fileExtension);

        try
        {
            // Check if the file exists at the given path
            if (File.Exists(path))
            {
                // Rename the file
                File.Move(path, newFilePath);
                Debug.Log($"File renamed to: {newFileName}");
                return true;
            }
            
            Debug.LogWarning("File not found at the specified path.");
        }
        catch (IOException ex)
        {
            // Log any IO exceptions that occur
            Debug.LogWarning($"An error occurred while renaming the file: {ex.Message}");
        }
        
        return false;
    }
    
    public static void SaveByFullPath(this string data, string filePath)
    {
        File.WriteAllText(filePath, data);

        Debug.Log($"File saved to: {filePath}");
    }
    
    public static List<string> GetTopLevelFolderPaths(string path)
    {
        List<string> folderPaths = new List<string>();

        if (Directory.Exists(path))
        {
            // Get only directories (top-level folders)
            string[] directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (string directory in directories)
            {
                folderPaths.Add(directory);
            }
        }
        else
        {
            Debug.LogWarning($"The StreamingAssets path does not exist: {path}");
        }

        return folderPaths;
    }
    
    public static string GetLastEditTimeFormatted(string path)
    {
        if (File.Exists(path) || Directory.Exists(path))
        {
            DateTime lastEditTime = File.GetLastWriteTime(path);
            string formattedTime = lastEditTime.ToString("d MMM h:mm tt");
            return formattedTime;
        }
        else
        {
            Debug.LogWarning($"The path does not exist: {path}");
            return null;
        }
    }
    
    public static string NormalizePath(string path)
    {
        return path.Replace("\\", "/");
    }
    
    public static string ConvertToModsPath(string originalPath)
    {
        string streamingAssetsPath = Application.streamingAssetsPath;

        if (originalPath.StartsWith(streamingAssetsPath))
        {
            string remainingPath = originalPath.Substring(streamingAssetsPath.Length);
            return Path.Combine("Mods", remainingPath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        return originalPath;
    }
    
    public static void CreateFolders(string parentFolderPath, params string[] subFolders)
    {
        // Ensure the parent folder exists
        if (!Directory.Exists(parentFolderPath))
        {
            Directory.CreateDirectory(parentFolderPath);
        }

        // Create each subfolder
        foreach (string subFolder in subFolders)
        {
            string subFolderPath = Path.Combine(parentFolderPath, subFolder);

            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }
        }
    }
}