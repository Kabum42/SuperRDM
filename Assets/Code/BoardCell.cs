using UnityEngine;
using System.Collections;

public class BoardCell : MonoBehaviour  {

    public string biome = "null";
    public GameObject root;

    public BoardCell()
    {
        
        root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;

        float aux = Random.Range(0f, 1f);

        if (aux < 1f/3f)
        {
            changeBiome("desert");
        }
        else if (aux < 2f/3f)
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
