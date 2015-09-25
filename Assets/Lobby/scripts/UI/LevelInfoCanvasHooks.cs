using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelInfoCanvasHooks : MonoBehaviour {
	public Text levelText;
	
	public void ShowInfo(int level){
		levelText.text = "" + level;
	}
	
}
