using UnityEngine;
using System.Collections;

public static class GlobalData {

    public static bool started = false;
    public static Agent[] agents = new Agent[6];

	// Use this for initialization
	public static void Start () {
        started = true;
        agents[0] = new Agent();
	}
	

}
