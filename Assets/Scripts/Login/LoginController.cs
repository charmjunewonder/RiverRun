using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {
		
	public InputField uName;
    public Button loginButton;
    public Image loginImage;
	public Text displayMessage;
    public LobbyConnectPanel lcp;
    public static string userName = "Player";
    public void OnLoginShowUp()
    {
        if (loginImage.fillAmount == 0.9f) return;
        StartCoroutine(showLoginImage());
    }

    IEnumerator showLoginImage()
    {
        for(int i = 0; i < 50; i++)
        {
            loginImage.fillAmount += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
        uName.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);
    }

    public void OnLoginPressed(){
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
		string serverUrl = ServerUtils.urlHeader + domain + "/login.php";
        
		string name = uName.text;
		bool isValid = true;
		
		if(!ServerUtils.CheckUsername(name))
		{
			displayMessage.text = "User Name Invalid";
			isValid = false;
		}
		
		if(isValid){			
			StartCoroutine (LoginData (name, serverUrl));
		}
	}

    public void OnMagicPressed()
    {
        userName = uName.text;
        lcp.OnClickJoin();
    }

    IEnumerator LoginData (string name, string serverUrl)
	{
		WWWForm form = new WWWForm ();
		
		form.AddField ("pname", name);
				
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
                        userName = name;
						Debug.Log("pid stored: "+ pid);

                        ex = @"<sl>.+</sl>";
                        m = Regex.Match(download.text, ex);
                        string sl = m.Value;
                        sl = sl.Replace("<sl>", "");
                        sl = sl.Replace("</sl>", "");
                        PlayerPrefs.SetString("sl", sl);

                        ex = @"<el>.+</el>";
                        m = Regex.Match(download.text, ex);
                        string el = m.Value;
                        el = el.Replace("<el>", "");
                        el = el.Replace("</el>", "");
                        PlayerPrefs.SetString("el", el);

                        ex = @"<dl>.+</dl>";
                        m = Regex.Match(download.text, ex);
                        string dl = m.Value;
                        dl = dl.Replace("<dl>", "");
                        dl = dl.Replace("</dl>", "");
                        PlayerPrefs.SetString("dl", dl);

                        Debug.Log("level stored: " + sl + " " + el + " " + dl);

                        lcp.OnClickJoin();
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
