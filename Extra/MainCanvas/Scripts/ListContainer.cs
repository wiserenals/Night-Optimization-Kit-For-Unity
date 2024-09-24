using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListContainer : MonoBehaviour
{
    public IList currentList;
    public Transform content;
    public ListItemContainer listItemContainerPrefab;
    private VisualizerPreparation tempPreparation;
    
    public ListItemContainer CreateItemContainer()
    {
        return Instantiate(listItemContainerPrefab, content);
    }

    public void AddNewItem()
    {
        CreateNewElement("NewElement", currentList?.AddNewItem());
    }

    public void Initialize(IList iList, VisualizerPreparation preparation)
    {
        currentList = iList;
        tempPreparation = preparation;
        
        var i = 0;
        foreach (var obj in iList)
        {
            CreateNewElement("Item " + i, obj);

            i++;
        }
    }

    private void CreateNewElement(string elementName, object obj)
    {
        var itemContainer = CreateItemContainer();
        itemContainer.targetObject = obj;
        itemContainer.label.text = elementName;
        itemContainer.deleteButton.onClick.AddListener(() =>
        {
            currentList.Remove(obj);
            Destroy(itemContainer.gameObject);
        });
            
        new PropertyVisualizer(obj).Visualize(tempPreparation, itemContainer.content);
    }
}