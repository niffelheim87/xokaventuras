using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Item",
                 "Change Item",
                 "Add or Remove an Item from the Inventory")]
    [AddComponentMenu("")]
    public class ChangeItem :Command
    {
        [Tooltip("Reference to an Inventory scriptable object that fill the Itemslots in the inventory")]
        [SerializeField] protected InventoryItem item;

        [Tooltip("if add is true item will add to the inventory if add is false item will be removed from inventory")]
        [SerializeField] protected bool add;

        public override void OnEnter()
        {
            if(item != null)
            {
                if(add)
                {
                    item.itemOwned = true;
                }else
                {
                    item.itemOwned = false;
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (item == null)
            {
                return "Error: No item selected";
            }

            return item.itemName;
        }
    }
}

