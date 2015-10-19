using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EngineerController : PlayerController {

    void Start()
    {
        playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
        {
            GameObject ui = (GameObject)Instantiate(uiPrefab, transform.position, Quaternion.identity);
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

    private void setEngineerController(GameObject ui)
    {
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
        for (int i = 0; i < 4; i++) {
            teammatePanel.GetChild(i).GetComponent<EngiTeammateController>().setEngineerController(this);
        }

        Transform crystalProductionPanel = ui.transform.GetChild(6);
        crystalProductionPanel.GetComponent<CrystalProductionController>().setEngineerConrtroller(this);

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        //GameObject.DontDestroyOnLoad(e);
    }

    [Command]
    public void CmdAssignCrystal(int slot, int crystal) {
        PlayerController plc = (PlayerController)NetworkManagerCustom.SingletonNM.gameplayerControllers[slot];

        plc.RpcAcceptCrystalFromEngineer(crystal);

    }


    void OnLevelWasLoaded(int level)
    {
        if (isLocalPlayer)
        {
            //if (level != 0)
            //ClientScene.Ready(connectionToServer);
        }

        if (level == 13)
            print("Woohoo");
    }
}
