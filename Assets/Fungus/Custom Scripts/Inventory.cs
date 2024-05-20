using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

public class Inventory : MonoBehaviour
{
    private MenuDialog[] menuDialogs;
    private SayDialog[] sayDialogs;
    public CanvasGroup canvasGroup;
    private Target target;

    public InventoryItem[] inventoryItem;
    public ItemSlot[] itemslots;
    private Flowchart[] flowCharts;


    // Start is called before the first frame update
    void Start()
    {
        menuDialogs = FindObjectsOfType<MenuDialog>();
        sayDialogs = FindObjectsOfType<SayDialog>();
        canvasGroup = GetComponent<CanvasGroup>();
        flowCharts = FindObjectsOfType<Flowchart>();
        target = FindObjectOfType<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)&& target.inDialog !=true)
        {
            ToggleInventory(!canvasGroup.interactable);
        }
    }

    public void ToggleInventory(bool setting)
    {
        ToggleCanvasGroup(canvasGroup, setting);
        InitializeItemSlot();
        if(!target.cutSceneInProgress)
        {
            target.inDialog = setting;
        }

        foreach (MenuDialog menuDialog in menuDialogs)
        {
            ToggleCanvasGroup(menuDialog.GetComponent<CanvasGroup>(),!setting);            
        }

        foreach (SayDialog sayDialog in sayDialogs)
        {
            sayDialog.dialgenabled = !setting;
            if (setting)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            ToggleCanvasGroup(sayDialog.GetComponent<CanvasGroup>(), !setting);
        }
    }

    public void InitializeItemSlot()
    {
        List<InventoryItem> ownedItems = GetOwnedItems(inventoryItem.ToList());
        for (int i = 0; i < itemslots.Length; i++)
        {
            if (i < ownedItems.Count)
            {
                itemslots[i].DisplayItem(ownedItems[i]);
            }
            else
            {
                itemslots[i].ClearItem();
            }
        }
    }

    public List<InventoryItem> GetOwnedItems(List<InventoryItem>inventoryItems)
    {
        List<InventoryItem> ownedItems = new List<InventoryItem>();
        foreach (InventoryItem item in inventoryItems)
        {
            if (item.itemOwned)
            {
                ownedItems.Add(item);
            }
        }
        return ownedItems;
    }

    public void CombineItems(InventoryItem item1,InventoryItem item2)
    {
        
        if (item1.combinable == true && item2.combinable == true)
        {
            SetMouseCursor.ResetMouseCursor();
            for (int i = 0; i < item1.combinableItems.Length; i++)
            {
                if (item1.combinableItems[i] == item2)
                {
                    foreach (Flowchart flowchart in flowCharts)
                    {
                        if(flowchart.HasBlock(item1.successblockName[i]))
                        {
                            
                            ToggleInventory(false);
                            target.EnterDialogue();
                            flowchart.ExecuteBlock(item1.successblockName[i]);
                            
                            return;
                        }
                    }
                }
            }            
        }

        foreach (Flowchart flowchart in flowCharts)
        {
            if (flowchart.HasBlock(item1.failBlockName))
            {
                SetMouseCursor.ResetMouseCursor();
                ToggleInventory(false);
                target.EnterDialogue();
                flowchart.ExecuteBlock(item1.failBlockName);
                
            }
        }
    }

    public void ToggleCanvasGroup(CanvasGroup canvasGroup,bool setting)
    {
        canvasGroup.alpha = setting ? 1f : 0f;
        canvasGroup.interactable = setting;
        canvasGroup.blocksRaycasts = setting;
    }
}
