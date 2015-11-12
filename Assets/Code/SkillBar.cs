using UnityEngine;
using System.Collections;

public class SkillBar : MonoBehaviour {

    public GameObject root;
    private GameObject miss;
    private GameObject bad;
    private GameObject good;
    private GameObject critical;
    private GameObject circle;

	private Vector2 vectorMiss = new Vector2(-1f, 1f);
    private Vector2 vectorBad = new Vector2(0f, 0f);
	private Vector2 vectorGood = new Vector2(0f, 0f);
	private Vector2 vectorCritical = new Vector2(0f, 0f);

    private float width = 4.5f*2f;
    private bool right = true;
    private float speed = 2f;

	private float circleX = -1f;

	// Use this for initialization
	void Start() {
        root = this.gameObject;
        circle = root.transform.FindChild("Circle").gameObject;
        circle.transform.localPosition = new Vector3(-width/2 +circle.GetComponent<Renderer>().bounds.size.x, circle.transform.localPosition.y, circle.transform.localPosition.z);

        miss = root.transform.FindChild("Miss").gameObject;
        bad = root.transform.FindChild("Bad").gameObject;
        good = root.transform.FindChild("Good").gameObject;
        critical = root.transform.FindChild("Critical").gameObject;
    }

    void Update()
    {
		if (root.transform.localScale.x < 0.99f) {
			float scaleRoot = Mathf.Lerp (root.transform.localScale.x, 1f, Time.deltaTime * 10f);
			root.transform.localScale = new Vector3 (scaleRoot, scaleRoot, scaleRoot);
		}

        bad.transform.localScale = new Vector3(Mathf.Lerp(bad.transform.localScale.x, (vectorBad.y-vectorBad.x)*miss.transform.localScale.x/2f, Time.deltaTime*15f), bad.transform.localScale.y, bad.transform.localScale.z);
        bad.transform.localPosition = new Vector3(Mathf.Lerp(bad.transform.localPosition.x, vectorBad.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + bad.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), bad.transform.localPosition.y, bad.transform.localPosition.z);

        good.transform.localScale = new Vector3(Mathf.Lerp(good.transform.localScale.x, (vectorGood.y - vectorGood.x) * miss.transform.localScale.x / 2f, Time.deltaTime*15f), good.transform.localScale.y, good.transform.localScale.z);
        good.transform.localPosition = new Vector3(Mathf.Lerp(good.transform.localPosition.x, vectorGood.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + good.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), good.transform.localPosition.y, good.transform.localPosition.z);

        critical.transform.localScale = new Vector3(Mathf.Lerp(critical.transform.localScale.x, (vectorCritical.y - vectorCritical.x) * miss.transform.localScale.x / 2f, Time.deltaTime*15f), critical.transform.localScale.y, critical.transform.localScale.z);
        critical.transform.localPosition = new Vector3(Mathf.Lerp(critical.transform.localPosition.x, vectorCritical.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + critical.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), critical.transform.localPosition.y, critical.transform.localPosition.z);


		if (circle.transform.localScale.x < 1.176569f + 0.05f) {

			if (Input.GetKeyDown(KeyCode.Return)) {
				pressedButton();
			}


			if (right) {
				circleX += Time.deltaTime * speed;

				if (circleX > 1) {
					circleX = 1;
					Bounce ();
				}
			} else {
				circleX -= Time.deltaTime * speed;

				if (circleX < -1) {
					circleX = -1;
					Bounce ();
				}
			}

			if (Input.GetKeyDown(KeyCode.R)) {
				Reset();
			}

		} else {

			float scale = Mathf.Lerp(circle.transform.localScale.x, 1.176569f, Time.deltaTime*speed);
			float color = Mathf.Lerp(circle.GetComponent<SpriteRenderer>().color.r, 0.7f, Time.deltaTime*speed);

			circle.transform.localScale = new Vector3(scale, scale, scale);

			if (circle.transform.localScale.x < 1.176569f + 0.05f) { color = 1f; }

			circle.GetComponent<SpriteRenderer>().color = new Color(color, color, color);

		}

		circle.transform.localPosition = new Vector3 (circleX * (miss.GetComponent<Renderer> ().bounds.size.x / 2f - circle.GetComponent<Renderer> ().bounds.size.x / 2f), circle.transform.localPosition.y, circle.transform.localPosition.z);


        

        
    }

	void pressedButton() {

		if (circleX > vectorCritical.x && circleX < vectorCritical.y) {
			Debug.Log ("CRITICAL HIT");
		} else if (circleX > vectorGood.x && circleX < vectorGood.y) {
			Debug.Log ("GOOD HIT");
		} else if (circleX > vectorBad.x && circleX < vectorBad.y) {
			Debug.Log ("BAD HIT");
		} else {
			Debug.Log ("MISS HIT");
		}

	}

    void Bounce()
    {
        right = !right;
        speed -= 0.4f;
        if (speed < 0.6f)
        {
            speed = 0.6f;
        }
        //setBad(Reduce(vectorBad, 0.9f).x, Reduce(vectorBad, 0.9f).y);
        setGood(Reduce(vectorGood, 0.8f).x, Reduce(vectorGood, 0.8f).y);
        setCritical(Reduce(vectorCritical, 0f).x, Reduce(vectorCritical, 0f).y);
    }

	public void Reset() {
		root.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f);
		circle.GetComponent<SpriteRenderer> ().color = new Color (0f, 0f, 0f);
		circle.transform.localScale = new Vector3 (2f, 2f, 2f);
		circleX = -1f;
		speed = 2f;
		right = true;
		//setBad(-0.95f, 0.65f);
		//setGood(-0.85f, 0.15f);
		//setCritical(-0.75f, -0.5f);
	}

    public void setBad(float x1, float x2)
    {
        vectorBad = new Vector2(x1, x2);
    }

    public void setGood(float x1, float x2)
    {
        vectorGood = new Vector2(x1, x2);
    }

    public void setCritical(float x1, float x2)
    {
        vectorCritical = new Vector2(x1, x2);
    }

    private Vector2 Reduce(Vector2 v, float amount) {

        float half = (v.x+v.y)/2f;
        v = new Vector2(half + (v.x - half) * amount, half + (v.y - half) * amount);

        return v;

    }

}
