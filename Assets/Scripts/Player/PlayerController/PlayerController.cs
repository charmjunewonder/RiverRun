using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;

    [SyncVar]
    public int slot;
    public string username;

    [SyncVar]
    public PlayerRole role;

    public Sprite[] skillIcons;
    public Sprite[] enemyIcons;

    [SerializeField] 
	public Camera cam;

	[SyncVar]
	protected int skillIndex;
    protected Skill[] skills;

    protected EventSystem e;
    
    
    protected int[] crystals;
    protected SkillController[] skillControllers;
    protected PlayerInfo playerInfo;
    protected UltiCrystalController ultiCrystalController;
    protected MainCrystalController mainCrystalController;
    protected ReminderController reminderController;

    public GameObject enemyManager;
    private Transform enemyUITarget;

    [SyncVar]
    protected bool isInGame;

    #region StartUpdate
    void Awake() {
        isInGame = false;
    }


    void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer) {
			GameObject ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity) as GameObject;
            GameObject.DontDestroyOnLoad(ui);
            NetworkManagerCustom.SingletonNM.DisableLobbyUI();
            setStrikerDefenderControllers(ui);
            
			cam.enabled = true;

            skillControllers = new SkillController[3];
			for(int i = 0; i <= 1; i++){
                skillControllers[i] = GameObject.Find("Skill" + i + "_Image").GetComponent<SkillController>();
                skillControllers[i].setSkill(playerInfo.getSkill(i));
			}

			skillIndex = 0;
            
		}
	}

	void Update () {
        if (!isLocalPlayer) {
            if (enemyManager == null)
            {
                LoadEnemyObject();
            }
            return;
        }
        if (role == PlayerRole.Engineer) return;

        if (isInGame) {
            if (enemyManager == null) LoadEnemyObject();
            else {
                for (int i = 0; i < 20; i++){
                    if (i < enemyManager.transform.childCount){
                        Transform enemy = enemyManager.transform.GetChild(i);
                        Vector3 screenPoint = cam.WorldToScreenPoint(enemy.position);
                        Transform target = enemyUITarget.GetChild(i);
                        target.position = screenPoint;
                        target.GetComponent<Image>().color = new Color(1, 1, 1, 1);

                        if (role == PlayerRole.Striker) {
                            Image image = target.GetChild(0).GetComponent<Image>();
                            image.color = new Color(1, 1, 1, 1);
                            float blood_perc = enemy.GetComponent<EnemyMotion>().getBlood() / enemy.GetComponent<EnemyMotion>().getMaxBlood();
                            image.fillAmount = blood_perc;
                        }
                        
                        
                    } else {
                        Transform target = enemyUITarget.GetChild(i);
                        target.position = new Vector3(0, 0, 0);
                        if (target.GetComponent<Image>().color.a == 0)
                            break;
                        target.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        target.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }
                }
            }
        }



#if UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0))
        {
            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {

            }
        }
        if (Input.GetMouseButton(0))
        {
        
        }
        if(Input.GetMouseButtonUp(0)){
            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {

                skillControllers[skillIndex].StartCoolDown();

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (role == PlayerRole.Striker)
                {
                    Debug.Log("children number " + enemyManager.transform.childCount);
                    for (int i = 0; i < enemyManager.transform.childCount; i++)
                    {
                        Transform enemy = enemyManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDoFire(skillIndex, ray, i);
                        }
                    }
                }
                else {
                    for (int i = 0; i < enemyManager.transform.childCount; i++){
                        Transform enemy = enemyManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDefendAttack(enemy.GetComponent<NetworkIdentity>().netId);
                        }
                        
                    }
                
                }
                
            }
            else if (!e.IsPointerOverGameObject() && skillControllers[skillIndex].getCoolDownStatus())
            {
                reminderController.setReminder("Skill is Cooling Down", 1);
            }
        }
#endif
#if UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {

                skillControllers[skillIndex].StartCoolDown();

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (role == PlayerRole.Striker)
                {
                    for (int i = 0; i < enemyManager.transform.childCount; i++)
                    {
                        Transform enemy = enemyManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDoFire(skillIndex, ray, i);
                        }
                    }
                }
                else {
                    for (int i = 0; i < enemyManager.transform.childCount; i++)
                    {
                        
                        Transform enemy = enemyManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDefendAttack(enemy.GetComponent<NetworkIdentity>().netId);
                        }   
                    }
                }
            }
            else if (!e.IsPointerOverGameObject() && skillControllers[skillIndex].getCoolDownStatus())
            {
                reminderController.setReminder("Skill is Cooling Down", 1);
            }
        }
#endif
    }

    #endregion

    #region Operation
    [Command]
	void CmdDoFire(int skillIndex, Ray ray, int enemyIndex)
	{
        Debug.Log("CmdDoFire " + skillIndex + enemyIndex);

        if (enemyIndex == -1){
            for (int i = 0; i < enemyManager.transform.childCount; i++)
            {
                enemyManager.transform.GetChild(i).GetComponent<EnemyMotion>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);
            }
        }
        else {
            enemyManager.transform.GetChild(enemyIndex).GetComponent<EnemyMotion>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);
        }



        /*
        GameObject bullet = (GameObject)Instantiate(playerInfo.getSkill(index).prefab, transform.position + ray.direction * 1, Quaternion.identity);
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		*/
	}

    [Command]
    void CmdDefendAttack(NetworkInstanceId netID) {
        GameObject obj = NetworkServer.FindLocalObject(netID);
        NetworkServer.Destroy(obj);
    }


    #endregion

    #region UltiActivation

    public void SetSkillIndex(int index) { skillIndex = index; }

    public void RequestUlti(){
        Debug.Log("Player Controller: Ulti Request");
        CmdRequestUlti();
    }

    [Command]
    protected void CmdRequestUlti(){
        Debug.Log("Player Controller Server: Ulti Request");
        if (!UltiController.checkUltiEnchanting()) {
            Debug.Log("Player Controller Server: Ulti Request Success");
            UltiController.setUltiEnchanting(true);
            UltiController.setUltiPlayerNumber(slot);
            
            RpcUltiActivationStatusUpdate(true);
        }
        else {
            RpcUltiActivationStatusUpdate(false);
        }
    }

    [ClientRpc]
    protected void RpcUltiActivationStatusUpdate(bool status)
    {
        if (isLocalPlayer){
            Debug.Log("Player Controller: Start Cooling Down");
            Debug.Log("skillIndex " + skillIndex);
            if (status){
                ultiCrystalController.GenerateUltiCrystals();
                int ulti_index = role == PlayerRole.Engineer ? 2 : 1;
                skillControllers[ulti_index].StartCoolDown();
            }
        }
    }

    public void ActivateUlti() {
        Debug.Log("Player Controller: Activate Ulti Success");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int ulti_index = role == PlayerRole.Engineer ? 2 : 1;
        CmdDoFire(ulti_index, ray, -1);
        ultiCrystalController.Clear();
        UltiController.setUltiEnchanting(false);
    }

    public void RevokeUlti() {
        Debug.Log("PlayerController RevokeUlti");
        int ulti_index = role == PlayerRole.Engineer ? 2 : 1;
        skillControllers[ulti_index].RevokeCoolDown();
    }
    #endregion

    #region SupportSkill

    [Command]
    public void CmdSupport(int crystalIndex){
        Debug.Log("Player Controller Server: Support Submission Success");
        if (UltiController.checkUltiEnchanting()) {
            PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[UltiController.getUltiPlayerNumber()];
            plc.RpcSupportGivenBack(crystalIndex);
        }
    }

    [ClientRpc]
    public void RpcSupportGivenBack(int crystalIndex) {
        if (isLocalPlayer){
            Debug.Log("Player Controller: Give Crystal " + crystalIndex + " Back to Client");
            ultiCrystalController.AcceptCrystal(crystalIndex);
        }
    }

    public void UltiFailureHandling() {

        Debug.Log("Player Controller: Failed to Activate Ulti");

        CmdUltiFailureHandling();

        reminderController.setReminder("Ultimate Skill Cannot be Activated.", 3);
    }

    [Command]
    public void CmdUltiFailureHandling() {
        Debug.Log("Player Controller Server: Unlock Ulti Slot");

        UltiController.setUltiEnchanting(false);
    }


    #endregion

    #region Engineer

    [ClientRpc]
    public void RpcAcceptCrystalFromEngineer(int crys_num) {
        if(isLocalPlayer)
            mainCrystalController.AcceptCrystal(crys_num);
    }

    [ClientRpc]
    public void RpcAcceptHealFromEngineer(float amount) {
        if (isLocalPlayer)
            GetComponent<PlayerInfo>().Damage(-amount);
    }

    #endregion
    
    #region Initialization Setup
    public void SetSlot(int s)
    {
        slot = s;
    }

    public void setInGame() {
        isInGame = true;
    }

    private void setStrikerDefenderControllers(GameObject ui)
    {
        Debug.Log(role);

        GetComponent<PlayerInfo>().setHealthController(ui.transform.GetChild(0).GetComponent<HealthController>());

        Transform skillPanel = ui.transform.GetChild(1);
        skillPanel.GetChild(0).GetComponent<SkillController>().setPlayerController(this);
        skillPanel.GetChild(0).GetComponent<Image>().sprite = role == PlayerRole.Striker ? skillIcons[0] : skillIcons[2];
        skillPanel.GetChild(1).GetComponent<SkillController>().setPlayerController(this);
        skillPanel.GetChild(1).GetComponent<Image>().sprite = role == PlayerRole.Striker ? skillIcons[1] : skillIcons[3];

        Transform supportCrystalPanel = ui.transform.GetChild(2);
        mainCrystalController = supportCrystalPanel.GetComponent<MainCrystalController>();
        mainCrystalController.SetPlayerController(this);

        GameObject ulticrystalObject = ui.transform.GetChild(3).gameObject;
        ultiCrystalController = ulticrystalObject.GetComponent<UltiCrystalController>();
        ultiCrystalController.setPlayerController(this);

        reminderController = ui.transform.GetChild(4).GetComponent<ReminderController>();

        enemyUITarget = ui.transform.GetChild(6);

        if (role == PlayerRole.Defender) {
            for (int i = 0; i < enemyUITarget.childCount; i++){
                enemyUITarget.GetChild(i).GetComponent<Image>().sprite = enemyIcons[2];
                enemyUITarget.GetChild(i).GetChild(0).GetComponent<Image>().sprite = enemyIcons[3];

            }
        }

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        //GameObject.DontDestroyOnLoad(e);
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelWasLoaded");
        if (isLocalPlayer)
        {
            //if(level != 0)
            //ClientScene.Ready(connectionToServer);
            isInGame = true;
            Debug.Log("Slot " + slot);
            if(role == PlayerRole.Striker)
                cam.cullingMask = (1 << (slot + 8)) | 1;
            mainCrystalController.AcceptCrystal(Random.Range(0, 4));
            mainCrystalController.AcceptCrystal(Random.Range(0, 4));
        
        }

        if (level == 13)
            print("Woohoo");
    }

    void LoadEnemyObject()
    {
        if (role == PlayerRole.Striker)
        {
            enemyManager = GameObject.Find("EnemyManager");
        }
        else
        {
            enemyManager = GameObject.Find("EnemySkills");
        }
    }
    #endregion

    #region Utility

    [ClientRpc]
    public void RpcDamage(float damage)
    {
        if(isLocalPlayer)
            GetComponent<PlayerInfo>().Damage(damage);
    }
    
    #endregion

}
