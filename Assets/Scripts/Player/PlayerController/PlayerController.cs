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

    [SyncVar]
    private int[] crystals;

	private EventSystem e;
	private SkillController[] uiControllers;
	private PlayerInfo playerInfo;

	private Skill[] skills;
    private SpriteRenderer uiTarget;

    private CrystalController crystalController;


	void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
		if (isLocalPlayer) {
			GameObject ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity);

			Transform skillPanel = ui.transform.GetChild (1);
			skillPanel.GetChild (0).GetComponent<SkillController>().setPlayerController(this);
			skillPanel.GetChild (1).GetComponent<SkillController>().setPlayerController(this);
			skillPanel.GetChild (2).GetComponent<SkillController>().setPlayerController(this);

            uiTarget = transform.GetChild (2).GetComponent<SpriteRenderer>();

			cam.enabled = true;


			e = GameObject.Find("EventSystem").GetComponent<EventSystem>();

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

        if(Input.GetMouseButtonDown(0)){
            if(!e.IsPointerOverGameObject() && !uiControllers[skillIndex].getCoolDownStatus()){
                Color color = uiTarget.color;

                uiTarget.color = new Color(color.r, color.g, color.b, 1);
                Debug.Log (uiTarget.color);
            }
        }
        if (Input.GetMouseButton (0)) {
            if(uiTarget.color.a != 0){
                uiTarget.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, uiTarget.transform.position.z);
            }
        }
		if (Input.GetMouseButtonUp(0)) {

			if(!e.IsPointerOverGameObject() && !uiControllers[skillIndex].getCoolDownStatus()){
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);		
                Debug.Log (uiControllers[skillIndex]);
				uiControllers[skillIndex].StartCoolDown();
			    CmdDoFire(10, ray);

                Color color = uiTarget.color;
                uiTarget.color = new Color(color.r, color.g, color.b, 0);
            } else if(!e.IsPointerOverGameObject() && !uiControllers[skillIndex].getCoolDownStatus()){
                
            }

		}		
	}
	
	[Command]
	void CmdDoFire(float lifeTime, Ray ray)
	{

		GameObject bullet = (GameObject)Instantiate (playerInfo.getSkill(skillIndex).prefab, transform.position + ray.direction * 1, Quaternion.identity);
		
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		
	}
	
	[Command]
	public void CmdSetSkillIndex(int index){ skillIndex = index; }

    [Command]
    public void CmdActivateUlti() {
        int len = (int)Random.Range(3.0f, 5.99f);
        crystals = new int[len];
        for (int i = 0; i < len; i++) {
            crystals[i] = (int)Random.Range(0.0f, 2.99f);
        }
    }

}
