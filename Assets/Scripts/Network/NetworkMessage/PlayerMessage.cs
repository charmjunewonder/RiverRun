using UnityEngine;
using UnityEngine.Networking;

public class PlayerMessage : MessageBase
{

    public static readonly short MsgType = short.MaxValue;

    public string userName;

    public override string ToString()
    {
        return string.Format("Message: Name - '{0}';", userName);
    }
}
