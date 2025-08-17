using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sfx, float volume = 1)
    {
        audioSource.PlayOneShot(sfx, volume);
    }
}
