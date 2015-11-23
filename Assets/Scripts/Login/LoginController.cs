using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LoginController : MonoBehaviour {
    public bool isServerReady = true;

	public InputField uName;
    public Button loginButton;
    public Image loginImage;
	public Text displayMessage;
    public LobbyConnectPanel lcp;
    public static string userName = "Player";
    public static int StrikerLevel = 0;
    public static int StrikerExp = 0;
    public static int EngineerLevel = 0;
    public static int EngineerExp = 0;
    public static int DefenderLevel = 0;
    public static int DefenderExp = 0;
    private bool isProcessing = false;

    void Start()
    {
        string un = PlayerPrefs.GetString("name");
        if (ServerUtils.CheckUsername(name))
        {
            uName.text = un;
            userName = un;
        }
    }
    public void OnLoginShowUp()
    {
        if (isProcessing) return;
        StartCoroutine(showLoginImage());
    }

    IEnumerator showLoginImage()
    {
        if (!isProcessing && loginImage.fillAmount < 1)
        {
            isProcessing = true;
            for (int i = 0; i < 50; i++)
            {
                loginImage.fillAmount += 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            uName.gameObject.SetActive(true);
            loginButton.gameObject.SetActive(true);
            loginImage.fillAmount = 1;
            isProcessing = false;
        }
        else if (!isProcessing && loginImage.fillAmount >= 1)
        {
            isProcessing = true;
            uName.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(false);
            for (int i = 0; i < 50; i++)
            {
                loginImage.fillAmount -= 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            isProcessing = false;
        }
    }

    public void OnLoginPressed(){
        Debug.Log("OnLoginPressed ");
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
            if (isServerReady)
			    StartCoroutine (LoginData (name, serverUrl));
            else
            {
                userName = name;
                PlayerPrefs.SetString("name", userName);
                lcp.OnClickJoin();
            }
                
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

        form.AddField("pname", name);
				
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

                        ex = @"<name>.+</name>";
						m = Regex.Match(download.text, ex);
                        string uname = m.Value;
                        uname = uname.Replace("<name>", "");
                        uname = uname.Replace("</name>", "");
                        userName = name;

                        ex = @"<sl>.+</sl>";
                        m = Regex.Match(download.text, ex);
                        string sl = m.Value;
                        sl = sl.Replace("<sl>", "");
                        sl = sl.Replace("</sl>", "");
                        StrikerLevel = int.Parse(sl);

                        ex = @"<sexp>.+</sexp>";
                        m = Regex.Match(download.text, ex);
                        string sexp = m.Value;
                        sexp = sexp.Replace("<sexp>", "");
                        sexp = sexp.Replace("</sexp>", "");
                        StrikerExp = int.Parse(sexp);

                        ex = @"<el>.+</el>";
                        m = Regex.Match(download.text, ex);
                        string el = m.Value;
                        el = el.Replace("<el>", "");
                        el = el.Replace("</el>", "");
                        EngineerLevel = int.Parse(el);

                        ex = @"<eexp>.+</eexp>";
                        m = Regex.Match(download.text, ex);
                        string eexp = m.Value;
                        eexp = eexp.Replace("<eexp>", "");
                        eexp = eexp.Replace("</eexp>", "");
                        EngineerExp = int.Parse(eexp);

                        ex = @"<dl>.+</dl>";
                        m = Regex.Match(download.text, ex);
                        string dl = m.Value;
                        dl = dl.Replace("<dl>", "");
                        dl = dl.Replace("</dl>", "");
                        DefenderLevel = int.Parse(dl);

                        ex = @"<dexp>.+</dexp>";
                        m = Regex.Match(download.text, ex);
                        string dexp = m.Value;
                        dexp = dexp.Replace("<dexp>", "");
                        dexp = dexp.Replace("</dexp>", "");
                        DefenderExp = int.Parse(dexp);

                        Debug.Log("level stored: " + StrikerLevel + " " + StrikerExp + " " + EngineerLevel + " " + EngineerExp + " " + DefenderLevel + " " + DefenderExp);

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
