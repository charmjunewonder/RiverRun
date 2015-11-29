using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    public static AudioController Singleton;
    public AudioSource aus;
    public AudioClip goodFeedBack;
    public AudioClip badFeedBack;
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
}
