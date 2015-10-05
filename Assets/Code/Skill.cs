using UnityEngine;
using System.Collections;

public class Skill {

	private int ID;
	private string Name;
	private float CostIP;
	private float CostHeatlh;
	private int TargetID;
	private int[] AgentID = new int[100];
	private float Damage;

	public Skill()
	{
		Debug.Log("Skill created");
	}

	public void Activate()
	{
	}
}

