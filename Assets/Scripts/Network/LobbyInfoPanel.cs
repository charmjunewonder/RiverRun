using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public Text infoText;
        public Button reconnectButton;
        public Button closeButton;

        public void DisplayWarning(string info, UnityEngine.Events.UnityAction buttonClbk)
        {
            reconnectButton.gameObject.SetActive(false);
            Display(info, buttonClbk);
        }

        public void DisplayDisconnectError(string info, UnityEngine.Events.UnityAction firstButtonClbk,
            UnityEngine.Events.UnityAction secondButtonClbk)
        {
            reconnectButton.onClick.RemoveAllListeners();

            if (secondButtonClbk != null)
            {
                reconnectButton.gameObject.SetActive(true);
                reconnectButton.onClick.AddListener(secondButtonClbk);
            }
            reconnectButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            Display(info, firstButtonClbk);
        }

        private void Display(string info, UnityEngine.Events.UnityAction buttonClbk)
        {
            infoText.text = info;

            closeButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                closeButton.onClick.AddListener(buttonClbk);
            }

            closeButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            gameObject.SetActive(true);
        }
    }
}