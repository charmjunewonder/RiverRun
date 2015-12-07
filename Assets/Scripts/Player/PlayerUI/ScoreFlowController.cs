using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreFlowController : MonoBehaviour {

    private Vector3 originPos;

	void Start () {
	    originPos = transform.localPosition;
	}
	
    public void StartFlow(string s, int score) {

        Debug.Log("ScoreFlowController StartFlow");

        if (transform.GetChild(0).GetComponent<Text>().color.a > 0)
            return;

        Debug.Log("ScoreFlowController StartFlow 11111");

        transform.GetChild(0).GetComponent<Text>().text = s;
        transform.GetChild(1).GetComponent<Text>().text = "+" + score;

        transform.localPosition = originPos;
        StartCoroutine("Flow");
    }

    IEnumerator Flow() {
        float alpha = 1;
        while (alpha > 0) {
            alpha -= 0.04f;
#if UNITY_STANDALONE_WIN
            transform.localPosition += new Vector3(0, 2.5f, 0);
#endif
#if UNITY_IOS
            transform.localPosition += new Vector3(0, 7, 0);
#endif
            transform.GetChild(0).GetComponent<Text>().color = new Color(1, 1, 1, alpha);
            transform.GetChild(1).GetComponent<Text>().color = new Color(1, 1, 1, alpha);
            yield return new WaitForSeconds(0.04f);
        }
        transform.localPosition = originPos;
    }
}
