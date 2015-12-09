using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;
    public GameObject strikerUlti;
    public GameObject defenderUlti;
    public GameObject shieldObj;

    [SyncVar]
    public int slot;
    [SyncVar]
    public string username;

    [SyncVar(hook = "OnRankChanged")]
    public int rank;
    [SyncVar]
    public int exp;

    [SyncVar(hook="OnScoreChanged")]
    public int score;

    [SyncVar]
    public int disconnectCrystal;

    [SyncVar]
    public PlayerRole role;

    public int skill1Counter = 0;
    public int skill2Counter = 0;
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
    protected ProgressBarController progressBarController;
    protected HealthController citizenshipHealthController;
    protected MainScoreFlowController msfController;
    protected GameObject ui;
    protected Text scoreText;

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

    public PlayerParameter playerParameter;

    [SyncVar(hook="OnPauseChanged")]
    public bool isPause;

    #region StartUpdate
    void Awake() {
        isInGame = false;
        disconnectedCrystalInitialized = false;
    }


    void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isServer) {
            if (role == PlayerRole.Striker) {
                transform.GetChild(1).GetChild(0).GetChild(0).position = new Vector3(-5.06f, 1.19f, 13.7f);
                transform.GetChild(1).GetChild(0).GetChild(1).position = new Vector3(-5.06f, 1.19f, 13.7f);
                transform.GetChild(1).GetChild(1).GetChild(0).position = new Vector3(5.06f, 1.19f, 13.7f);
                transform.GetChild(1).GetChild(1).GetChild(1).position = new Vector3(5.06f, 1.19f, 13.7f);

                strikerUlti.transform.position = new Vector3(0, 3.6f, 4.3f);
                strikerUlti.transform.GetChild(0).localPosition = new Vector3(0, -0.18f, 8.46f);
                strikerUlti.transform.GetChild(2).GetChild(0).localPosition = new Vector3(-4.18f, -1.74f, 8.66f);
                strikerUlti.transform.GetChild(2).GetChild(1).localPosition = new Vector3(-4.18f, -1.74f, 8.66f);

                strikerUlti.transform.GetChild(3).GetChild(0).localPosition = new Vector3(4.18f, -1.74f, 8.66f);
                strikerUlti.transform.GetChild(3).GetChild(1).localPosition = new Vector3(4.18f, -1.74f, 8.66f);

                strikerUlti.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().emissionRate = 5;
                strikerUlti.transform.GetChild(0).GetChild(1).GetComponent<ParticleSystem>().startSize = 3;
                strikerUlti.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>().emissionRate = 100;
                strikerUlti.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>().startSize = 0.7f;
                strikerUlti.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystem>().gameObject.transform.localScale = new Vector3(1, 1, 1);
                
            }
            else {
                defenderUlti.transform.position = new Vector3(0, 3.16f, 8.52f);
                defenderUlti.transform.localScale = new Vector3(2, 1, 1);

                defenderUlti.transform.GetChild(0).localPosition = new Vector3(0, -0.45f, 8.21f);
                defenderUlti.transform.GetChild(0).GetComponent<ParticleSystem>().startSize = 3;
                defenderUlti.transform.GetChild(0).GetComponent<ParticleSystem>().startSpeed = 0.9f;

                defenderUlti.transform.GetChild(2).GetComponent<ParticleSystem>().startSpeed = 5;
                defenderUlti.transform.GetChild(2).GetComponent<ParticleSystem>().startSize = 1;
                defenderUlti.transform.GetChild(3).GetComponent<ParticleSystem>().startSpeed = 5;
                defenderUlti.transform.GetChild(3).GetComponent<ParticleSystem>().startSize = 1;


            }
        }

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
            scoreText = ui.transform.GetChild(5).GetChild(0).GetComponent<Text>();
            switch (role)
            {
                case PlayerRole.Striker:
                    rank = LoginController.StrikerLevel;
                    exp = LoginController.StrikerExp;
                    break;
                case PlayerRole.Engineer:
                    rank = LoginController.EngineerLevel;
                    exp = LoginController.EngineerExp;
                    break;
                case PlayerRole.Defender:
                    rank = LoginController.DefenderLevel;
                    exp = LoginController.DefenderExp;
                    break;
            }
            CmdChangeRank(rank, exp);
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
            initializeData();
            scoreText.text = score + "";

            GetComponent<PlayerInfo>().setHealth(GetComponent<PlayerInfo>().getHealth());

        }
        if (isPause) return;
        if (isInGame)
        {
            if (enemyUIManager == null) LoadEnemyObject();
            else {
                int enemy_count = 0;
                for (int i = 0; i < 20; i++){
                    if (enemy_count < enemyUIManager.transform.childCount)
                    {
                        Transform enemy = enemyUIManager.transform.GetChild(enemy_count);
                        enemy_count++;
                        if(enemy.position.z < 20 || Mathf.Abs(enemy.position.x) > 350){
                            i--;
                            continue;
                        } 
                        Vector3 screenPoint = cam.WorldToScreenPoint(enemy.position);
                        Transform target = enemyUITarget.GetChild(i);
                        target.position = screenPoint;
                        target.GetComponent<Image>().color = new Color(1, 1, 1, 1);

                        if (role == PlayerRole.Striker) {
                            Image image = target.GetChild(0).GetComponent<Image>();
                            image.color = new Color(1, 1, 1, 1);
                            
                            float blood_perc;
                            blood_perc = enemy.GetComponent<EnemyMotion>().getBlood() / enemy.GetComponent<EnemyMotion>().getMaxBlood();

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
                            AudioController.Singleton.PlaySkill1Success();

                            CmdAttack(enemy.GetComponent<NetworkIdentity>().netId);
                            break;
                        }
                    }
                }
                else {
                    if (Vector3.Distance(Input.mousePosition, shieldPoint1) > 60)
                    {
                        if (GetComponent<PlayerInfo>().getHealth() == 0)
                        {
                            reminderController.setReminder("Device damaged. Please ask engineer to repair", 3.0f);
                            return;
                        }

                        Vector3 shieldCenter = (shieldPoint1 + Input.mousePosition) / 2;
                        float radius = Vector3.Distance(Input.mousePosition, shieldCenter);
                        radius = radius <= 100 + rank * 8 ? radius : 100 + rank * 8;

                        Ray ray = cam.ScreenPointToRay(shieldCenter);

                        CmdCreateShield(ray, radius / 30);

                    }
                    else {
                        for (int i = 0; i < enemyUIManager.transform.childCount; i++)
                        {
                            Transform enemy = enemyUIManager.transform.GetChild(i);
                            Vector3 camPos = cam.WorldToScreenPoint(enemy.position);
                            Vector2 enemyPos2d = new Vector2(camPos.x, camPos.y);
                            Vector2 mousePos2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                            if (Vector2.Distance(enemyPos2d, mousePos2d) < 50)
                            {
                                AudioController.Singleton.PlaySkill1Success();

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
                    radius = radius <= Screen.width / 5f + rank * 20 ? radius : Screen.width / 5f + rank * 20;
                    Ray ray = cam.ScreenPointToRay(shieldCenter);

                    CmdCreateShield(ray, radius / 100);

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

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 75)
                        {
                            CmdAttack(enemy.GetComponent<NetworkIdentity>().netId);
                            break;
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

                        if (Vector2.Distance(enemyPos2d, mousePos2d) < 85)
                        {
                            AudioController.Singleton.PlaySkill1Success();

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
	void CmdDoFire()
	{

        if (role == PlayerRole.Striker) {
            
            float d = playerInfo.getSkill(1).damage;
            for (int i = 0; i < enemyUIManager.transform.childCount; i++)
            {
                Transform enemy = enemyUIManager.transform.GetChild(i);
                if (enemy.tag == "Enemy")
                    enemy.GetComponent<EnemyMotion>().DecreaseBlood(d);
                else
                    enemy.GetComponent<BossController>().DecreaseBlood(d * 10);
            }
            skill2Counter++;
            score += ScoreParameter.Stricker_Util_Score;
            strikerUlti.GetComponent<strikerUltimate>().Succeed(isServer);
            RpcStrikerUlti(1);
                
            DoneUlti();
            AudioController.Singleton.PlayStrickerUtliExplosionSound();

        }
        else {
            DoneUlti();
            RpcDefenderUlti(1);
            defenderUlti.GetComponent<defenderUltimate>().Succeed(isServer);
            float freezeTime = GetComponent<StrikerSkill2>().damage;
            freezeTime = freezeTime < 8 ? 8 : freezeTime;
            defenderEnemyManager.GetComponent<EnemySpawnManager>().Freeze(freezeTime);
            enemyUIManager.GetComponent<EnemyAttackFreezer>().Freeze();
            skill2Counter++;
            score += ScoreParameter.Defender_Util_Score;
            
            AudioController.Singleton.PlayStrickerUtliExplosionSound();

        }
        
	}

    public void FireLightening(Vector3 targetPos) {

        transform.GetChild(1).GetChild(0).GetChild(0).position = targetPos;
        transform.GetChild(1).GetChild(1).GetChild(0).position = targetPos;
        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<LighteningAutoBack>().ResetPos(0.1f);
        transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<LighteningAutoBack>().ResetPos(0.1f);

        RpcFireLightening(targetPos);
          
    }

    [ClientRpc]
    public void RpcFireLightening(Vector3 targetPos) {

        transform.GetChild(1).GetChild(0).GetChild(0).position = targetPos;
        transform.GetChild(1).GetChild(1).GetChild(0).position = targetPos;
        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<LighteningAutoBack>().ResetPos(0.1f);
        transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<LighteningAutoBack>().ResetPos(0.1f);
        
    }

    [ClientRpc]
    public void RpcStrikerUlti(int status) {
        
        if (status == -2) {
            strikerUlti.GetComponent<strikerUltimate>().TriggerUlti();
        }
        else if (status == -1) {
            strikerUlti.GetComponent<strikerUltimate>().Fail(isServer);
        }
        else if (status == 0){
            strikerUlti.GetComponent<strikerUltimate>().AddCrystal(isServer);
        }
        else if (status == 1){
            strikerUlti.GetComponent<strikerUltimate>().Succeed(isServer);
        }
    }

    [ClientRpc]
    public void RpcDefenderUlti(int status)
    {
        if (status == -2)
        {
            defenderUlti.GetComponent<defenderUltimate>().TriggerUlti();
        }
        else if (status == -1)
        {
            defenderUlti.GetComponent<defenderUltimate>().Fail(isServer);
        }
        else if (status == 0)
        {
            defenderUlti.GetComponent<defenderUltimate>().AddCrystal(isServer);
        }
        else if (status == 1)
        {
            defenderUlti.GetComponent<defenderUltimate>().Succeed(isServer);
        }
    }

    [Command]
    protected void CmdAttack(NetworkInstanceId netID) {
        GameObject obj = NetworkServer.FindLocalObject(netID);
        if (obj == null) return;
        Transform enemy = obj.transform;

        if (enemy.tag == "Enemy")
            enemy.GetComponent<EnemyMotion>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);
        else
            enemy.GetComponent<BossController>().DecreaseBlood(playerInfo.getSkill(skillIndex).damage);

        FireLightening(enemy.transform.position);

        skill1Counter++;
        score += ScoreParameter.Stricker_Skill1_Score;
    }

    [Command]
    protected void CmdDefendAttack(NetworkInstanceId netID)
    {
        GameObject obj = NetworkServer.FindLocalObject(netID);
        skill1Counter++;
        score += ScoreParameter.Defender_Skill1_Score;
        NetworkServer.Destroy(obj);
    }

    [Command]
    protected void CmdCreateShield(Ray ray, float scale)
    {
        if (shieldExist) {
            RpcDeclineShieldCreation();
            return;
        }

        Vector3 shieldPos = ray.direction.normalized * 8;

        shieldObj.transform.position = shieldPos;

        shieldObj.transform.localScale *= scale;

        shieldObj.transform.LookAt(-ray.direction * 20);

        shieldObj.GetComponent<ShieldCollisionBehaviour>().setMaximumDefendNumber(shieldNumber);

        shieldObj.GetComponent<ShieldCollisionBehaviour>().setCountDown(shieldTime);

        shieldExist = true;

        RpcPutSheild(ray, scale);
    }

    [ClientRpc]
    public void RpcPutSheild(Ray ray, float scale)
    {
        Vector3 shieldPos = ray.direction.normalized * 8;

        shieldObj.transform.position = shieldPos;

        shieldObj.transform.localScale *= scale;

        shieldObj.transform.LookAt(-ray.direction * 20);
        
    }

    public void HideShield() {
        if (isServer) {
            shieldObj.transform.position = new Vector3(0, 0, -100);
            shieldObj.transform.localScale = new Vector3(1, 1, 1);
            shieldExist = false;
            RpcHideShield();
        }
    }

    [ClientRpc]
    public void RpcHideShield() {
        shieldObj.transform.position = new Vector3(0, 0, -100);
        shieldObj.transform.localScale = new Vector3(1, 1, 1);
    }

    [ClientRpc]
    protected void RpcDeclineShieldCreation()
    {
        if (isLocalPlayer) {
            reminderController.setReminder("Shield already exists.", 1.0f);
        }
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

            AudioController.Singleton.PlayStrickerUtliStartSound();

            if (role == PlayerRole.Striker) {
                strikerUlti.GetComponent<strikerUltimate>().TriggerUlti();
                RpcStrikerUlti(-2);
            }
            else {
                defenderUlti.GetComponent<defenderUltimate>().TriggerUlti();
                RpcDefenderUlti(-2);
            }
            for (int i = 0; i < NetworkManagerCustom.SingletonNM.gameplayerControllers.Count; i++) {
                if (NetworkManagerCustom.SingletonNM.gameplayerControllers[i] != null)
                {
                    if (i != slot)
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
                mainCrystalController.OpenCrystalPortal(username);
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
    protected void CmdFailUlti()
    {
        if (role == PlayerRole.Striker) {
            strikerUlti.GetComponent<strikerUltimate>().Fail(isServer);
            RpcStrikerUlti(-1);
        }
        else {
            defenderUlti.GetComponent<defenderUltimate>().Fail(isServer);
            RpcDefenderUlti(-1);
        }

        DoneUlti();
    }

    public void DoneUlti() {
        if (isServer) {
            UltiController.setUltiEnchanting(false);
            for (int i = 0; i < NetworkManagerCustom.SingletonNM.gameplayerControllers.Count; i++)
            {
                if (i != slot && NetworkManagerCustom.SingletonNM.gameplayerControllers[i] != null)
                {
                    ((PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[i]).RpcUnlockUlti();
                }
            }

            AudioController.Singleton.StopStrickerUtliStartSound();
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
        CmdDoFire();
        mainCrystalController.CloseCrystalPortal();
        ultiCrystalController.Clear();
        
    }

    public void RevokeUlti() {
        Debug.Log("PlayerController RevokeUlti");
        int ulti_index = 1;
        skillControllers[ulti_index].RevokeCoolDown();
        mainCrystalController.CloseCrystalPortal();
        CmdFailUlti();
    }

    #endregion

    #region SupportSkill

    [Command]
    public void CmdSupport(int crystalIndex){
        Debug.Log("Player Controller Server: Support Submission Success");
        if (UltiController.checkUltiEnchanting()) {
            PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[UltiController.getUltiPlayerNumber()];
            plc.RpcSupportGivenBack(slot, crystalIndex);
        }
    }

    [ClientRpc]
    public void RpcSupportGivenBack(int slot, int crystalIndex) {
        if (isLocalPlayer){
            Debug.Log("Player Controller: Give Crystal " + crystalIndex + " Back to Client");
            ultiCrystalController.AcceptCrystalFrom(slot, crystalIndex);
        }
    }

    public void UltiFailureHandling() {
        mainCrystalController.CloseCrystalPortal();
        CmdUltiFailureHandling();
        CmdFailUlti();

        reminderController.setReminder("Ultimate Skill Activation Fails.", 3);
    }

    [Command]
    public void CmdUltiFailureHandling() {

        UltiController.setUltiEnchanting(false);
    }

    public void setDraggingCrystal(bool f) {
        isDraggingCrystal = f;
    }

    [Command]
    public void CmdUltiSupportFeedback(int slot, bool feedback) {
        Debug.Log("CmdUltiSupportFeedback " + username);
        if (role == PlayerRole.Striker) {
            strikerUlti.GetComponent<strikerUltimate>().AddCrystal(isServer);
            RpcStrikerUlti(0);
        }
        else {
            defenderUlti.GetComponent<defenderUltimate>().AddCrystal(isServer);
            RpcDefenderUlti(0);
        }
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];
        plc.RpcReceiveSupportFeedback(feedback);
        if (feedback)
        {
            plc.supportCounter++;
            plc.score += ScoreParameter.Support_Score;
        }
    }

    [ClientRpc]
    public void RpcReceiveSupportFeedback(bool feedback) {
        if (isLocalPlayer) {
            if (feedback)
            {
                mainCrystalController.PortalShine();
                AudioController.Singleton.PlayCystalSuccess();
            }
            else
            {
                reminderController.setReminder("Support Wrong Crystals", 3.0f);
                AudioController.Singleton.PlayCystalFail();
            }
        }
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
        //Debug.Log(role);

        GetComponent<PlayerInfo>().setHealthController(ui.transform.GetChild(0).GetChild(0).GetComponent<HealthController>());

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

        msfController = ui.transform.GetChild(5).GetComponent<MainScoreFlowController>();

        citizenshipHealthController = ui.transform.GetChild(0).GetChild(1).GetComponent<HealthController>();

        enemyUITarget = ui.transform.GetChild(6);

        if (role == PlayerRole.Defender) {
            for (int i = 0; i < enemyUITarget.childCount; i++){
                enemyUITarget.GetChild(i).GetComponent<Image>().sprite = enemyIcons[2];
                enemyUITarget.GetChild(i).GetChild(0).GetComponent<Image>().sprite = enemyIcons[3];

            }
        }

        warningController = ui.transform.GetChild(7).GetComponent<WarningController>();

        if (GameObject.Find("EventSystem") != null)
            e = GameObject.Find("EventSystem").GetComponent<EventSystem>();

    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("OnLevelWasLoaded");
        if (level == 3)
        {
            initializeData();

            if (isLocalPlayer)
            {
                //if(level != 0)
                //ClientScene.Ready(connectionToServer);
                isInGame = true;
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
        GameObject t1;
        if (isServer)
            t1 = GameObject.Find("EnemySpawnManager");
        else
            t1 = GameObject.Find("EnemyManager");
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


    public void initializeData() {

        if (playerParameter == null){
            playerParameter = NetworkManagerCustom.SingletonNM.playerData.GetComponent<PlayerParameter>();
        }

        playerParameter = playerParameter.getPlayer(role, rank);
        //Debug.Log("playerParameter " + playerParameter.maxHp);
        GetComponent<PlayerInfo>().max_health = playerParameter.maxHp;
        GetComponent<PlayerInfo>().setHealth(playerParameter.maxHp);

        GetComponent<StrikerSkill1>().coolDown = playerParameter.coolingDown_1;
        GetComponent<StrikerSkill1>().damage =  playerParameter.attackPt;
        GetComponent<StrikerSkill2>().coolDown = playerParameter.coolingDown_2;
        GetComponent<StrikerSkill2>().damage = role == PlayerRole.Striker ? playerParameter.ultiPt : playerParameter.ultiTime;
        if (role == PlayerRole.Defender)
        {
            shieldTime = playerParameter.sheildTime;
            shieldNumber = playerParameter.sheildHp;
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

        perc = perc > 10 ? 10 : perc;

        NetworkManagerCustom.SingletonNM.sGamePanel.GetComponent<ServerGamePanel>().playerInfos[slot].SetHealth(health / GetComponent<PlayerInfo>().getMaxHealth());
    }

    [ClientRpc]
    public void RpcDamage(float damage)
    {
        if (isLocalPlayer) {
            warningController.TriggerWarning();
        }
    }

    [ClientRpc]
    public void RpcAddProgress(float perc) {
        if(isLocalPlayer)
            progressBarController.SetProgress(perc);
    }

    [ClientRpc]
    public void RpcSetCitizenshipHealth(float f) {
        if (isLocalPlayer) {
            citizenshipHealthController.setHealthPerc(f / 10);
        }
            
    }

    public void InitializeDisconnectCrystals(int crystal) {
        disconnectCrystal = crystal;
        Debug.Log("InitializeDisconnectCrystals " + crystal);
    }

    [ClientRpc]
    public void RpcMissionComplete(int s1Counter, int s2Counter, int suCounter, int crank, int cexp)
    {
        if (isLocalPlayer) {
            Debug.Log("Mission Complete Local");
            ui.SetActive(false);
            //int score = ScoreParameter.CalcuateScore(s1Counter, s2Counter, suCounter);
            int currentFullExp = ScoreParameter.CurrentFullExp(crank);
            
            int roleIndex = 0;
            switch (role)
            {
                case PlayerRole.Striker: roleIndex = 1; break;
                case PlayerRole.Engineer: roleIndex = 2; break;
                case PlayerRole.Defender: roleIndex = 3; break;
            }
            DataServerUtil.Singleton.UpdateRankExp(crank, cexp, roleIndex);
            int stars = ScoreParameter.CalcuateStar(score);
            NetworkManagerCustom.SingletonNM.MissionComplete(score, stars, username, role, crank, cexp, currentFullExp,
                s1Counter, s2Counter, suCounter);
        }
    }


    [Command]
    public void CmdChangeRank(int newrank, int newexp)
    {
        rank = newrank;
        exp = newexp;
        initializeData();
    }

    [ClientRpc]
    public void RpcClosePortal() {
        if (isLocalPlayer) {
            GameObject portal = GameObject.Find("newPortal-blue");
            for (int i = 1; i < portal.transform.childCount; i++) {
                portal.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    [ClientRpc]
    public void RpcSetTotalWave(int num) {
        if (isLocalPlayer) {
            ui.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = num.ToString();
        }
    }

    [ClientRpc]
    public void RpcSetCurrentWave(int num)
    {
        if (isLocalPlayer)
        {
            ui.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = num.ToString();
        }
    }

    public void OnScoreChanged(int ns)
    {
        if (!isLocalPlayer) return;
        //Debug.Log("OnScoreChanged " + ns);

        int tempnum = ns - score;

        if (tempnum == ScoreParameter.Stricker_Util_Score || tempnum == ScoreParameter.Defender_Util_Score)
        {
            msfController.Flow(0, "Ultimate Skill ", tempnum);
        }
        else if (tempnum == ScoreParameter.Support_Score)
        {
            msfController.Flow(1, "Support ", tempnum);
        }
        else if (tempnum == ScoreParameter.Engineer_Skill1_Score)
        {
            msfController.Flow(0, "Heal ", tempnum);
        }
        else if (tempnum == ScoreParameter.Engineer_Skill2_Score)
        {
            msfController.Flow(2, "Production ", tempnum);
        }

        score = ns;
        //update score
        if (scoreText != null)
            scoreText.text = "" + score;
    }
    public void OnRankChanged(int nr)
    {
        if (!isLocalPlayer) return;
        rank = nr;
    }

    public bool CheckServer() {
        return isServer;
    }

    #endregion

    //[ClientRpc]
    //public void RpcPauseGame(bool value)
    //{
    //    if (isLocalPlayer)
    //    {
    //        if (e == null)
    //            e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    //        e.gameObject.SetActive(!value);
    //    }
    //}

    public void OnPauseChanged(bool value)
    {
        if (!isLocalPlayer) return;

        isPause = value;

        //Debug.Log("OnPauseChanged " + value);
        if (e == null)
            e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        e.enabled = !value;
    }


}
