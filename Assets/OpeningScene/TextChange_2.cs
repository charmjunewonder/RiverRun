using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextChange_2:MonoBehaviour{
    //private string m_text;
    public float text_ID;
    private int Id;
    private Text text;
	// Use this for initialization
    private string[] all_content = 
    {" ",
    "In the year 2200AD, humans discover an ancient Lunar underground city.\nFive years later, humans decode technology from documents discovered in \nthat city.\nWith that technology, humans generate four energy crystals.",
    "By combining crystals, humans harness a power they never imagined. The technology helps them to emigrate to new planets far from the solar system.",
    "Crystals act as the fuel for interstellar spaceships. In addition, space is always dangerous, so humans equip their fleets with crystal energy weapons and shields to defend themselves and protect innocent citizenships from asteroids and evil space pirates.",
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
        //fade effect
        if (text_ID - (int)(text_ID) < 0.1)
        {
            text.color = new Color(1, 1, 1, 10 * (text_ID - (int)text_ID));
        }
	
	}
}
