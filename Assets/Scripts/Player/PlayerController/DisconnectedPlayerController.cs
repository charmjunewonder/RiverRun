using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DisconnectedPlayerController : NetworkBehaviour {

    public int slot;
    public int connId;
    public PlayerRole currentRole;
    public string username;
    public float health;

    public int crystals;

    public int rank;

    public int exp;
    public int score;
    public int skill1Counter = 0;
    public int skill2Counter = 0;
    public int supportCounter = 0;
    public bool isPause;
}
