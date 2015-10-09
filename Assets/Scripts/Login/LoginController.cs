using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {
		
	public InputField uName;
	public InputField password;
	public Text displayMessage;
	
	public void OnLoginPressed(){
		string serverUrl = ServerUtils.urlHeader + ServerUtils.domainName + "/login.php";
		string name = uName.text;
		string pswd = password.text;
		bool isValid = true;
		
		if(!ServerUtils.CheckUsername(name))
		{
			displayMessage.text = "User Name Invalid";
			isValid = false;
		} else if(!ServerUtils.CheckPass(pswd)){
			displayMessage.text = "Password Invalid, start with letter and has length 4-17.";
			isValid = false;
		} 
		
		if(isValid){			
			StartCoroutine (LoginData (name, pswd, serverUrl));
		}
	}

    public void OnMagicPressed()
    {
        Application.LoadLevel("Lobby");

    }

    IEnumerator LoginData (string name, string pw, string serverUrl)
	{
		WWWForm form = new WWWForm ();
		
		form.AddField ("pname", name);
		
		form.AddField ("ppwd", pw);
		
		// Create a download object
		WWW download = new WWW (serverUrl, form);
		Debug.Log (serverUrl);
		// Wait until the download is done
		yield return download;
		
		if (download.error != null)
			Debug.Log("fail to request..." + download.error);
		else
		{
			if (download.isDone)
			{
				Debug.Log ("OK - - " + download.text);
				
				string ex = @"<login>[\S\s\t]*?</login>";
				Match m = Regex.Match(download.text, ex);
				if (m.Success)
				{
					string result = m.Value;
					result = result.Substring(result.IndexOf(">") + 1, result.LastIndexOf("<") - result.IndexOf(">") - 1).Trim();
					if (result == "success")
					{
						displayMessage.text = "Login Success";
						
						ex = @"<pid>.+</pid>";
						m = Regex.Match(download.text, ex);
						string pid = m.Value;
						pid = pid.Replace("<pid>", "");
						pid = pid.Replace("</pid>", "");
						PlayerPrefs.SetString("pid", pid);
						PlayerPrefs.SetString("name", name);
						Debug.Log("pid stored: "+ pid);
						Application.LoadLevel("Lobby");
						
					}
					else if (result == "no user")
					{
						displayMessage.text = "User Don't Exist";
					}
					else if (result == "empty")
					{
						displayMessage.text = "User name or password is empty";
					}
					else
					{
						displayMessage.text = "Unkown Error: " + download.text;
					}
				}
			}
		}
	}
}
