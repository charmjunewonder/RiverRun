using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCanvasHooks : MonoBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnReadyHook;
	public CanvasHook OnColorChangeHook;
	public CanvasHook OnRemoveHook;

	public CanvasHook OnChooseStriker;
	public CanvasHook OnChooseDefender;
	public CanvasHook OnChooseEngineer;
	public CanvasHook OnChooseSimoner;
	
	public Button playButton;
	public Button colorButton;
	public Button removeButton;
	public Text readyText;
	public Text nameText;
	public RectTransform panelPos;
	public Button[] buttons;
	public Text roleText;
	
	bool isLocalPlayer;

	void Awake()
	{
		removeButton.gameObject.SetActive(false);
	}
	
	public void ChooseRole(Roles r){
		foreach(Button b in buttons){
			b.gameObject.SetActive(false);
		}
		roleText.gameObject.SetActive(true);
		roleText.text = r.ToString();
	}

	public void ChooseStriker()
	{
		if (OnChooseStriker != null)
			OnChooseStriker.Invoke();
	}
	
	public void ChooseDefender()
	{
		if (OnChooseDefender != null)
			OnChooseDefender.Invoke();
	}
	
	public void ChooseEngineer()
	{
		if (OnChooseEngineer != null)
			OnChooseEngineer.Invoke();
	}
	
	public void ChooseSimoner()
	{
		if (OnChooseSimoner != null)
			OnChooseSimoner.Invoke();
	}
	
	public void UIReady()
	{
		if (OnReadyHook != null)
			OnReadyHook.Invoke();
	}

	public void UIColorChange()
	{
		if (OnColorChangeHook != null)
			OnColorChangeHook.Invoke();
	}

	public void UIRemove()
	{
		if (OnRemoveHook != null)
			OnRemoveHook.Invoke();
	}

	public void SetLocalPlayer()
	{
		isLocalPlayer = true;
		nameText.text = "YOU";
		readyText.text = "Play";
		//removeButton.gameObject.SetActive(true);
	}

	public void SetColor(Color color)
	{
		colorButton.GetComponent<Image>().color = color;
	}

	public void SetReady(bool ready)
	{
		if (ready)
		{
			readyText.text = "Ready";
		}
		else
		{
			if (isLocalPlayer)
			{
				readyText.text = "Play";
			}
			else
			{
				readyText.text = "Not Ready";
			}
		}
	}
}
