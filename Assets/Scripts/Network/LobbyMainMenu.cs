using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public NetworkManagerCustom networkManager;

        public RectTransform lobbyPanel;

        public InputField ipInput;

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
            networkManager.ChangeTo(lobbyPanel);

            networkManager.networkAddress = ipInput.text;
            networkManager.StartClient();

            networkManager.backDelegate = networkManager.StopClientClbk;
            networkManager.DisplayIsConnecting();

            networkManager.SetServerInfo("Connecting...", networkManager.networkAddress);
        }

        public void OnClickDedicated()
        {
            networkManager.ChangeTo(null);
            networkManager.StartServer();

            networkManager.backDelegate = networkManager.StopServerClbk;

            networkManager.SetServerInfo("Dedicated Server", networkManager.networkAddress);
        }


        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

    }
}
