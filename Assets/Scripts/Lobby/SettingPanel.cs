using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour {
    public InputField gameInput;
    public InputField dataInput;

    public Text gameText;
    public Text dataText;
    public Slider bgmSlider;
    public Slider sfxSlider;
    private GameObject bgmAudio;
    private GameObject sfxAudio;
    public Sprite greySprite;
    public Image noPlayerImage;
    public Text noPlayerText;
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
#if UNITY_IOS
        noPlayerImage.sprite = greySprite;
        noPlayerText.gameObject.SetActive(false);
#endif
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

    public void OnSoundEffectChanged()
    {
        //Debug.Log("OnSoundEffectChanged " + sfxSlider.value);

        if (sfxAudio == null)
            sfxAudio = GameObject.Find("BGMAudio");
        if (sfxAudio != null)
            sfxAudio.GetComponent<AudioSource>().volume = sfxSlider.value;
    }

    public void OnBGMChanged()
    {
        //Debug.Log("OnBGMChanged " + bgmSlider.value);
        if(bgmAudio == null)
            bgmAudio = GameObject.Find("BGMAudio");
        if(bgmAudio != null)
            bgmAudio.GetComponent<AudioSource>().volume = bgmSlider.value;
    }
}
