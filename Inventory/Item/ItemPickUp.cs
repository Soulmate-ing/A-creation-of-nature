using MyGame.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public SoundName pickup;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if(item != null )
        {
            if(item.itemDetails.canPickedup)
            {
                if (pickup != SoundName.none)
                {
                    var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(pickup);
                    EventHandler.CallInitSoundEffect(soundDetails);
                }
                InventoryManager.Instance.AddItem(item,true);
            }
        }
    }
}
