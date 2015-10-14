using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;
    public int slot;

    [SyncVar]
    public PlayerRole role;

    public Sprite[] skillIcons;

    [SerializeField] 
	protected Camera cam;

	[SyncVar]
	protected int skillIndex;

    protected int[] crystals;

    protected EventSystem e;
    protected SkillController[] skillControllers;
    protected PlayerInfo playerInfo;

    protected Skill[] skills;
    private SpriteRenderer uiTarget;

    protected UltiCrystalController ultiCrystalController;
    protected MainCrystalController mainCrystalController;

    protected ReminderController reminderController;

	void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
        GameObject.DontDestroyOnLoad(gameObject);

        if (isLocalPlayer) {
			GameObject ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity);
            GameObject.DontDestroyOnLoad(ui);

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
	
	// Update is called once per frame
	void Update () {		
		if(!isLocalPlayer) return;


        cam.enabled = true;

#if UNITY_STANDALONE_WIN  
        if (Input.GetMouseButtonDown(0))
        {
            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {
                Color color = uiTarget.color;

                uiTarget.color = new Color(color.r, color.g, color.b, 1);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (uiTarget.color.a != 0)
            {
                uiTarget.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, uiTarget.transform.position.z);
            }
        }
        if(Input.GetMouseButtonUp(0)){
            if (!e.IsPointerOverGameObject() && !skillControllers[skillIndex].getCoolDownStatus())
            {

                skillControllers[skillIndex].StartCoolDown();

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                Debug.Log("fire " + skillIndex);

                CmdDoFire(skillIndex, ray);

                Color color = uiTarget.color;
                uiTarget.color = new Color(color.r, color.g, color.b, 0);
            }
            else if (!e.IsPointerOverGameObject() && skillControllers[skillIndex].getCoolDownStatus())
            {
                reminderController.setReminder("Skill is Cooling Down", 1);
            }
        }
#endif
#if UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (!e.IsPointerOverGameObject() && !uiControllers[skillIndex].getCoolDownStatus())
            {

                uiControllers[skillIndex].StartCoolDown();

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                CmdDoFire(skillIndex, ray);

                Color color = uiTarget.color;
                uiTarget.color = new Color(color.r, color.g, color.b, 0);
            }
            else if (!e.IsPointerOverGameObject() && uiControllers[skillIndex].getCoolDownStatus())
            {
                reminderController.setReminder("Skill is Cooling Down", 1);
            }
        }
#endif
	}

    void OnLevelWasLoaded(int level)
    {
        if (isLocalPlayer)
        {
            ClientScene.Ready(connectionToServer);
        }

        if (level == 13)
            print("Woohoo");

    }

    private void setStrikerDefenderControllers(GameObject ui) {
        Debug.Log(role);
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

        uiTarget = transform.GetChild(2).GetComponent<SpriteRenderer>();

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(e);
    }



    #region Operation
    [Command]
	void CmdDoFire(int index, Ray ray)
	{
        Debug.Log("CmdDoFire " + index);

        GameObject bullet = (GameObject)Instantiate(playerInfo.getSkill(index).prefab, transform.position + ray.direction * 1, Quaternion.identity);
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		
	}
    #endregion

    #region UltiActivation

    public void SetSkillIndex(int index) { skillIndex = index; }

    public void RequestUlti(){
        Debug.Log("Player Controller: Ulti Request");
        CmdRequestUlti();
    }

    [Command]
    private void CmdRequestUlti(){
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
    private void RpcUltiActivationStatusUpdate(bool status) {
        if (isLocalPlayer){
            Debug.Log("Player Controller: Start Cooling Down");
            Debug.Log("skillIndex " + skillIndex);
            if (status){
                ultiCrystalController.GenerateUltiCrystals();
                skillControllers[1].StartCoolDown();
            }
        }
    }

    public void ActivateUlti() {
        Debug.Log("Player Controller: Activate Ulti Success");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        CmdDoFire(2, ray);
    }

    public void RevokeUlti() {
        Debug.Log("PlayerController RevokeUlti");
        skillControllers[1].RevokeCoolDown();
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
        mainCrystalController.AcceptCrystal(crys_num);
    }

    #endregion

    public void SetSlot(int s)
    {
        slot = s;
    }

}
