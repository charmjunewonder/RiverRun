using UnityEngine;
using System.Collections;

public class MainScoreFlowController : MonoBehaviour {

    public void Flow(int num, string s, int score) {
        Debug.Log("MainScoreFlowController " + num + " " + s + " " + score);
        transform.GetChild(num + 1).GetComponent<ScoreFlowController>().StartFlow(s, score);
    }
}
