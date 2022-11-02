using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;

    public void PlayClip(AudioClip ac)
    {
        source.clip = ac;
        source.Play();
    }

}
