using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setbook : MonoBehaviour
{
    public GameObject book;
    public GameObject mainbutton;//"查看按钮"
    public GameObject bookexitbutton;//书本退出按钮

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
