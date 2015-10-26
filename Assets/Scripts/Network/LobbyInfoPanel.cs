using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Network 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public Text infoText;
        public Button singleButton;
        public Button secondButton;

        public void DisplayWarning(string info, UnityEngine.Events.UnityAction buttonClbk)
        {
            Display(info, buttonClbk);
        }

        public void DisplayDisconnectError(string info, UnityEngine.Events.UnityAction firstButtonClbk,
            UnityEngine.Events.UnityAction secondButtonClbk)
        {
            secondButton.onClick.RemoveAllListeners();

            if (secondButtonClbk != null)
            {
                secondButton.gameObject.SetActive(true);
                secondButton.onClick.AddListener(secondButtonClbk);
            }
            secondButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            Display(info, firstButtonClbk);
        }

        private void Display(string info, UnityEngine.Events.UnityAction buttonClbk)
        {
            infoText.text = info;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            gameObject.SetActive(true);
        }
    }
}