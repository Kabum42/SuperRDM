using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    public GameObject logo;
    private float radius = 0f;
    private Vector3 logoPosition;
    public GameObject startText;
    public GameObject startText2;
    private float radius2 = 300f;
    public GameObject button1;
    public GameObject button2;
    public AudioSource intro;
    private float transition = 1f;

    // Use this for initialization
    void Start () {

        intro = gameObject.AddComponent<AudioSource>();
        intro.clip = Resources.Load("Music/Intro") as AudioClip;
        intro.volume = 1f;
        intro.playOnAwake = true;
        intro.loop = true;
        intro.Play();

        logoPosition = logo.transform.position;
        logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, 0f);

	}
	
	// Update is called once per frame
	void Update () {

        if (transition < 1f)
        {
            transition -= Time.deltaTime;
            if (transition < 0f)
            {
                Application.LoadLevel("Preparation");
            }

            logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, transition);

            startText.GetComponent<TextMesh>().color = new Color(startText.GetComponent<TextMesh>().color.r, startText.GetComponent<TextMesh>().color.g, startText.GetComponent<TextMesh>().color.b, transition);
            startText2.GetComponent<TextMesh>().color = new Color(startText2.GetComponent<TextMesh>().color.r, startText2.GetComponent<TextMesh>().color.g, startText2.GetComponent<TextMesh>().color.b, transition);

            button1.GetComponent<SpriteRenderer>().color = new Color(button1.GetComponent<SpriteRenderer>().color.r, button1.GetComponent<SpriteRenderer>().color.g, button1.GetComponent<SpriteRenderer>().color.b, transition);
            button2.GetComponent<SpriteRenderer>().color = new Color(button2.GetComponent<SpriteRenderer>().color.r, button2.GetComponent<SpriteRenderer>().color.g, button2.GetComponent<SpriteRenderer>().color.b, transition);

            intro.volume = transition;

        }

        else
        {

            if (logo.GetComponent<SpriteRenderer>().color.a < 0.99f)
            {
                logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, Mathf.Lerp(logo.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime));
            }

            radius2 += Time.deltaTime * 100f;
            if (radius2 > 360f) { radius2 -= 360f; }

            startText.GetComponent<TextMesh>().color = new Color(startText.GetComponent<TextMesh>().color.r, startText.GetComponent<TextMesh>().color.g, startText.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
            startText2.GetComponent<TextMesh>().color = new Color(startText2.GetComponent<TextMesh>().color.r, startText2.GetComponent<TextMesh>().color.g, startText2.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

            button1.GetComponent<SpriteRenderer>().color = new Color(button1.GetComponent<SpriteRenderer>().color.r, button1.GetComponent<SpriteRenderer>().color.g, button1.GetComponent<SpriteRenderer>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
            button2.GetComponent<SpriteRenderer>().color = new Color(button2.GetComponent<SpriteRenderer>().color.r, button2.GetComponent<SpriteRenderer>().color.g, button2.GetComponent<SpriteRenderer>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));


        }

        radius += Time.deltaTime*100f;
        if (radius > 360f) { radius -= 360f; }

        logo.transform.localPosition = logoPosition + new Vector3(0f, Mathf.Sin(radius * Mathf.PI / 180)*0.25f, 0f);

        
        int controllerConnected = -1;
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                controllerConnected = i;
                break;
            }
        }

        if (controllerConnected != -1)
        {
            // CONTROLLER PLUGGED
            if (button1.activeInHierarchy) { button1.SetActive(false); }
            if (!button2.activeInHierarchy) { button2.SetActive(true); }

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsWebPlayer)
            {
                // WINDOWS
                if (Input.GetKeyDown("joystick button 0"))
                {
                    transition -= Time.deltaTime;
                }
            }
            if (Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer)
            {
                // MAC
                if (Input.GetKeyDown("joystick button 16"))
                {
                    transition -= Time.deltaTime;
                }
            }
            if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                // LINUX
                if (Input.GetKeyDown("joystick button 0"))
                {
                    transition -= Time.deltaTime;
                }
            }

        }
        else
        {
            // CONTROLLER NOT PLUGGED
            if (button2.activeInHierarchy) { button2.SetActive(false); }
            if (!button1.activeInHierarchy) { button1.SetActive(true); }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                transition -= Time.deltaTime;
            }

        }

        

    }
}
