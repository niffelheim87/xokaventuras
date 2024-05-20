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


    // Encuentra y almacena referencias a los objetos MenuDialog, SayDialog, CanvasGroup y Flowchart en la escena.
    void Start()
    {
        menuDialogs = FindObjectsOfType<MenuDialog>();
        sayDialogs = FindObjectsOfType<SayDialog>();
        canvasGroup = GetComponent<CanvasGroup>();
        flowCharts = FindObjectsOfType<Flowchart>();
        target = FindObjectOfType<Target>();
    }

    // Si se presiona la tecla "I" y el personaje no está en diálogo, se llama a la función ToggleInventory para mostrar u ocultar el inventario.
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)&& target.inDialog !=true)
        {
            ToggleInventory(!canvasGroup.interactable);
        }
    }

    public void ToggleInventory(bool setting)
    {
        // Activa o desactiva el CanvasGroup del inventario según el parámetro setting.
        ToggleCanvasGroup(canvasGroup, setting);
        // Llama a la función InitializeItemSlot para actualizar los slots del inventario.
        InitializeItemSlot();
        
        if(!target.cutSceneInProgress)
        {
            // Establece la propiedad inDialog del objeto Target según el parámetro setting.
            target.inDialog = setting;
        }

        // Activa o desactiva los CanvasGroup de los objetos MenuDialog y SayDialog.
        foreach (MenuDialog menuDialog in menuDialogs)
        {
            ToggleCanvasGroup(menuDialog.GetComponent<CanvasGroup>(),!setting);            
        }

        foreach (SayDialog sayDialog in sayDialogs)
        {
            sayDialog.dialgenabled = !setting;
            // Ajusta la escala de tiempo según si el inventario está abierto o cerrado.
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

    /* 
    Obtiene una lista de los ítems que el jugador posee llamando a la función GetOwnedItems
    Recorre los slots del inventario y muestra u oculta los ítems según la lista de ítems poseídos.
    */
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

    // Crea una nueva lista que contiene solo los ítems que el jugador posee (cuya propiedad itemOwned es verdadera).
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
        // Verifica si ambos ítems son combinables.
        if (item1.combinable == true && item2.combinable == true)
        {
            SetMouseCursor.ResetMouseCursor();
            for (int i = 0; i < item1.combinableItems.Length; i++)
            {
                //  Si son combinables, busca en los bloques de los Flowcharts si hay un bloque de éxito correspondiente a la combinación.
                if (item1.combinableItems[i] == item2)
                {
                    foreach (Flowchart flowchart in flowCharts)
                    {
                        //  Si hay un bloque de éxito, cierra el inventario, entra en diálogo y ejecuta el bloque de éxito.
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

        // Si no hay un bloque de éxito, busca y ejecuta el bloque de fallo.
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
    // Activa o desactiva el CanvasGroup según el parámetro setting, ajustando su transparencia, interactividad y capacidad de recibir raycast.
    public void ToggleCanvasGroup(CanvasGroup canvasGroup,bool setting)
    {
        canvasGroup.alpha = setting ? 1f : 0f;
        canvasGroup.interactable = setting;
        canvasGroup.blocksRaycasts = setting;
    }
}
