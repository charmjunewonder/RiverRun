using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;
    public int slot;
	[SerializeField] 
	private Camera cam;

	[SyncVar]
	private int skillIndex;

    private int[] crystals;

	private EventSystem e;
	public SkillController[] uiControllers;
	private PlayerInfo playerInfo;

	private Skill[] skills;
    private SpriteRenderer uiTarget;

    private UltiCrystalController ultiCrystalController;
    private ReminderController reminderController;

	void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
		if (isLocalPlayer) {
			GameObject ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity);
            GameObject.DontDestroyOnLoad(ui);
            GameObject.DontDestroyOnLoad(gameObject);

            setControllers(ui);
			
			cam.enabled = true;

			uiControllers = new SkillController[3];
			for(int i = 1; i <= 3; i++){
				uiControllers[i-1] = GameObject.Find ("Skill" + i + "_Image").GetComponent<SkillController>();
				uiControllers[i-1].setSkill(playerInfo.getSkill(i-1));
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
            if (!e.IsPointerOverGameObject() && !uiControllers[skillIndex].getCoolDownStatus())
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


    private void setControllers(GameObject ui) {

        Transform skillPanel = ui.transform.GetChild(1);
        skillPanel.GetChild(0).GetComponent<SkillController>().setPlayerController(this);
        skillPanel.GetChild(1).GetComponent<SkillController>().setPlayerController(this);
        skillPanel.GetChild(2).GetComponent<SkillController>().setPlayerController(this);

        Transform supportCrystalPanel = ui.transform.GetChild(2);
        supportCrystalPanel.GetChild(0).GetComponent<CrystalController>().setPlayerController(this);
        supportCrystalPanel.GetChild(1).GetComponent<CrystalController>().setPlayerController(this);
        supportCrystalPanel.GetChild(2).GetComponent<CrystalController>().setPlayerController(this);
        supportCrystalPanel.GetChild(3).GetComponent<CrystalController>().setPlayerController(this);


        ultiCrystalController = ui.transform.GetChild(3).GetComponent<UltiCrystalController>();
        ultiCrystalController.setPlayerController(this);
        reminderController = ui.transform.GetChild(4).GetComponent<ReminderController>();

        uiTarget = transform.GetChild(2).GetComponent<SpriteRenderer>();

        e = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }


    #region Operation
    [Command]
	void CmdDoFire(int index, Ray ray)
	{
        GameObject bullet = (GameObject)Instantiate(playerInfo.getSkill(index).prefab, transform.position + ray.direction * 1, Quaternion.identity);
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		
	}
    #endregion

    #region UltiActivation

    [Command]
	public void CmdSetSkillIndex(int index){ skillIndex = index; }
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
            if (status)
            {
                ultiCrystalController.GenerateUltiCrystals();
                uiControllers[2].StartCoolDown();
            }
        }
    }

    public void ActivateUlti() {
        Debug.Log("Player Controller: Activate Ulti Success");
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        CmdDoFire(2, ray);
        
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

    public void SetSlot(int s)
    {
        slot = s;
    }
}
