using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class FileExplorer : Singleton<FileExplorer>
{
    public Transform content;
    public TMP_Text targetFileTypeText;
    public TMP_Text pathText;
    public FileRect fileRectPrefab;

    public string selectedFilePath;
    public string currentParentPath;

    private Action<string> onCompletedAction;
    private string[] currentFileTypes;

    public static void ShowMods(Action<string> onCompleted, params string[] fileTypes)
    {
        Instance.ShowPath(Application.streamingAssetsPath, onCompleted, fileTypes);
    }
    
    public void ShowPath(string path, Action<string> onCompleted = null, params string[] fileTypes)
    {
        gameObject.SetActive(true);

        Clear();
        
        if(onCompleted != null) onCompletedAction = onCompleted;

        currentParentPath = path;
        currentFileTypes = fileTypes;
        targetFileTypeText.text = fileTypes.Length > 0 ? string.Join(", ", fileTypes.Select(f => "." + f)) : "All Files";

        
        pathText.text = FileHelpers.ConvertToModsPath(path);

        List<string> entries = new List<string>();
        entries.AddRange(Directory.GetDirectories(path));
        entries.AddRange(Directory.GetFiles(path));

        foreach (string entry in entries)
        {
            FileType fileType = Directory.Exists(entry) ? FileType.Folder : FileType.Data;
            
            var extension = Path.GetExtension(entry).TrimStart('.');
            
            if (fileType == FileType.Data && fileTypes.Length > 0 && !fileTypes.ToList().Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }
            
            FileRect fileRect = Instantiate(fileRectPrefab, content);
            string entryName = Path.GetFileName(entry);
            
            
            fileRect.Apply(fileType, entryName);
            fileRect.button.onClick.AddListener(() =>
            {
                if (fileType == FileType.Folder) ShowPath(entry, null, fileTypes);
                else selectedFilePath = entry;
            });
        }
    }

    public void GoBack()
    {
        if (currentParentPath == Application.streamingAssetsPath) return;
        
        DirectoryInfo parentDirectory = Directory.GetParent(currentParentPath);

        if (parentDirectory == null) return;
        
        ShowPath(FileHelpers.NormalizePath(parentDirectory.FullName), null, currentFileTypes);
    }

    public void Complete()
    {
        onCompletedAction(selectedFilePath);
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    public void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        selectedFilePath = "";
    }
}