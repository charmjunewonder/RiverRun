using UnityEngine;
using System.Collections;

public class PlayAudioWindows : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if UNITY_STANDALONE_WIN
        GetComponent<AudioSource>().Play();
#endif
	}
}
