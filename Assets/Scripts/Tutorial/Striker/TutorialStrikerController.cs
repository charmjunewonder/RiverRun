using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialStrikerController : MonoBehaviour {

    public EventSystem e;
    public int stage;

    public GameObject[] clickObjects;
    public GameObject[] textObjects;
    public GameObject[] dragObjects;

    private int textNum;

	void Start () {
        textNum = 0;
        stage = 0;
	}
	
	void Update () {
	    if (Input.GetMouseButton(0)) {
            if (stage == 0) {
                clickObjects[0].SetActive(true);
                textObjects[0].SetActive(true);
                stage++;
            }
        }
	}


    public void SetStage(int index) {
        
    }
}
