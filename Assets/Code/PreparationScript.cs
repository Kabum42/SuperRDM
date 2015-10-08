using UnityEngine;
using System.Collections;

public class PreparationScript : MonoBehaviour {

    private GameObject fading;

    private AudioSource preparation;
    private float transition = 0f;
    private int phase = 6;
    private selectableAgent[] selectables = new selectableAgent[6];
    private Vector3 lastMousePosition;
    public GameObject playBackground;
    public GameObject playText;
    public GameObject playText2;
    private GameObject star;
    private GameObject support;
    private AudioSource selectEffect;
    private AudioSource acceptEffect;
    private int playOption = 0;
    private GameObject background2;
    private AudioSource menuOk;
    private AudioSource menuBack;
    private bool starting = false;

    private float radius2 = 300f;

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

        background2 = GameObject.Find("Background2");

        selectEffect = gameObject.AddComponent<AudioSource>();
        selectEffect.clip = Resources.Load("Sounds/SelectCell") as AudioClip;
        selectEffect.volume = 1f;
        selectEffect.playOnAwake = false;

        acceptEffect = gameObject.AddComponent<AudioSource>();
        acceptEffect.clip = Resources.Load("Sounds/whip_01") as AudioClip;
        acceptEffect.volume = 1f;
        acceptEffect.playOnAwake = false;

        preparation = gameObject.AddComponent<AudioSource>();
        preparation.clip = Resources.Load("Music/Menu/Preparation") as AudioClip;
        preparation.volume = 1f;
        preparation.loop = true;
        preparation.Play();

        menuOk = gameObject.AddComponent<AudioSource>();
        menuOk.clip = Resources.Load("Music/Menu/MenuOk") as AudioClip;
        menuOk.volume = 0.85f;

        menuBack = gameObject.AddComponent<AudioSource>();
        menuBack.clip = Resources.Load("Music/Menu/MenuBack") as AudioClip;
        menuBack.volume = 0.85f;

        for (int i = 0; i < selectables.Length; i++)
        {
            selectables[i] = new selectableAgent(i);
        }

        if (GlobalData.online)
        {
            Debug.Log("LOL1");
            if (GlobalData.hosting)
            {
                Debug.Log("LOL2");
                NetworkManager.StartServer("test");
            }
            else
            {
                Debug.Log("LOL3");
                NetworkManager.RefreshHostList();
            }
        }

        

	}


    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        GlobalData.connected = true;
    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            NetworkManager.hostList = MasterServer.PollHostList();
            NetworkManager.JoinServer(NetworkManager.hostList[0]);
        }
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");
        GlobalData.connected = true;
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Application.LoadLevel("Menu");
    }

    void OnPlayerConnected(NetworkPlayer player)
    {

        for (int i = 0; i < selectables.Length; i++)
        {
            if (selectables[i].controller == "CPU")
            {
                selectables[i].player = player;
                selectables[i].tick.SetActive(false);
                selectables[i].status = "opened";
                selectables[i].controller = "Player";
                break;
            }
        }

        updateAllPlayers();

    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {

        for (int i = 0; i < selectables.Length; i++)
        {
            if (selectables[i].player.ToString() == player.ToString())
            {
                selectables[i].controller = "CPU";
                selectables[i].tick.SetActive(true);
                break;
            }
        }

        updateAllPlayers();

    }

    [RPC]
    void startMatch(float seed)
    {
        GlobalData.boardSeed = seed;
        starting = true;
    }

    [RPC]
    void readyRequest(NetworkPlayer player)
    {

        for (int i = 1; i < selectables.Length; i++)
        {
            if (int.Parse(selectables[i].player.ToString()) != 0 && selectables[i].player.ToString() == player.ToString())
            {
                selectables[i].tick.SetActive(true);
                updatePlayer(i);
            }
        }

    }

    [RPC]
    void disconnect(NetworkPlayer target)
    {

        if (Network.player.ToString() == target.ToString())
        {
            Network.Disconnect();
        }

    }

    void updatePlayer(int i)
    {
        if (GlobalData.online)
        {
            GetComponent<NetworkView>().RPC("updatePlayerRPC", RPCMode.All, i, selectables[i].player, selectables[i].tick.activeInHierarchy, selectables[i].status, selectables[i].controller);
        }
    }

    void updateAllPlayers()
    {
        for (int i = 0; i < selectables.Length; i++)
        {
            updatePlayer(i);
        }
    }

    [RPC]
    void updatePlayerRPC(int position, NetworkPlayer nPlayer, bool bTick, string trueStatus, string subjectiveController)
    {

        selectables[position].player = nPlayer;
        selectables[position].tick.SetActive(bTick);
        selectables[position].status = trueStatus;
        selectables[position].controller = subjectiveController;

        bool isServer = false;
        if (int.Parse(Network.player.ToString()) == 0) { isServer = true; }

        if (subjectiveController == "You" || subjectiveController == "Player")
        {
            // IT'S TRULY SUBJECTIVE
            if (selectables[position].player.ToString() == Network.player.ToString())
            {
                selectables[position].controller = "You";
                selectables[position].controllerText.GetComponent<TextMesh>().color = new Color(1f, 0.35f, 0.35f, selectables[position].controllerText.GetComponent<TextMesh>().color.a);
                selectables[position].controllerText2.GetComponent<TextMesh>().color = new Color(0.65f, 0f, 0f, selectables[position].controllerText2.GetComponent<TextMesh>().color.a);

                selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Door");
            }
            else
            {
                selectables[position].controller = "Player";
                selectables[position].controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 1f, 0.35f, selectables[position].controllerText.GetComponent<TextMesh>().color.a);
                selectables[position].controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0.65f, 0f, selectables[position].controllerText2.GetComponent<TextMesh>().color.a);

                if (isServer) { selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Door"); }
                else { selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock"); }
            }
        }
        else
        {
            if (isServer && position >= 3)
            {
                selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow");
            }
            else
            {
                selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
                selectables[position].arrow.transform.eulerAngles = new Vector3(0f, 0f, 180f);
            }
            selectables[position].controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 0.35f, 1f, selectables[position].controllerText.GetComponent<TextMesh>().color.a);
            selectables[position].controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0f, 0.65f, selectables[position].controllerText2.GetComponent<TextMesh>().color.a);
        }

    }

    

    // Update is called once per frame
    void Update () {


        if (phase != 1)
        {
            radius2 += Time.deltaTime * 100f;
        }
        if (radius2 > 360f) { radius2 -= 360f; }

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

        
        if (phase == 6 && (!GlobalData.online || (GlobalData.online && GlobalData.connected)))
        {

            if (int.Parse(Network.player.ToString()) == 0 || !GlobalData.online)
            {
                bool allReady = true;
                for (int i = 0; i < selectables.Length; i++)
                {
                    if (!selectables[i].tick.activeInHierarchy && selectables[i].status == "opened")
                    {
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    if (GlobalData.online)
                    {
                        GetComponent<NetworkView>().RPC("startMatch", RPCMode.All, Random.Range(0f, 9999999f));
                    }
                    else
                    {
                        GlobalData.boardSeed = Random.Range(0f, 9999999f);
                        starting = true;
                    }
                }
            }

            if (starting)
            {
                phase = 7;
                transition = 0f;
            }

            if (transition < 1f)
            {
                transition += Time.deltaTime*2f;
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

                clickOnReady();

            }

            playBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

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
                    if (lastDPadX != Input.GetAxis("DPad1") && Input.GetAxis("DPad1") != 0 && selectableY != 6 && selectableY >= 2 && selectables[selectableY].status == "opened")
                    {
                        if (Input.GetAxis("DPad1") == 1)
                        {
                            selectableX += 1;
                            if (selectableX > 2) { selectableX = 0; }
                            // TEMPORARY (?)
                            if (selectableX == 1) { selectableX = 2; }
                            selectEffect.Play();
                        }
                        else if (Input.GetAxis("DPad1") == -1)
                        {
                            selectableX -= 1;
                            if (selectableX < 0) { selectableX = 2; }
                            // TEMPORARY (?)
                            if (selectableX == 1) { selectableX = 0; }
                            selectEffect.Play();
                        }
                    }
                    if (lastDPadY != Input.GetAxis("DPad2") && Input.GetAxis("DPad2") != 0)
                    {
                        if (Input.GetAxis("DPad2") == 1)
                        {
                            selectableY -= 1;
                            if (selectableY < 0) { selectableY = 6; }
                            selectEffect.Play();
                        }
                        else if (Input.GetAxis("DPad2") == -1)
                        {
                            selectableY += 1;
                            if (selectableY > 6) { selectableY = 0; }
                            selectEffect.Play();
                        }
                    }

                    lastDPadX = Input.GetAxis("DPad1");
                    lastDPadY = Input.GetAxis("DPad2");

                    //CHECK BUTTON A
                    if (Input.GetKeyDown("joystick button 0"))
                    {
                        if (selectableY < 6)
                        {
                            if (selectables[selectableY].status == "closed" && selectables[selectableY].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
                            {
                                selectables[selectableY].status = "opened";
                                selectables[selectableY].tick.SetActive(true);
                                updatePlayer(selectableY);
                                acceptEffect.Play();
                            } else
                            {
                                if (selectableX == 0 || selectableY <= 1)
                                {
                                    // CHANGE CHAMPION
                                }
                                else if (selectableX == 2 && selectables[selectableY].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
                                {
                                    selectables[selectableY].status = "closed";
                                    selectables[selectableY].tick.SetActive(false);
                                    updatePlayer(selectableY);
                                    acceptEffect.Play();
                                }
                            }
                        }
                        else if (selectableY == 6)
                        {
                            clickOnReady();
                        }
                    }

                }

            }
            else
            {
                // CONTROLLER NOT PLUGGED
                if (isOver(playBackground))
                {
                    playBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }

            }

            if (selectableY == 6 && controllerConnected != -1)
            {
                playBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
            }

            for (int i = 0; i < selectables.Length; i++)
                {
                selectables[i].nameBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                selectables[i].arrowBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

                selectables[i].controllerText.GetComponent<TextMesh>().text = selectables[i].controller;
                selectables[i].controllerText2.GetComponent<TextMesh>().text = selectables[i].controller;


                if (selectableY == i && controllerConnected != -1)
                {
                    if (i <= 1)
                    {
                        selectables[i].nameBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                    else if (selectableX == 0 && selectables[i].status == "opened")
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
                else if (controllerConnected == -1)
                {
                    if (isOver(selectables[i].nameBackground))
                    {
                        selectables[i].nameBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                    else if (isOver(selectables[i].arrowBackground) && (selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow") || selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Door")))
                    {
                        selectables[i].arrowBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                }

                if (ClickedOn(selectables[i].arrowBackground))
                {
                    if (selectables[i].status == "closed" && selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
                    {
                        selectables[i].status = "opened";
                        selectables[i].tick.SetActive(true);
                        updatePlayer(i);
                        acceptEffect.Play();
                    }
                    else if (selectables[i].status == "opened" && (selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow") || selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Door")))
                    {
                        if (selectables[i].controller == "You")
                        {
                            if (GlobalData.online) { Network.Disconnect(); }
                            Application.LoadLevel("Menu");
                        }
                        if (selectables[i].controller == "Player")
                        {
                            GetComponent<NetworkView>().RPC("disconnect", RPCMode.All, selectables[i].player);
                            selectables[i].controller = "CPU";
                            selectables[i].tick.SetActive(true);
                        }
                        if (i >= 3)
                        {
                            selectables[i].status = "closed";
                            selectables[i].tick.SetActive(false);
                        }
                        updatePlayer(i);
                        acceptEffect.Play();
                    }
                }

                if (selectables[i].status == "closed")
                {
                    if (selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
                    {
                        selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(selectables[i].arrow.transform.localEulerAngles.z, 0f, Time.deltaTime * 10f));
                    }
                    selectables[i].nameBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].nameBackground.transform.localScale.x, 0f, Time.deltaTime*10f), 1f, 1f);
                    selectables[i].controllerBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localScale.x, 0f, Time.deltaTime * 10f), 1f, 1f);
                }
                else if (selectables[i].status == "opened")
                {
                    if (selectables[i].arrow.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
                    {
                        selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(selectables[i].arrow.transform.localEulerAngles.z, 180f, Time.deltaTime * 10f));
                    }
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
        else if (phase == 7)
        {

            transition += Time.deltaTime*2f;
            if (transition >= 1f)
            {
                transition = 1f;
                toWorld();
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
        public string controller = "CPU";
        public NetworkPlayer player;

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
        public GameObject tick;

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
            tick = root.transform.FindChild("PictureHolder/Tick").gameObject;
            tick.SetActive(false);

            nameText.GetComponent<TextMesh>().color = new Color(nameText.GetComponent<TextMesh>().color.r, nameText.GetComponent<TextMesh>().color.g, nameText.GetComponent<TextMesh>().color.b, 0f);
            nameText2.GetComponent<TextMesh>().color = new Color(nameText2.GetComponent<TextMesh>().color.r, nameText2.GetComponent<TextMesh>().color.g, nameText2.GetComponent<TextMesh>().color.b, 0f);

            controllerText.GetComponent<TextMesh>().color = new Color(controllerText.GetComponent<TextMesh>().color.r, controllerText.GetComponent<TextMesh>().color.g, controllerText.GetComponent<TextMesh>().color.b, 0f);
            controllerText2.GetComponent<TextMesh>().color = new Color(controllerText2.GetComponent<TextMesh>().color.r, controllerText2.GetComponent<TextMesh>().color.g, controllerText2.GetComponent<TextMesh>().color.b, 0f);

            controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 0.35f, 1f, controllerText.GetComponent<TextMesh>().color.a);
            controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0f, 0.65f, controllerText2.GetComponent<TextMesh>().color.a);

            if (number == 0)
            {
                controller = "You";
                controllerText.GetComponent<TextMesh>().color = new Color(1f, 0.35f, 0.35f, controllerText.GetComponent<TextMesh>().color.a);
                controllerText2.GetComponent<TextMesh>().color = new Color(0.65f, 0f, 0f, controllerText2.GetComponent<TextMesh>().color.a);
                changeLegend("barbarian");
                status = "opened";
                arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Door");
                arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
            }
            else if (number <= 2)
            {
                changeLegend("pilumantic");
                status = "opened";
                arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
                arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                tick.SetActive(true);
            }

            controllerText.GetComponent<TextMesh>().text = controller;
            controllerText2.GetComponent<TextMesh>().text = controller;

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

    private void toWorld()
    {

        int aux = 0;

        for (int i = 0; i < selectables.Length; i++)
        {
            if (selectables[i].status == "opened")
            {
                GlobalData.agents[aux] = new MainCharacter();
                if (selectables[i].controller == "CPU")
                {
                    GlobalData.agents[aux].IA = true;
                }
                else
                {
                    GlobalData.agents[aux].player = selectables[i].player;
                    if (GlobalData.agents[aux].player.ToString() == Network.player.ToString())
                    {
                        GlobalData.myAgent = aux;
                    }
                }
                
                aux++;
            }
        }

        GlobalData.activeAgents = aux;
        Application.LoadLevel("World");

    }

    private void clickOnReady()
    {
        selectableAgent myAgent = null;

        if (int.Parse(Network.player.ToString()) == 0 || !GlobalData.online)
        {
            myAgent = selectables[0];
        }
        else
        {
            for (int i = 1; i < selectables.Length; i++)
            {
                if (int.Parse(selectables[i].player.ToString()) != 0 && selectables[i].player.ToString() == Network.player.ToString())
                {
                    myAgent = selectables[i];
                }
            }
        }

        if (!myAgent.tick.activeInHierarchy)
        {
            if (GlobalData.online)
            {
                if (int.Parse(Network.player.ToString()) == 0)
                {
                    // ES EL SERVER
                    selectables[0].tick.SetActive(true);
                    updatePlayer(0);
                }
                else
                {
                    GetComponent<NetworkView>().RPC("readyRequest", RPCMode.Server, Network.player);
                }
            }
            else
            {
                selectables[0].tick.SetActive(true);
            }
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

    private bool isOver(GameObject target)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // CLICKABLE MASK
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, 0f, LayerMask.GetMask("Clickable"));

        for (int i = 0; i < hits.Length; i++)
        {

            if (i < hits.Length)
            {
                if (hits[i].collider.gameObject == hits[i].collider.gameObject && hits[i].collider.gameObject == target) { return true; }
            }

        }

        return false;

    }

}
