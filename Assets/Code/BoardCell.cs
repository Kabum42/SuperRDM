using UnityEngine;
using System.Collections;

public class BoardCell {

    public Biome biome;
    public GameObject root;

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

		Debug.Log (Biome.Forest);

    }

    public void changeBiome(Biome auxBiome)
    {

        biome = auxBiome;
        root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + GlobalData.biomeNames[(int)biome]);
    
    }

}
