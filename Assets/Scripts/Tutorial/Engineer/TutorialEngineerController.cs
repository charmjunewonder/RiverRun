using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TutorialEngineerController : MonoBehaviour {

    public EventSystem e;

    public GameObject shinningText;

    public LobbyTutorialController ltController;
    public TutorialTeammatePanelController tpController;
    public CrystalProductionController cpController;
    public TutorialMainCrystalController tmcController;
    public ReminderController rController;

    public GameObject[] clickObjects;
    public GameObject[] textObjects;
    public GameObject[] dragObjects;
    public GameObject[] frames;

    public int stage;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private int textNum;

    private int crystalCounter;

	void Start () {
        textNum = 0;
	    stage = -3;
        crystalCounter = 0;
	}
	
	void Update () {

        if (stage <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log(stage);
                SetStage(stage);
            }
        }

        if (stage == 8) {
            if (Input.GetMouseButtonDown(0))
            {
                SetStage(stage);
            }
        }

        if (stage == 10) {
            if (Input.GetMouseButtonDown(0))
            {
                ltController.GoToLobbyScene();
            }
        }
	}

    public void SetStage(int index) {

        //Debug.Log(stage + " " + index);

        if (stage < 0 && index == stage)
        {
            ShowNextText();
            frames[index + 3].SetActive(true);
            if (index + 3 != 0)
                frames[index + 2].SetActive(false);
            stage++;
            return;
        }

        if (stage == 0 && index == 0) {
            frames[2].SetActive(false);
            shinningText.SetActive(false);
            ShowNextText();
            clickObjects[0].SetActive(true);
            stage++;
        }

        if (stage == 1 && index == 1) {

            clickObjects[0].SetActive(false);
            ShowNextText();

            for(int i = 1; i <= 4; i++)
                clickObjects[i].SetActive(true);
            clickObjects[10].SetActive(true);

            tpController.SetGlow();
            stage++;

            return;
        }

        if (stage == 2 && index < 0) {
            tpController.NoGlow();
            for (int i = 1; i <= 4; i++)
                clickObjects[i].SetActive(false);
            clickObjects[10].SetActive(false);

            ShowNextText();
            clickObjects[5].SetActive(true);

            stage++;

            return;
        }

        if (stage == 3 && index == 3) {
            textObjects[textNum].SetActive(false);
            clickObjects[5].SetActive(false);
            cpController.TriggerAnimation(false);

            Invoke("InvokeFunction1", 0.8f);

            stage++;
            return;
        }

        if (stage == 4 && index == 4) {
            ShowNextText();
            for (int i = 2; i <= 4; i++)
                clickObjects[i].SetActive(true);
            stage++;
            return;
        }

        if (stage == 5 && index < -1) {
            cpController.Revoke();
            for (int i = 2; i <= 4; i++)
                clickObjects[i].SetActive(false);
            ShowNextText();
            clickObjects[5].SetActive(true);

            stage++;
        }

        if (stage == 6 && index == 3) {

            clickObjects[5].SetActive(false);


            cpController.TriggerAnimation(false);

            stage++;
            return;
        }

        if (stage == 7 && index == 4) {
            clickObjects[1].SetActive(true);
            return;
        }

        if (stage == 7 && index == -1) {
            GameObject cpPanel =  cpController.gameObject;

            bool flag = false;
            for (int i = 0; i < 4; i++) {
                if (cpPanel.transform.GetChild(i + 1).GetChild(0).GetComponent<Image>().color.a != 0) {
                    tmcController.SetCrystal(3, i);
                    ShowNextText();
                    StartCoroutine(highlightFlash(tmcController.gameObject.transform.GetChild(3).GetChild(1).GetComponent<Image>()));
                    flag = true;
                    break;
                }
            }

            if (!flag) {
                rController.setReminder("Please Select a crystal", 3.0f);
                return;
            }

            cpController.Revoke();
            clickObjects[1].SetActive(false);

            stage++;

            shinningText.SetActive(true);

            return;
        }

        if (stage == 8 && index == 8) {
            shinningText.SetActive(false);
            ShowNextText();
            tmcController.SetPortal(true, "Eric");
            
            audioSource.clip = audioClips[0];
            audioSource.Play();

            dragObjects[0].SetActive(true);
            stage++;
            return;
        }

        if(stage == 9 && index == 9){
            dragObjects[0].SetActive(false);
            ShowNextText();

            audioSource.clip = audioClips[1];
            audioSource.Play();
            stage++;
            return;
        }

        if (stage == 10 && index == 10) {
            ShowNextText();
        }

    }

    private void InvokeFunction1() {
        ShowNextText();
    }

    private void ShowNextText() {
        textObjects[textNum].SetActive(false);
        textNum++;
        textObjects[textNum].SetActive(true);
    }

    IEnumerator highlightFlash(Image image) {
        
        int counter = 2;

        while (counter >= 0) {
            image.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.5f);
            image.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.5f);
            counter--;
        }
    }

}
