using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fungus;

public class Verbs : MonoBehaviour
{
    // Textura del cursor del mouse
    public Texture2D mouseTexture;

    // Cadenas de texto para los verbos "Walk" y "Use"
    public string walkString = " Andar hacia ";
    public string useString = " Usar en ";

    // Nombre del objeto interactivo actual
    public string currentClickable;

    // Objeto de inventario actual
    public InventoryItem currentItem;

    // Nombre del slot de inventario sobre el que se está pasando el mouse
    public string hoveredItemSlot;

    // Indica si los objetos son combinables
    public bool combinability;

    // Referencia al componente Inventory
    public Inventory inventory;

    // Enum para los posibles verbos de acción
    public enum Action { Walk, Use };

    // Verbo de acción actual
    public Action verb = Action.Walk;

    // Referencia al componente TextMeshProUGUI para el cuadro de texto del verbo de acción
    private TextMeshProUGUI verbTextBox;

    // Array de componentes Flowchart
    private Flowchart[] flowcharts;

    // Indica si el modo "Use" está activo
    public bool isUseActive;

    // Inicializamos los componentes
    void Start()
    {
        // Obtiene el componente TextMeshProUGUI que es hijo de este objeto y lo asigna a verbTextBox
        verbTextBox = GetComponentInChildren<TextMeshProUGUI>();

        // Establece el texto del cuadro de texto del verbo como una cadena vacía
        verbTextBox.text = "";

        // Encuentra todos los objetos de tipo Flowchart en la escena y los asigna al array flowcharts
        flowcharts = FindObjectsOfType<Flowchart>();

        // Encuentra el objeto de tipo Inventory en la escena y lo asigna a la variable inventory
        inventory = FindObjectOfType<Inventory>();

    }

    // Actualiza el texto del flowchart en base si estamos andando o usando un objeto, desactivando la acción opuesta para evitar errores
    public void UpdateVerbTextBox(string currentClickable)
    {
        // Establece el verbo actual en los flowcharts
        SetVerbInFlowchart();

        // Si el verbo actual es "Walk"
        if (verb == Action.Walk)
        {
            // Establece que no se pueden combinar objetos
            combinability = false;

            // Actualiza el texto del cuadro de texto del verbo con la cadena "Andar hacia " y el nombre del objeto interactivo actual
            verbTextBox.text = walkString + currentClickable;

            // Establece que el modo "Use" no está activo
            isUseActive = false;

        }
        // Si el verbo actual es "Use"
        else if (verb == Action.Use)
        {
            // Establece que el modo "Use" está activo
            isUseActive = true;

            // Si el inventario está activo (interactable)
            if (inventory.canvasGroup.interactable == true)
            {
                // Establece que se pueden combinar objetos
                combinability = true;

                // Actualiza el texto del cuadro de texto del verbo con la cadena "Usar" y los nombres del objeto actual y el slot de inventario sobre el que se está pasando el mouse
                verbTextBox.text = useString + " " + currentItem.itemName + " con " + hoveredItemSlot;

                // Establece el cursor del mouse a la icono del objeto actual
                Cursor.SetCursor(currentItem.mouseIcon, Vector2.zero, CursorMode.Auto);
            }
            // Si no hay un objeto interactivo actual
            else if (currentClickable == null)
            {
                // Actualiza el texto del cuadro de texto del verbo con la cadena "Usar" y el nombre del objeto actual
                verbTextBox.text = useString + " " + currentItem.itemName + " con ";
            }
            // Si hay un objeto interactivo actual
            else
            {
                // Establece que no se pueden combinar objetos
                combinability = false;

                // Actualiza el texto del cuadro de texto del verbo con la cadena "Usar" y los nombres del objeto actual y el objeto interactivo actual
                verbTextBox.text = useString + " " + currentItem.itemName + " con " + currentClickable;

            }

        }
    }

    // Actualizamos las variables en el Flowchart y comprobamos si el flowchart contiene currentitem para gestionar el item actual para la combinación de items
    public void SetVerbInFlowchart()
    {
        // Iteramos sobre todos los flowcharts en la escena
        foreach (Flowchart flowchart in flowcharts)
        {
            // Comprobamos si el flowchart tiene una variable llamada "verb"
            if (flowchart.HasVariable("verb"))
            {
                // Establecemos el valor de la variable "verb" en el flowchart con el valor actual del verbo
                flowchart.SetStringVariable("verb", verb.ToString());
            }

            // Comprobamos si el flowchart tiene una variable llamada "currentItem"
            if (currentItem == null) { return; }
            if (flowchart.HasVariable("currentItem"))
            {
                // Establecemos el valor de la variable "currentItem" en el flowchart con el nombre del item actual
                flowchart.SetStringVariable("currentItem", currentItem.itemName);
            }
        }
    }
}
