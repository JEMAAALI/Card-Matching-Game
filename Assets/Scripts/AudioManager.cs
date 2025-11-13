using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip matchSound;
    [SerializeField] private AudioClip notMatchSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    public void PlayMatch() => source.PlayOneShot(matchSound);
    public void PlayNotMatch() => source.PlayOneShot(notMatchSound);
    public void PlayWin() => source.PlayOneShot(winSound);
    public void PlayLose() => source.PlayOneShot(loseSound);

    /*public void PlayMatchSound()
    {
        // Play your match audio clip
        GetComponent<AudioSource>().Play();
    }*/
}
