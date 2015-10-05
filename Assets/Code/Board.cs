using UnityEngine;
using System.Collections;

public class Board {
	
	private Cell[] Cells = new Cell[37];
	private int Seeding;
	private MainCharacter[] Players = new MainCharacter[6];
	private Stack Missions;
	private int CurrentPlayer;
	
	public Board()
	{
		Debug.Log("Board created");
	}
	
	public void WorldGenerator(){
	}

	public void ChangeView(){
	}

	public void RequestMovement(){
	}


	
	
}



