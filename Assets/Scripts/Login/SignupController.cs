using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class SignupController : MonoBehaviour {
	public GameObject signupCanvas;
	public GameObject loginController;
	
	public InputField uName;
	public InputField password;
	public InputField password2;
	public Text displayMessage;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnEnable() {
		signupCanvas.SetActive(true);
		loginController.SetActive(false);
	}
	
	public void OnLoginPressed(){
		signupCanvas.SetActive(false);
		loginController.SetActive(true);
	}
	
	public void OnSignupPressed(){
		string serverUrl = ServerUtils.urlHeader + ServerUtils.domainName + "/register.php";
		string name = uName.text;
		string pswd = password.text;
		string pswd2 = password2.text;
		bool isValid = true;
		
		if(!ServerUtils.CheckUsername(name))
		{
			displayMessage.text = "User Name Invalid";
			isValid = false;
		} else if(!ServerUtils.CheckPass(pswd)){
			displayMessage.text = "Password Invalid, start with letter and has length 4-17.";
			isValid = false;
		} else if(!pswd.Equals(pswd2)){
			displayMessage.text = "Two Password is Different";
			isValid = false;
		}
		
		if(isValid){
			
			StartCoroutine (RegisterData (name, pswd, serverUrl));
		}
	}
	
	IEnumerator RegisterData (string name, string pwd, string serverUrl)
	{
		WWWForm form = new WWWForm ();
		
		form.AddField ("pname", name);
		form.AddField ("ppwd", pwd);
		
		// Create a download object
		WWW download = new WWW (serverUrl, form);
		
		// Wait until the download is done
		yield return download;
		
		Debug.Log (serverUrl);
		
		if (download.error != null)
			Debug.Log("fail to request..." + download.error);
		else
		{
			if (download.isDone)
			{
				Debug.Log ("OK - - " + download.text);
				string ex = @"<register>[\S\s\t]*?</register>";
				Match m = Regex.Match(download.text, ex);
				if (m.Success)
				{
					string result = m.Value;
					result = result.Substring(result.IndexOf(">") + 1, result.LastIndexOf("<") - result.IndexOf(">") - 1).Trim();
					
					if (result == "success")
					{
						displayMessage.text = "Register Success";
					}
					else if (result == "fail")
					{
						displayMessage.text = "Fail to Register";
					}
					else if (result == "dbError")
					{
						displayMessage.text = "Fail to connect to the database";
					}
					else if (result == "empty")
					{
						displayMessage.text = "User name or password is empty";
					}
					else
					{
						displayMessage.text = "Unkown Error";
					}
				}
			}
		}		
	}
}
