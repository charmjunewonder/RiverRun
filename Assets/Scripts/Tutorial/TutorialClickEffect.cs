using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialClickEffect : MonoBehaviour {

    public GameObject ring;

    private float size;

	void Start () {
        size = 10;
	}
	
	void Update () {
        if (size < 60)
        {
            size += Time.deltaTime * 50;
            ring.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        }
        else {
            size = 10;
        }
	}
}
