using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ServerMessage : MessageBase{
    public static readonly short MsgType = short.MaxValue;

    public NetworkMode currentMode;

    public override string ToString()
    {
        return string.Format("Message: Server currentMode - '{0}';", currentMode);
    }
}
