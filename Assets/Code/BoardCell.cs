using UnityEngine;
using System.Collections;

public class BoardCell {

    public string biome = "null";
    public GameObject root;

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
        float aux = Random.Range(0f, 1f);

        if (aux < 1f / 3f)
        {
            changeBiome("desert");
        }
        else if (aux < 2f / 3f)
        {
            changeBiome("mountain");
        }
    }

    public void changeBiome(string auxBiome)
    {
        biome = auxBiome;
        root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + biome);
    }

}
