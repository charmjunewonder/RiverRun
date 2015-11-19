using UnityEngine;
using System.Collections;

public class UltiController : MonoBehaviour {

    public static bool isUltiEnchanting;
    public static int ultiPlayerNumber;

	// Use this for initialization
	void Start () {
        isUltiEnchanting = false;
        ultiPlayerNumber = -1;
	}

    public static bool checkUltiEnchanting() { return isUltiEnchanting; }
    public static void setUltiEnchanting(bool status) { isUltiEnchanting = status; }
    public static int getUltiPlayerNumber() { return ultiPlayerNumber; }
    public static void setUltiPlayerNumber(int num) { ultiPlayerNumber = num; }

}
