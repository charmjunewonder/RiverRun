using UnityEngine;
using UnityEngine.Networking;

public class ErrorMessage : MessageBase
{ 
    public static readonly short MsgType = short.MaxValue-1;

    public string errorMessage;

    public override string ToString()
    {
        return string.Format("Error: '{0}';", errorMessage);
    }
}
