using UnityEngine;
using System.Collections;

public class VisualCharacter : MonoBehaviour {

    public GameObject root;
    public int performing = -1;
    private VisualCharacter targetPerformance = null;
    private bool side = true;
    private float[] importantFloats;
    private GameObject character;
    private GameObject animated;
    private Bounds characterBounds;
    private GameObject text1;
    private GameObject text2;
    private GameObject blood1;
    private GameObject blood2;
    private GameObject bloodBF;
    private GameObject bloodBFAnimation;
    private GameObject bloodBFSplat;
    private Vector3 bloodBFSplatOriginalPosition;
    private Vector3 bloodBFSplatOriginalScale;

    private AudioSource audio1;
    private AudioSource audio2;
    private AudioSource audio3;

    private string statusPerformance = "none";

	// Use this for initialization
	void Awake () {
        root = this.gameObject;

        //Time.timeScale = 0.25f;

        setClass(GlobalData.Classes[0]);

        text1 = root.transform.FindChild("Text1").gameObject;
        text1.SetActive(false);

        text2 = root.transform.FindChild("Text2").gameObject;
        text2.SetActive(false);

        blood1 = root.transform.FindChild("Blood1").gameObject;
        blood1.SetActive(false);

        blood2 = root.transform.FindChild("Blood2").gameObject;
        blood2.SetActive(false);

        bloodBF = root.transform.FindChild("BloodBF").gameObject;
        bloodBF.SetActive(false);

        bloodBFAnimation = root.transform.FindChild("BloodBFAnimation").gameObject;
        bloodBFAnimation.SetActive(false);

        bloodBFSplat = root.transform.FindChild("BloodBFSplat").gameObject;
        bloodBFSplat.transform.localScale = new Vector3(0f, 0f, 0f);
        Hacks.SpriteRendererAlpha(bloodBFSplat, 1f);
        bloodBFSplat.SetActive(false);

        audio1 = gameObject.AddComponent<AudioSource>();
        audio2 = gameObject.AddComponent<AudioSource>();
        audio3 = gameObject.AddComponent<AudioSource>();
        
	}

    public void setClass(Class c)
    {
        string prefab = null;

        if (c.getID() == 0)
        {
            // ES EL BOAR RIDER
            //prefab = "Barbarian";
            character = Instantiate(Resources.Load("Prefabs/Barbarian")) as GameObject;
            character.transform.parent = root.transform;
            animated = character.transform.FindChild("Animated").gameObject;
            characterBounds = GetChildRendererBounds(character);
        }
    }

    Bounds GetChildRendererBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (statusPerformance == "chasing")
        {
            // ACERCARSE

            bool executing = false;

            if (side)
            {
                if ((root.transform.position.x + characterBounds.size.x) < targetPerformance.root.transform.position.x)
                {
                    root.transform.position = new Vector3(root.transform.position.x + Time.deltaTime * 20f, root.transform.position.y, root.transform.position.z);
                }
                else
                {
                    executing = true;
                }
            }
            else
            {
                if ((root.transform.position.x - characterBounds.size.x) > targetPerformance.root.transform.position.x)
                {
                    root.transform.position = new Vector3(root.transform.position.x - Time.deltaTime * 20f, root.transform.position.y, root.transform.position.z);
                }
                else
                {
                    executing = true;
                }
            }

            if (executing)
            {
                statusPerformance = "executing";
                animated.GetComponent<Animator>().CrossFade("S1", GlobalData.crossfadeAnimation, 0, 0f);
                int aux = Random.Range(1, 5);
                audio1.clip = Resources.Load("Music/Battle/Barbarian_"+aux.ToString("00")) as AudioClip;
                audio1.Play();
            }

        }
        else if (statusPerformance == "returning")
        {
            // VUELVE A SU SITIO
            if (side)
            {
                if (root.transform.position.x > -4.95f)
                {
                    //root.transform.position = new Vector3(root.transform.position.x - Time.deltaTime * 20f, root.transform.position.y, root.transform.position.z);
                    root.transform.position = new Vector3(Mathf.Lerp(root.transform.position.x, -5f, Time.deltaTime * 10f), root.transform.position.y, root.transform.position.z);
                }
                else
                {
                    statusPerformance = "none";
                    animated.GetComponent<Animator>().CrossFade("Idle", GlobalData.crossfadeAnimation, 0, 0f);

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
            else
            {
                if (root.transform.position.x < 4.95f)
                {
                    //root.transform.position = new Vector3(root.transform.position.x + Time.deltaTime * 20f, root.transform.position.y, root.transform.position.z);
                    root.transform.position = new Vector3(Mathf.Lerp(root.transform.position.x, 5f, Time.deltaTime * 10f), root.transform.position.y, root.transform.position.z);
                }
                else
                {
                    statusPerformance = "none";
                    animated.GetComponent<Animator>().CrossFade("Idle", GlobalData.crossfadeAnimation, 0, 0f);

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

                if (animated.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f && targetPerformance != null)
                {
                    targetPerformance.Represent(GlobalData.Skills[performing], importantFloats);
                    targetPerformance = null;
                }

                if (animated.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    performing = -1;
                    statusPerformance = "returning";
                    //animated.GetComponent<Animator>().CrossFade("Return", GlobalData.crossfadeAnimation, 0, 0f);
                    animated.GetComponent<Animator>().Play("Return");
                }

                
            }
            
        }

        



        if (bloodBF.activeInHierarchy)
        {

            if (bloodBFSplat.GetComponent<SpriteRenderer>().color.a < 0.01f)
            {
                text1.SetActive(false);
                text2.SetActive(false);
                blood1.SetActive(false);
                blood2.SetActive(false);
                bloodBF.SetActive(false);
                bloodBFAnimation.SetActive(false);
                bloodBFSplat.transform.localScale = new Vector3(0f, 0f, 0f);
                Hacks.SpriteRendererAlpha(bloodBFSplat, 1f);
                bloodBFSplat.SetActive(false);
            }
            else
            {
                if (bloodBFAnimation.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    Hacks.SpriteRendererAlpha(bloodBFSplat, bloodBFSplat.GetComponent<SpriteRenderer>().color.a - Time.deltaTime * 0.75f);
                }

                
                float auxScale = Mathf.Lerp(-Mathf.Abs(bloodBFSplat.transform.localScale.x), -0.5f, Time.deltaTime * 40f);
                if ((bloodBFSplatOriginalScale.x < 0 && root.transform.localScale.x > 0) || (bloodBFSplatOriginalScale.x > 0 && root.transform.localScale.x < 0)) { auxScale = -auxScale; }
                bloodBFSplat.transform.localScale = new Vector3(auxScale, Mathf.Abs(auxScale), Mathf.Abs(auxScale));
                bloodBFSplat.transform.position = new Vector3(bloodBFSplatOriginalPosition.x + bloodBFSplat.GetComponent<SpriteRenderer>().bounds.size.x * 1 / 2f * -(bloodBFSplatOriginalScale.x / Mathf.Abs(bloodBFSplatOriginalScale.x)), bloodBFSplatOriginalPosition.y - characterBounds.size.y * 1 / 2f, bloodBFSplat.transform.position.z);

                auxScale = 1;
                if (bloodBFSplatOriginalScale.x < 0) { auxScale = -auxScale; }
                if ((bloodBFSplatOriginalScale.x < 0 && root.transform.localScale.x > 0) || (bloodBFSplatOriginalScale.x > 0 && root.transform.localScale.x < 0)) { auxScale = -auxScale; }
                text1.transform.localScale = new Vector3(Mathf.Abs(text1.transform.localScale.x)*auxScale, text1.transform.localScale.y, text1.transform.localScale.z);
                text2.transform.localScale = text1.transform.localScale;

                text1.transform.localPosition = new Vector3(text1.transform.localPosition.x, text1.transform.localPosition.y +Time.deltaTime*1f, text1.transform.localPosition.z);
                Hacks.TextAlpha(text1, text1.GetComponent<TextMesh>().color.a - Time.deltaTime);
                Hacks.TextAlpha(text2, text1.GetComponent<TextMesh>().color.a);
                text2.transform.localPosition = new Vector3(text1.transform.localPosition.x + 0.05f*auxScale, text1.transform.localPosition.y - 0.05f, text2.transform.localPosition.z);
            
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

    public void Represent(Skill s, float[] aux)
    {
        if (s.getID() == 2)
        {
            // ES EL FINISHER DEL BARBARO
            audio1.clip = Resources.Load("Music/Hits/AxeHit") as AudioClip;
            audio1.Play();
            blood1.SetActive(true);
            blood2.SetActive(true);
            bloodBF.SetActive(true);
            if (!bloodBFSplat.activeInHierarchy)
            {
                text1.GetComponent<TextMesh>().text = aux[0].ToString();
                text1.transform.localPosition = new Vector3(text1.transform.localPosition.x, characterBounds.size.y / 2f, text1.transform.localPosition.z);
                Hacks.TextAlpha(text1, 1f);
                text1.SetActive(true);
                text2.GetComponent<TextMesh>().text = text1.GetComponent<TextMesh>().text;
                text2.transform.localPosition = new Vector3(text1.transform.localPosition.x + 0.05f, text1.transform.localPosition.y - 0.05f, text2.transform.localPosition.z);
                Hacks.TextAlpha(text2, 1f);
                text2.SetActive(true);
                bloodBFSplatOriginalPosition = root.transform.position;
                bloodBFSplatOriginalScale = new Vector3(root.transform.localScale.x, root.transform.localScale.y, root.transform.localScale.z);
            }
            bloodBFSplat.SetActive(true);
        }
    }

    public void Perform(Skill s, VisualCharacter v, float[] aux)
    {

        performing = s.getID();
        targetPerformance = v;
        importantFloats = aux;

        if (performing == 2)
        {
            statusPerformance = "chasing";
            animated.GetComponent<Animator>().CrossFade("Move", GlobalData.crossfadeAnimation, 0, 0f);
        }

    }
}
