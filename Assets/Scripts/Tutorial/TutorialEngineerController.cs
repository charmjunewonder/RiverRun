using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class TutorialEngineerController : MonoBehaviour {

    public EventSystem e;

    public LobbyTutorialController ltController;
    public TutorialTeammatePanelController tpController;
    public CrystalProductionController cpController;
    public TutorialMainCrystalController tmcController;
    public ReminderController rController;

    public GameObject[] clickObjects;
    public GameObject[] textObjects;
    public GameObject[] dragObjects;

    public int stage;

    private int textNum;

    private int crystalCounter;

	void Start () {
        textNum = 0;
	    stage = 0;
        crystalCounter = 0;
	}
	
	void Update () {

        if (Input.GetMouseButton(0)) {

            if (stage == 0) {
                clickObjects[0].SetActive(true);
                textObjects[0].SetActive(true);
                stage++;
            }

            if (stage == 10) {
                ltController.GoToLobbyScene();
            }
        }
	}

    public void SetStage(int index) {
        if (stage == 1 && index == 1) {

            clickObjects[0].SetActive(false);
            ShowNextText();

            for(int i = 1; i <= 4; i++)
                clickObjects[i].SetActive(true);

            tpController.SetGlow();
            stage++;

            return;
        }

        if (stage == 2 && index < 0) {
            tpController.NoGlow();
            for (int i = 1; i <= 4; i++)
                clickObjects[i].SetActive(false);
            ShowNextText();
            clickObjects[5].SetActive(true);

            stage++;

            return;
        }

        if (stage == 3 && index == 3) {
            textObjects[textNum].SetActive(false);
            clickObjects[5].SetActive(false);
            cpController.TriggerAnimation();

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


            cpController.TriggerAnimation();

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

            StartCoroutine(AutoNextStage(stage, 6.0f));

            return;
        }

        if (stage == 8 && index == 8) {
            
            ShowNextText();
            tmcController.SetPortal(true, "Eric");
            
            dragObjects[0].SetActive(true);
            
            return;
        }

        if(stage == 9 && index == 9){
            dragObjects[1].SetActive(true);
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

    IEnumerator AutoNextStage(int s, float t) {
        yield return new WaitForSeconds(t);
        SetStage(s);
    }


    public void CrystalSupported(int num) {
        if (num == 1) {
            dragObjects[1].SetActive(false);
            crystalCounter++;
        }
        else {
            dragObjects[0].SetActive(false);
            crystalCounter++;
        }

        if (crystalCounter == 1) {
            stage++;
            SetStage(stage);
        }

        if (crystalCounter == 2) {
            
            stage++;
            StartCoroutine(AutoNextStage(stage, 0.5f));
        }
    }
}
