using UnityEngine;
using System.Collections;

public class TutorialClicked : MonoBehaviour {

	public TutorialEngineerController teController;

    public void ClickedDelay(int index) {
        StartCoroutine(call(index, 2.0f));
    }

    IEnumerator call(int index, float t) {
        yield return new WaitForSeconds(t);

        teController.SetStage(index);
    }

}
