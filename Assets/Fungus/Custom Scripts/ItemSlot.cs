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
        //Encuentra y almacena referencias a los objetos Inventory, Verbs y Target en la escena.
        inventory = FindObjectOfType<Inventory>();
        verb = FindObjectOfType<Verbs>();
        target = FindObjectOfType<Target>();
        // Encuentra y almacena una referencia al componente TextMeshProUGUI de este objeto.
        textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Muestra el ítem en el inventario
    public void DisplayItem(InventoryItem thisItem)
    {
        // Almacena la referencia al objeto InventoryItem en la variable item.
        item = thisItem;
        // Establece el texto del TextMeshProUGUI con el nombre del ítem.
        textBox.text = item.itemName;
        // Establece el sprite del componente Image con el icono del ítem.
        image.sprite = item.itemIcon;
        // Activa el objeto.
        gameObject.SetActive(true);
    }

    // Quita el ítem del inventario
    public void ClearItem()
    {
        // Establece la variable item a null.
        item = null;
        // Establece el sprite del componente Image a null.
        image.sprite = null;
        // Desactiva el objeto.
        gameObject.SetActive(false);
    }


    public void OnItemClick()
    {

        // Si hay una escena en progreso, no hace nada y sale de la función.
        if (target.cutSceneInProgress) { return; }
        // Si el verbo actual es "Use" y hay un ítem seleccionado, llama a la función CombineItems del objeto Inventory pasando el ítem actual y el ítem de este slot
        if(verb.verb == Verbs.Action.Use && verb.currentItem != null)
        {
            inventory.CombineItems(verb.currentItem, item);
             
        }
        // Establece el verbo actual a "Use".
        verb.verb = Verbs.Action.Use;
        // Establece el ítem actual a este ítem.
        verb.currentItem = item;
        // Llama a la función UpdateVerbTextBox del objeto Verbs pasando null.
        verb.UpdateVerbTextBox(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Establece el nombre del ítem actual en la variable hoveredItemSlot del objeto Verbs.
        verb.hoveredItemSlot = item.itemName;
        // Llama a la función UpdateVerbTextBox del objeto Verbs pasando null.
        verb.UpdateVerbTextBox(null);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Establece la variable hoveredItemSlot del objeto Verbs a null.
        verb.hoveredItemSlot = null;
        // Llama a la función UpdateVerbTextBox del objeto Verbs pasando null.
        verb.UpdateVerbTextBox(null);
    }
}
