using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public SoundName button;

    private void Start()
    { 
        // ����ť�� onClick �¼��󶨵� PlaySound ����
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (button != SoundName.none)
        {
            var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(button);
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }
}