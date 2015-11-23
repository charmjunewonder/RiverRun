using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class DataServerUtil : MonoBehaviour
{

    public static DataServerUtil Singleton;

    void Start()
    {
        Singleton = this;
        //UpdateRankExp(2, 100, 2);
    }
    public void SendTeamRecord(ArrayList names, int score)
    {
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
        string serverUrl = ServerUtils.urlHeader + domain + "/updateRank.php";

        names.Sort();
        for (int i = names.Count; i < 4; i++)
        {
            names.Add("");
        }
        StartCoroutine(SendTeamRecordData((string)names[0], (string)names[1], (string)names[2], (string)names[3], score, serverUrl));
    }

    IEnumerator SendTeamRecordData(string name1, string name2, string name3, string name4, int score, string serverUrl)
    {
        WWWForm form = new WWWForm();
        form.AddField("name1", name1);
        form.AddField("name2", name2);
        form.AddField("name3", name3);
        form.AddField("name4", name4);
        form.AddField("score", score);

        // Create a download object
        WWW download = new WWW(serverUrl, form);
        Debug.Log(serverUrl);
        // Wait until the download is done
        yield return download;

        if (download.error != null)
            Debug.Log("fail to request..." + download.error);
        else
        {
            if (download.isDone)
            {
                Debug.Log("OK - - " + download.text);

                string ex = @"<rank>[\S\s\t]*?</rank>";
                Match m = Regex.Match(download.text, ex);
                if (m.Success)
                {
                    string result = m.Value;
                    result = result.Substring(result.IndexOf(">") + 1, result.LastIndexOf("<") - result.IndexOf(">") - 1).Trim();
                    if (result == "success")
                    {
                        Debug.Log("update data success");
                    }
                    else if (result == "fail")
                    {
                        Debug.Log("update data fail");
                    }
                }
            }
        }
    }

    public void UpdateRankExp(int rank, int exp, int role)
    {
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
        string serverUrl = ServerUtils.urlHeader + domain + "/updateRank.php";

        StartCoroutine(UpdateRankExpData(rank, exp, role, serverUrl));
    }

    IEnumerator UpdateRankExpData(int rank, int exp, int role, string serverUrl)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", LoginController.userName);
        //form.AddField("name", "a");
        form.AddField("rank", rank);
        form.AddField("exp", exp);
        form.AddField("role", role);

        // Create a download object
        WWW download = new WWW(serverUrl, form);
        Debug.Log(serverUrl);
        // Wait until the download is done
        yield return download;

        if (download.error != null)
            Debug.Log("fail to request..." + download.error);
        else
        {
            if (download.isDone)
            {
                Debug.Log("OK - - " + download.text);

                string ex = @"<rank>[\S\s\t]*?</rank>";
                Match m = Regex.Match(download.text, ex);
                if (m.Success)
                {
                    string result = m.Value;
                    result = result.Substring(result.IndexOf(">") + 1, result.LastIndexOf("<") - result.IndexOf(">") - 1).Trim();
                    if (result == "success")
                    {
                        Debug.Log("update data success");
                    }
                    else if (result == "fail")
                    {
                        Debug.Log("update data fail");
                    }
                }
            }
        }
    }
}
