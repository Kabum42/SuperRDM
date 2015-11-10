using UnityEngine;
using System.Collections;

public class VisualInterface : MonoBehaviour {

    public GameObject root;
    public GameObject skills;
    public GameObject pointer;
    public GameObject skillSelected;

    private GameObject[] skillHolders = new GameObject[3];
    private int currentSkillHolder = 0;

	// Use this for initialization
	void Awake () {

        root = this.gameObject;
        skills = root.transform.FindChild("Skills").gameObject;
        pointer = root.transform.FindChild("Skills/Pointer").gameObject;
        skillSelected = root.transform.FindChild("Skills/SkillSelected").gameObject;

        for (int i = 0; i < skillHolders.Length; i++)
        {
            skillHolders[i] = root.transform.FindChild("Skills/Skill"+(i+1)).gameObject;
        }
	
	}

    // Update is called once per frame
    void Update() {

        pointer.transform.localPosition = new Vector3(Mathf.Lerp(pointer.transform.localPosition.x, skillHolders[currentSkillHolder].transform.localPosition.x, Time.deltaTime*10f), pointer.transform.localPosition.y, pointer.transform.localPosition.z);
        skillSelected.transform.localPosition = new Vector3(skillHolders[currentSkillHolder].transform.localPosition.x, skillSelected.transform.localPosition.y, skillSelected.transform.localPosition.z);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSkillHolder++;
            if (currentSkillHolder >= skillHolders.Length) { currentSkillHolder = 0; }
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSkillHolder--;
            if (currentSkillHolder < 0) { currentSkillHolder = skillHolders.Length-1; }
        }
	
	}
}
