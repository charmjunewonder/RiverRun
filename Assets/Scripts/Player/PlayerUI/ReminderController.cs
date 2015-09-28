using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReminderController : MonoBehaviour {

    private Text reminder;

    private float timer;

	void Start () {
        reminder = gameObject.GetComponent<Text>();
        timer = 0;
	}
	
	void Update () {
        if (timer > 0) {
            timer -= Time.deltaTime;
            if (timer <= 0) {
                reminder.text = "";
            }
        }
	}

    public void setReminder(string text, float t){
        reminder.text = text;
        timer = t;
    }

}
