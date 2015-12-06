using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialShinningImage : MonoBehaviour {

    private float t;

    void Start()
    {
        t = 0.5f;
    }

    void Update()
    {
        t -= Time.deltaTime;

        if (t <= 0)
        {
            if (GetComponent<Image>().color.a == 0)
                GetComponent<Image>().color = new Color(1, 1, 1, 1);
            else
                GetComponent<Image>().color = new Color(1, 1, 1, 0);
            t = 0.5f;
        }
    }
}
