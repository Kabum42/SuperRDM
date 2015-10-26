using UnityEngine;
using System.Collections;

public class Effect {
	
	private int ID;
	private string Name;
	private int StackedNumber;
	private int Duration;
	private int DamageValue;
	
	public Effect(int ID, string Name, int StackedNumber, int Duration, int DamageValue)
	{
		this.ID = ID;
		this.Name = Name;
		this.StackedNumber = StackedNumber;
		this.Duration = Duration;
		this.DamageValue = DamageValue;
	}
	
	public void Update()
	{
	}

	// Getters and Setters

	public string getName(){
		return this.Name;
	}

	public void setStackedNumber(int StackedNumber){
		this.StackedNumber = StackedNumber;
	}

	public int getStackedNumber(){
		return StackedNumber;
	}
}



