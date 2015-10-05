using UnityEngine;
using System.Collections;

public class Cell {
	
	private int ID;
	private bool Exhausted;
	private int LinkCost;
	private Biome OwnBiome;
	private Cell[] BorderCells = new Cell[6];
	private Cell Teleport;
	private int MissionID;
	
	public Cell()
	{
		Debug.Log("Cell created");
	}
	
	public void Interact(){
	}

	
	
}

