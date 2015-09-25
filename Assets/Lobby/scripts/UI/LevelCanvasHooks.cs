using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelCanvasHooks : MonoBehaviour {
	public delegate void CanvasHook();
	
	public CanvasHook OnLevel1;
	public PlayerLobby pLobby;
	
	public Text infoText;
	public GameObject levelButtons;
	
	public void ShowInfo(){
		infoText.gameObject.SetActive(true);
	}
	
	public void ShowLevel(){
		infoText.gameObject.SetActive(false);
		levelButtons.SetActive(true);
	}
	
	public void OnChooseLevel1(){
		pLobby.CmdChooseLevel(1);
	}

}
