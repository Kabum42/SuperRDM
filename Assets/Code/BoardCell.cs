using UnityEngine;
using System.Collections;

public class BoardCell {

    public string biome = "null";
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
            changeBiome("boss");
        }
        else if (ring == 1)
        {
            float aux = Random.Range(0f, 50f);
            aux += Random.Range(0f, 50f);

            if (aux >= 25f && aux <= 75f)
            {
                changeBiome("mountain");
            }
            else
            {
                changeBiome("desert");
            }
        }
        else if (ring == 2)
        {
            float aux = Random.Range(0f, 50f);
            aux += Random.Range(0f, 50f);

            if (aux >= 45f && aux <= 55f)
            {
                changeBiome("grass");
            }
            else
            {
                // NO ES GRASS
                aux = Random.Range(0f, 50f);
                aux += Random.Range(0f, 50f);
                if (aux >= 40f && aux <= 60f)
                {
                    changeBiome("forest");
                }
                else
                {
                    // NO ES GRASS NI FOREST
                    aux = Random.Range(0f, 50f);
                    aux += Random.Range(0f, 50f);
                    if (aux >= 35f && aux <= 65f)
                    {
                        changeBiome("swamp");
                    }
                    else
                    {
                        // NO ES GRASS NI FOREST NI SWAMP
                        aux = Random.Range(0f, 50f);
                        aux += Random.Range(0f, 50f);
                        if (aux >= 30f && aux <= 70f)
                        {
                            changeBiome("water");
                        }
                        else
                        {
                            // NO ES GRASS NI FOREST NI SWAMP NI WATER
                            changeBiome("desert");
                        }
                    }
                }
            }
        }
        else if (ring == 3)
        {
            float aux = Random.Range(0f, 50f);
            aux += Random.Range(0f, 50f);

            if (aux >= 35f && aux <= 65f)
            {
                changeBiome("grass");
            }
            else
            {
                // NO ES GRASS
                aux = Random.Range(0f, 50f);
                aux += Random.Range(0f, 50f);
                if (aux >= 20f && aux <= 80f)
                {
                    changeBiome("forest");
                }
                else
                {
                    // NO ES GRASS NI FOREST
                    changeBiome("swamp");
                }
            }
        }

        /*
        float aux = Random.Range(0f, 1f);

        if (aux < 1f / 6f)
        {
            changeBiome("grass");
        }
        else if (aux < 2f / 6f)
        {
            changeBiome("forest");
        }
        else if (aux < 3f / 6f)
        {
            changeBiome("swamp");
        }
        else if (aux < 4f / 6f)
        {
            changeBiome("desert");
        }
        else if (aux < 5f / 6f)
        {
            changeBiome("mountain");
        }
        else if (aux <= 6f / 6f)
        {
            changeBiome("water");
        }
        */
    }

    public void changeBiome(string auxBiome)
    {
        biome = auxBiome;
        root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + biome);
    }

}
