using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    public GameObject logo;
    private float radius = 0f;
    private Vector3 logoPosition;
    public GameObject startText;
    public GameObject startText2;
    private float radius2 = 0f;
    public GameObject button1;
    public GameObject button2;

	// Use this for initialization
	void Start () {

        logoPosition = logo.transform.position;
        logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, 0f);

	}
	
	// Update is called once per frame
	void Update () {

        if (logo.GetComponent<SpriteRenderer>().color.a < 0.99f)
        {
            logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, Mathf.Lerp(logo.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime));
        }

        radius += Time.deltaTime*100f;
        if (radius > 360f) { radius -= 360f; }

        logo.transform.localPosition = logoPosition + new Vector3(0f, Mathf.Sin(radius * Mathf.PI / 180)*0.25f, 0f);

        radius2 += Time.deltaTime * 100f;
        if (radius2 > 360f) { radius2 -= 360f; }

        startText.GetComponent<TextMesh>().color = new Color(startText.GetComponent<TextMesh>().color.r, startText.GetComponent<TextMesh>().color.g, startText.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
        startText2.GetComponent<TextMesh>().color = new Color(startText2.GetComponent<TextMesh>().color.r, startText2.GetComponent<TextMesh>().color.g, startText2.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

        button2.GetComponent<SpriteRenderer>().color = new Color(button2.GetComponent<SpriteRenderer>().color.r, button2.GetComponent<SpriteRenderer>().color.g, button2.GetComponent<SpriteRenderer>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

	}
}
