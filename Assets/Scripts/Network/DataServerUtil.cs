using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public class DataServerUtil : MonoBehaviour
{

    public static DataServerUtil Singleton;

    void Start()
    {
        Singleton = this;

        //UpdateRankExp(2, 100, 2);
        
        //ArrayList names = new ArrayList();
        //names.Add("eric");
        //SendTeamRecord(names, 1000);

        //GetLeaderBoard();

    }

    public void GetLeaderBoard()
    {
        Debug.Log("OnLoginPressed ");
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
        string serverUrl = ServerUtils.urlHeader + domain + "/getLeaderBoard.php";

        StartCoroutine(LeaderBoardData(serverUrl));
    }

    IEnumerator LeaderBoardData(string serverUrl)
    {
        WWWForm form = new WWWForm();

        DateTime thisDay = DateTime.Today.AddDays(-7);
        string date = thisDay.Year + "-" + thisDay.Month + "-" + thisDay.Day;
        form.AddField("date", date);

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

                string ex = @"<result>[\S\s\t]*?</result>";
                Match m = Regex.Match(download.text, ex);
                if (m.Success)
                {
                    string result = m.Value;
                    result = result.Substring(result.IndexOf(">") + 1, result.LastIndexOf("<") - result.IndexOf(">") - 1).Trim();
                    if (result == "success")
                    {
                        Debug.Log("get leader board data success");

                        for (int i = 1; i < 6; i++)
                        {
                            ex = @"<" + i + @">.+</" + i + @">";
                            m = Regex.Match(download.text, ex);
                            string record = m.Value;
                            if (record.Length == 0) continue;
                            ex = @"<n1>.+</n1>";
                            m = Regex.Match(record, ex);
                            string name1 = m.Value;
                            name1 = name1.Replace("<n1>", "");
                            name1 = name1.Replace("</n1>", "");

                            ex = @"<n2>.+</n2>";
                            m = Regex.Match(record, ex);
                            string name2 = m.Value;
                            name2 = name2.Replace("<n2>", "");
                            name2 = name2.Replace("</n2>", "");

                            ex = @"<n3>.+</n3>";
                            m = Regex.Match(record, ex);
                            string name3 = m.Value;
                            name3 = name3.Replace("<n3>", "");
                            name3 = name3.Replace("</n3>", "");

                            ex = @"<n4>.+</n4>";
                            m = Regex.Match(record, ex);
                            string name4 = m.Value;
                            name4 = name4.Replace("<n4>", "");
                            name4 = name4.Replace("</n4>", "");

                            ex = @"<s>.+</s>";
                            m = Regex.Match(record, ex);
                            string ss = m.Value;
                            ss = ss.Replace("<s>", "");
                            ss = ss.Replace("</s>", "");
                            int score = int.Parse(ss);
                            LeaderBoardPanel.Singleton.teamRecords[i - 1].SetRecord(name1, name2, name3, name4, score);
                            Debug.Log(name1 + " " + name2 + " " + name3 + " " + name4 + " " + score);
                        }
                        LeaderBoardPanel.Singleton.ShowTeamPanel();
                    }
                    else if (result == "fail")
                    {
                        Debug.Log("get leader board data fail");
                    }
                }
            }
        }
    }

    public void SendTeamRecord(ArrayList names, int score)
    {
        string domain = ServerUtils.domainName + ":80";
        string storedDomain = PlayerPrefs.GetString("DataIp");
        if (ServerUtils.CheckIpAddress(storedDomain))
        {
            domain = storedDomain + ":80";
        }
        string serverUrl = ServerUtils.urlHeader + domain + "/sendTeamScore.php";

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
        DateTime thisDay = DateTime.Today;
        string date = thisDay.Year + "-" + thisDay.Month + "-" + thisDay.Day;
        form.AddField("date", date);

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
                        Debug.Log("send team data success");
                    }
                    else if (result == "fail")
                    {
                        Debug.Log("send team data fail");
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
