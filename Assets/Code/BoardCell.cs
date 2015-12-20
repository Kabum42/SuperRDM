﻿using UnityEngine;
using System.Collections;

public class BoardCell : System.IEquatable<BoardCell> {

    public Biome biome;
    public GameObject root;
    public GameObject text;
    public GameObject chains;
    public int chainsCountDown = 0;
    public Color chainsColor;
    public float chainsBrightness = 0f;
    public int positionInArray = 0;

    public int ring = 0;

    public BoardCell northWest;
    public BoardCell north;
    public BoardCell northEast;

    public BoardCell southEast;
    public BoardCell south;
    public BoardCell southWest;

    public BoardCell()
    {

    }

    public void randomBiome()
    {
        if (ring == 0)
        {
            // OBVIOUSLY, THE BOSS
            changeBiome(Biome.TheEvil);
        }
        else if (ring == 1)
        {
            // DESERT BY DEFAULT
            changeBiome(Biome.Desert);
        }
        else if (ring == 2)
        {
            // FOREST BY DEFAULT
            changeBiome(Biome.Forest);
        }
        else if (ring == 3)
        {
            // FOREST BY DEFAULT
            changeBiome(Biome.Forest);
        }

    }

    public void changeBiome(Biome auxBiome)
    {

        biome = auxBiome;
        root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + GlobalData.biomeNames[(int)biome]);
    
    }

    public bool isConnected(BoardCell b)
    {
        if (northWest == b) { return true; }
        if (north == b) { return true; }
        if (northEast == b) { return true; }

        if (southWest == b) { return true; }
        if (south == b) { return true; }
        if (southEast == b) { return true; }

        return false;
    }

    public bool Equals(BoardCell other)
    {
        return positionInArray == other.positionInArray;
    }

}
