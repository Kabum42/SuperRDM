using UnityEngine;
using System.Collections;

public class BoardCell : MonoBehaviour  {

    public string biome = "null";
    public GameObject root;

    public BoardCell(string auxBiome)
    {
        biome = auxBiome;
        root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
        root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" +biome);
    }

}
