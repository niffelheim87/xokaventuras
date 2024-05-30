using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // Array de componentes MenuDialog en la escena
    private MenuDialog[] menuDialogs;

    // Array de componentes SayDialog en la escena
    private SayDialog[] sayDialogs;

    // Referencia al componente CanvasGroup del inventario
    public CanvasGroup canvasGroup;

    // Referencia al componente Target
    private Target target;

    // Array de objetos InventoryItem
    public InventoryItem[] inventoryItem;

    // Array de slots de inventario
    public ItemSlot[] itemslots;

    // Array de componentes Flowchart en la escena
    private Flowchart[] flowCharts;


    // Encuentra y almacena referencias a los objetos MenuDialog, SayDialog, CanvasGroup y Flowchart en la escena
    void Start()
    {
        // Encuentra todas las instancias de MenuDialog en la escena y las asigna al array menuDialogs
        menuDialogs = FindObjectsOfType<MenuDialog>();

        // Encuentra todas las instancias de SayDialog en la escena y las asigna al array sayDialogs
        sayDialogs = FindObjectsOfType<SayDialog>();

        // Obtiene el componente CanvasGroup adjunto a este objeto y lo asigna a la variable canvasGroup
        canvasGroup = GetComponent<CanvasGroup>();

        // Encuentra todas las instancias de Flowchart en la escena y las asigna al array flowCharts
        flowCharts = FindObjectsOfType<Flowchart>();

        // Encuentra la instancia de Target en la escena y la asigna a la variable target
        target = FindObjectOfType<Target>();
    }

    // Si se presiona la tecla "I" y el personaje no está en diálogo, se llama a la función ToggleInventory para mostrar u ocultar el inventario
    void Update()
    {
        // Verifica si se presionó la tecla "I" (KeyCode.I)
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Verifica si el personaje no está en diálogo (target.inDialog != true)
            if (target.inDialog != true)
            {
                // Llama a la función ToggleInventory pasando el estado opuesto de la interactividad del canvasGroup
                ToggleInventory(!canvasGroup.interactable);
            }
        }
    }

    public void ToggleInventory(bool setting)
    {
        // Activa o desactiva el CanvasGroup del inventario según el parámetro setting.
        ToggleCanvasGroup(canvasGroup, setting);

        // Llama a la función InitializeItemSlot para actualizar los slots del inventario.
        InitializeItemSlot();

        // Si no hay una escena en progreso, establece la propiedad inDialog del objeto Target según el parámetro setting.
        if (!target.cutSceneInProgress)
        {
            target.inDialog = setting;
        }

        // Activa o desactiva los CanvasGroup de los objetos MenuDialog y SayDialog.
        foreach (MenuDialog menuDialog in menuDialogs)
        {
            ToggleCanvasGroup(menuDialog.GetComponent<CanvasGroup>(), !setting);
        }

        foreach (SayDialog sayDialog in sayDialogs)
        {
            // Activa o desactiva la capacidad de diálogo en SayDialog según el parámetro setting.
            sayDialog.dialgenabled = !setting;

            // Ajusta la escala de tiempo según si el inventario está abierto o cerrado.
            if (setting)
            {
                Time.timeScale = 0f; // Pausa el tiempo si el inventario está abierto
            }
            else
            {
                Time.timeScale = 1f; // Restaura el tiempo a la velocidad normal si el inventario está cerrado
            }

            // Activa o desactiva el CanvasGroup de SayDialog según el parámetro setting.
            ToggleCanvasGroup(sayDialog.GetComponent<CanvasGroup>(), !setting);
        }
    }

    public void InitializeItemSlot()
    {
        // Obtiene una lista de los ítems que el jugador posee llamando a la función GetOwnedItems
        List<InventoryItem> ownedItems = GetOwnedItems(inventoryItem.ToList());

        // Recorre los slots del inventario
        for (int i = 0; i < itemslots.Length; i++)
        {
            // Si el índice actual es menor que la cantidad de ítems poseídos
            if (i < ownedItems.Count)
            {
                // Muestra el ítem actual en el slot correspondiente
                itemslots[i].DisplayItem(ownedItems[i]);
            }
            else
            {
                // Si no hay más ítems poseídos, limpia el slot
                itemslots[i].ClearItem();
            }
        }
    }

    // Crea una nueva lista que contiene solo los ítems que el jugador posee (cuya propiedad itemOwned es verdadera)
    public List<InventoryItem> GetOwnedItems(List<InventoryItem> inventoryItems)
    {
        // Crea una nueva lista para almacenar los ítems poseídos
        List<InventoryItem> ownedItems = new List<InventoryItem>();

        // Recorre la lista de ítems de inventario
        foreach (InventoryItem item in inventoryItems)
        {
            // Verifica si el jugador posee el ítem (item.itemOwned es verdadero)
            if (item.itemOwned)
            {
                // Si el jugador posee el ítem, lo agrega a la lista de ítems poseídos
                ownedItems.Add(item);
            }
        }

        // Devuelve la lista de ítems poseídos
        return ownedItems;
    }

    public void CombineItems(InventoryItem item1, InventoryItem item2)
    {
        // Verifica si ambos ítems son combinables
        if (item1.combinable == true && item2.combinable == true)
        {
            // Restablece el cursor del mouse a su estado predeterminado
            SetMouseCursor.ResetMouseCursor();

            // Recorre el array de ítems combinables del primer ítem
            for (int i = 0; i < item1.combinableItems.Length; i++)
            {
                // Si el segundo ítem es combinable con el primer ítem
                if (item1.combinableItems[i] == item2)
                {
                    // Recorre los Flowcharts en busca de un bloque de éxito correspondiente a la combinación
                    foreach (Flowchart flowchart in flowCharts)
                    {
                        // Si se encuentra un bloque de éxito
                        if (flowchart.HasBlock(item1.successblockName[i]))
                        {
                            // Cierra el inventario
                            ToggleInventory(false);

                            // Entra en diálogo
                            target.EnterDialogue();

                            // Ejecuta el bloque de éxito en el Flowchart
                            flowchart.ExecuteBlock(item1.successblockName[i]);

                            // Sale de la función
                            return;
                        }
                    }
                }
            }
        }

        // Si no se encontró un bloque de éxito, busca y ejecuta el bloque de fallo
        foreach (Flowchart flowchart in flowCharts)
        {
            if (flowchart.HasBlock(item1.failBlockName))
            {
                // Restablece el cursor del mouse a su estado predeterminado
                SetMouseCursor.ResetMouseCursor();

                // Cierra el inventario
                ToggleInventory(false);

                // Entra en diálogo
                target.EnterDialogue();

                // Ejecuta el bloque de fallo en el Flowchart
                flowchart.ExecuteBlock(item1.failBlockName);
            }
        }
    }

    // Activa o desactiva el CanvasGroup según el parámetro setting, ajustando su transparencia, interactividad y capacidad de recibir raycast
    public void ToggleCanvasGroup(CanvasGroup canvasGroup, bool setting)
    {
        // Establece la transparencia del CanvasGroup según el parámetro setting
        canvasGroup.alpha = setting ? 1f : 0f;

        // Establece la interactividad del CanvasGroup según el parámetro setting
        canvasGroup.interactable = setting;

        // Establece la capacidad del CanvasGroup para recibir raycast según el parámetro setting
        canvasGroup.blocksRaycasts = setting;
    }
}