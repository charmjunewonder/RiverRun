using UnityEngine;
using System.Collections;

public class LighteningAutoBack : MonoBehaviour {

    public void ResetPos(float time) {
        StartCoroutine(ResetToOrigin(time));
    }

    IEnumerator ResetToOrigin(float time) {
        yield return new WaitForSeconds(time);

        transform.position = transform.parent.GetChild(1).position;
    }
    
}
