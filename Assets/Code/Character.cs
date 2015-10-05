using UnityEngine;
using System.Collections;

public class Character {

	private int ID;
	private string Name;
	private float CurrentHealth;
	private float MaxHealth;
	private float ProgressIPBar;
	private float MaxIPBar;
	private Stack CurrentEffects;
	private bool Aerial;
	private Class OwnClass;

	public Character()
	{
		Debug.Log("Character created");
	}

	public void EndBattleTurn(){
	}

	public void StartBattleTurn(){
	}

	public void EndBattle(){
	}

	
}
