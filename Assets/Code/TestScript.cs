using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //SkillBar.root = Instantiate(Resources.Load("Prefabs/SkillBarObject")) as GameObject;
        SkillBar s = (Instantiate(Resources.Load("Prefabs/SkillBarObject")) as GameObject).GetComponent<SkillBar>();
        s.setBad(-0.95f, 0.65f);
        s.setGood(-0.85f, 0.15f);
        s.setCritical(-0.75f, -0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
