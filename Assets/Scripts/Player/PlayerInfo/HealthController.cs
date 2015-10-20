using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthController : MonoBehaviour {

    public Sprite[] sprites;

    public void setHealth(int health) {

        for (int i = 0; i < 10; i++){
            transform.GetChild(i).GetComponent<Image>().sprite = i < health ? sprites[0] : sprites[1];
        }
    }
}
