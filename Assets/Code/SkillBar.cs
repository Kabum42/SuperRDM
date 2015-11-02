using UnityEngine;
using System.Collections;

public class SkillBar : MonoBehaviour {

    public GameObject root;
    private GameObject miss;
    private GameObject bad;
    private GameObject good;
    private GameObject critical;
    private GameObject circle;

    private Vector2 vectorMiss;
    private Vector2 vectorBad;
    private Vector2 vectorGood;
    private Vector2 vectorCritical;

    private float width = 4.5f*2f;
    private bool right = true;
    private float speed = 10f;

	// Use this for initialization
	void Start() {
        root = this.gameObject;
        circle = root.transform.FindChild("Circle").gameObject;
        circle.transform.localPosition = new Vector3(-width/2 +circle.GetComponent<Renderer>().bounds.size.x, circle.transform.localPosition.y, circle.transform.localPosition.z);
        vectorMiss = new Vector2(-1f, 1f);

        miss = root.transform.FindChild("Miss").gameObject;
        bad = root.transform.FindChild("Bad").gameObject;
        good = root.transform.FindChild("Good").gameObject;
        critical = root.transform.FindChild("Critical").gameObject;
    }

    void Update()
    {
        bad.transform.localScale = new Vector3(Mathf.Lerp(bad.transform.localScale.x, (vectorBad.y-vectorBad.x)*miss.transform.localScale.x/2f, Time.deltaTime*15f), bad.transform.localScale.y, bad.transform.localScale.z);
        bad.transform.localPosition = new Vector3(Mathf.Lerp(bad.transform.localPosition.x, vectorBad.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + bad.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), bad.transform.localPosition.y, bad.transform.localPosition.z);

        good.transform.localScale = new Vector3(Mathf.Lerp(good.transform.localScale.x, (vectorGood.y - vectorGood.x) * miss.transform.localScale.x / 2f, Time.deltaTime*15f), good.transform.localScale.y, good.transform.localScale.z);
        good.transform.localPosition = new Vector3(Mathf.Lerp(good.transform.localPosition.x, vectorGood.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + good.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), good.transform.localPosition.y, good.transform.localPosition.z);

        critical.transform.localScale = new Vector3(Mathf.Lerp(critical.transform.localScale.x, (vectorCritical.y - vectorCritical.x) * miss.transform.localScale.x / 2f, Time.deltaTime*15f), critical.transform.localScale.y, critical.transform.localScale.z);
        critical.transform.localPosition = new Vector3(Mathf.Lerp(critical.transform.localPosition.x, vectorCritical.x * miss.GetComponent<Renderer>().bounds.size.x / 2f + critical.GetComponent<Renderer>().bounds.size.x / 2f, Time.deltaTime*15f), critical.transform.localPosition.y, critical.transform.localPosition.z);


        if (right)
        {
            circle.transform.localPosition = new Vector3(circle.transform.localPosition.x + Time.deltaTime*speed, circle.transform.localPosition.y, circle.transform.localPosition.z);

            if (circle.transform.localPosition.x > width / 2 - circle.GetComponent<Renderer>().bounds.size.x)
            {
                Bounce();
            }
        }
        else
        {
            circle.transform.localPosition = new Vector3(circle.transform.localPosition.x - Time.deltaTime*speed, circle.transform.localPosition.y, circle.transform.localPosition.z);

            if (circle.transform.localPosition.x < -width / 2 + circle.GetComponent<Renderer>().bounds.size.x)
            {
                Bounce();
            }
        }

        
    }

    void Bounce()
    {
        right = !right;
        speed -= 2f;
        if (speed < 2f)
        {
            speed = 2f;
        }
        //setBad(Reduce(vectorBad, 0.9f).x, Reduce(vectorBad, 0.9f).y);
        setGood(Reduce(vectorGood, 0.8f).x, Reduce(vectorGood, 0.8f).y);
        setCritical(Reduce(vectorCritical, 0f).x, Reduce(vectorCritical, 0f).y);
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
