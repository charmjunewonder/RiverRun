using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialShiningText : MonoBehaviour {

    public string s;
    private float t;

	void Start () {
	    s = GetComponent<Text>().text;
        t = 0.5f;
	}
	
	void Update () {
	    t -= Time.deltaTime;

        if (t <= 0) {
            t = 0.5f;
            if(GetComponent<Text>().text == "")
                GetComponent<Text>().text = s;
            else
                GetComponent<Text>().text = "";
        }
	}
}
