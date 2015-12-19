using UnityEngine;
using System.Collections;

public class SkillBar : MonoBehaviour {

    public GameObject root;
    private GameObject miss;
    private GameObject bad;
    private GameObject good;
    private GameObject critical;
    private GameObject circle;
    private GameObject circleBase;

	private Vector2 vectorMiss = new Vector2(-1f, 1f);
    private Vector2 vectorBad = new Vector2(0f, 0f);
	private Vector2 vectorGood = new Vector2(0f, 0f);
	private Vector2 vectorCritical = new Vector2(0f, 0f);

    private int numBounces = 42;

    private Vector2 originalVectorBad;
    private Vector2 originalVectorGood;
    private Vector2 originalVectorCritical;

    private float width = 4.5f*2f;
    private bool right = false;
    private float speed = 2f;

	private float circleX = 1.5f;

    private float delay = 0f;

    private AudioSource BolingaBounceSound;
    private AudioSource BolingaByeSound;
    private AudioSource BolingaPressedSound;

    public bool isActive = false;
    public float precision = -1f;

	// Use this for initialization
	void Start() {

        root = this.gameObject;

        circle = root.transform.FindChild("Circle").gameObject;
        circle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Battle/Bolinga");
        circle.transform.localPosition = new Vector3(-width/2 +circle.GetComponent<Renderer>().bounds.size.x, circle.transform.localPosition.y, circle.transform.localPosition.z);

        circleBase = circle.transform.FindChild("Base").gameObject;

        miss = root.transform.FindChild("Miss").gameObject;
        bad = root.transform.FindChild("Bad").gameObject;
        good = root.transform.FindChild("Good").gameObject;
        critical = root.transform.FindChild("Critical").gameObject;

        BolingaBounceSound = gameObject.AddComponent<AudioSource>();
        BolingaBounceSound.clip = Resources.Load("Music/Battle/BolingaBounce") as AudioClip;
        BolingaBounceSound.playOnAwake = false;

        BolingaByeSound = gameObject.AddComponent<AudioSource>();
        BolingaByeSound.clip = Resources.Load("Music/Battle/BolingaBye") as AudioClip;
        BolingaByeSound.playOnAwake = false;

        BolingaPressedSound = gameObject.AddComponent<AudioSource>();
        BolingaPressedSound.clip = Resources.Load("Music/Battle/BolingaPressed") as AudioClip;
        BolingaPressedSound.playOnAwake = false;

    }

    void Update()
    {
		if (root.transform.localScale.x < 0.99f && numBounces < 2) {
			float scaleRoot = Mathf.Lerp (root.transform.localScale.x, 1f, Time.deltaTime * 10f);
			root.transform.localScale = new Vector3 (scaleRoot, scaleRoot, scaleRoot);
		}
        else if (root.transform.localScale.x > 0.01f && numBounces >= 2)
        {
            if (delay > 0f)
            {
                delay -= Time.deltaTime;
            }
            else
            {
                float scaleRoot = Mathf.Lerp(root.transform.localScale.x, 0f, Time.deltaTime * 25f);
                root.transform.localScale = new Vector3(scaleRoot, scaleRoot, scaleRoot);
                if (scaleRoot <= 0.01f)
                {
                    isActive = false;
                }
            }
        }

        if (numBounces < 2)
        {
            bad.transform.localScale = new Vector3(Mathf.Lerp(bad.transform.localScale.x, (vectorBad.y - vectorBad.x) * miss.transform.localScale.x / 2f, Time.deltaTime * 15f), bad.transform.localScale.y, bad.transform.localScale.z);
            bad.transform.localPosition = new Vector3(Mathf.Lerp(bad.transform.localPosition.x, vectorBad.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + bad.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime * 15f), bad.transform.localPosition.y, bad.transform.localPosition.z);

            good.transform.localScale = new Vector3(Mathf.Lerp(good.transform.localScale.x, (vectorGood.y - vectorGood.x) * miss.transform.localScale.x / 2f, Time.deltaTime * 15f), good.transform.localScale.y, good.transform.localScale.z);
            good.transform.localPosition = new Vector3(Mathf.Lerp(good.transform.localPosition.x, vectorGood.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + good.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime * 15f), good.transform.localPosition.y, good.transform.localPosition.z);

            critical.transform.localScale = new Vector3(Mathf.Lerp(critical.transform.localScale.x, (vectorCritical.y - vectorCritical.x) * miss.transform.localScale.x / 2f, Time.deltaTime * 15f), critical.transform.localScale.y, critical.transform.localScale.z);
            critical.transform.localPosition = new Vector3(Mathf.Lerp(critical.transform.localPosition.x, vectorCritical.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + critical.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime * 15f), critical.transform.localPosition.y, critical.transform.localPosition.z);


            if (Input.GetKeyDown(KeyCode.Return) && numBounces < 2 && circle.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Battle/Bolinga") && root.transform.localScale.x >= 0.99f)
            {
                pressedButton();
            }


            if (right)
            {
                circleX += Time.deltaTime * speed;

                if (circleX > 1)
                {
                    circleX = 1;
                    Bounce();
                }
            }
            else
            {
                circleX -= Time.deltaTime * speed;

                if (circleX < -1)
                {
                    circleX = -1;
                    Bounce();
                }
            }
            circle.transform.localPosition = new Vector3(circleX * (miss.GetComponent<Renderer>().bounds.size.x / 2f - circle.GetComponent<Renderer>().bounds.size.x / 2f), circle.transform.localPosition.y, circle.transform.localPosition.z);

            if (circleX > vectorCritical.x && circleX < vectorCritical.y)
            {
                circleBase.GetComponent<SpriteRenderer>().color = critical.GetComponent<SpriteRenderer>().color;
            }
            else if (circleX > vectorGood.x && circleX < vectorGood.y)
            {
                circleBase.GetComponent<SpriteRenderer>().color = good.GetComponent<SpriteRenderer>().color;
            }
            else if (circleX > vectorBad.x && circleX < vectorBad.y)
            {
                circleBase.GetComponent<SpriteRenderer>().color = bad.GetComponent<SpriteRenderer>().color;
            }
            else
            {
                circleBase.GetComponent<SpriteRenderer>().color = miss.GetComponent<SpriteRenderer>().color;
            }
        }

        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        */

        
    }

	void pressedButton() {

		if (circleX > vectorCritical.x && circleX < vectorCritical.y) {
			Debug.Log ("CRITICAL HIT");
            precision = 1f;
		} else if (circleX > vectorGood.x && circleX < vectorGood.y) {
			Debug.Log ("GOOD HIT");
            precision = 0.66f;
		} else if (circleX > vectorBad.x && circleX < vectorBad.y) {
			Debug.Log ("BAD HIT");
            precision = 0.33f;
		} else {
			Debug.Log ("MISS HIT");
            precision = 0f;
		}

        numBounces = 2;
        delay = 0.5f;
        BolingaPressedSound.Play();

	}

    void Bounce()
    {
        numBounces++;
        

        if (numBounces >= 2)
        {
            // NOTHING
            BolingaByeSound.Play();
        }
        else
        {

            right = !right;
            BolingaBounceSound.Play();

            if (circle.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Battle/Bolinga2"))
            {
                circle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Battle/Bolinga");
            }
            else
            {
                /*
                speed -= 0.4f;
                if (speed < 0.6f)
                {
                    speed = 0.6f;
                }
                */
                //setBad(Reduce(vectorBad, 0.9f).x, Reduce(vectorBad, 0.9f).y);
                setGood(Reduce(vectorGood, 0.8f).x, Reduce(vectorGood, 0.8f).y);
                setCritical(Reduce(vectorCritical, 0f).x, Reduce(vectorCritical, 0f).y);
            }
        }

        
        
    }

	public void Reset() {

        numBounces = 0;

		root.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f);

        circle.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Battle/Bolinga");

		circleX = -1.5f;
		speed = 2f;
		right = true;

        vectorBad = originalVectorBad;
        vectorGood = originalVectorGood;
        vectorCritical = originalVectorCritical;

        isActive = true;
        precision = -1f;

	}

    public void setRandom(float widthBad, float widthGood, float widthCritical) {

        originalVectorBad = new Vector2(0f, 0f);
        originalVectorGood = new Vector2(0f, 0f);
        originalVectorCritical = new Vector2(0f, 0f);

        float aux = 2f - widthBad * 2f;
        float rand = Random.Range(0f, aux) -1f;
        setBad(rand, rand + widthBad * 2f);

        aux = (vectorBad.y - vectorBad.x) - widthGood * 2f;
        rand = Random.Range(0f, aux) + vectorBad.x;
        setGood(rand, rand + widthGood * 2f);

        aux = (vectorGood.y - vectorGood.x) - widthCritical * 2f;
        rand = Random.Range(0f, aux) + vectorGood.x;
        setCritical(rand, rand + widthCritical * 2f);

    }

    public void setBad(float x1, float x2)
    {
        vectorBad = new Vector2(x1, x2);
        if (originalVectorBad.x == 0f && originalVectorBad.y == 0f)
        {
            originalVectorBad = vectorBad;
        }
    }

    public void setGood(float x1, float x2)
    {
        vectorGood = new Vector2(x1, x2);
        if (originalVectorGood.x == 0f && originalVectorGood.y == 0f)
        {
            originalVectorGood = vectorGood;
        }
    }

    public void setCritical(float x1, float x2)
    {
        vectorCritical = new Vector2(x1, x2);
        if (originalVectorCritical.x == 0f && originalVectorCritical.y == 0f)
        {
            originalVectorCritical = vectorCritical;
        }
    }

    private Vector2 Reduce(Vector2 v, float amount) {

        float half = (v.x+v.y)/2f;
        v = new Vector2(half + (v.x - half) * amount, half + (v.y - half) * amount);

        return v;

    }

}
