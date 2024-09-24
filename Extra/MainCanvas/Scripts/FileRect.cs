using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileRect : MonoBehaviour
{
    public Image fileImage;
    public TMP_Text fileNameText;
    
    public Sprite folderSprite, dataSprite;

    public Button button;
    
    public void Apply(FileType fileType, string fileName)
    {
        fileImage.sprite = fileType == FileType.Folder ? folderSprite : dataSprite;
        fileNameText.text = fileName;
    }
}

public enum FileType
{
    Folder,
    Data
}
