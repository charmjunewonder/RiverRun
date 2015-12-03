using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthController : MonoBehaviour {

    public Sprite[] sprites;

    public Text text;

    private bool shinning;

    private float timer;

    private Color red = new Color(176, 46, 69, 1);
    private Color blue = new Color(124, 195, 255, 1);

    void Start() {
        shinning = false;
        timer = 0.5f;
        text.color = blue;
    }

    void Update(){
        if (shinning) {
            timer -= Time.deltaTime;

            if (timer <= 0) {
                if (transform.GetChild(0).GetComponent<Image>().color.a == 0) {
                    for (int i = 0; i < 10; i++) {
                        transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                else {
                    for (int i = 0; i < 10; i++)
                    {
                        transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }
                }
                timer = 0.5f;
            }
        }
    }

    public void setHealth(int health) {

        for (int i = 0; i < 10; i++){
            int index = health > 5 ? 0 : 2;
            transform.GetChild(i).GetComponent<Image>().sprite = i < health ? sprites[index] : sprites[index + 1];
        }

        shinning = health <= 3;

        if (!shinning){
            timer = 0.5f;
            for (int i = 0; i < 10; i++)
            {
                transform.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void setHealthPerc(float perc) {
        text.text = ((int)(perc * 100)).ToString() + "%";
        if (perc <= 0.5f)
            text.color = red;
        else
            text.color = blue;
    }
}
