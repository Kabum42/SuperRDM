using UnityEngine;
using System.Collections;

public class VisualCharacter : MonoBehaviour {

    public GameObject root;
    public int performing = -1;
    private VisualCharacter targetPerformance = null;
    private bool side = true;
    private GameObject character;
    private GameObject bloodBarbarian;
    private AudioSource axeHit;

    private string statusPerformance = "none";

	// Use this for initialization
	void Awake () {
        root = this.gameObject;
        character = root.transform.FindChild("Character").gameObject;
        bloodBarbarian = root.transform.FindChild("Character/BloodBarbarian").gameObject;
        bloodBarbarian.SetActive(false);

        axeHit = gameObject.AddComponent<AudioSource>();
        axeHit.clip = Resources.Load("Music/Hits/AxeHit") as AudioClip;
	}
	
	// Update is called once per frame
	void Update () {

        if (statusPerformance == "chasing")
        {
            // ACERCARSE
            if (side)
            {
                if ((root.transform.position.x + character.GetComponent<SpriteRenderer>().bounds.size.x / 2f) < targetPerformance.root.transform.position.x)
                {
                    root.transform.position = new Vector3(Mathf.Lerp(root.transform.position.x, targetPerformance.root.transform.position.x, Time.deltaTime * 10f), root.transform.position.y, root.transform.position.z);

                }
                else
                {
                    statusPerformance = "executing";
                }
            }

        }
        else if (statusPerformance == "returning")
        {
            // VUELVE A SU SITIO
            if (side)
            {
                if (root.transform.position.x > -4.95f)
                {
                    root.transform.position = new Vector3(Mathf.Lerp(root.transform.position.x, -5f, Time.deltaTime * 10f), root.transform.position.y, root.transform.position.z);
                }
                else
                {
                    statusPerformance = "none";

                    float scaleX = root.transform.localScale.x;
                    if (side)
                    {
                        scaleX = Mathf.Abs(scaleX);
                    }
                    else
                    {
                        scaleX = -Mathf.Abs(scaleX);
                    }
                    root.transform.localScale = new Vector3(scaleX, root.transform.localScale.y, root.transform.localScale.z);
                }
            }
        }
        else if (statusPerformance == "executing")
        {
            // HACE EL ATAQUE
            if (performing == 2)
            {
                // ES EL FINISHER DEL BARBARO
                targetPerformance.Represent(GlobalData.Skills[performing]);
                performing = -1;
                targetPerformance = null;
                statusPerformance = "returning";

                float scaleX = root.transform.localScale.x;
                if (side)
                {
                    scaleX = -Mathf.Abs(scaleX);
                }
                else
                {
                    scaleX = Mathf.Abs(scaleX);
                }
                root.transform.localScale = new Vector3(scaleX, root.transform.localScale.y, root.transform.localScale.z);
            }
            
        }

        



        if (bloodBarbarian.activeInHierarchy)
        {
            if (bloodBarbarian.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                bloodBarbarian.SetActive(false);
            }
        }
	
	}

    public void SetBattlePosition(bool auxSide, int number)
    {
        if (auxSide)
        {
            root.transform.position = new Vector3(-5f, root.transform.position.y, root.transform.position.z);
        }
        else
        {
            root.transform.localScale = new Vector3(-root.transform.localScale.x, root.transform.localScale.y, root.transform.localScale.z);
            root.transform.position = new Vector3(5f, root.transform.position.y, root.transform.position.z);
        }
        side = auxSide;
    }

    public void Represent(Skill s)
    {
        if (s.getID() == 2)
        {
            // ES EL FINISHER DEL BARBARO
            axeHit.Play();
            bloodBarbarian.SetActive(true);
            Hacks.SpriteRendererAlpha(bloodBarbarian, 1f);
        }
    }

    public void Represent(Skill s, float stack)
    {

    }


    public void Perform(Skill s, VisualCharacter v)
    {

        performing = s.getID();
        targetPerformance = v;

        if (performing == 2)
        {
            statusPerformance = "chasing";
        }
        
    }

    public void Perform(Skill s, VisualCharacter v, float stack)
    {

    }
}
