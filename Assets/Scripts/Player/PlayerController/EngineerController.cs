using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EngineerController : PlayerController {

    public struct teammateInfo {
        public int slot;
        public PlayerRole role;
        public string un;
    }

    public class SyncListTeammateInfo : SyncListStruct<teammateInfo>{}

    public SyncListTeammateInfo teammatesInfo = new SyncListTeammateInfo();

    public Sprite[] teammatePhotoes;

    private bool teammateInitialized = false;

    void Start()
    {
        playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
        {
            Debug.Log("ui initialized");
            ui = (GameObject)Instantiate(uiPrefab, transform.position, Quaternion.identity);
            GameObject.DontDestroyOnLoad(ui);

            setEngineerController(ui);

            cam.enabled = true;

            skillControllers = new SkillController[2];
            for (int i = 0; i <= 1; i++)
            {
                skillControllers[i] = GameObject.Find("Skill" + i + "_Image").GetComponent<SkillController>();
                skillControllers[i].setSkill(playerInfo.getSkill(i));
            }

            skillIndex = 0;

            Debug.Log("Start " + teammatesInfo.Count);
            if (teammatesInfo.Count == 0)
            {
                teammateInitialized = false;
            }
            else {
                teammateInitialized = true;
                initializeTeammateUI();
            }

            //foreach (DCCrystalInfo ci in crystalInfoList)
            //{
                //mainCrystalController.SetCrystal(ci.key, ci.value);
            //}
            scoreText = ui.transform.GetChild(7).GetChild(0).GetComponent<Text>();
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

    void Update() {
        
        if (!isLocalPlayer) return;

        if (!disconnectedCrystalInitialized)
        {
            int a = disconnectCrystal;
            for (int i = 3; i >= 0; i--)
            {
                mainCrystalController.SetCrystal(i, (a & 7) - 1);
                a >>= 8;
            }
            disconnectedCrystalInitialized = true;
            initializeData();
            GetComponent<PlayerInfo>().setHealth(GetComponent<PlayerInfo>().getHealth());
        }

        if (!teammateInitialized) {
            if (teammatesInfo.Count != 0) {
                teammateInitialized = true;
                initializeTeammateUI();
            }
        }
    }

 

    #region Engineer
    [Command]
    public void CmdAssignCrystal(int slot, int crystal) {
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];

        if (plc != null)
        {
            plc.RpcAcceptCrystalFromEngineer(crystal);
            skill2Counter++;
            score += ScoreParameter.Engineer_Skill2_Score;
        }
    }

    [Command]
    public void CmdHealTeammate(int slot) {
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];

        if (plc != null)
        {
            plc.Damage(-GetComponent<EngineerSkill1>().heal);
            skill1Counter++;
            score += ScoreParameter.Engineer_Skill1_Score;
        }
    }

    [Command]
    public void CmdHealCitizenship() {
        NetworkManagerCustom.SingletonNM.AttackCitizenship(-(2 + rank / 2));
        skill1Counter++;
        score += ScoreParameter.Engineer_Skill1_Score;
    }
    #endregion

    #region Initialization

    [Command]
    private void CmdRequestTeammateInfo(int s) {

        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[s];
        
        if (plc != null){
            RpcReceiveTeammateInfo(false, s, plc.role, plc.username);
        }
        else{
            RpcReceiveTeammateInfo(true, s, PlayerRole.Defender, "whatever");
        }
            
    }


    [ClientRpc]
    private void RpcReceiveTeammateInfo(bool isNull, int s, PlayerRole r, string un) {

        if (isLocalPlayer) {

            Transform child = ui.transform.GetChild(5).GetChild(s);

            if (isNull)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                if (r == PlayerRole.Striker)
                {
                    child.GetComponent<Image>().sprite = teammatePhotoes[0];
                }
                else if (r == PlayerRole.Defender)
                {
                    child.GetComponent<Image>().sprite = teammatePhotoes[1];
                }
                else
                {
                    child.GetComponent<Image>().sprite = teammatePhotoes[2];
                }
                child.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                child.GetChild(1).GetComponent<Text>().text = un;
            }
        }
    }
    
    public void initializeTeammateUI() {

        for (int i = 0; i < teammatesInfo.Count; i++)
        {
            Transform child = ui.transform.GetChild(5).GetChild(teammatesInfo[i].slot);
            PlayerRole r = teammatesInfo[i].role;
            if (r == PlayerRole.Striker)
            {
                child.GetComponent<Image>().sprite = teammatePhotoes[0];
            }
            else if (r == PlayerRole.Defender)
            {
                child.GetComponent<Image>().sprite = teammatePhotoes[1];
            }
            else
            {
                child.GetComponent<Image>().sprite = teammatePhotoes[2];
            }
            child.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            child.GetChild(1).GetComponent<Text>().text = teammatesInfo[i].un;
        }
    }

    public void initializeTeammate(int slot, PlayerRole role, string un) {

        teammateInfo info = new teammateInfo();
        info.slot = slot;
        info.role = role;
        info.un = un;
        teammatesInfo.Add(info);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 3)
        {
            initializeData();

            Debug.Log("On Level was Loaded");

            if (isLocalPlayer)
            {
                //if (level != 0)
                //ClientScene.Ready(connectionToServer);
                if (level == 0) return;
                //initializeTeammateUI();
                cam.cullingMask = (1 << (slot + 8)) | 1 | 1 << 13 | 1 << 12;

                mainCrystalController.AcceptCrystal(Random.Range(0, 4));
                mainCrystalController.AcceptCrystal(Random.Range(0, 4));
            }
        }


    }


    private void setEngineerController(GameObject ui)
    {
        GetComponent<PlayerInfo>().setHealthController(ui.transform.GetChild(0).GetChild(0).GetComponent<HealthController>());
        ui.transform.GetChild(0).GetChild(1).GetComponent<EngiCitizenshipController>().engineerController = this;
        
        Transform skillPanel = ui.transform.GetChild(1);

        skillPanel.GetChild(0).GetComponent<EngiSkill0Controller>().setPlayerController(this);
        skillPanel.GetChild(1).GetComponent<EngiSkill1Controller>().setPlayerController(this);
        skillPanel.GetChild(2).GetComponent<SkillController>().setPlayerController(this);

        Transform supportCrystalPanel = ui.transform.GetChild(2);
        mainCrystalController = supportCrystalPanel.GetComponent<MainCrystalController>();
        mainCrystalController.SetPlayerController(this);

        GameObject ulticrystalObject = ui.transform.GetChild(3).gameObject;
        ultiCrystalController = ulticrystalObject.GetComponent<UltiCrystalController>();
        ultiCrystalController.setPlayerController(this);

        reminderController = ui.transform.GetChild(4).GetComponent<ReminderController>();

        
        citizenshipHealthController = ui.transform.GetChild(0).GetChild(1).GetComponent<HealthController>();

        Transform teammatePanel = ui.transform.GetChild(5);
        for (int i = 0; i < 4; i++)
        {
            teammatePanel.GetChild(i).GetComponent<EngiTeammateController>().setEngineerController(this);
        }

        Transform crystalProductionPanel = ui.transform.GetChild(6);
        crystalProductionPanel.GetComponent<CrystalProductionController>().setEngineerConrtroller(this);

        warningController = ui.transform.GetChild(8).GetComponent<WarningController>();

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //GameObject.DontDestroyOnLoad(e);
    }

    public void initializeData()
    {
        if (playerParameter == null) {
            playerParameter = NetworkManagerCustom.SingletonNM.playerData.GetComponent<PlayerParameter>();
        }
        playerParameter = playerParameter.getPlayer(role, rank);
        Debug.Log("playerParameter " + playerParameter.maxHp);
        Debug.Log("playerParameter " + playerParameter.coolingDown_1);
        Debug.Log("playerParameter " + playerParameter.healPt);
        GetComponent<PlayerInfo>().max_health = playerParameter.maxHp;
        GetComponent<PlayerInfo>().setHealth(playerParameter.maxHp);

        GetComponent<EngineerSkill1>().coolDown = playerParameter.coolingDown_1;
        GetComponent<EngineerSkill1>().heal = playerParameter.healPt;

        
    }

    #endregion
}
