using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    public static AudioController Singleton;
    public AudioSource aus;
    public AudioClip goodFeedBack;
    public AudioClip badFeedBack;
    public AudioClip bloodLowSound;
    void Start()
    {
        Singleton = this;
	}

    public void PlayGoodFeedBack()
    {
        aus.Stop();
        aus.clip = goodFeedBack;
        aus.Play();
    }

    public void PlayBadFeedBack()
    {
        aus.Stop();
        aus.clip = badFeedBack;
        aus.Play();
    }

    public void PlayBloodLowSound()
    {
        aus.Stop();
        aus.clip = bloodLowSound;
        aus.Play();
    }
}
