using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField]
    private SimpleAudioEvent audio;

    public void PlaySound()
    {
        AudioManager.Instance.PlaySoundEffect(audio);
    }
}
