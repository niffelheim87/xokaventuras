using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public InventoryItem item;
    private Inventory inventory;
    public Image image;
    private TextMeshProUGUI textBox;
    private Verbs verb;
    private Target target;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        textBox = GetComponentInChildren<TextMeshProUGUI>();
        verb = FindObjectOfType<Verbs>();
        target = FindObjectOfType<Target>();
    }

    public void DisplayItem(InventoryItem thisItem)
    {
        item = thisItem;
        textBox.text = item.itemName;
        image.sprite = item.itemIcon;
        gameObject.SetActive(true);
    }

    public void ClearItem()
    {
        item = null;
        image.sprite = null;
        gameObject.SetActive(false);
    }

    public void OnItemClick()
    {

        if (target.cutSceneInProgress) { return; }
        if(verb.verb == Verbs.Action.Use && verb.currentItem != null)
        {
            inventory.CombineItems(verb.currentItem, item);
             
        }

        verb.verb = Verbs.Action.Use;
        verb.currentItem = item;
        verb.UpdateVerbTextBox(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        verb.hoveredItemSlot = item.itemName;
        verb.UpdateVerbTextBox(null);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        verb.hoveredItemSlot = null;
        verb.UpdateVerbTextBox(null);
    }
}
