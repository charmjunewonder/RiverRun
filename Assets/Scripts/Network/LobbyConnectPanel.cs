using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LobbyConnectPanel : MonoBehaviour {

    public bool isInGame = false;

    protected bool isDisplayed = true;
    protected Image panelImage;

    public NetworkManagerCustom networkManager;

    public RectTransform lobbyPanel;

    public InputField ipInput;

    void Start()
    {
        //We keep it in game to be able to disconnect/have info on server
        DontDestroyOnLoad(gameObject);

        panelImage = GetComponent<Image>();

        string gameIp = PlayerPrefs.GetString("GameIp");
        if (ServerUtils.CheckIpAddress(gameIp))
        {
            ipInput.text = gameIp;
        }
    }


    void Update()
    {
        if (!isInGame)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleVisibility(!isDisplayed);
        }

    }

    public void ToggleVisibility(bool visible)
    {
        isDisplayed = visible;
        //foreach (Transform t in transform)
        //{
        //    t.gameObject.SetActive(isDisplayed);
        //}

        if (panelImage != null)
        {
            panelImage.enabled = isDisplayed;
        }
    }

    public void OnEnable()
    {
        networkManager.topPanel.ToggleVisibility(true);

        ipInput.onEndEdit.RemoveAllListeners();
        ipInput.onEndEdit.AddListener(onEndEditIP);

    }

    public void OnClickHost()
    {
        networkManager.StartHost();
    }

    public void OnClickJoin()
    {

        networkManager.networkAddress = ipInput.text;
        networkManager.StartClient();

        networkManager.backDelegate = networkManager.StopClientClbk;
        networkManager.DisplayIsConnecting();

        //networkManager.SetServerInfo("Connecting...", networkManager.networkAddress);
    }

    public void OnClickDedicated()
    {
        networkManager.ChangeTo(null);
        networkManager.StartServer();

        networkManager.backDelegate = networkManager.StopServerClbk;

        //networkManager.SetServerInfo("Dedicated Server", networkManager.networkAddress);
    }

    public void OnClickSetting()
    {
        NetworkManagerCustom.SingletonNM.ChangeToSettingPanel();
    }

    void onEndEditIP(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClickJoin();
        }
    }
}
