using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    
    private VisualCharacter player1;
    private VisualCharacter player2;

	// Use this for initialization
	void Start () {
        /*
        SkillBar s = (Instantiate(Resources.Load("Prefabs/SkillBarObject")) as GameObject).GetComponent<SkillBar>();
        s.setBad(-0.95f, 0.65f);
        s.setGood(-0.85f, 0.15f);
        s.setCritical(-0.75f, -0.5f);
         * */

        GlobalData.Start();


        player1 = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
        player2 = (Instantiate(Resources.Load("Prefabs/VisualCharacterObject")) as GameObject).GetComponent<VisualCharacter>();
		player1.setClass (GlobalData.Classes [0]);
		player2.setClass (GlobalData.Classes [0]);
		player1.SetBattlePosition(true, 0);
		player2.SetBattlePosition(false, 0);


	}
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.Q)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(GlobalData.Skills[0], player2, aux);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(GlobalData.Skills[1], player2, aux);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            float[] aux = {Mathf.Floor(Random.Range(0f, 100f))};
            player1.Perform(GlobalData.Skills[2], player2, aux);
        }

	
	}
}
