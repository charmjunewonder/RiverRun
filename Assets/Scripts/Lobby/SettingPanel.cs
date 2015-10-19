using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour {
    public InputField gameInput;
    public InputField dataInput;

    public Text gameText;
    public Text dataText;

	// Use this for initialization
	void Start () {
        string gameIp = PlayerPrefs.GetString("GameIp");
        if (ServerUtils.CheckIpAddress(gameIp))
        {
            gameInput.text = gameIp;
        }

        string dataIp = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(dataIp))
        {
            dataInput.text = dataIp;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnEndEditGameIp()
    {
        Debug.Log("OnEndEditGameIp " + gameInput.text);
        if (ServerUtils.CheckIpAddress(gameInput.text))
        {
            Debug.Log("OnEndEditGameIp Success" + gameInput.text);
            PlayerPrefs.SetString("GameIp", gameInput.text);
            gameText.text = "";

        }
        else
        {
            gameText.text = "IP Invalid";
        }
    }

    public void OnEndEditDataIp()
    {
        Debug.Log("OnEndEditDataIp " + dataInput.text);
        if (ServerUtils.CheckIpAddress(dataInput.text))
        {
            Debug.Log("OnEndEditDataIp Success" + dataInput.text);
            PlayerPrefs.SetString("DataIp", dataInput.text);
            dataText.text = "";
        }
        else
        {
            dataText.text = "IP Invalid";
        }
    }
}
