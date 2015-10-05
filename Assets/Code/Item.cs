using UnityEngine;
using System.Collections;

public class Item {
	
	private int ID;
	private string Name;
	private Skill EffectType;
	private Sprite Image;
	private int Uses;
	
	public Item()
	{
		Debug.Log("Item created");
	}
}
