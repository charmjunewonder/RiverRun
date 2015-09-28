using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class ServerUtils : MonoBehaviour{
	public static string urlHeader = "http://";
	public static string domainName = "128.2.236.211:80";
	
	public static bool IsNum(string str)
	{
		return Regex.IsMatch(str, @"^[-]?\d+[.]?\d*$");
	}
	
	public static bool IsNumAndEnCh(string input)   { 
		string pattern = @"^[A-Za-z0-9]+$";
		Regex regex = new Regex(pattern);
		return regex.IsMatch(input);   
	}
	
	public static bool CheckPass(string pwd)
	{
		if (string.IsNullOrEmpty(pwd))
		{
			return false;
		}
		else
		{
			Regex reg = new Regex("^[a-zA-Z][0-9a-zA-Z]{3,17}");
			
			return reg.IsMatch(pwd);
		}
	}
	
	public static bool CheckEmail(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return false;
		}
		else
		{
			Regex reg = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
			                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
			
			return reg.IsMatch(email);
		}
	}
	
	public static bool CheckUsername(string username)
	{
		if (string.IsNullOrEmpty(username))
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
