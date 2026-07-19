using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class InteractAudio : MonoBehaviour
{
   private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.volume = 0.2f;
        _audioSource.clip = _audioClip;
    }

    public void PlaySound()
    {
        _audioSource.Play();
    }

    public void PlaySoundSeparate()
    {
        AudioSource.PlayClipAtPoint(_audioClip, transform.position, 0.2f);
    }
}
