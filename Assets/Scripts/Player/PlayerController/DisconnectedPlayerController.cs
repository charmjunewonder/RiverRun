using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DisconnectedPlayerController : NetworkBehaviour {

    public int slot;
    public int connId;
    public PlayerRole currentRole;
    public string username;
    public float health;
}
