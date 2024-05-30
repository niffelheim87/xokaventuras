using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Crea un menú para crear un nuevo InventoryItem en el menú de Unity
[CreateAssetMenu(menuName = "New InventoryItem",order = 1)]
public class InventoryItem : ScriptableObject
{
    
    // Indica si el jugador posee el item
    public bool itemOwned;
    
    // Nombre del item
    public string itemName;
    
    // Sprite del icono del item
    public Sprite itemIcon;
    
    // Textura del icono del cursor del mouse para este item
    public Texture2D mouseIcon;
    
    // Indica si el item es combinable con otros items
    public bool combinable;
    
    // Array de items con los que este item es combinable
    public InventoryItem[] combinableItems;
    
    // Array de nombres de bloques de éxito para la combinación de items
    public string[] successblockName;
    
    // Nombre del bloque de fallo para la combinación de items
    public string failBlockName;
}