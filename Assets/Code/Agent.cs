using UnityEngine;
using System.Collections;

public class Agent {

    // Use this for initialization
    public string champion = "Barbarian";
    public int level = 1;
    public double experience = 0f;
    public double gold = 0f;
	public double currentHealth = 0f;
	
    public Agent()
    {
        Debug.Log("Agent created");
    }

}
