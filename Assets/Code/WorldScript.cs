﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldScript : MonoBehaviour {

    public GameObject board;
    public BoardCell[] boardCells;
    private int currentCell = 0;
    private int[] cellsPerRing;

    private float cellWidth = 1.30f;
    private float cellHeight = 1.51f;

    private Vector3 lastMousePosition;
    private BoardCell selected;

    private const int numRings = 3;

    private int phase = 0;
    private float transition = 0f;
    private GameObject fading;
    private AudioSource musicWorld;
    private GameObject selectedSprite;
    private bool usedDijkstra = false;
    private List<BoardCell> unvisited;
    private int[] distances;

    private float lastDPadX = 0f;
    private float lastDPadY = 0f;
    private string lastHorizontal = "down";

    private GameObject[] UIAgents;

    private AudioSource selectCellEffect;

    private int currentMountains = 0;
    private int goalMountains = 0;

    private int currentLakes = 0;
    private int goalLakes = 0;

    private int currentSwamps = 0;
    private int goalSwamps = 0;

    // Use this for initialization
    void Start () {

        if (!GlobalData.started)
        {
            GlobalData.Start();
        }

        //boardCells = new BoardCell[37];

        musicWorld = gameObject.AddComponent<AudioSource>();
        musicWorld.clip = Resources.Load("Music/World") as AudioClip;
        musicWorld.volume = 0f;
        musicWorld.loop = true;
        musicWorld.Play();

        selectCellEffect = gameObject.AddComponent<AudioSource>();
        selectCellEffect.clip = Resources.Load("Sounds/SelectCell") as AudioClip;
        selectCellEffect.volume = 1f;
        selectCellEffect.playOnAwake = false;

        fading = GameObject.Find("Fading");

        selectedSprite = GameObject.Find("SelectedSprite");

        UIAgents = new GameObject[GlobalData.activeAgents];

        int numCells = 1;
        
        cellsPerRing = new int[numRings+1];
        cellsPerRing[0] = 1;

        for (int i = 1; i <= numRings; i++)
        {
            cellsPerRing[i] = 6 * i;
            numCells += 6 * i;
        }

        // POSIBLES SANTUARIOS
        numCells += GlobalData.activeAgents;

        boardCells = new BoardCell[numCells];
        //boardCells = new BoardCell[numCells + GlobalData.activeAgents];

        board = new GameObject();
        board.name = "Board";
        board.transform.position = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < numCells; i++)
        {
            BoardCell b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
            boardCells[i] = b;
            b.positionInArray = i;
        }

        GenerateBoard();
        RandomizeTurns();

        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.agents[i] != null)
            {
                //Debug.Log(i);
                GameObject g = Instantiate(Resources.Load("Prefabs/UIAgent") as GameObject);
                g.name = "UIAgent" + i;
                g.transform.parent = GameObject.Find("UIAgents").transform;
                g.transform.FindChild("PictureHolder").gameObject.GetComponent<SpriteRenderer>().color = GlobalData.colorCharacters[i];
                UIAgents[i] = g;
            }
        }

        
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            AddSanctuary(i, i);
        }

	}

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Application.LoadLevel("Menu");
    }

    [RPC]
    void moveRequest(NetworkPlayer player, int i)
    {

        if (GlobalData.agents[GlobalData.currentAgentTurn].player == player && boardCells[GlobalData.agents[GlobalData.currentAgentTurn].currentCell].isConnected(boardCells[i]))
        {
            GlobalData.agents[GlobalData.currentAgentTurn].currentCell = i;
            NextTurn();
            updateBoard();
        }

    }

    void updateBoard()
    {
        if (GlobalData.online)
        {
            GetComponent<NetworkView>().RPC("updateBoardRPC", RPCMode.Others, GlobalData.currentAgentTurn);
            for (int i = 0; i < GlobalData.activeAgents; i++)
            {
                GetComponent<NetworkView>().RPC("updateAgentRPC", RPCMode.Others, i, GlobalData.agents[i].currentCell);
            }
        }
    }

    [RPC]
    void updateBoardRPC(int currentAgentTurn)
    {
        GlobalData.currentAgentTurn = currentAgentTurn;
    }

    [RPC]
    void updateAgentRPC(int position, int currentCell)
    {
        GlobalData.agents[position].currentCell = currentCell;
    }


    void RandomizeTurns()
    {
        GlobalData.order = new int[GlobalData.activeAgents];

        for (int i = 0; i < GlobalData.order.Length; i++)
        {
            // DEFAULT ORDER
            GlobalData.order[i] = i;
        }

        float startingPerlin = 0.2347234747f;

        for (int i = 0; i < 10; i++)
        {
            // SWAPS
            //int first_element = Random.Range(0, GlobalData.order.Length);
            int first_element = (int) (Mathf.PerlinNoise(startingPerlin, GlobalData.boardSeed) * GlobalData.order.Length);
            startingPerlin += 1.233427424f;
            int second_element = (int) (Mathf.PerlinNoise(startingPerlin, GlobalData.boardSeed) * GlobalData.order.Length);
            startingPerlin += 1.233427424f;
            int aux = GlobalData.order[first_element];
            GlobalData.order[first_element] = GlobalData.order[second_element];
            GlobalData.order[second_element] = aux;
        }

        GlobalData.currentAgentTurn = (int)(Mathf.PerlinNoise(startingPerlin, GlobalData.boardSeed) * GlobalData.order.Length);
        Debug.Log(GlobalData.currentAgentTurn);

    }

    void NextTurn()
    {

        int auxOrder = 0;
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.currentAgentTurn == GlobalData.order[i]) { auxOrder = i; }
        }
        auxOrder++;
        if (auxOrder > GlobalData.activeAgents - 1) { auxOrder = 0; }
        GlobalData.currentAgentTurn = GlobalData.order[auxOrder];
        usedDijkstra = false;
        resetBoardBrightness();

    }

    void Dijkstra()
    {
        usedDijkstra = true;

        distances = new int[boardCells.Length];
        unvisited = new List<BoardCell>();
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            unvisited.Add(boardCells[i]);
        }

        distances[GlobalData.agents[GlobalData.myAgent].currentCell] = 0;
        visitCell(boardCells[GlobalData.agents[GlobalData.myAgent].currentCell]);

        for (int i = 0; i < boardCells.Length; i++)
        {
            if (distances[i] > GlobalData.agents[GlobalData.myAgent].getCurrentSteps())
            {
                boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
            }
        }
    }

    private void visitCell(BoardCell b)
    {

        unvisited.Remove(b);

        // CHANGE DISTANCES
        if (b.northWest != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.northWest.biome] < distances[b.northWest.positionInArray]) { distances[b.northWest.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.northWest.biome]; }
        if (b.north != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.north.biome] < distances[b.north.positionInArray]) { distances[b.north.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.north.biome]; }
        if (b.northEast != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.northEast.biome] < distances[b.northEast.positionInArray]) { distances[b.northEast.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.northEast.biome]; }

        if (b.southWest != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.southWest.biome] < distances[b.southWest.positionInArray]) { distances[b.southWest.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.southWest.biome]; }
        if (b.south != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.south.biome] < distances[b.south.positionInArray]) { distances[b.south.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.south.biome]; }
        if (b.southEast != null && distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.southEast.biome] < distances[b.southEast.positionInArray]) { distances[b.southEast.positionInArray] = distances[b.positionInArray] + GlobalData.biomeCosts[(int)b.southEast.biome]; }
        

        // VISIT NEIGHBOURS
        if (unvisited.Count > 0)
        {
            // NORTH
            if (b.northWest != null) {
                if (unvisited.Contains(b.northWest))
                {
                    visitCell(b.northWest); 
                }
            }

            if (b.north != null) { 
                if (unvisited.Contains(b.north)) {
                    visitCell(b.north); 
                }
            }
            
            if (b.northEast != null) { 
                if (unvisited.Contains(b.northEast)) {
                    visitCell(b.northEast); 
                }
            }

            // SOUTH
            if (b.southWest != null) {
                if (unvisited.Contains(b.southWest))
                {
                    visitCell(b.southWest); 
                }
            }

            if (b.south != null) {
                if (unvisited.Contains(b.south))
                {
                    visitCell(b.south); 
                }
            }

            if (b.southEast != null) { 
                if (unvisited.Contains(b.southEast)) {
                    visitCell(b.southEast); 
                }
            }

        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (GlobalData.currentAgentTurn == GlobalData.myAgent && !usedDijkstra)
        {
            Dijkstra();
        }

        if (Input.GetKey(KeyCode.Return))
        {
            GlobalData.boardSeed = Random.Range(0f, 100f);
            GenerateBoard();
        }

        if (Input.GetKeyDown(KeyCode.Space) && GlobalData.currentAgentTurn == GlobalData.myAgent)
        {

            //NextTurn();
            
        }

        // Place Champions
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            GlobalData.agents[i].cellChampion.transform.position = new Vector3(Mathf.Lerp(GlobalData.agents[i].cellChampion.transform.position.x, boardCells[GlobalData.agents[i].currentCell].root.transform.position.x, Time.deltaTime * 10f), Mathf.Lerp(GlobalData.agents[i].cellChampion.transform.position.y, boardCells[GlobalData.agents[i].currentCell].root.transform.position.y, Time.deltaTime * 10f), GlobalData.agents[i].cellChampion.transform.position.z);
        }

        // Place UI Agents
        float default_y = 1.85f;
        float total_y = (GlobalData.activeAgents-1) * default_y  +(default_y+1f);
        float current_y = 0f;

        for (int i = 0; i < GlobalData.order.Length; i++)
        {
            int j = GlobalData.order[i];

            if (GlobalData.currentAgentTurn == GlobalData.order[i])
            {
                current_y += 0.5f;
            }

            UIAgents[j].transform.localPosition = new Vector3(9.64f, Mathf.Lerp(UIAgents[j].transform.localPosition.y, (total_y - 1) / 2f - current_y, Time.deltaTime*10f), 0f);

            if (GlobalData.currentAgentTurn == GlobalData.order[i])
            {
                UIAgents[j].transform.localScale = new Vector3(Mathf.Lerp(UIAgents[j].transform.localScale.x, 1.62f, Time.deltaTime * 10f), Mathf.Lerp(UIAgents[j].transform.localScale.y, 1.62f, Time.deltaTime * 10f), Mathf.Lerp(UIAgents[j].transform.localScale.z, 1.62f, Time.deltaTime * 10f));
                current_y += default_y + 0.5f;
            }
            else
            {
                UIAgents[j].transform.localScale = new Vector3(Mathf.Lerp(UIAgents[j].transform.localScale.x, 1f, Time.deltaTime * 10f), Mathf.Lerp(UIAgents[j].transform.localScale.y, 1f, Time.deltaTime * 10f), Mathf.Lerp(UIAgents[j].transform.localScale.z, 1f, Time.deltaTime * 10f));
                current_y += default_y;
            }
            
        }

        //Debug.Log(boardCells[1].south);
        //boardCells[1].south.root.transform.position = boardCells[1].south.root.transform.position + new Vector3(0f, 0.02f, 0f);

        if (GlobalData.agents[GlobalData.currentAgentTurn].IA && (int.Parse(Network.player.ToString()) == 0 || !GlobalData.online))
        {
            NextTurn();
            updateBoard();
        }

        if (phase == 0)
        {
            // QUITANDO EL FADING
            transition += Time.deltaTime;
            if (transition >= 1f)
            {
                transition = 1f;
                fading.SetActive(false);
                phase = 1;
            }

            musicWorld.volume = transition*0.7f;
            fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, 1f - transition);

        }
        else if (phase == 1)
        {
            // TIME TO SELECT YOUR SANCTUARY
            phase = 2;
        }
        else
        {


            if (Hacks.ControllerAnyConnected())
            {
                // CONTROLLER PLUGGED
                if (!selectedSprite.activeInHierarchy)
                {
                    selected = boardCells[0];
                    selectedSprite.SetActive(true);
                }

                if (GlobalData.OS == "Windows")
                {
                    // WINDOWS
                    if (lastDPadX != Input.GetAxis("DPad1") && Input.GetAxis("DPad1") != 0)
                    {
                        if (Input.GetAxis("DPad1") == -1)
                        {
                            // REGISTRADO INPUT DPAD-X -1
                            if (lastHorizontal == "down")
                            {
                                if (selected.northWest != null)
                                {
                                    selected = selected.northWest;
                                    lastHorizontal = "up";
                                    selectCellEffect.Play();
                                }
                                else if (selected.southWest != null)
                                {
                                    selected = selected.southWest;
                                    selectCellEffect.Play();
                                }
                            }
                            else
                            {
                                if (selected.southWest != null)
                                {
                                    selected = selected.southWest;
                                    lastHorizontal = "down";
                                    selectCellEffect.Play();
                                }
                                else if (selected.northWest != null)
                                {
                                    selected = selected.northWest;
                                    selectCellEffect.Play();
                                }
                            }
                        }
                        else if (Input.GetAxis("DPad1") == 1)
                        {
                            // REGISTRADO INPUT DPAD-X +1
                            if (lastHorizontal == "down")
                            {
                                if (selected.northEast != null)
                                {
                                    selected = selected.northEast;
                                    lastHorizontal = "up";
                                    selectCellEffect.Play();
                                }
                                else if (selected.southEast != null)
                                {
                                    selected = selected.southEast;
                                    selectCellEffect.Play();
                                }
                            }
                            else
                            {
                                if (selected.southEast != null)
                                {
                                    selected = selected.southEast;
                                    lastHorizontal = "down";
                                    selectCellEffect.Play();
                                }
                                else if (selected.northEast != null)
                                {
                                    selected = selected.northEast;
                                    selectCellEffect.Play();
                                }
                            }
                        }
                    }
                    if (lastDPadY != Input.GetAxis("DPad2") && Input.GetAxis("DPad2") != 0)
                    {
                        if (Input.GetAxis("DPad2") == -1)
                        {
                            // REGISTRADO INPUT DPAD-Y -1
                            if (selected.south != null)
                            {
                                selected = selected.south;
                                selectCellEffect.Play();
                            }
                            else if (selected.southEast != null)
                            {
                                selected = selected.southEast;
                                selectCellEffect.Play();
                            }
                            else if (selected.southWest != null)
                            {
                                selected = selected.southWest;
                                selectCellEffect.Play();
                            }
                        }
                        else if (Input.GetAxis("DPad2") == 1)
                        {
                            // REGISTRADO INPUT DPAD-Y +1
                            if (selected.north != null)
                            {
                                selected = selected.north;
                                selectCellEffect.Play();
                            }
                            else if (selected.northEast != null)
                            {
                                selected = selected.northEast;
                                selectCellEffect.Play();
                            }
                            else if (selected.northWest != null)
                            {
                                selected = selected.northWest;
                                selectCellEffect.Play();
                            }
                        }
                    }

                    lastDPadX = Input.GetAxis("DPad1");
                    lastDPadY = Input.GetAxis("DPad2");
                }
                /*
                if (GlobalData.OS == "Mac")
                {
                    // MAC
                    // NOT IMPLEMENTED
                }
                if (GlobalData.OS == "Linux")
                {
                    // LINUX
                    DPadX = Input.GetAxis("DPad2");
                    DPadY = Input.GetAxis("DPad3");
                }
                */



            }
            else
            {
                // CONTROLLER NOT PLUGGED
                selectedSprite.SetActive(false);

                for (int i = 0; i < boardCells.Length; i++)
                {
                    if (isOver(boardCells[i].root))
                    {
                        selectedSprite.SetActive(true);
                        selected = boardCells[i];
                    }

                    if (GlobalData.currentAgentTurn == GlobalData.myAgent && ClickedOn(boardCells[i].root) && boardCells[GlobalData.agents[GlobalData.myAgent].currentCell].isConnected(boardCells[i]))
                    {
                        // MOVE TO THE CELL??
                        if (GlobalData.online)
                        {
                            if (int.Parse(Network.player.ToString()) == 0)
                            {
                                // ES EL SERVER
                                GlobalData.agents[GlobalData.myAgent].currentCell = i;
                                NextTurn();
                                updateBoard();
                            }
                            else
                            {
                                // ES UN CLIENTE
                                GetComponent<NetworkView>().RPC("moveRequest", RPCMode.Server, Network.player, i);
                            }
                        }
                        else
                        {
                            GlobalData.agents[GlobalData.myAgent].currentCell = i;
                            NextTurn();
                        }
                    }
                }

            }

            if (selected != null)
            {
                selectedSprite.transform.position = new Vector3(selected.root.transform.position.x, selected.root.transform.position.y, selectedSprite.transform.position.z);
            }

        }


    }

    void AddSanctuary(int agent, int num)
    {
        BoardCell b = boardCells[currentCell];
        b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
        b.ring = 4;
        b.changeBiome(Biome.Sanctuary);
        b.root.GetComponent<SpriteRenderer>().color = GlobalData.colorCharacters[agent];
        b.root.transform.parent = board.transform;
        b.root.name = "Cell_" + 4 + "_" + num;

        GlobalData.agents[agent].currentCell = currentCell;
        GlobalData.agents[agent].cellChampion = Instantiate(Resources.Load("Prefabs/Champion")) as GameObject;
        GlobalData.agents[agent].cellChampion.name = "Champion_" + agent;
        GlobalData.agents[agent].cellChampion.transform.parent = GameObject.Find("Champions").transform;

        if (num == 0) {
            positionCell(b, 2, 3);
            connectCells(b, "southWest", boardCells[20]);
            connectCells(b, "south", boardCells[21]);
        }
        else if (num == 1) {
            positionCell(b, 4, 0);
            connectCells(b, "northWest", boardCells[23]);
            connectCells(b, "southWest", boardCells[24]);
        }
        else if (num == 2)
        {
            positionCell(b, 2, -3);
            connectCells(b, "north", boardCells[26]);
            connectCells(b, "northWest", boardCells[27]);
        }
        else if (num == 3)
        {
            positionCell(b, -2, -3);
            connectCells(b, "northEast", boardCells[29]);
            connectCells(b, "north", boardCells[30]);
        }
        else if (num == 4)
        {
            positionCell(b, -4, 0);
            connectCells(b, "southEast", boardCells[32]);
            connectCells(b, "northEast", boardCells[33]);
        }
        else if (num == 5)
        {
            positionCell(b, -2, 3);
            connectCells(b, "south", boardCells[35]);
            connectCells(b, "southEast", boardCells[36]);
        }

        GlobalData.agents[agent].cellChampion.transform.position = new Vector3(boardCells[GlobalData.agents[agent].currentCell].root.transform.position.x, boardCells[GlobalData.agents[agent].currentCell].root.transform.position.y, GlobalData.agents[agent].cellChampion.transform.position.z);

        currentCell++;

    }

    void GenerateBoard()
    {

        currentCell = 0;
        currentMountains = 0;
        currentLakes = 0;
        currentSwamps = 0;

        // 2-3 PLAYERS : 1 MOUNTAIN
        // 4-5 PLAYERS : 1-2 MOUNTAINS
        // 6 PLAYERS : 1-3 MOUNTAINS
        goalMountains = 1 + (int) Mathf.Floor((Mathf.PerlinNoise(0.1f, GlobalData.boardSeed) * (GlobalData.activeAgents / 2 )));
        // 2-4 LAKES
        goalLakes = 2 + (int)Mathf.Floor((Mathf.PerlinNoise(3.1f, GlobalData.boardSeed) * 3f));
        // 5-6 SWAMPS
        goalSwamps = 5 + (int)Mathf.Floor((Mathf.PerlinNoise(6.1f, GlobalData.boardSeed) * 2f));

        // RINGS
        for (int i = 0; i <= numRings; i++)
        {
            if (i == 0) { makeRing(i); }
            else if (i == 1)
            {
                makeRing(i);

                float startingX = 10.2f;

                while (currentMountains < goalMountains)
                {

                    int target = 1 + (int)Mathf.Floor((Mathf.PerlinNoise(startingX, GlobalData.boardSeed) * 6f));
                    if (boardCells[target].biome == Biome.Desert)
                    {
                        boardCells[target].changeBiome(Biome.Mountain);
                        currentMountains++;
                    }

                    startingX += 1.103903918344755788f;

                }

            }
            else if (i == 2) 
            { 
                makeRing(i);

                float startingX = 110.3f;

                while (currentLakes < goalLakes)
                {

                    int target = 7 + (int)Mathf.Floor((Mathf.PerlinNoise(startingX, GlobalData.boardSeed) * 12f));
                    if (boardCells[target].biome == Biome.Forest)
                    {
                        boardCells[target].changeBiome(Biome.Lake);
                        currentLakes++;
                    }

                    startingX += 1.21313843078579875f;

                }
                
            }
            else if (i == 3) 
            { 
                makeRing(i);

                float doublePrairie = 20f;

                // NORTH-EAST SANCTUARY
                if (Mathf.PerlinNoise(210.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[20].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(213.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[21].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[21].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(216.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[20].changeBiome(Biome.Prairie); }
                }

                // EAST SANCTUARY
                if (Mathf.PerlinNoise(220.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[23].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(223.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[24].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[24].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(226.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[23].changeBiome(Biome.Prairie); }
                }

                // SOUTH-EAST SANCTUARY
                if (Mathf.PerlinNoise(230.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[26].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(233.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[27].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[27].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(236.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[26].changeBiome(Biome.Prairie); }
                }

                // SOUTH-WEST SANCTUARY
                if (Mathf.PerlinNoise(240.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[29].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(243.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[30].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[30].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(246.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[29].changeBiome(Biome.Prairie); }
                }

                // WEST SANCTUARY
                if (Mathf.PerlinNoise(250.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[32].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(253.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[33].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[33].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(256.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[32].changeBiome(Biome.Prairie); }
                }

                // NORTH-WEST SANCTUARY
                if (Mathf.PerlinNoise(260.4f, GlobalData.boardSeed) * 100f >= 50f)
                {
                    boardCells[35].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(263.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[36].changeBiome(Biome.Prairie); }
                }
                else
                {
                    boardCells[36].changeBiome(Biome.Prairie);
                    if (Mathf.PerlinNoise(266.4f, GlobalData.boardSeed) * 100f <= doublePrairie) { boardCells[35].changeBiome(Biome.Prairie); }
                }
            
            }
            
        }

        // OUT OF RINGS

        float startingX_2 = 310.5f;

        while (currentSwamps < goalSwamps)
        {

            int target = 7 + (int)Mathf.Floor((Mathf.PerlinNoise(startingX_2, GlobalData.boardSeed) * 30f));
            //int target = Random.Range(7, 36 + 1);
            if (boardCells[target].biome == Biome.Forest)
            {
                boardCells[target].changeBiome(Biome.Swamp);
                currentSwamps++;
            }

            startingX_2 += 1.4875785627462734674f;

        }

        selected = boardCells[0];

    }

    private void makeRing(int number)
    {
        BoardCell b;
        int currentCounter = 0;
        float currentWidth = 0f;
        float currentHeight = number;
        

        //INITIAL CELL
        b = boardCells[currentCell];
        b.ring = number;
        b.randomBiome();
        b.root.transform.parent = board.transform;
        b.root.name = "Cell_" + number + "_" + currentCounter;
        positionCell(b, currentWidth, currentHeight);
        if (currentCell != 0)
        {
            connectCells(b, "south", boardCells[currentCell - cellsPerRing[number-1]]);
        }
        currentCounter++;
        boardCells[currentCell] = b;
        currentCell++;

        //DIAGONAL TO THE RIGHT COLUMN
        while (currentWidth < number)
        {
            currentWidth += 1f;
            currentHeight -= 0.5f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "northWest", boardCells[currentCell - 1]);
            connectCells(b, "southWest", boardCells[currentCell - cellsPerRing[number - 1] -1]);
            if (currentWidth != number)
            {
                connectCells(b, "south", boardCells[currentCell - cellsPerRing[number - 1]]);
            }
            

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

        //RIGHT COLUMN
        for (int i = 0; i < number; i++)
        {
            currentHeight -= 1f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "north", boardCells[currentCell - 1]);
            connectCells(b, "northWest", boardCells[currentCell - cellsPerRing[number-1] -2]);
            if (i+1 < number)
            {
                connectCells(b, "southWest", boardCells[currentCell - cellsPerRing[number - 1] - 1]);
            }

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

        // DIAGONAL TO THE CENTRAL DOWN
        while (currentWidth > 0)
        {
            currentWidth -= 1f;
            currentHeight -= 0.5f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "northEast", boardCells[currentCell - 1]);
            connectCells(b, "north", boardCells[currentCell - cellsPerRing[number - 1] - 3]);
            if (currentWidth > 0)
            {
                connectCells(b, "northWest", boardCells[currentCell - cellsPerRing[number - 1] - 2]);
            }

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

        // DIAGONAL TO THE LEFT COLUMN
        while (currentWidth > -number)
        {
            currentWidth -= 1f;
            currentHeight += 0.5f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "southEast", boardCells[currentCell - 1]);
            connectCells(b, "northEast", boardCells[currentCell - cellsPerRing[number - 1] - 4]);
            if (currentWidth > -number)
            {
                connectCells(b, "north", boardCells[currentCell - cellsPerRing[number - 1] - 3]);
            }

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

        //LEFT COLUMN
        for (int i = 0; i < number; i++)
        {
            currentHeight += 1f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "south", boardCells[currentCell - 1]);
            connectCells(b, "southEast", boardCells[currentCell - cellsPerRing[number - 1] - 5]);
            if (i+1 < number)
            {
                connectCells(b, "northEast", boardCells[currentCell - cellsPerRing[number - 1] - 4]);
            }

            if ((currentCounter + 1) == cellsPerRing[number])
            {
                connectCells(b, "northEast", boardCells[currentCell - cellsPerRing[number] + 1]);
            }

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

        // DIAGONAL TO THE INITIAL
        while (currentWidth < -1)
        {
            currentWidth += 1f;
            currentHeight += 0.5f;

            b = boardCells[currentCell];
            b.ring = number;
            b.randomBiome();
            b.root.transform.parent = board.transform;
            b.root.name = "Cell_" + number + "_" + currentCounter;
            positionCell(b, currentWidth, currentHeight);

            connectCells(b, "southWest", boardCells[currentCell - 1]);
            //connectCells(b, "southEast", boardCells[currentCell - cellsPerRing[number - 1] - 5]);
            connectCells(b, "south", boardCells[currentCell - cellsPerRing[number]]);

            if (currentWidth < -1)
            {
                connectCells(b, "southEast", boardCells[currentCell - cellsPerRing[number] +1]);
            }
            else
            {
                connectCells(b, "southEast", boardCells[currentCell - cellsPerRing[number] - cellsPerRing[number-1] +1]);
            }

            if ((currentCounter+1) == cellsPerRing[number])
            {
                connectCells(b, "northEast", boardCells[currentCell - cellsPerRing[number] +1]);
            }

            currentCounter++;
            boardCells[currentCell] = b;
            currentCell++;
        }

    }

    private void positionCell(BoardCell b, float width, float height)
    {
        b.root.transform.position = board.transform.position + new Vector3(width * cellWidth, height * cellHeight, -0.1f);
    }

    private void connectCells(BoardCell b1, string direction, BoardCell b2)
    {
        if (direction == "northWest")
        {
            b1.northWest = b2;
            b2.southEast = b1;
        }
        else if (direction == "north")
        {
            b1.north = b2;
            b2.south = b1;
        }
        else if (direction == "northEast")
        {
            b1.northEast = b2;
            b2.southWest = b1;
        }
        else if (direction == "southEast")
        {
            b1.southEast = b2;
            b2.northWest = b1;
        }
        else if (direction == "south")
        {
            b1.south = b2;
            b2.north = b1;
        }
        else if (direction == "southWest")
        {
            b1.southWest = b2;
            b2.northEast = b1;
        }


    }

    private void resetBoardBrightness() {
        for (int i = 0; i < boardCells.Length; i++) {
            boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
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
