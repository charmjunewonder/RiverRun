using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;
    public GameObject shieldPrefab;
    public GameObject lighteningPrefab;

    [SyncVar]
    public int slot;
    [SyncVar]
    public string username;

    [SyncVar]
    public int rank;

    [SyncVar]
    public int disconnectCrystal;

    [SyncVar]
    public PlayerRole role;
    [SyncVar]
    public int skill1Counter = 0;
    [SyncVar]
    public int skill2Counter = 0;
    [SyncVar]
    public int supportCounter = 0;

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
    protected WarningController warningController;
    protected GameObject ui;

    public GameObject enemyUIManager;
    public GameObject defenderEnemyManager;
    private Transform enemyUITarget;
    private Transform skillPanel;

    [SyncVar]
    protected bool isInGame;
    protected bool disconnectedCrystalInitialized;

    protected bool isDraggingCrystal;

    private Vector3 shieldPoint1, shieldPoint2;
    private bool shieldExist;
    private float shieldTime;
    private int shieldNumber;

    protected PlayerParameter playerParameter;


    #region StartUpdate
    void Awake() {
        isInGame = false;
        disconnectedCrystalInitialized = false;
    }


    void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer) {
			ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity) as GameObject;
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
            
            shieldPoint1 = Vector3.zero;
            shieldExist = false;
            isDraggingCrystal = false;

		}
	}

	void Update () {
        if (isServer) {
            if (defenderEnemyManager == null || enemyUIManager == null)
            {
                LoadEnemyObject();
            }
            return;
        }
        else if (!isLocalPlayer) {
            return;
        }


        if (role == PlayerRole.Engineer) return;

        if (!disconnectedCrystalInitialized) {
            int a = disconnectCrystal;
            for (int i = 3; i >= 0; i--) {
                mainCrystalController.SetCrystal(i, (a & 7) - 1);
                a >>= 8;
            }
            disconnectedCrystalInitialized = true;

            GetComponent<PlayerInfo>().setHealth(GetComponent<PlayerInfo>().getHealth());

        }

        if (isInGame) {
            if (enemyUIManager == null) LoadEnemyObject();
            else {
                for (int i = 0; i < 20; i++){
                    if (i < enemyUIManager.transform.childCount)
                    {
                        Transform enemy = enemyUIManager.transform.GetChild(i);
                        if(enemy.position.z < 10) continue;
                        Vector3 screenPoint = cam.WorldToScreenPoint(enemy.position);
                        Transform target = enemyUITarget.GetChild(i);
                        target.position = screenPoint;
                        target.GetComponent<Image>().color = new Color(1, 1, 1, 1);

                        if (role == PlayerRole.Striker) {
                            Image image = target.GetChild(0).GetComponent<Image>();
                            image.color = new Color(1, 1, 1, 1);
                            
                            float blood_perc;
                            if(enemy.tag == "Enemy")
                                blood_perc = enemy.GetComponent<EnemyMotion>().getBlood() / enemy.GetComponent<EnemyMotion>().getMaxBlood();
                            else
                                blood_perc = enemy.GetComponent<BossController>().getBlood() / enemy.GetComponent<BossController>().getMaxBlood();
                            
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
                shieldPoint1 = Input.mousePosition;
            }
        }
        if (Input.GetMouseButton(0))
        {

        }
        if(Input.GetMouseButtonUp(0)){
            if (isDraggingCrystal) {
                isDraggingCrystal = false;
                return;
            }

            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {
                skillControllers[skillIndex].StartCoolDown();

                if (role == PlayerRole.Striker)
                {
                    for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                    {
                        Transform enemy = enemyUIManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDoFire(skillIndex, i);
                        }
                    }
                }
                else {
                    if (Vector3.Distance(Input.mousePosition, shieldPoint1) > 30)
                    {
                        if (GetComponent<PlayerInfo>().getHealth() == 0)
                        {
                            reminderController.setReminder("Device damaged. Please ask engineer to repair", 3.0f);
                            return;
                        }

                        Vector3 shieldCenter = (shieldPoint1 + Input.mousePosition) / 2;
                        float radius = Vector3.Distance(Input.mousePosition, shieldCenter);

                        Ray ray = cam.ScreenPointToRay(shieldCenter);

                        Vector3 shieldPos = ray.direction.normalized * 10;

                        CmdCreateShield(shieldPos, radius / 30);

                        

                    }
                    else {
                        for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                        {
                            Transform enemy = enemyUIManager.transform.GetChild(i);
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
                
            }
            else if (!e.IsPointerOverGameObject() && skillControllers[skillIndex].getCoolDownStatus())
            {
                reminderController.setReminder("Skill is Cooling Down", 1);
            }
            
        }
#endif
#if UNITY_IOS
        if(Input.touchCount == 0)
            return;

        if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved){
            shieldPoint1 = Input.GetTouch(0).position;
        }

        if(Input.touchCount > 1 && (Input.GetTouch(1).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Moved)){
            shieldPoint2 = Input.GetTouch(1).position;
        }

        if(Input.touchCount > 1 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)){
            if (role == PlayerRole.Defender && Vector3.Distance(shieldPoint1, shieldPoint2) > 30){

                    if (GetComponent<PlayerInfo>().getHealth() == 0) {
                        reminderController.setReminder("Device damaged. Please ask engineer to repair", 3.0f);
                        return;
                    }

                    Vector3 shieldCenter = (shieldPoint1 + shieldPoint2) / 2;
                    float radius = Vector3.Distance(shieldPoint2, shieldCenter);

                    Ray ray = cam.ScreenPointToRay(shieldCenter);

                    Vector3 shieldPos = ray.direction.normalized * 10;

                    CmdCreateShield(shieldPos, radius / 30);

                    return;
                }
        } else if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended){

            if (isDraggingCrystal) {
                isDraggingCrystal = false;
                return;
            }

            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {
                
                skillControllers[skillIndex].StartCoolDown();

                if (role == PlayerRole.Striker)
                {
                    for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                    {
                        Transform enemy = enemyUIManager.transform.GetChild(i);
                        Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                        Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                        Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 40)
                        {
                            CmdDoFire(skillIndex, i);
                        }
                    }
                }
                else {
                    for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                    {
                        Transform enemy = enemyUIManager.transform.GetChild(i);
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
	void CmdDoFire(int skillIndex, int enemyIndex)
	{
        Debug.Log("CmdDoFire " + skillIndex + enemyIndex);
        if (role == PlayerRole.Striker) {
            if (enemyIndex == -1)
            {
                for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                {
                    enemyUIManager.transform.GetChild(i).GetComponent<EnemyMotion>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);
                }
            }
            else
            {
                Transform enemy = enemyUIManager.transform.GetChild(enemyIndex);

                if(enemy.tag == "Enemy")
                    enemy.GetComponent<EnemyMotion>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);
                else
                    enemy.GetComponent<BossController>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);

                GameObject lightening = Instantiate(lighteningPrefab, transform.position, Quaternion.identity) as GameObject;

                lightening.transform.GetChild(0).position = enemy.position;
                lightening.transform.GetChild(1).position = transform.GetChild(Random.Range(1, 3)).position;

                lightening.GetComponent<SyncTransformLightening>().setTransform(lightening.transform.GetChild(0), lightening.transform.GetChild(1));
                skill1Counter++;
                NetworkServer.Spawn(lightening);
            }
        }
        else {
            float freezeTime = GetComponent<StrikerSkill2>().damage;
            freezeTime = freezeTime < 8 ? 8 : freezeTime;
            defenderEnemyManager.GetComponent<EnemySpawnManager>().Freeze(freezeTime);
            enemyUIManager.GetComponent<EnemyAttackFreezer>().Freeze();
            NetworkManagerCustom.SingletonNM.FreezeAI(freezeTime);
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
    protected void CmdDefendAttack(NetworkInstanceId netID)
    {
        GameObject obj = NetworkServer.FindLocalObject(netID);
        skill1Counter++;
        NetworkServer.Destroy(obj);
    }

    [Command]
    protected void CmdCreateShield(Vector3 pos, float scale)
    {

        if (shieldExist) {
            RpcDeclineShieldCreation();
            return;
        }

        GameObject shield = Instantiate(shieldPrefab, pos, Quaternion.identity) as GameObject;

        shield.transform.localScale *= scale;

        shield.transform.LookAt(Vector3.forward);

        shield.GetComponent<SyncTransform>().setTransform(shield.transform);

        shield.GetComponent<ShieldCollisionBehaviour>().setMaximumDefendNumber(shieldNumber);

        shield.GetComponent<ShieldCollisionBehaviour>().playerController = this;
        shield.GetComponent<ShieldCollisionBehaviour>().setCountDown(shieldTime);

        NetworkServer.Spawn(shield);

        shieldExist = true;

    }

    [ClientRpc]
    protected void RpcDeclineShieldCreation()
    {
        if (isLocalPlayer) {
            reminderController.setReminder("Shield alreayd exists.", 1.0f);
        }
    }

    [ClientRpc]
    public void RpcFreezeAI(float t) {
        if (isLocalPlayer) {
            GameObject.Find("AllianceSpaceshipObject").GetComponent<AllianceSpaceshipSpawnController>().Freeze(t);
        }
    }

    [Command]
    public void CmdFreezeAI(float t) {
        GameObject.Find("AllianceSpaceshipObject").GetComponent<AllianceSpaceshipSpawnController>().Freeze(t);
        NetworkManagerCustom.SingletonNM.FreezeAI(t);
    }

    public void CloseShield() {
        shieldExist = false;
    }
    #endregion

    #region UltiActivation

    public void SetSkillIndex(int index) { skillIndex = index; }

    public void RequestUlti(){
        if (GetComponent<PlayerInfo>().getHealth() == 0) {
            reminderController.setReminder("Device damaged. Please ask engineer to repair", 3.0f);
            return;
        }
        CmdRequestUlti();
    }

    [Command]
    protected void CmdRequestUlti(){
        Debug.Log("Player Controller Server: Ulti Request");
        Debug.Log("UltiController.checkUltiEnchanting() " + UltiController.checkUltiEnchanting());
        if (!UltiController.checkUltiEnchanting()) {
            Debug.Log("Player Controller Server: Ulti Request Success");
            UltiController.setUltiEnchanting(true);
            UltiController.setUltiPlayerNumber(slot);
            
            RpcUltiActivationStatusUpdate(true);
            for (int i = 0; i < NetworkManagerCustom.SingletonNM.gameplayerControllers.Count; i++) {
                if (i != slot && NetworkManagerCustom.SingletonNM.gameplayerControllers[i] != null)
                {
                    ((PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[i]).RpcLockUlti(username);
                }
            }
        }
        else {
            RpcUltiActivationStatusUpdate(false);
        }
    }

    [ClientRpc]
    protected void RpcUltiActivationStatusUpdate(bool status)
    {
        if (isLocalPlayer){
            if (status){
                ultiCrystalController.GenerateUltiCrystals();
                int ulti_index = 1;
                skillControllers[ulti_index].StartCoolDown();
            }
            else {
                reminderController.setReminder("Teammate is activating ultimate skill", 3.0f);
            }
        }
    }

    [ClientRpc]
    public void RpcLockUlti(string n) {
        if (isLocalPlayer) {
            mainCrystalController.OpenCrystalPortal(n);
            if(role != PlayerRole.Engineer)
                skillPanel.GetChild(1).GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    [Command]
    protected void CmdDoneUlti()
    {
        skill2Counter++;
        UltiController.setUltiEnchanting(false);
        for (int i = 0; i < NetworkManagerCustom.SingletonNM.gameplayerControllers.Count; i++)
        {
            if (i != slot && NetworkManagerCustom.SingletonNM.gameplayerControllers[i] != null)
            {
                ((PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[i]).RpcUnlockUlti();
            }
        }
    }

    public void UnlockUlti() {
        if (isServer) {
            RpcUnlockUlti();
        }
    }

    [ClientRpc]
    public void RpcUnlockUlti(){
        if (isLocalPlayer) {
            mainCrystalController.CloseCrystalPortal();
            if (role != PlayerRole.Engineer)
                skillPanel.GetChild(1).GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public void ActivateUlti() {
        Debug.Log("Player Controller: Activate Ulti Success");
        int ulti_index = 1;
        CmdDoFire(ulti_index, -1);
        CmdDoneUlti();
        ultiCrystalController.Clear();
        
    }

    public void RevokeUlti() {
        Debug.Log("PlayerController RevokeUlti");
        int ulti_index = 1;
        skillControllers[ulti_index].RevokeCoolDown();
        CmdDoneUlti();
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
        CmdDoneUlti();

        reminderController.setReminder("Ultimate Skill Activation Fails.", 3);
    }

    [Command]
    public void CmdUltiFailureHandling() {
        Debug.Log("Player Controller Server: Unlock Ulti Slot");

        UltiController.setUltiEnchanting(false);
    }

    public void setDraggingCrystal(bool f) {
        isDraggingCrystal = f;
    }


    #endregion

    #region Engineer

    [ClientRpc]
    public void RpcAcceptCrystalFromEngineer(int crys_num) {
        if(isLocalPlayer)
            mainCrystalController.AcceptCrystal(crys_num);
    }

    [Command]
    public void CmdUpdateDisconnectionCrystal(int v1, int v2, int v3, int v4) {
        int a = 0;
        a |= (v1 << 24);
        a |= (v2 << 16);
        a |= (v3 << 8);
        a |= v4;
        disconnectCrystal = a;
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

        skillPanel = ui.transform.GetChild(1);
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

        warningController = ui.transform.GetChild(7).GetComponent<WarningController>();

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();



        //GameObject.DontDestroyOnLoad(e);
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelWasLoaded");
        if (level == 1)
        {
            initializeData();

            if (isLocalPlayer)
            {
                //if(level != 0)
                //ClientScene.Ready(connectionToServer);
                isInGame = true;
                Debug.Log("Slot " + slot);
                if (role == PlayerRole.Striker)
                    cam.cullingMask = (1 << (slot + 8)) | 1 | 1 << 13 | 1 << 12;
                InitializeCrystal();
            }
        }
        if (level == 13)
            print("Woohoo");
    }

    private void LoadEnemyObject()
    {
        GameObject t1 = GameObject.Find("EnemyManager");
        GameObject t2 = GameObject.Find("EnemySkills");
        if (role == PlayerRole.Striker)
        {
            enemyUIManager = t1;
            defenderEnemyManager = t2;
        }
        else
        {
            defenderEnemyManager = t1;
            enemyUIManager = t2;
        }
    }

    private void InitializeCrystal() {

        Debug.Log("InitializeSetCrystal");
        int ran = Random.Range(0, 4);
        mainCrystalController.AcceptCrystal(ran);

        ran = Random.Range(0, 4);
        mainCrystalController.AcceptCrystal(ran);
        
    }


    protected void initializeData() {
        playerParameter = GameObject.FindGameObjectWithTag("DataSource").GetComponent<PlayerParameter>().getPlayer(role, rank);
        Debug.Log("playerParameter " + playerParameter.maxHp);
        GetComponent<PlayerInfo>().setHealth(playerParameter.maxHp);
        if (role == PlayerRole.Engineer)
        {
            GetComponent<EngineerSkill1>().coolDown = playerParameter.coolingDown_1;
            GetComponent<EngineerSkill1>().heal = playerParameter.healPt;

        }
        else
        {
            GetComponent<StrikerSkill1>().coolDown = playerParameter.coolingDown_1;
            GetComponent<StrikerSkill2>().coolDown = playerParameter.coolingDown_2;
            GetComponent<StrikerSkill2>().damage = role == PlayerRole.Striker ? playerParameter.ultiPt : playerParameter.ultiTime;
            if (role == PlayerRole.Defender)
            {
                shieldTime = playerParameter.sheildTime;
                shieldNumber = playerParameter.sheildHp;
            }
        }
    }
    #endregion

    #region Utility

    public void Damage(float damage) {
        GetComponent<PlayerInfo>().Damage(damage);
        if(damage > 0)
            RpcDamage(damage);

        float health = (int)GetComponent<PlayerInfo>().getHealth();

        int perc = health == 0 ? 0 : (int)(health / (int)GetComponent<PlayerInfo>().getMaxHealth() * 10) + 1;

        NetworkManagerCustom.SingletonNM.sGamePanel.GetComponent<ServerGamePanel>().playerInfos[slot].SetHealth(perc);
    }

    [ClientRpc]
    public void RpcDamage(float damage)
    {
        if (isLocalPlayer) {
            warningController.TriggerWarning();
        }
    }

    public void InitializeDisconnectCrystals(int crystal) {
        disconnectCrystal = crystal;
        Debug.Log("InitializeDisconnectCrystals " + crystal);
    }

    [ClientRpc]
    public void RpcMissionComplete() {
        if (isLocalPlayer) {
            Debug.Log("Mission Complete Local");
            ui.SetActive(false);
            int exp = Random.Range(50, 200);
            NetworkManagerCustom.SingletonNM.MissionComplete(skill1Counter * 3 + skill2Counter * 10, 4, username, role, rank, exp, 200, 
                skill1Counter, skill2Counter);
        }
    }
    #endregion

}
