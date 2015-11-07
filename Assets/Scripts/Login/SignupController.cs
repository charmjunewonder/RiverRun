using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class SignupController : MonoBehaviour {
	
	public InputField uName;
    public InputField uName2;
    public Button signupButton;
    public Image signupImage;
    public Text displayMessage;

    public void OnSignupShowUp()
    {
        StartCoroutine(showSignupImage());
    }

    IEnumerator showSignupImage()
    {
        for (int i = 0; i < 50; i++)
        {
            signupImage.fillAmount += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
        uName.gameObject.SetActive(true);
        uName2.gameObject.SetActive(true);
        signupButton.gameObject.SetActive(true);
    }

    public void OnSignupPressed(){
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
        
        string serverUrl = ServerUtils.urlHeader + domain + "/register.php";
		string name = uName.text;
		string name2 = uName2.text;
		bool isValid = true;
		
		if(!ServerUtils.CheckUsername(name))
		{
			displayMessage.text = "User Name Invalid";
			isValid = false;
		} else if(!name.Equals(name2)){
			displayMessage.text = "Two User Names Are Different";
			isValid = false;
		}
		
		if(isValid){
			
			StartCoroutine (RegisterData (name, serverUrl));
		}
	}
	
	IEnumerator RegisterData (string name, string serverUrl)
	{
		WWWForm form = new WWWForm ();
		
		form.AddField ("pname", name);
		
		// Create a download object
		WWW download = new WWW (serverUrl, form);
        Debug.Log(serverUrl);
		// Wait until the download is done
		yield return download;
		
		
		
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
						displayMessage.text = "User name is empty";
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
