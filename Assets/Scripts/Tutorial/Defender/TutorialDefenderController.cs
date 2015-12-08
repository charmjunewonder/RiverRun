using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialDefenderController : MonoBehaviour {


    public Camera cam;
    public GameObject shieldPrefab;
    public GameObject disintegrationPrefab;

    public int stage;


    public GameObject[] clickObjects;
    public GameObject[] textObjects;
    public GameObject[] dragObjects;

    public GameObject[] frames;

    public GameObject shiningText;
    public GameObject enemyManager;
    public GameObject enemies;
    public Transform enemyUITarget;

    public TutorialMainCrystalController tmcController;
    public UltiCrystalController ucController;
    public LobbyTutorialController ltController;

    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private int textNum;

    private Vector3 shieldPoint1, shieldPoint2;

    void Start()
    {
        textNum = 0;
        stage = 0;
    }

    void Update()
    {
        if (stage <= 3)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetStage(stage);
            }
        }

        if (stage >= 4 && stage < 6)
        {
            for (int i = 0; i < enemyManager.transform.childCount; i++)
            {
                Transform enemy = enemyManager.transform.GetChild(i);
                Vector3 screenPoint = cam.WorldToScreenPoint(enemy.position);
                Transform target = enemyUITarget.GetChild(i);
                target.position = screenPoint;
                target.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        if (stage == 4)
        {
#if UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0))
            {
                Transform enemy = enemyManager.transform.GetChild(0);
                Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //Debug.Log("distance " + Vector2.Distance(enemyPos2d, mousePos2d));

                if (Vector2.Distance(enemyPos2d, mousePos2d) < 30)
                {
                    ShowNextText();

                    enemyUITarget.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    Destroy(enemyManager.transform.GetChild(0).gameObject);

                    SetStage(stage);
                    
                }
            }
#endif
#if UNITY_IOS
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {

                Transform enemy = enemyManager.transform.GetChild(0);
                Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                Vector2 mousePos2d = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

                if (Vector2.Distance(enemyPos2d, mousePos2d) < 30)
                {
                    ShowNextText();

                    enemyUITarget.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    Destroy(enemyManager.transform.GetChild(0).gameObject);

                    SetStage(stage);
                    
                }
            }
#endif
        }

        if (stage == 5) {
#if UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0)) {
                shieldPoint1 = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0)) {
                if (Vector3.Distance(Input.mousePosition, shieldPoint1) > 30) {
                    Vector3 shieldCenter = (shieldPoint1 + Input.mousePosition) / 2;
                    float radius = Vector3.Distance(Input.mousePosition, shieldCenter);

                    radius = radius <= Screen.width / 5 ? radius : Screen.width / 5;
                    

                    Ray ray = cam.ScreenPointToRay(shieldCenter);
                    createShield(ray, radius);
                    SetStage(++stage);
                }
            }
#endif
#if UNITY_IOS
            if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved){
                shieldPoint1 = Input.GetTouch(0).position;
            }

            if(Input.touchCount > 1 && (Input.GetTouch(1).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Moved)){
                shieldPoint2 = Input.GetTouch(1).position;
            }

            if(Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)){
                if(Vector3.Distance(shieldPoint1, shieldPoint2) > 30){
                    Vector3 shieldCenter = (shieldPoint1 + shieldPoint2) / 2;
                    float radius = Vector3.Distance(shieldPoint2, shieldCenter);
                    radius = radius <= Screen.width / 5 ? radius : Screen.width / 5;
                    Ray ray = cam.ScreenPointToRay(shieldCenter);
                    createShield(ray, radius);
                    SetStage(++stage);
                }
            
            }
#endif
        }

        if (stage == 8)
        {
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

        if (stage == 13 || stage == 14)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowNextText();
                stage++;
                return;
            }
        }

        if (stage == 15)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ltController.GoToLobbyScene();
            }
        }

    }



    public void SetStage(int index)
    {

        //Debug.Log(stage + " " + index);

        if (stage < 3)
        {

            ShowNextText();
            frames[index].SetActive(true);
            if (index != 0)
                frames[index - 1].SetActive(false);
            stage++;
            return;
        }

        if (stage == 3 && index == 3)
        {
            ShowNextText();
            frames[2].SetActive(false);
            enemyManager.GetComponent<TutorialGenerateAttack>().GenerateAttack();
            clickObjects[0].SetActive(true);
            shiningText.SetActive(false);
            stage++;
            return;
        }

        if (stage == 4 && index == 4)
        {
            ShowNextText();
            clickObjects[0].SetActive(false);
            dragObjects[3].SetActive(true);
            dragObjects[4].SetActive(true);
            stage++;
            return;
        }

        if (stage == 6 && index == 6)
        {
            ShowNextText();
            dragObjects[3].SetActive(false);
            dragObjects[4].SetActive(false);
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

        if (stage == 9 && index == 9)
        {
            ShowNextText();
            frames[3].SetActive(false);
            dragObjects[0].SetActive(false);
            ucController.AcceptCrystal(0);
            StartCoroutine("WaitForCrystal");
            stage++;
            return;
        }

        if (stage == 10 && index == 10)
        {
            ShowNextText();

            for (int i = 0; i < enemyUITarget.childCount; i++) {
                enemyUITarget.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }

            for (int i = enemyManager.transform.childCount - 1; i >= 0 ; i--) {
                Destroy(enemyManager.transform.GetChild(i).gameObject);
            }

            shiningText.SetActive(true);
            stage++;
            return;
        }

        if (stage == 11 && index == 11)
        {
            ShowNextText();

            tmcController.SetPortal(true, "Eric");

            audioSource.clip = audioClips[0];
            audioSource.Play();

            dragObjects[1].SetActive(true);

            stage++;
            return;
        }

        if (stage == 12 && index == 9)
        {
            dragObjects[1].SetActive(false);
            audioSource.clip = audioClips[1];
            audioSource.Play();

            for(int i = 0; i < enemies.transform.childCount; i++)
                Instantiate(disintegrationPrefab, enemies.transform.GetChild(i).position, Quaternion.identity);

            for (int i = enemies.transform.childCount - 1; i >= 0; i--)
                Destroy(enemies.transform.GetChild(i).gameObject);

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


    IEnumerator WaitForCrystal()
    {
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

    private void createShield(Ray ray, float radius) {
        Vector3 shieldPos = ray.direction.normalized * 8;

        GameObject shield = Instantiate(shieldPrefab, shieldPos, Quaternion.identity) as GameObject;
#if UNITY_STANDALONE_WIN
        shield.transform.localScale *= (radius / 30);
#endif
#if UNITY_IOS
        shield.transform.localScale *= (radius / 100);
#endif

        shield.transform.LookAt(-ray.direction.normalized * 20);

        StartCoroutine(DestroyShield(shield));
    }

    IEnumerator DestroyShield(GameObject g) {
        yield return new WaitForSeconds(3);

        Destroy(g);

    }

}
