using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualInterface : MonoBehaviour {

    public GameObject root;
    public GameObject skills;
    public GameObject pointer;
    public GameObject skillSelected;

    private GameObject[] skillHolders = new GameObject[3];
    private int currentSkillHolder = 0;
    private int currentCharacterTarget = 0;

    public VisualBattle vBattle;

    private bool skillSelectable = false;
    private bool characterSelectable = false;

	private List<int>[] availableTargets = new List<int>[3];

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

        skills.transform.localScale = new Vector3(0f, 0f, 0f);
        skills.SetActive(false);
	
	}

    public void AllowInteraction()
    {
        skills.SetActive(true);
        skillSelectable = true;
		availableTargets [0] = vBattle.bs.getTargets (vBattle.myCharacter, 0);
		availableTargets [1] = vBattle.bs.getTargets (vBattle.myCharacter, 1);
		availableTargets [2] = vBattle.bs.getTargets (vBattle.myCharacter, 2);
    }

    // Update is called once per frame
    void Update() {

        if (skillSelectable)
        {

            float aux = Mathf.Lerp(skills.transform.localScale.x, 0.5934315f, Time.deltaTime * 15f);
            skills.transform.localScale = new Vector3(aux, aux, aux);

            if (characterSelectable)
            {
                // SE ELIGE EL ENEMIGO

                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentCharacterTarget++;
					while (currentCharacterTarget >= availableTargets[currentSkillHolder].Count || vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]] == null)
                    {
						if (currentCharacterTarget >= availableTargets[currentSkillHolder].Count)
                        {
							currentCharacterTarget = 0;
                        }
                        else
                        {
                            currentCharacterTarget++;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentCharacterTarget--;
					while (currentCharacterTarget < 0 || vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]] == null)
                    {
                        if (currentCharacterTarget < 0)
                        {
							currentCharacterTarget = availableTargets[currentSkillHolder].Count - 1;
                        }
                        else
                        {
                            currentCharacterTarget--;
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    characterSelectable = false;
                }

                pointer.transform.position = new Vector3(Mathf.Lerp(pointer.transform.position.x, vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]].transform.position.x, Time.deltaTime * 15f), Mathf.Lerp(pointer.transform.position.y, vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]].transform.position.y, Time.deltaTime * 15f), pointer.transform.position.z);
                pointer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                pointer.transform.localScale = new Vector3(1f * (vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]].root.transform.localScale.x / Mathf.Abs(vBattle.visualCharacters[availableTargets[currentSkillHolder][currentCharacterTarget]].root.transform.localScale.x)), 1f, 1f);


                if (Input.GetKeyDown(KeyCode.Return))
                {
                    skillSelectable = false;
                    characterSelectable = false;
                    vBattle.setOrders(currentSkillHolder, availableTargets[currentSkillHolder][currentCharacterTarget]);
                    currentSkillHolder = 0;
                    currentCharacterTarget = 0;
                }

				
            }
            else
            {
                // SE ELIGE LA HABILIDAD
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentSkillHolder++;
                    if (currentSkillHolder >= skillHolders.Length) { currentSkillHolder = 0; }
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentSkillHolder--;
                    if (currentSkillHolder < 0) { currentSkillHolder = skillHolders.Length - 1; }
                }

                skills.transform.position = new Vector3(vBattle.visualCharacters[vBattle.myCharacter].root.transform.position.x, vBattle.visualCharacters[vBattle.myCharacter].root.transform.position.y + vBattle.visualCharacters[vBattle.myCharacter].characterBounds.size.y / 2f + 3f, skills.transform.position.z);

                pointer.transform.localPosition = new Vector3(Mathf.Lerp(pointer.transform.localPosition.x, skillHolders[currentSkillHolder].transform.localPosition.x, Time.deltaTime * 15f), Mathf.Lerp(pointer.transform.localPosition.y, -3.74f, Time.deltaTime * 15f), pointer.transform.localPosition.z);
                pointer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                pointer.transform.localScale = new Vector3(1f, 1f, 1f);
                skillSelected.transform.localPosition = new Vector3(skillHolders[currentSkillHolder].transform.localPosition.x, skillSelected.transform.localPosition.y, skillSelected.transform.localPosition.z);

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (availableTargets[currentSkillHolder].Count == 0)
                    {
                        skillSelectable = false;
                        characterSelectable = false;
                        vBattle.setOrders(currentSkillHolder, -1);
                        currentSkillHolder = 0;
                        currentCharacterTarget = 0;
                    }
                    else
                    {
                        characterSelectable = true;
                        currentCharacterTarget = 0;
                    }
                }

            }

        }
        else
        {
            if (skills.activeInHierarchy)
            {
                float aux = Mathf.Lerp(skills.transform.localScale.x, 0f, Time.deltaTime * 15f);
                skills.transform.localScale = new Vector3(aux, aux, aux);
                if (skills.transform.localScale.x < 0.01f)
                {
                    skills.SetActive(false);
                }
            }
            
        }

        
	}
}
