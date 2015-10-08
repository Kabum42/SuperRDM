using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    private GameObject fading;

    public GameObject logo;
    private float radius = 0f;
    private Vector3 logoPosition;
    public GameObject startText;
    public GameObject startText2;
    private float radius2 = 300f;
    public GameObject button1;
    public GameObject button2;
    private AudioSource intro;
    private float transition = 0f;
    private int phase = 0;
    private Vector3 lastMousePosition;
    private GameObject star;
    private GameObject support;
    private GameObject pointer;
    private GameObject pointer2;
    private GameObject offline1;
    private GameObject offline2;
    private GameObject online1;
    private GameObject online2;
    private int playOption = 0;
    private GameObject host1;
    private GameObject host2;
    private GameObject join1;
    private GameObject join2;
    private GameObject background;
    private AudioSource menuOk;
    private AudioSource menuBack;
    private bool starting = false;

    private int selectableX = 0;
    private int selectableY = 0;

    private float lastDPadX = 0f;
    private float lastDPadY = 0f;

    // Use this for initialization
    void Start () {

        if (!GlobalData.started)
        {
            GlobalData.Start();
        }

        fading = GameObject.Find("Fading");
        Hacks.SpriteRendererAlpha(fading, 1f); 

        star = GameObject.Find("Star");
        Hacks.SpriteRendererAlpha(star, 0f);

        support = GameObject.Find("Support");
        Hacks.SpriteRendererAlpha(support, 0f);

        pointer = GameObject.Find("Pointer");
        pointer.SetActive(false);

        pointer2 = GameObject.Find("Pointer2");
        pointer2.SetActive(false);

        offline1 = GameObject.Find("Offline1");
        offline1.SetActive(false);

        offline2 = GameObject.Find("Offline2");
        offline2.SetActive(false);

        online1 = GameObject.Find("Online1");
        online1.SetActive(false);

        online2 = GameObject.Find("Online2");
        online2.SetActive(false);

        host1 = GameObject.Find("Host1");
        host1.SetActive(false);

        host2 = GameObject.Find("Host2");
        host2.SetActive(false);

        join1 = GameObject.Find("Join1");
        join1.SetActive(false);

        join2 = GameObject.Find("Join2");
        join2.SetActive(false);

        background = GameObject.Find("Background");

        intro = gameObject.AddComponent<AudioSource>();
        intro.clip = Resources.Load("Music/Menu/Intro") as AudioClip;
        intro.volume = 0f;
        intro.loop = true;
        intro.Play();

        menuOk = gameObject.AddComponent<AudioSource>();
        menuOk.clip = Resources.Load("Music/Menu/MenuOk") as AudioClip;
        menuOk.volume = 0.85f;

        menuBack = gameObject.AddComponent<AudioSource>();
        menuBack.clip = Resources.Load("Music/Menu/MenuBack") as AudioClip;
        menuBack.volume = 0.85f;
        
        logoPosition = logo.transform.position;


	}

    // Update is called once per frame
    void Update () {


        radius += Time.deltaTime * 100f;
        if (radius > 360f) { radius -= 360f; }

        logo.transform.localPosition = logoPosition + new Vector3(0f, Mathf.Sin(radius * Mathf.PI / 180) * 0.25f, 0f);

        if (phase != 1)
        {
            radius2 += Time.deltaTime * 100f;
        }
        if (radius2 > 360f) { radius2 -= 360f; }

        Hacks.TextAlpha(startText, 0.15f + Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
        Hacks.TextAlpha(startText2, 0.15f + Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

        Hacks.SpriteRendererAlpha(button1, 0.15f + Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
        Hacks.SpriteRendererAlpha(button2, 0.15f + Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

        Hacks.SpriteRendererAlpha(pointer, 0.35f + Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
        Hacks.SpriteRendererAlpha(pointer2, 1f);


        star.transform.eulerAngles = new Vector3(star.transform.eulerAngles.x, star.transform.eulerAngles.y, star.transform.eulerAngles.z - Time.deltaTime * 50f);

        if (Hacks.ControllerAnyConnected())
        {
            // CONTROLLER PLUGGED
            Hacks.SpriteRendererAlpha(support, Mathf.Lerp(support.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime * 2.5f));
            Hacks.SpriteRendererAlpha(star, Mathf.Lerp(star.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime * 2.5f));
        }
        else
        {
            // CONTROLLER NOT PLUGGED
            Hacks.SpriteRendererAlpha(support, Mathf.Lerp(support.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 2.5f));
            Hacks.SpriteRendererAlpha(star, Mathf.Lerp(star.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 2.5f));
        }

        if (phase == 0)
        {
            if (transition < 1f)
            {
                transition += Time.deltaTime;

                if (transition >= 1f)
                {
                    transition = 1f;
                }
               
                intro.volume = transition;
                Hacks.SpriteRendererAlpha(fading, 1f - transition);

            }


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

                if (GlobalData.OS == "Windows")
                {
                    // WINDOWS
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        transition = 0f;
                        phase = 1;
                        menuOk.Play();
                    }
                }
                if (GlobalData.OS == "Mac")
                {
                    // MAC
                    if (Input.GetKeyDown("joystick button 16"))
                    {
                        transition = 0f;
                        phase = 1;
                        menuOk.Play();
                    }
                }
                if (GlobalData.OS == "Linux")
                {
                    // LINUX
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        transition = 0f;
                        phase = 1;
                        menuOk.Play();
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
                    transition = 0f;
                    phase = 1;
                    menuOk.Play();
                }

            }

        }
        else if (phase == 1)
        {

            Hacks.SpriteRendererAlpha(fading, Mathf.Lerp(fading.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime*5f));

            transition += Time.deltaTime*5f;

            startText.GetComponent<TextMesh>().color = new Color(startText.GetComponent<TextMesh>().color.r, startText.GetComponent<TextMesh>().color.g, startText.GetComponent<TextMesh>().color.b, startText.GetComponent<TextMesh>().color.a * (1f - transition));
            startText2.GetComponent<TextMesh>().color = new Color(startText2.GetComponent<TextMesh>().color.r, startText2.GetComponent<TextMesh>().color.g, startText2.GetComponent<TextMesh>().color.b, startText2.GetComponent<TextMesh>().color.a * (1f - transition));

            button1.GetComponent<SpriteRenderer>().color = new Color(button1.GetComponent<SpriteRenderer>().color.r, button1.GetComponent<SpriteRenderer>().color.g, button1.GetComponent<SpriteRenderer>().color.b, startText.GetComponent<TextMesh>().color.a);
            button2.GetComponent<SpriteRenderer>().color = new Color(button2.GetComponent<SpriteRenderer>().color.r, button2.GetComponent<SpriteRenderer>().color.g, button2.GetComponent<SpriteRenderer>().color.b, startText.GetComponent<TextMesh>().color.a);

            if (transition >= 1f)
            {
                phase = 2;

                startText.SetActive(false);
                startText2.SetActive(false);
                button1.SetActive(false);
                button2.SetActive(false);

                Hacks.SpriteRendererAlpha(pointer, 0f);
                pointer.SetActive(true);
                Hacks.SpriteRendererAlpha(pointer2, 0f);
                pointer2.SetActive(true);
                Hacks.TextAlpha(offline1, 0f);
                offline1.SetActive(true);
                Hacks.TextAlpha(offline2, 0f);
                offline2.SetActive(true);
                Hacks.TextAlpha(online1, 0f);
                online1.SetActive(true);
                Hacks.TextAlpha(online2, 0f);
                online2.SetActive(true);

                transition = 0f;
            }

        }
        else if (phase == 2)
        {

            if (transition < 1f)
            {

                transition += Time.deltaTime*5f;

                Hacks.SpriteRendererAlpha(pointer, transition * pointer.GetComponent<SpriteRenderer>().color.a);
                Hacks.SpriteRendererAlpha(pointer2, transition * pointer2.GetComponent<SpriteRenderer>().color.a);
                Hacks.TextAlpha(offline1, transition);
                Hacks.TextAlpha(offline2, transition);
                Hacks.TextAlpha(online1, transition);
                Hacks.TextAlpha(online2, transition);

                if (transition >= 1f)
                {
                    transition = 1f;
                }

            }

            if (Hacks.ControllerAnyConnected())
            {
                // CONTROLLER PLUGGED
                if (lastDPadY != Input.GetAxis("DPad2") && Input.GetAxis("DPad2") != 0)
                {
                    if (Input.GetAxis("DPad2") == 1)
                    {
                        playOption--;
                        if (playOption < 0) { playOption = 1; }
                    }
                    else if (Input.GetAxis("DPad2") == -1)
                    {
                        playOption++;
                        if (playOption > 1) { playOption = 0; }
                    }
                }
                lastDPadY = Input.GetAxis("DPad2");

                if (Input.GetKeyDown("joystick button 0"))
                {
                    if (playOption == 0)
                    {
                        phase = 5;
                        transition = 0f;
                        menuOk.Play();
                    }
                    else if (playOption == 1)
                    {
                        phase = 3;
                        transition = 0f;
                        menuOk.Play();
                    }
                }
            }
            else
            {
                // CONTROLLER NOT PLUGGED
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playOption--;
                    if (playOption < 0) { playOption = 1; }
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    playOption++;
                    if (playOption > 1) { playOption = 0; }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (playOption == 0)
                    {
                        phase = 5;
                        transition = 0f;
                        menuOk.Play();
                    }
                    else if (playOption == 1)
                    {
                        phase = 3;
                        transition = 0f;
                        menuOk.Play();
                    }
                }
            }

            if (playOption == 0)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -3.56f, Time.deltaTime * 10f), 0f);
                pointer2.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer2.transform.position.y, -3.6f, Time.deltaTime * 10f), 0f);
            }
            else if (playOption == 1)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -5.46f, Time.deltaTime * 10f), 0f);
                pointer2.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer2.transform.position.y, -5.5f, Time.deltaTime * 10f), 0f);
            }

        }
        else if (phase == 3)
        {

            if (transition < 1f)
            {
                transition += Time.deltaTime*5f;

                Hacks.TextAlpha(offline1, 1f - transition);
                Hacks.TextAlpha(offline2, 1f - transition);
                Hacks.TextAlpha(online1, 1f - transition);
                Hacks.TextAlpha(online2, 1f - transition);

                if (transition >= 1f)
                {
                    offline1.SetActive(false);
                    offline2.SetActive(false);
                    online1.SetActive(false);
                    online2.SetActive(false);

                    Hacks.TextAlpha(host1, 0f);
                    host1.SetActive(true);

                    Hacks.TextAlpha(host2, 0f);
                    host2.SetActive(true);

                    Hacks.TextAlpha(join1, 0f);
                    join1.SetActive(true);

                    Hacks.TextAlpha(join2, 0f);
                    join2.SetActive(true);

                    phase = 4;
                    transition = 0f;
                    playOption = 0;
                }
            }

            

        }
        else if (phase == 4)
        {

            if (transition < 1f)
            {
                transition += Time.deltaTime*5f;

                Hacks.TextAlpha(host1, transition);
                Hacks.TextAlpha(host2, transition);
                Hacks.TextAlpha(join1, transition);
                Hacks.TextAlpha(join2, transition);

                if (transition >= 1f)
                {
                    transition = 1f;
                }
            }

                if (Hacks.ControllerAnyConnected())
            {
                // CONTROLLER PLUGGED
                if (lastDPadY != Input.GetAxis("DPad2") && Input.GetAxis("DPad2") != 0)
                {
                    if (Input.GetAxis("DPad2") == 1)
                    {
                        playOption--;
                        if (playOption < 0) { playOption = 1; }
                    }
                    else if (Input.GetAxis("DPad2") == -1)
                    {
                        playOption++;
                        if (playOption > 1) { playOption = 0; }
                    }
                }
                lastDPadY = Input.GetAxis("DPad2");

                if (Input.GetKeyDown("joystick button 0"))
                {
                    if (playOption == 0)
                    {
                        GlobalData.hosting = false;
                    }
                    else if (playOption == 1)
                    {
                        GlobalData.hosting = true;
                    }
                    GlobalData.online = true;
                    phase = 5;
                    transition = 0f;
                    menuOk.Play();
                }
            }
            else
            {
                // CONTROLLER NOT PLUGGED
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playOption--;
                    if (playOption < 0) { playOption = 1; }
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    playOption++;
                    if (playOption > 1) { playOption = 0; }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (playOption == 0)
                    {
                        GlobalData.hosting = false;
                    }
                    else if (playOption == 1)
                    {
                        GlobalData.hosting = true;
                    }
                    GlobalData.online = true;
                    phase = 5;
                    transition = 0f;
                    menuOk.Play();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    phase = 1;
                    menuBack.Play();
                    join1.SetActive(false);
                    join2.SetActive(false);
                    host1.SetActive(false);
                    host2.SetActive(false);
                }
            }

            if (playOption == 0)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -3.56f, Time.deltaTime * 10f), 0f);
                pointer2.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer2.transform.position.y, -3.6f, Time.deltaTime * 10f), 0f);
            }
            else if (playOption == 1)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -5.46f, Time.deltaTime * 10f), 0f);
                pointer2.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer2.transform.position.y, -5.5f, Time.deltaTime * 10f), 0f);
            }

        }
        else if (phase == 5)
        {

            // CHARACTER SELECTION

            if (transition < 1f)
            {
                transition += Time.deltaTime*2f;
                fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, transition);
                intro.volume = 1f - transition;

                if (transition >= 1f)
                {
                    Application.LoadLevel("Preparation");
                }
            }

        }

    }

}
