using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fungus;

public class Verbs : MonoBehaviour
{
    public Texture2D mouseTexture;
    public string walkString = " Walk to ";
    public string useString = " Use to ";
    public string currentClickable;
    public InventoryItem currentItem;
    public string hoveredItemSlot;
    public bool combinability;
    public Inventory inventory;
    public enum Action { Walk,Use};
    public Action verb = Action.Walk;
    private TextMeshProUGUI verbTextBox;
    private Flowchart[] flowcharts;
    public bool isUseActive;
    // Start is called before the first frame update
    void Start()
    {
        verbTextBox = GetComponentInChildren<TextMeshProUGUI>();
        verbTextBox.text = "";
        flowcharts = FindObjectsOfType<Flowchart>();
        inventory = FindObjectOfType<Inventory>();
        
    }

    public void UpdateVerbTextBox(string currentClickable)
    {
        SetVerbInFlowchart();
        if(verb == Action.Walk)
        {
            combinability = false;
            verbTextBox.text = walkString + currentClickable;
            isUseActive = false;
            
        }
        else if(verb == Action.Use)
        {
            isUseActive = true;
            if (inventory.canvasGroup.interactable == true)
            {
                combinability = true;
                verbTextBox.text = useString + " " + currentItem.itemName + " with " + hoveredItemSlot;
                Cursor.SetCursor(currentItem.mouseIcon, Vector2.zero, CursorMode.Auto);

            }
            else if( currentClickable == null)
            {
                verbTextBox.text = useString + " " + currentItem.itemName + " with " ;
                
            }
            else
            {
                combinability = false;
                verbTextBox.text = useString + " " + currentItem.itemName + " with " + currentClickable;
                
            }

        }
    }

    public void SetVerbInFlowchart()
    {
        foreach (Flowchart flowchart in flowcharts)
        {
            if(flowchart.HasVariable("verb"))
            {
                flowchart.SetStringVariable("verb", verb.ToString());
            }
            if(currentItem == null) { return; }
            if (flowchart.HasVariable("currentItem"))
            {
                flowchart.SetStringVariable("currentItem", currentItem.itemName);
            }
        }
    }
}
