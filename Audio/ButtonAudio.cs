using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public SoundName button;

    private void Start()
    { 
        // 将按钮的 onClick 事件绑定到 PlaySound 方法
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