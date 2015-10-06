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
    private GameObject star;
    private GameObject support;
    private AudioSource selectEffect;
    private AudioSource acceptEffect;
    private GameObject pointer;
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

        selectEffect = gameObject.AddComponent<AudioSource>();
        selectEffect.clip = Resources.Load("Sounds/SelectCell") as AudioClip;
        selectEffect.volume = 1f;
        selectEffect.playOnAwake = false;

        acceptEffect = gameObject.AddComponent<AudioSource>();
        acceptEffect.clip = Resources.Load("Sounds/whip_01") as AudioClip;
        acceptEffect.volume = 1f;
        acceptEffect.playOnAwake = false;

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

        playBackground.SetActive(false);
        playText.SetActive(false);
        playText2.SetActive(false);

        for (int i = 0; i < selectables.Length; i++)
        {
            selectables[i] = new selectableAgent(i);
            selectables[i].root.SetActive(false);
        }

	}

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
        GlobalData.connected = true;
        selectables[0].player = Network.player;
        updatePlayer(0);
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
        Debug.Log("Player connected from " + player.ipAddress + ":" + player.port);

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

                selectables[position].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow");
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

    /*
    [RPC]
    void updatePlayers(NetworkPlayer player1, NetworkPlayer player2, NetworkPlayer player3, NetworkPlayer player4, NetworkPlayer player5, NetworkPlayer player6, bool tick1, bool tick2, bool tick3, bool tick4, bool tick5, bool tick6)
    {

        selectables[0].player = player1;
        selectables[1].player = player2;
        selectables[2].player = player3;
        selectables[3].player = player4;
        selectables[4].player = player5;
        selectables[5].player = player6;

        selectables[0].tick.SetActive(tick1);
        selectables[1].tick.SetActive(tick2);
        selectables[2].tick.SetActive(tick3);
        selectables[3].tick.SetActive(tick4);
        selectables[4].tick.SetActive(tick5);
        selectables[5].tick.SetActive(tick6);

        if (int.Parse(Network.player.ToString()) == 0)
        {
            selectables[0].controller = "You";
            selectables[0].controllerText.GetComponent<TextMesh>().color = new Color(1f, 0.35f, 0.35f, selectables[0].controllerText.GetComponent<TextMesh>().color.a);
            selectables[0].controllerText2.GetComponent<TextMesh>().color = new Color(0.65f, 0f, 0f, selectables[0].controllerText2.GetComponent<TextMesh>().color.a);
            selectables[0].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow");
        }
        else
        {
            selectables[0].controller = "Player";
            selectables[0].controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 1f, 0.35f, selectables[0].controllerText.GetComponent<TextMesh>().color.a);
            selectables[0].controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0.65f, 0f, selectables[0].controllerText2.GetComponent<TextMesh>().color.a);
            selectables[0].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
            selectables[0].arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        }

        for (int i = 1; i < selectables.Length; i++)
        {
            if (int.Parse(selectables[i].player.ToString()) != 0)
            {
                if (selectables[i].player.ToString() == Network.player.ToString())
                {
                    selectables[i].controller = "You";
                    selectables[i].controllerText.GetComponent<TextMesh>().color = new Color(1f, 0.35f, 0.35f, selectables[i].controllerText.GetComponent<TextMesh>().color.a);
                    selectables[i].controllerText2.GetComponent<TextMesh>().color = new Color(0.65f, 0f, 0f, selectables[i].controllerText2.GetComponent<TextMesh>().color.a);
                    selectables[i].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow");
                    selectables[i].status = "opened";
                }
                else
                {
                    selectables[i].controller = "Player";
                    selectables[i].controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 1f, 0.35f, selectables[i].controllerText.GetComponent<TextMesh>().color.a);
                    selectables[i].controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0.65f, 0f, selectables[i].controllerText2.GetComponent<TextMesh>().color.a);
                    if (int.Parse(Network.player.ToString()) == 0) { selectables[0].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow"); }
                    else { selectables[i].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock"); selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f); }
                    selectables[i].status = "opened";
                }
            }
            else
            {
                selectables[i].controller = "CPU";
                selectables[i].controllerText.GetComponent<TextMesh>().color = new Color(0.35f, 0.35f, 1f, selectables[i].controllerText.GetComponent<TextMesh>().color.a);
                selectables[i].controllerText2.GetComponent<TextMesh>().color = new Color(0f, 0f, 0.65f, selectables[i].controllerText2.GetComponent<TextMesh>().color.a);
                if (int.Parse(Network.player.ToString()) == 0) { selectables[0].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Arrow");}
                else { selectables[i].arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock"); selectables[i].arrow.transform.localEulerAngles = new Vector3(0f, 0f, 180f); }
            }
        }
    }
    */

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

            Hacks.SpriteRendererAlpha(fading, Mathf.Lerp(fading.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime*5f));

            transition += Time.deltaTime;

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

                transition += Time.deltaTime;

                Hacks.SpriteRendererAlpha(pointer, transition * pointer.GetComponent<SpriteRenderer>().color.a);
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
                    }
                    else if (playOption == 1)
                    {
                        phase = 3;
                        transition = 0f;
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
                    }
                    else if (playOption == 1)
                    {
                        phase = 3;
                        transition = 0f;
                    }
                }
            }

            if (playOption == 0)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -3.58f, Time.deltaTime * 10f), 0f);
            }
            else if (playOption == 1)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -5.48f, Time.deltaTime * 10f), 0f);
            }

        }
        else if (phase == 3)
        {

            if (transition < 1f)
            {
                transition += Time.deltaTime;

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
                transition += Time.deltaTime;

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
                        NetworkManager.RefreshHostList();
                    }
                    else if (playOption == 1)
                    {
                        NetworkManager.StartServer("test");
                    }
                    GlobalData.online = true;
                    phase = 5;
                    transition = 0f;
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
                        NetworkManager.RefreshHostList();
                    }
                    else if (playOption == 1)
                    {
                        NetworkManager.StartServer("test");
                    }
                    GlobalData.online = true;
                    phase = 5;
                    transition = 0f;
                }
            }

            if (playOption == 0)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -3.58f, Time.deltaTime * 10f), 0f);
            }
            else if (playOption == 1)
            {
                pointer.transform.position = new Vector3(-4.36f, Mathf.Lerp(pointer.transform.position.y, -5.48f, Time.deltaTime * 10f), 0f);
            }

        }
        else if (phase == 5)
        {

            // CHARACTER SELECTION

            if (transition < 1f)
            {
                transition += Time.deltaTime;
                fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, transition);
                intro.volume = 1f - transition;

                if (transition >= 1f)
                {
                    phase = 6;

                    logo.SetActive(false);
                    startText.SetActive(false);
                    startText2.SetActive(false);
                    button1.SetActive(false);
                    button2.SetActive(false);
                    pointer.SetActive(false);
                    offline1.SetActive(false);
                    offline2.SetActive(false);
                    online1.SetActive(false);
                    online2.SetActive(false);
                    host1.SetActive(false);
                    host2.SetActive(false);
                    join1.SetActive(false);
                    join2.SetActive(false);
                    background.SetActive(false);
                    transition = 0f;

                    playBackground.SetActive(true);
                    playText.SetActive(true);
                    playText2.SetActive(true);

                    intro.Stop();
                    preparation.Play();

                    for (int i = 0; i < selectables.Length; i++)
                    {
                        selectables[i].root.SetActive(true);
                    }

                }
            }

        }
        else if (phase == 6 && (!GlobalData.online || (GlobalData.online && GlobalData.connected)))
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

            transition += Time.deltaTime;
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
                arrow.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
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
