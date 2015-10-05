using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
    public static Agent[] agents = new Agent[6];
	// public static Events[] Event;
	public static string[] NameItems;
	public static int MaxEnemies;

	// Use this for initialization
	public static void Start () {
        started = true;
        agents[0] = new Agent();
	}
	

}
