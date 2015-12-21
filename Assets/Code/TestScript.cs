using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    
    private VisualCharacter player1;
    private VisualCharacter player2;

    private SkillBar s;

	// Use this for initialization
	void Start () {
        
        s = (Instantiate(Resources.Load("Prefabs/SkillBarObject")) as GameObject).GetComponent<SkillBar>();

        s.setRandom(0.8f, 0.5f, 0.125f);

        //s.setBad(-0.95f, 0.65f);
        //s.setGood(-0.85f, 0.15f);
        //s.setCritical(-0.75f, -0.5f);

        GlobalData.Start();

        player1 = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
        player2 = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
		player1.setClass (GlobalData.Classes [2]);
		player2.setClass (GlobalData.Classes [0]);
		player1.SetBattlePosition(true, 0);
		player2.SetBattlePosition(false, 0);
		/*
		int resolution = 512;
		Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);

		float offsetX = Random.Range (1000f, 2000f);
		float offsetY = Random.Range (1000f, 2000f);
		float offsetZ = 0f;

		float division = (float)resolution;

		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				Vector3 point = new Vector3(offsetX + (float)x/division, offsetY + (float)y/division, offsetZ);
				float value = Noise.Sum(Noise.valueMethods[1], point, 2f, 10, 3f, 0.75f); 
				//float value = Noise.Perlin2D(n, 1f);
				texture.SetPixel(x, y, new Color(value, value, value));
			}
		}

		texture.Apply();

		GameObject.Find ("Chest").GetComponent<SpriteRenderer> ().material.SetTexture ("_AlphaTex", texture);
		*/
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.R))
        {
            s.setRandom(0.8f, 0.5f, 0.125f);
            s.Reset();
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(player1.ownClass.getSkill(0), player2, aux);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(player1.ownClass.getSkill(1), player2, aux);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(player1.ownClass.getSkill(2), player2, aux);
        }

	
	}
}
