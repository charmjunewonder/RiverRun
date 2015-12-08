using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialStrikerController : MonoBehaviour {

    public Camera cam;
    public GameObject disintegrationPrefab;
    
    
    public int stage;


    public GameObject[] clickObjects;
    public GameObject[] textObjects;
    public GameObject[] dragObjects;

    public GameObject[] frames;

    public GameObject shiningText;
    public GameObject enemyManager;
    public Transform enemyUITarget;

    public TutorialMainCrystalController tmcController;
    public UltiCrystalController ucController;
    public LobbyTutorialController ltController;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private int textNum;

	void Start () {
        textNum = 0;
        stage = 0;
	}
	
	void Update () {
        if (stage <= 4) { 
	        if (Input.GetMouseButtonDown(0)) {
                SetStage(stage);
            }
        }

        if (stage >= 4 && stage < 6) {
            for (int i = 0; i < 3; i++) {
                Transform enemy = enemyManager.transform.GetChild(i);
                Vector3 screenPoint = cam.WorldToScreenPoint(enemy.position);
                Transform target = enemyUITarget.GetChild(i);
                target.position = screenPoint;
                target.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                Image image = target.GetChild(0).GetComponent<Image>();
                image.color = new Color(1, 1, 1, 1);
            }
        }

        if(stage == 5){
#if UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0))
            {
                Transform enemy = enemyManager.transform.GetChild(2);
                Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                if (Vector2.Distance(enemyPos2d, mousePos2d) < 30)
                {
                    ShowNextText();
                    enemyUITarget.GetChild(2).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;

                    if (enemyUITarget.GetChild(2).GetChild(0).GetComponent<Image>().fillAmount <= 0)
                    {
                        enemyUITarget.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        enemyUITarget.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        Destroy(enemyManager.transform.GetChild(2).gameObject);
                        Instantiate(disintegrationPrefab, enemyManager.transform.GetChild(2).position, Quaternion.identity);
                        SetStage(++stage);
                    }
                }
            }
#endif
#if UNITY_IOS
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {

                Transform enemy = enemyManager.transform.GetChild(2);
                Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                Vector2 mousePos2d = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

                if (Vector2.Distance(enemyPos2d, mousePos2d) < 80) {
                    enemyUITarget.GetChild(2).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;
                    ShowNextText();
                    if (enemyUITarget.GetChild(2).GetChild(0).GetComponent<Image>().fillAmount <= 0) {
                        SetStage(++stage);
                    }
                }
            }
#endif
        }


        if (stage == 8) {
            if (Input.GetMouseButtonDown(0))
            {
                SetStage(stage);
            }
        }

        if (stage == 11)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetStage(stage);
                
            }
        }

        if (stage == 13 || stage == 14) {
            if (Input.GetMouseButtonDown(0))
            {
                ShowNextText();
                stage++;
                return;
            }
        }

        if (stage == 15) {
            if (Input.GetMouseButtonDown(0))
            {
                ltController.GoToLobbyScene();
            }
        }

	}



    public void SetStage(int index) {
        
        //Debug.Log(stage + " " + index);

        if (stage < 3) {
            
            ShowNextText();
            frames[index].SetActive(true);
            if(index != 0)
                frames[index-1].SetActive(false);
            stage++;
            return;
        }

        if (stage == 3 && index == 3)
        {
            ShowNextText();
            frames[2].SetActive(false);
            stage++;
            return;
        }

        if (stage == 4 && index == 4)
        {
            ShowNextText();
            clickObjects[0].SetActive(true);
            shiningText.SetActive(false);
            stage++;
            return;
        }

        if (stage == 6 && index == 6)
        {
             clickObjects[0].SetActive(false);
             clickObjects[1].SetActive(true);
            stage++;
            return;
        }

        if (stage == 7 && index == 7)
        {
            ShowNextText();
            clickObjects[1].SetActive(false);
            shiningText.SetActive(true);
            stage++;
            return;
        }

        if (stage == 8 && index == 8)
        { 
            ShowNextText();
            dragObjects[0].SetActive(true);
            shiningText.SetActive(false);
            frames[3].SetActive(true);

            tmcController.SetPortal(true, "Me");

            stage++;
            return;
        }

        if (stage == 9 && index == 9) {
            ShowNextText();
            frames[3].SetActive(false);
            dragObjects[0].SetActive(false);
            ucController.AcceptCrystal(0);
            StartCoroutine("WaitForCrystal");
            stage++;
            return;
        }

        if (stage == 10 && index == 10) {
            ShowNextText();

            enemyUITarget.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;
            enemyUITarget.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;

            shiningText.SetActive(true);
            stage++;
            return;
        }

        if (stage == 11 && index == 11) {
            ShowNextText();
            
            tmcController.SetPortal(true, "Eric");

            audioSource.clip = audioClips[0];
            audioSource.Play();

            dragObjects[1].SetActive(true);

            stage++;
            return;
        }

        if (stage == 12 && index == 9) {
            dragObjects[1].SetActive(false);
            audioSource.clip = audioClips[1];
            audioSource.Play();

            enemyUITarget.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;
            enemyUITarget.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount -= 0.5f;
            
            enemyUITarget.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            enemyUITarget.GetChild(1).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            enemyUITarget.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            enemyUITarget.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);

            Instantiate(disintegrationPrefab, enemyManager.transform.GetChild(0).position, Quaternion.identity);
            Instantiate(disintegrationPrefab, enemyManager.transform.GetChild(1).position, Quaternion.identity);

            ShowNextText();
            stage++;
            shiningText.SetActive(true);
        }
    }

    private void ShowNextText()
    {
        textObjects[textNum].SetActive(false);
        textNum++;
        textObjects[textNum].SetActive(true);
    }


    IEnumerator WaitForCrystal(){
        audioSource.clip = audioClips[2];
        audioSource.Play();
        yield return new WaitForSeconds(3);

        ucController.AcceptCrystal(3);
        audioSource.clip = audioClips[3];
        audioSource.Play();
        yield return new WaitForSeconds(3);

        ucController.AcceptCrystal(3);
        ucController.Clear();
        SetStage(10);
    }

}
