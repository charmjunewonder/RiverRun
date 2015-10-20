using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EngineerController : PlayerController {

    public Sprite[] teammatePhotoes;

    protected GameObject ui;

    void Start()
    {
        playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
        {
            ui = (GameObject)Instantiate(uiPrefab, transform.position, Quaternion.identity);
            GameObject.DontDestroyOnLoad(ui);

            setEngineerController(ui);

            cam.enabled = true;

            skillControllers = new SkillController[3];
            for (int i = 0; i <= 2; i++)
            {
                skillControllers[i] = GameObject.Find("Skill" + i + "_Image").GetComponent<SkillController>();
                skillControllers[i].setSkill(playerInfo.getSkill(i));
            }

            skillIndex = 0;


        }
	}

    void Update() {
    
    
    }

 

    #region Engineer
    [Command]
    public void CmdAssignCrystal(int slot, int crystal) {
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];
        
        if(plc != null)
            plc.RpcAcceptCrystalFromEngineer(crystal);
    }

    [Command]
    public void CmdHealTeammate(int slot) {
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];

        if (plc != null)
            plc.RpcAcceptHealFromEngineer(GetComponent<EngineerSkill1>().heal);
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

    private void initializeTeammate() {
        for (int i = 0; i < 4; i++)
        {
            CmdRequestTeammateInfo(i);
        }
    }


    void OnLevelWasLoaded(int level)
    {
        if (isLocalPlayer)
        {
            //if (level != 0)
            //ClientScene.Ready(connectionToServer);
            if (level == 0) return;
            Invoke("initializeTeammate", 0.5f);
        }

        if (level == 13)
            print("Woohoo");
    }


    private void setEngineerController(GameObject ui)
    {
        GetComponent<PlayerInfo>().setHealthController(ui.transform.GetChild(0).GetComponent<HealthController>());

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

        Transform teammatePanel = ui.transform.GetChild(5);
        for (int i = 0; i < 4; i++)
        {
            teammatePanel.GetChild(i).GetComponent<EngiTeammateController>().setEngineerController(this);
        }

        Transform crystalProductionPanel = ui.transform.GetChild(6);
        crystalProductionPanel.GetComponent<CrystalProductionController>().setEngineerConrtroller(this);

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //GameObject.DontDestroyOnLoad(e);
    }


    #endregion
}
