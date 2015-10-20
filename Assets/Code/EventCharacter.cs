using UnityEngine;
using System.Collections;

public class EventCharacter : Character {

	// private Type OwnType;
	private Biome OwnBiome; 	
	public EventCharacter(int ID, string Name, float MaxHealth, float MaxIP, Class OwnClass, Biome OwnBiome)
	{
		this.ID = ID; 
		this.Name = Name;
		this.MaxHealth = MaxHealth;
		this.MaxIPBar = MaxIP;
		this.OwnClass = OwnClass;
		this.OwnBiome = OwnBiome;

		switch (OwnClass.getName()) {
			case "Wolf":
				this.Aerial = false;
				break;
		}

		Debug.Log(this.Name + " created");
	}

	public Biome getBiome(){
		return this.OwnBiome;
	}
	
}
