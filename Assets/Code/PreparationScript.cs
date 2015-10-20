using UnityEngine;
using System.Collections;

public class PreparationScript : MonoBehaviour {

    private GameObject fading;

    private AudioSource preparation;
    private float transition = 0f;
    private int phase = 6;
    private selectableAgent[] selectables = new selectableAgent[6];
    private characterScroll[] characters = new characterScroll[8];
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
    private GameObject scrollInfo;
    private bool starting = false;

    private static Color otherColor = new Color(123f / 255f, 150f / 255f, 229f / 255f);
    private static Color youColor = new Color(144f / 255f, 229f / 255f, 123f / 255f);

    private float radius2 = 300f;

    private int selectableX = 0;
    private int selectableY = 0;

    private float lastDPadX = 0f;
    private float lastDPadY = 0f;

    // Use this for initialization
    void Start () {

        if (!GlobalData.started) { GlobalData.Start(); }

        fading = GameObject.Find("Fading");
        Hacks.SpriteRendererAlpha(fading, 1f); 

        star = GameObject.Find("Star");
        Hacks.SpriteRendererAlpha(star, 0f);

        support = GameObject.Find("Support");
        Hacks.SpriteRendererAlpha(support, 0f);

        background2 = GameObject.Find("Background2");

        scrollInfo = GameObject.Find("Scroll/Info");

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

        int current_x = 0;
        int current_y = 0;
        int max_per_x = 4;
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = new characterScroll();
            characters[i].root = Instantiate(Resources.Load("Prefabs/ScrollOption") as GameObject);
            characters[i].root.transform.parent = GameObject.Find("Scroll").transform;
            characters[i].root.transform.localPosition = new Vector3((-1.5f +current_x)*0.002f, -0.0018f + (-current_y*0.0018f), -0.2f);


            if (i == 0) { characters[i].champion = "random"; }
            else if (i == 1) { characters[i].champion = "barbarian"; }
            else if (i == 2) { characters[i].champion = "pilumantic"; }
            else if (i == 3) { characters[i].champion = "henmancer"; }
            else if (i == 4) { characters[i].champion = "dreamwalker"; }
            else if (i == 5) { characters[i].champion = "disembodied"; }
            else if (i == 6) { characters[i].champion = "buckler"; }
            else { characters[i].root.SetActive(false); }

            characters[i].root.transform.FindChild("Icon").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Legends/"+characters[i].champion);

            current_x++;
            if (current_x >= max_per_x)
            {
                current_x = 0;
                current_y++;
            }
        }


        if (GlobalData.online)
        {
            if (GlobalData.hosting)
            {
                NetworkManager.StartServer("test");
            }
            else
            {
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

        bool welcome = false;

        for (int i = 0; i < selectables.Length; i++)
        {
            if (selectables[i].controller == "WaitingPlayer")
            {
                selectables[i].player = player;
                selectables[i].tick.SetActive(false);
                selectables[i].status = "opened";
                selectables[i].controller = "Player";
                welcome = true;
                break;
            }
        }

        if (welcome)
        {
            updateAllPlayers();
        }
        else
        {
            GetComponent<NetworkView>().RPC("disconnect", RPCMode.All, player);
        }

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
    void championRequest(NetworkPlayer player, string champion)
    {

        for (int i = 1; i < selectables.Length; i++)
        {
            if (int.Parse(selectables[i].player.ToString()) != 0 && selectables[i].player.ToString() == player.ToString())
            {
                selectables[i].changeLegend(champion);
                updatePlayer(i);
            }
        }

    }

    [RPC]
    void disconnect(NetworkPlayer target)
    {

        if (Network.player.ToString() == target.ToString())
        {
            menuBack.Play();
            Network.Disconnect();
        }

    }

    void updatePlayer(int i)
    {
        if (GlobalData.online)
        {
            GetComponent<NetworkView>().RPC("updatePlayerRPC", RPCMode.All, i, selectables[i].player, selectables[i].tick.activeInHierarchy, selectables[i].status, selectables[i].controller, selectables[i].currentLegend);
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
    void updatePlayerRPC(int position, NetworkPlayer nPlayer, bool bTick, string trueStatus, string subjectiveController, string currentLegend)
    {

        selectables[position].player = nPlayer;
        selectables[position].tick.SetActive(bTick);
        selectables[position].status = trueStatus;
        selectables[position].controller = subjectiveController;
        selectables[position].changeLegend(currentLegend);

        bool isServer = false;
        if (int.Parse(Network.player.ToString()) == 0 || !GlobalData.online) { isServer = true; }

        if (subjectiveController == "You" || subjectiveController == "Player")
        {
            // IT'S TRULY SUBJECTIVE
            if (selectables[position].player.ToString() == Network.player.ToString())
            {
                selectables[position].controller = "You";
                selectables[position].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, selectables[position].controllerBackground.GetComponent<SpriteRenderer>().color.a);
                selectables[position].interactBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, selectables[position].interactBackground.GetComponent<SpriteRenderer>().color.a);
                selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Remove");
            }
            else
            {
                selectables[position].controller = "Player";
                selectables[position].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, selectables[position].controllerBackground.GetComponent<SpriteRenderer>().color.a);
                selectables[position].interactBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, selectables[position].interactBackground.GetComponent<SpriteRenderer>().color.a);
                if (isServer) { selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Remove"); }
                else { selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock"); }
            }
            selectables[position].controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Player");
        }
        else
        {
            if (isServer && position >= 3)
            {
                if (selectables[position].status == "opened")
                {
                    selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Remove");
                }
                else
                {
                    selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Add");
                }
            }
            else
            {
                selectables[position].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
            }

            if (selectables[position].controller == "CPU")
            {
                if (selectables[position].status == "opened") { selectables[position].tick.SetActive(true); }
                else { selectables[position].tick.SetActive(false); }
                selectables[position].controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Cpu");
            }
            else if (selectables[position].controller == "WaitingPlayer")
            {
                selectables[position].controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/WaitingPlayer");
                selectables[position].tick.SetActive(false);
            }
        }

    }

    

    // Update is called once per frame
    void Update () {


        radius2 += Time.deltaTime * 100f;

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
                menuOk.Play();
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

            Hacks.SpriteRendererAlpha(playBackground, 1f);
            playBackground.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            if (Hacks.ControllerAnyConnected())
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
                            if (selectables[selectableY].status == "closed" && selectables[selectableY].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
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
                                else if (selectableX == 2 && selectables[selectableY].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Arrow"))
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

                if (selectableY == 6)
                {
                    playBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
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

            for (int i = 0; i < characters.Length; i++)
            {
                // RESET A COLOR DEFAULT
                characters[i].root.transform.FindChild("Icon").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

                if (isOver(characters[i].root))
                {
                    characters[i].root.transform.FindChild("Icon").GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }

                if (ClickedOn(characters[i].root))
                {
                    if (!GlobalData.online) {
                        selectables[0].changeLegend(characters[i].champion);
                        scrollInfo.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/" + characters[i].champion + "_info");
                    }
                    else {
                        for (int j = 0; j < selectables.Length; j++) {
                            if (selectables[j].player.ToString() == Network.player.ToString() && selectables[j].controller == "You")
                            {
                                if (int.Parse(Network.player.ToString()) == 0)
                                {
                                    selectables[j].changeLegend(characters[i].champion);
                                    scrollInfo.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/" + characters[i].champion+"_info");
                                    updatePlayer(j);
                                }
                                else
                                {
                                    GetComponent<NetworkView>().RPC("championRequest", RPCMode.Server, Network.player, characters[i].champion);
                                    scrollInfo.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/" + characters[i].champion + "_info");
                                }
                                
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < selectables.Length; i++) 
            {
                // ICON COLOR
                if (selectables[i].status == "closed")
                {
                    float aux = Mathf.Lerp(selectables[i].icon.GetComponent<SpriteRenderer>().color.r, 0f, Time.deltaTime * 10f);
                    selectables[i].icon.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                }
                else
                {
                    float aux = Mathf.Lerp(selectables[i].icon.GetComponent<SpriteRenderer>().color.r, 1f, Time.deltaTime * 10f);
                    selectables[i].icon.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                }

                // RESET A COLOR DEFAULT
                if (selectables[i].controller == "You")
                {
                    selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, 1f);
                    selectables[i].interactBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, 1f);
                }
                else
                {
                    selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, 1f);
                    selectables[i].interactBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, 1f);
                }

                if (selectableY == i && Hacks.ControllerAnyConnected())
                {
                    if (i <= 1)
                    {
                        
                    }
                    else if (selectableX == 0 && selectables[i].status == "opened")
                    {
                        
                    }
                    else if (selectableX == 1 && selectables[i].status == "opened")
                    {
                        //selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                    else if (selectableX == 2 || selectables[i].status == "closed")
                    {
                        //selectables[i].interactBackground.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
                    }
                }
                else if (!Hacks.ControllerAnyConnected())
                {
                    // COLORES OSCURECIDOS
                    if (isOver(selectables[i].controllerBackground) && (GlobalData.online && int.Parse(Network.player.ToString()) == 0))
                    {
                        if (selectables[i].controller == "You")
                        {
                            selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r*0.7f, youColor.g*0.7f, youColor.b*0.7f, 1f);
                        }
                        else
                        {
                            selectables[i].controllerBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r * 0.7f, otherColor.g * 0.7f, otherColor.b * 0.7f, 1f);
                        }
                    }
                    else if (isOver(selectables[i].interactBackground) && (selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Remove") || selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Add")))
                    {
                        if (selectables[i].controller == "You")
                        {
                            selectables[i].interactBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r * 0.7f, youColor.g * 0.7f, youColor.b * 0.7f, 1f);
                        }
                        else
                        {
                            selectables[i].interactBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r * 0.7f, otherColor.g * 0.7f, otherColor.b * 0.7f, 1f);
                        }
                    }
                }

                if (ClickedOn(selectables[i].interactBackground))
                {
                    if (selectables[i].status == "closed" && selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Add"))
                    {
                        selectables[i].status = "opened";
                        if (GlobalData.online)
                        {
                            selectables[i].controller = "WaitingPlayer";
                        }
                        else
                        {
                            selectables[i].controller = "CPU";
                        }
                        selectables[i].tick.SetActive(true);
                        selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Remove");
                        updatePlayer(i);
                        acceptEffect.Play();
                    }
                    else if (selectables[i].status == "opened" && (selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite == Resources.Load<Sprite>("Menu/Remove")))
                    {
                        if (selectables[i].controller == "You")
                        {
                            menuBack.Play();
                            if (GlobalData.online) { Network.Disconnect(); }
                            Application.LoadLevel("Menu");
                        }
                        if (selectables[i].controller == "Player")
                        {
                            GetComponent<NetworkView>().RPC("disconnect", RPCMode.All, selectables[i].player);
                            selectables[i].controller = "CPU";
                            selectables[i].tick.SetActive(true);
                            selectables[i].changeLegend("random");
                        }
                        if (i >= 3)
                        {
                            selectables[i].status = "closed";
                            selectables[i].tick.SetActive(false);
                            selectables[i].interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Add");
                        }
                        updatePlayer(i);
                        acceptEffect.Play();
                    }
                }
                else if (ClickedOn(selectables[i].controllerBackground) && GlobalData.online)
                {
                    if (selectables[i].controller == "CPU")
                    {
                        selectables[i].controller = "WaitingPlayer";
                        //selectables[i].controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/WaitingPlayer");
                    }
                    else if (selectables[i].controller == "WaitingPlayer")
                    {
                        selectables[i].controller = "CPU";
                        //selectables[i].controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Cpu");
                    }
                    updatePlayer(i);
                }

                if (selectables[i].status == "opened")
                {
                    selectables[i].controllerBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localScale.x, 1.336029f, Time.deltaTime * 10f), 1.336029f, 1.336029f);
                    selectables[i].controllerBackground.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localPosition.x, 3.47f, Time.deltaTime * 10f), selectables[i].controllerBackground.transform.localPosition.y, selectables[i].controllerBackground.transform.localPosition.z);
                    
                    selectables[i].controllerIcon.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].controllerIcon.transform.localPosition.x, 4.57f, Time.deltaTime * 10f), selectables[i].controllerIcon.transform.localPosition.y, selectables[i].controllerIcon.transform.localPosition.z);
                    Hacks.SpriteRendererAlpha(selectables[i].controllerIcon, Mathf.Lerp(selectables[i].controllerIcon.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 10f));

                    selectables[i].interactBackground.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].interactBackground.transform.localPosition.x, 7.55f, Time.deltaTime * 10f), selectables[i].interactBackground.transform.localPosition.y, selectables[i].interactBackground.transform.localPosition.z);
                    selectables[i].interactIcon.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].interactIcon.transform.localPosition.x, 7.78f, Time.deltaTime * 10f), selectables[i].interactIcon.transform.localPosition.y, selectables[i].interactIcon.transform.localPosition.z);
                }
                else
                {
                    selectables[i].controllerBackground.transform.localScale = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localScale.x, 0f, Time.deltaTime * 10f), 1.336029f, 1.336029f);
                    selectables[i].controllerBackground.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].controllerBackground.transform.localPosition.x, 0f, Time.deltaTime * 10f), selectables[i].controllerBackground.transform.localPosition.y, selectables[i].controllerBackground.transform.localPosition.z);

                    selectables[i].controllerIcon.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].controllerIcon.transform.localPosition.x, 0f, Time.deltaTime * 10f), selectables[i].controllerIcon.transform.localPosition.y, selectables[i].controllerIcon.transform.localPosition.z);
                    Hacks.SpriteRendererAlpha(selectables[i].controllerIcon, Mathf.Lerp(selectables[i].controllerIcon.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime * 10f));

                    selectables[i].interactBackground.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].interactBackground.transform.localPosition.x, 4.21f, Time.deltaTime * 10f), selectables[i].interactBackground.transform.localPosition.y, selectables[i].interactBackground.transform.localPosition.z);
                    selectables[i].interactIcon.transform.localPosition = new Vector3(Mathf.Lerp(selectables[i].interactIcon.transform.localPosition.x, 4.44f, Time.deltaTime * 10f), selectables[i].interactIcon.transform.localPosition.y, selectables[i].interactIcon.transform.localPosition.z);
                }

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


    private class characterScroll
    {

        public GameObject root;
        public string champion;

        public characterScroll()
        {

        }
    }

    private class selectableAgent
    {

        public GameObject root;

        public string status = "closed";
        public string currentLegend = "random";
        public string currentName = "Random";
        public string controller = "CPU";
        public NetworkPlayer player;

        public GameObject pictureHolder;
        public GameObject icon;
        public GameObject controllerIcon;
        public GameObject controllerBackground;
        public GameObject interactIcon;
        public GameObject interactBackground;
        public GameObject tick;
        public GameObject crown;

        public selectableAgent(int number)
        {
            root = Instantiate(Resources.Load("Prefabs/SelectableAgent") as GameObject);
            root.name = "Agent" + number;
            root.transform.localPosition = new Vector3(root.transform.localPosition.x, (+2.5f -number +0.5f)*2.7f, 0.1f);
            pictureHolder = root.transform.FindChild("PictureHolder").gameObject;
            icon = root.transform.FindChild("Icon").gameObject;
            controllerIcon = root.transform.FindChild("Controller").gameObject;
            controllerBackground = root.transform.FindChild("ControllerBackground").gameObject;
            interactIcon = root.transform.FindChild("Interact").gameObject;
            interactBackground = root.transform.FindChild("InteractBackground").gameObject;
           
            tick = root.transform.FindChild("PictureHolder/Tick").gameObject;
            tick.SetActive(false);

            crown = root.transform.FindChild("Crown").gameObject;
            crown.SetActive(false);

            controllerBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, controllerBackground.GetComponent<SpriteRenderer>().color.a);
            interactBackground.GetComponent<SpriteRenderer>().color = new Color(otherColor.r, otherColor.g, otherColor.b, interactBackground.GetComponent<SpriteRenderer>().color.a);

            changeLegend("random");

            if (number == 0)
            {
                crown.SetActive(true);
                controller = "You";
                status = "opened";
                interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Remove");
                controllerBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, controllerBackground.GetComponent<SpriteRenderer>().color.a);
                interactBackground.GetComponent<SpriteRenderer>().color = new Color(youColor.r, youColor.g, youColor.b, interactBackground.GetComponent<SpriteRenderer>().color.a);
                controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Player");
            }
            else if (number <= 2)
            {
                status = "opened";
                tick.SetActive(true);
                if (GlobalData.online)
                {
                    controller = "WaitingPlayer";
                    controllerIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/WaitingPlayer");
                    tick.SetActive(false);
                }
                interactIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Menu/Lock");
            }

        }

        public void changeAlpha(float amount)
        {
            pictureHolder.GetComponent<SpriteRenderer>().color = new Color(pictureHolder.GetComponent<SpriteRenderer>().color.r, pictureHolder.GetComponent<SpriteRenderer>().color.g, pictureHolder.GetComponent<SpriteRenderer>().color.b, amount);
            icon.GetComponent<SpriteRenderer>().color = new Color(icon.GetComponent<SpriteRenderer>().color.r, icon.GetComponent<SpriteRenderer>().color.g, icon.GetComponent<SpriteRenderer>().color.b, amount);
            interactBackground.GetComponent<SpriteRenderer>().color = new Color(interactBackground.GetComponent<SpriteRenderer>().color.r, interactBackground.GetComponent<SpriteRenderer>().color.g, interactBackground.GetComponent<SpriteRenderer>().color.b, amount);
            interactIcon.GetComponent<SpriteRenderer>().color = new Color(interactIcon.GetComponent<SpriteRenderer>().color.r, interactIcon.GetComponent<SpriteRenderer>().color.g, interactIcon.GetComponent<SpriteRenderer>().color.b, amount);
            controllerBackground.GetComponent<SpriteRenderer>().color = new Color(controllerBackground.GetComponent<SpriteRenderer>().color.r, controllerBackground.GetComponent<SpriteRenderer>().color.g, controllerBackground.GetComponent<SpriteRenderer>().color.b, amount);
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
                GlobalData.agents[aux] = new MainCharacter(0, "Player1", 175, 100, GlobalData.Classes[0]);
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
