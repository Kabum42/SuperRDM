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
    private AudioSource preparation;
    private float transition = 0f;
    private int phase = 0;
    private selectableAgent[] selectables = new selectableAgent[6];
    private Vector3 lastMousePosition;
    public GameObject playBackground;
    public GameObject playText;
    public GameObject playText2;

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
        fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.g, 1f);

        intro = gameObject.AddComponent<AudioSource>();
        intro.clip = Resources.Load("Music/Intro") as AudioClip;
        intro.volume = 0f;
        intro.loop = true;
        intro.Play();

        preparation = gameObject.AddComponent<AudioSource>();
        preparation.clip = Resources.Load("Music/Preparation") as AudioClip;
        preparation.volume = 1f;
        preparation.loop = true;
        

        logoPosition = logo.transform.position;
        //logo.GetComponent<SpriteRenderer>().color = new Color(logo.GetComponent<SpriteRenderer>().color.r, logo.GetComponent<SpriteRenderer>().color.g, logo.GetComponent<SpriteRenderer>().color.b, 0f);

        playBackground.SetActive(false);
        playText.SetActive(false);
        playText2.SetActive(false);


	}
	
	// Update is called once per frame
	void Update () {

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
                fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.g, 1f -transition);

            }

            radius += Time.deltaTime * 100f;
            if (radius > 360f) { radius -= 360f; }

            logo.transform.localPosition = logoPosition + new Vector3(0f, Mathf.Sin(radius * Mathf.PI / 180) * 0.25f, 0f);

            radius2 += Time.deltaTime * 100f;
            if (radius2 > 360f) { radius2 -= 360f; }

            startText.GetComponent<TextMesh>().color = new Color(startText.GetComponent<TextMesh>().color.r, startText.GetComponent<TextMesh>().color.g, startText.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
            startText2.GetComponent<TextMesh>().color = new Color(startText2.GetComponent<TextMesh>().color.r, startText2.GetComponent<TextMesh>().color.g, startText2.GetComponent<TextMesh>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));

            button1.GetComponent<SpriteRenderer>().color = new Color(button1.GetComponent<SpriteRenderer>().color.r, button1.GetComponent<SpriteRenderer>().color.g, button1.GetComponent<SpriteRenderer>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));
            button2.GetComponent<SpriteRenderer>().color = new Color(button2.GetComponent<SpriteRenderer>().color.r, button2.GetComponent<SpriteRenderer>().color.g, button2.GetComponent<SpriteRenderer>().color.b, Mathf.Abs(Mathf.Sin(radius2 * Mathf.PI / 180)));


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
                    }
                }
                if (GlobalData.OS == "Mac")
                {
                    // MAC
                    if (Input.GetKeyDown("joystick button 16"))
                    {
                        transition = 0f;
                        phase = 1;
                    }
                }
                if (GlobalData.OS == "Linux")
                {
                    // LINUX
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        transition = 0f;
                        phase = 1;
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
                }

            }

        }
        else if (phase == 1)
        {

            if (transition < 1f)
            {
                transition += Time.deltaTime;
                fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, transition);
                intro.volume = 1f - transition;

                if (transition >= 1f)
                {
                    phase = 2;

                    logo.SetActive(false);
                    startText.SetActive(false);
                    startText2.SetActive(false);
                    button1.SetActive(false);
                    button2.SetActive(false);
                    transition = 0f;

                    playBackground.SetActive(true);
                    playText.SetActive(true);
                    playText2.SetActive(true);

                    intro.Stop();
                    preparation.Play();

                    for (int i = 0; i < selectables.Length; i++)
                    {
                        selectables[i] = new selectableAgent(i);
                    }

                }
            }

        }
        else if (phase == 2)
        {
            if (transition < 1f)
            {
                transition += Time.deltaTime;
                if (transition > 1f)
                {
                    transition = 1f;
                }
                //agent1_pictureHolder.GetComponent<SpriteRenderer>().color = new Color(agent1_pictureHolder.GetComponent<SpriteRenderer>().color.r, agent1_pictureHolder.GetComponent<SpriteRenderer>().color.g, agent1_pictureHolder.GetComponent<SpriteRenderer>().color.b, transition);
                preparation.volume = transition;

                fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, 1f - transition);

            }
            else if (ClickedOn(playBackground))
            {
                phase = 3;
                transition = 0f;
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

                if (GlobalData.OS == "Windows")
                {
                    // WINDOWS
                    if (lastDPadX != Input.GetAxis("DPad1") && Input.GetAxis("DPad1") != 0 && selectableY != 6 && selectables[selectableY].status == "opened")
                    {
                        if (Input.GetAxis("DPad1") == 1)
                        {
                            selectableX += 1;
                            if (selectableX > 2) { selectableX = 0; }
                            // TEMPORARY (?)
                            if (selectableX == 1) { selectableX = 2; }
                        }
                        else if (Input.GetAxis("DPad1") == -1)
                        {
                            selectableX -= 1;
                            if (selectableX < 0) { selectableX = 2; }
                            // TEMPORARY (?)
                            if (selectableX == 1) { selectableX = 0; }
                        }
                    }
                    if (lastDPadY != Input.GetAxis("DPad2") && Input.GetAxis("DPad2") != 0)
                    {
                        if (Input.GetAxis("DPad2") == 1)
                        {
                            selectableY -= 1;
                            if (selectableY < 0) { selectableY = 6; }
                        }
                        else if (Input.GetAxis("DPad2") == -1)
                        {
                            selectableY += 1;
                            if (selectableY > 6) { selectableY = 0; }
                        }
                    }

                    lastDPadX = Input.GetAxis("DPad1");
                    lastDPadY = Input.GetAxis("DPad2");

                    //CHECK BUTTON A
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        if (selectableY < 6)
                        {
                            if (selectables[selectableY].status == "closed")
                            {
                                selectables[selectableY].status = "opened";
                            } else
                            {
                                if (selectableX == 0)
                                {
                                    // CHANGE CHAMPION
                                }
                                else if (selectableX == 2)
                                {
                                    selectables[selectableY].status = "closed";
                                }
                            }
                        }
                        else if (selectableY == 6)
                        {
                            phase = 3;
                            transition = 0f;
                        }
                    }

                }

            }

            playBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            if (selectableY == 6)
            {
                playBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
            }

            for (int i = 0; i < selectables.Length; i++)
                {
                 selectables[i].nameBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                 selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                 selectables[i].arrowBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

                if (selectableY == i)
                {
                    if (selectableX == 0 && selectables[i].status == "opened")
                    {
                        selectables[i].nameBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                    else if (selectableX == 1 && selectables[i].status == "opened")
                    {
                        selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                    else if (selectableX == 2 || selectables[i].status == "closed")
                    {
                        selectables[i].arrowBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                }

                if (ClickedOn(selectables[i].arrowBackground))
                {
                    if (selectables[i].status == "closed")
                    {
                        selectables[i].status = "opened";
                    }
                    else if (selectables[i].status == "opened")
                    {
                        selectables[i].status = "closed";
                    }
                }

                if (selectables[i].status == "closed")
                {
                    selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(selectables[i].arrow.transform.localEulerAngles.z, 0f, Time.deltaTime * 10f));
                    selectables[i].nameBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].nameBackground.transform.localScale.x, 0f, Time.deltaTime*10f), 1f, 1f);
                    selectables[i].controllerBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localScale.x, 0f, Time.deltaTime * 10f), 1f, 1f);
                }
                else if (selectables[i].status == "opened")
                {
                    selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(selectables[i].arrow.transform.localEulerAngles.z, 180f, Time.deltaTime * 10f));
                    selectables[i].nameBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].nameBackground.transform.localScale.x, 1f, Time.deltaTime*10f), 1f, 1f);
                    selectables[i].controllerBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localScale.x, 0.45f, Time.deltaTime * 10f), 1f, 1f);
                }

                selectables[i].nameText.GetComponent<TextMesh>().text = selectables[i].currentName;
                selectables[i].nameText2.GetComponent<TextMesh>().text = selectables[i].currentName;

                selectables[i].nameText.GetComponent<TextMesh>().color = new Color(selectables[i].nameText.GetComponent<TextMesh>().color.r, selectables[i].nameText.GetComponent<TextMesh>().color.g, selectables[i].nameText.GetComponent<TextMesh>().color.b, selectables[i].nameBackground.transform.localScale.x);
                selectables[i].nameText2.GetComponent<TextMesh>().color = new Color(selectables[i].nameText2.GetComponent<TextMesh>().color.r, selectables[i].nameText2.GetComponent<TextMesh>().color.g, selectables[i].nameText2.GetComponent<TextMesh>().color.b, selectables[i].nameBackground.transform.localScale.x);
                selectables[i].controllerText.GetComponent<TextMesh>().color = new Color(selectables[i].controllerText.GetComponent<TextMesh>().color.r, selectables[i].controllerText.GetComponent<TextMesh>().color.g, selectables[i].controllerText.GetComponent<TextMesh>().color.b, selectables[i].nameBackground.transform.localScale.x);
                selectables[i].controllerText2.GetComponent<TextMesh>().color = new Color(selectables[i].controllerText2.GetComponent<TextMesh>().color.r, selectables[i].controllerText2.GetComponent<TextMesh>().color.g, selectables[i].controllerText2.GetComponent<TextMesh>().color.b, selectables[i].nameBackground.transform.localScale.x);

                selectables[i].nameBackground.transform.localPosition = new Vector3(selectables[i].nameBackground.GetComponent<SpriteRenderer>().bounds.size.x*0.9f +2.0f, 0.26f, 0.1f);
                selectables[i].controllerBackground.transform.localPosition = new Vector3(selectables[i].nameBackground.GetComponent<SpriteRenderer>().bounds.size.x * 1.78f + selectables[i].controllerBackground.GetComponent<SpriteRenderer>().bounds.size.x + 1.8f, 0.26f, 0.1f);
                selectables[i].arrowBackground.transform.localPosition = new Vector3(3.74f + selectables[i].nameBackground.GetComponent<SpriteRenderer>().bounds.size.x * 1.78f + selectables[i].controllerBackground.GetComponent<SpriteRenderer>().bounds.size.x*1.81f, 0.26f, 0.1f);
                selectables[i].arrow.transform.localPosition = new Vector3(selectables[i].arrowBackground.transform.localPosition.x + 0.00f, 0.26f, 0f);


            }

        }
        else if (phase == 3)
        {

            transition += Time.deltaTime;
            if (transition >= 1f)
            {
                transition = 1f;
                Application.LoadLevel("World");
            }

            preparation.volume = 1f - transition;
            fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, transition);

        }




    }

    private class selectableAgent
    {

        public GameObject root;

        public string status = "closed";
        public string currentLegend = "barbarian";
        public string currentName = "Retired Barbarian";

        public GameObject pictureHolder;
        public GameObject icon;
        public GameObject arrowBackground;
        public GameObject arrow;
        public GameObject nameBackground;
        public GameObject controllerBackground;
        public GameObject nameText;
        public GameObject nameText2;
        public GameObject controllerText;
        public GameObject controllerText2;

        public selectableAgent(int number)
        {
            root = Instantiate(Resources.Load("Prefabs/SelectableAgent") as GameObject);
            root.name = "Agent" + number;
            root.transform.localPosition = new Vector3(root.transform.localPosition.x, (+2.5f -number +0.5f)*2.7f, 0.1f);
            pictureHolder = root.transform.FindChild("PictureHolder").gameObject;
            icon = root.transform.FindChild("Icon").gameObject;
            arrowBackground = root.transform.FindChild("ArrowBackground").gameObject;
            arrow = root.transform.FindChild("Arrow").gameObject;
            nameBackground = root.transform.FindChild("NameBackground").gameObject;
            controllerBackground = root.transform.FindChild("ControllerBackground").gameObject;
            nameText = root.transform.FindChild("NameText").gameObject;
            nameText2 = root.transform.FindChild("NameText2").gameObject;
            controllerText = root.transform.FindChild("ControllerText").gameObject;
            controllerText2 = root.transform.FindChild("ControllerText2").gameObject;

            nameText.GetComponent<TextMesh>().color = new Color(nameText.GetComponent<TextMesh>().color.r, nameText.GetComponent<TextMesh>().color.g, nameText.GetComponent<TextMesh>().color.b, 0f);
            nameText2.GetComponent<TextMesh>().color = new Color(nameText2.GetComponent<TextMesh>().color.r, nameText2.GetComponent<TextMesh>().color.g, nameText2.GetComponent<TextMesh>().color.b, 0f);

            controllerText.GetComponent<TextMesh>().color = new Color(controllerText.GetComponent<TextMesh>().color.r, controllerText.GetComponent<TextMesh>().color.g, controllerText.GetComponent<TextMesh>().color.b, 0f);
            controllerText2.GetComponent<TextMesh>().color = new Color(controllerText2.GetComponent<TextMesh>().color.r, controllerText2.GetComponent<TextMesh>().color.g, controllerText2.GetComponent<TextMesh>().color.b, 0f);

            if (number == 0)
            {
                changeLegend("barbarian");
                status = "opened";
            }
            if (number == 1)
            {
                changeLegend("pilumantic");
                status = "opened";
            }

        }

        public void changeAlpha(float amount)
        {
            pictureHolder.GetComponent<SpriteRenderer>().color = new Color(pictureHolder.GetComponent<SpriteRenderer>().color.r, pictureHolder.GetComponent<SpriteRenderer>().color.g, pictureHolder.GetComponent<SpriteRenderer>().color.b, amount);
            icon.GetComponent<SpriteRenderer>().color = new Color(icon.GetComponent<SpriteRenderer>().color.r, icon.GetComponent<SpriteRenderer>().color.g, icon.GetComponent<SpriteRenderer>().color.b, amount);
            arrowBackground.GetComponent<SpriteRenderer>().color = new Color(arrowBackground.GetComponent<SpriteRenderer>().color.r, arrowBackground.GetComponent<SpriteRenderer>().color.g, arrowBackground.GetComponent<SpriteRenderer>().color.b, amount);
            arrow.GetComponent<SpriteRenderer>().color = new Color(arrow.GetComponent<SpriteRenderer>().color.r, arrow.GetComponent<SpriteRenderer>().color.g, arrow.GetComponent<SpriteRenderer>().color.b, amount);
            nameBackground.GetComponent<SpriteRenderer>().color = new Color(nameBackground.GetComponent<SpriteRenderer>().color.r, nameBackground.GetComponent<SpriteRenderer>().color.g, nameBackground.GetComponent<SpriteRenderer>().color.b, amount);
            controllerBackground.GetComponent<SpriteRenderer>().color = new Color(controllerBackground.GetComponent<SpriteRenderer>().color.r, controllerBackground.GetComponent<SpriteRenderer>().color.g, controllerBackground.GetComponent<SpriteRenderer>().color.b, amount);
            if (status == "opened")
            {
                nameText.GetComponent<TextMesh>().color = new Color(nameText.GetComponent<TextMesh>().color.r, nameText.GetComponent<TextMesh>().color.g, nameText.GetComponent<TextMesh>().color.b, amount);
                nameText2.GetComponent<TextMesh>().color = new Color(nameText2.GetComponent<TextMesh>().color.r, nameText2.GetComponent<TextMesh>().color.g, nameText2.GetComponent<TextMesh>().color.b, amount);
                controllerText.GetComponent<TextMesh>().color = new Color(controllerText.GetComponent<TextMesh>().color.r, controllerText.GetComponent<TextMesh>().color.g, controllerText.GetComponent<TextMesh>().color.b, amount);
                controllerText2.GetComponent<TextMesh>().color = new Color(controllerText2.GetComponent<TextMesh>().color.r, controllerText2.GetComponent<TextMesh>().color.g, controllerText2.GetComponent<TextMesh>().color.b, amount);
            }
        }

        public void changeLegend(string newLegend)
        {
            currentLegend = newLegend;
            if (currentLegend == "barbarian") { currentName = "Retired Barbarian"; }
            if (currentLegend == "pilumantic") { currentName = "X, the Pilumantic"; }
            icon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Legends/"+currentLegend);
        }

    }


    private bool ClickedOn(GameObject target)
    {

        if (Input.GetMouseButtonDown(0))
        {

            lastMousePosition = Input.mousePosition;

        }
        else if (Input.GetMouseButtonUp(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(lastMousePosition);

            // CLICKABLE MASK
            RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, 0f, LayerMask.GetMask("Clickable"));

            for (int i = 0; i < hits.Length; i++)
            {
                Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit2D[] hits2 = Physics2D.RaycastAll(new Vector2(ray2.origin.x, ray2.origin.y), Vector2.zero, 0f, LayerMask.GetMask("Clickable"));

                for (int j = 0; j < hits2.Length; j++)
                {

                    if (j < hits.Length)
                    {
                        if (hits[j].collider.gameObject == hits2[j].collider.gameObject && hits[j].collider.gameObject == target) { return true; }
                    }

                }

            }


        }

        return false;

    }

}
