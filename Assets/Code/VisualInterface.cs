using UnityEngine;
using System.Collections;

public class VisualInterface : MonoBehaviour {

    public GameObject root;
    public GameObject skills;
    public GameObject pointer;
    public GameObject skillSelected;

    private GameObject[] skillHolders = new GameObject[3];
    private int currentSkillHolder = 0;
    private int currentCharacterTarget = 0;

    public VisualBattle vBatlle;

    private bool skillSelectable = false;
    private bool characterSelectable = false;

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
                    while (currentCharacterTarget >= vBatlle.visualCharacters.Length || vBatlle.visualCharacters[currentCharacterTarget] == null)
                    {
                        if (currentCharacterTarget >= vBatlle.visualCharacters.Length)
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
                    while (currentCharacterTarget < 0 || vBatlle.visualCharacters[currentCharacterTarget] == null)
                    {
                        if (currentCharacterTarget < 0)
                        {
                            currentCharacterTarget = vBatlle.visualCharacters.Length - 1;
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

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    skillSelectable = false;
                    characterSelectable = false;
                    vBatlle.setOrders(vBatlle.bs.getCurrentCharacters()[vBatlle.myCharacter].getOwnClass().getSkillID(currentSkillHolder), currentCharacterTarget);
                    currentSkillHolder = 0;
                    currentCharacterTarget = 0;
                }

                pointer.transform.position = new Vector3(Mathf.Lerp(pointer.transform.position.x, vBatlle.visualCharacters[currentCharacterTarget].transform.position.x, Time.deltaTime * 15f), Mathf.Lerp(pointer.transform.position.y, vBatlle.visualCharacters[currentCharacterTarget].transform.position.y, Time.deltaTime * 15f), pointer.transform.position.z);
                pointer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                pointer.transform.localScale = new Vector3(1f * (vBatlle.visualCharacters[currentCharacterTarget].root.transform.localScale.x / Mathf.Abs(vBatlle.visualCharacters[currentCharacterTarget].root.transform.localScale.x)), 1f, 1f);

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

                skills.transform.position = new Vector3(vBatlle.visualCharacters[vBatlle.myCharacter].root.transform.position.x, vBatlle.visualCharacters[vBatlle.myCharacter].root.transform.position.y + vBatlle.visualCharacters[vBatlle.myCharacter].characterBounds.size.y / 2f + 3f, skills.transform.position.z);

                pointer.transform.localPosition = new Vector3(Mathf.Lerp(pointer.transform.localPosition.x, skillHolders[currentSkillHolder].transform.localPosition.x, Time.deltaTime * 15f), Mathf.Lerp(pointer.transform.localPosition.y, -3.74f, Time.deltaTime * 15f), pointer.transform.localPosition.z);
                pointer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
                pointer.transform.localScale = new Vector3(1f, 1f, 1f);
                skillSelected.transform.localPosition = new Vector3(skillHolders[currentSkillHolder].transform.localPosition.x, skillSelected.transform.localPosition.y, skillSelected.transform.localPosition.z);

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    characterSelectable = true;
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
