using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setbook : MonoBehaviour
{
    public GameObject book;
    public GameObject mainbutton;//"�鿴��ť"
    public GameObject bookexitbutton;//�鱾�˳���ť

    public SoundName button;
    public void setbook()
    {
        if (button != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(button);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
        book.SetActive(true);
    }
    public void exitbook()
    {
        if (button != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(button);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
        book.SetActive (false);
    }
}
