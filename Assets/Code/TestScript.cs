﻿using UnityEngine;
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
        player1.SetBattlePosition(true, 0);
        player2.SetBattlePosition(false, 0);

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //player1.Perform(GlobalData.Skills[2], player2);
            player2.Perform(GlobalData.Skills[2], player1);
            //henmancer.Represent(GlobalData.Skills[2]);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player1.Perform(GlobalData.Skills[2], player2);
        }
	
	}
}
