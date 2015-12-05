using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextChange:MonoBehaviour{
    //private string m_text;
    public float text_ID;
    private int Id;
    private Text text;
	// Use this for initialization
    private string[] all_content = 
    {" ",
    "Peacekeeping Association in Region Number 6553 has decided to form a new fleet to help escort interplanet traveller ships and important cargo. Welcome aboard, new recruits.",
    "As members of this ship, everyone should cooperate together to complete our missions. We may have challenges and dangers, but I believe as a team we can overcome them.",
    "We have three crew positions for you now.",
    "Strikers, are in charge of getting rid of hostile units.\nDefenders, build up the shield and protect our ship.\nEngineers, repair all the systems and produce the energy crystals.",
    "Our mission is to protect the citizenship and close the pirates portals.",
    "Remember, you should work as a team to complete the mission, \nARE YOU READY?",
    ""
    };
	void Start () {
        text_ID = 0;
        //get component 
        Id = 0;
        text = GetComponent<Text>();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Id != (int)(text_ID)){
            Id = (int)text_ID;
            int id = Mathf.Clamp(Id, 0, all_content.Length - 1);
            text.text = all_content[id];
        }
        if (text_ID - (int)(text_ID) < 0.05)
        {
            text.color = new Color(1, 1, 1, 20 * (text_ID - (int)text_ID));
        }
        if (text_ID >= all_content.Length)
        {
            JumpToLobbyScene();
        }
	
	}

    public void JumpToLobbyScene()
    {
        Application.LoadLevel("Lobby");
    }
}
