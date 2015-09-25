using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class PlayerController : NetworkBehaviour {

	public GameObject uiPrefab;

	[SerializeField] 
	private Camera cam;

	[SyncVar]
	private int skillIndex;


	private EventSystem e;
	private UIController[] uiControllers;
	private PlayerInfo playerInfo;

	private Skill[] skills;

	void Start () {
		playerInfo = gameObject.GetComponent<PlayerInfo>();
		if (isLocalPlayer) {
			GameObject ui = (GameObject)Instantiate (uiPrefab, transform.position, Quaternion.identity);

			Transform skillPanel = ui.transform.GetChild (1);
			skillPanel.GetChild (0).GetComponent<UIController>().setPlayerController(this);
			skillPanel.GetChild (1).GetComponent<UIController>().setPlayerController(this);
			skillPanel.GetChild (2).GetComponent<UIController>().setPlayerController(this);

			cam.enabled = true;


			e = GameObject.Find("EventSystem").GetComponent<EventSystem>();

			uiControllers = new UIController[3];
			for(int i = 1; i <= 3; i++){
				uiControllers[i-1] = GameObject.Find ("Skill" + i + "_Image").GetComponent<UIController>();
				uiControllers[i-1].setSkill(playerInfo.getSkill(i-1));
			}

			skillIndex = 0;
		}

	}
	
	// Update is called once per frame
	void Update () {		
		if(!isLocalPlayer) return;
		cam.enabled = true;
#if UNITY_IOS
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
#endif
#if UNITY_STANDALONE_WIN
		if (Input.GetMouseButtonUp(0)) {
#endif
			if(!e.IsPointerOverGameObject()){
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);		
				Debug.Log (skillIndex);

				Debug.Log (ray);
				CmdDoFire(10, ray);
			}
#if UNITY_IOS
#endif
		}		
	}
	
	[Command]
	void CmdDoFire(float lifeTime, Ray ray)
	{
		Debug.Log (skillIndex);

		GameObject bullet = (GameObject)Instantiate (playerInfo.getSkill(skillIndex).prefab, transform.position + ray.direction * 1, Quaternion.identity);
		
		
		NetworkServer.Spawn(bullet);
		bullet.AddComponent<Rigidbody>();
		bullet.GetComponent<Rigidbody>().useGravity = false;
		bullet.GetComponent<Rigidbody>().velocity = ray.direction * 100;
		
	}
	
	[Command]
	public void CmdSetSkillIndex(int index){ skillIndex = index; }
	
}
