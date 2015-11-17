using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldScript : MonoBehaviour {

    public GameObject board;
    public BoardCell[] boardCells;
    private int currentCell = 0;
    private int[] cellsPerRing;

    private BoardCell[] auxSanctuaryCells = new BoardCell[6];
    private List<int> auxSanctuaryAvailable = new List<int> { 0, 1, 2, 3, 4, 5 };
    private int selectedSanctuaries = 0;

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
    private int clickedCell = -1;
    private bool usedDijkstra = false;
    private bool usedAction = false;
    private bool usedTurn = false;
    private List<BoardCell> unvisited;
    private int[] distances;
    private List<List<int>> candidates;
    private List<int> reachables;
    private int lastDijkstraTargetSteps = 0;
    private int lastTurn = -1;

    private float lastDPadX = 0f;
    private float lastDPadY = 0f;
    private string lastHorizontal = "down";

    private GameObject[] UIAgents;

    private AudioSource selectCellEffect;
    private AudioSource pieceSound;

    private int currentMountains = 0;
    private int goalMountains = 0;

    private int currentLakes = 0;
    private int goalLakes = 0;

    private int currentSwamps = 0;
    private int goalSwamps = 0;

    private int movingAgent = 0;
    private float movingDelta = 0f;
    private List<int> movingList = null;
    private int previousAuxPosition = -1;
    private float movingDelay = 0.25f;

    private GameObject ribbon;
    private GameObject ribbonOver;
    private bool ribbonAppearing = false;
    private AudioSource ribbonSound;

    private GameObject action1;
    private GameObject action1Option1;
    private GameObject action2;
    private GameObject action2Option1;
    private GameObject action2Option2;

	private bool fadingBattle = false;

    private float thinkingIA = 1f;


    // Use this for initialization
    void Start () {

        if (!GlobalData.started)
        {
            GlobalData.Start();
        }

        GlobalData.worldObject = GameObject.Find("World");

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

        pieceSound = gameObject.AddComponent<AudioSource>();
        pieceSound.clip = Resources.Load("Music/Pieces/Piece_01") as AudioClip;
        pieceSound.volume = 1f;
        pieceSound.playOnAwake = false;

        fading = GameObject.Find("Fading");

        selectedSprite = GameObject.Find("SelectedSprite");

        ribbon = GameObject.Find("Ribbon");
        ribbonOver = GameObject.Find("Ribbon_Over");
        ribbonOver.SetActive(false);
        ribbon.SetActive(false);

        ribbonSound = gameObject.AddComponent<AudioSource>();
        ribbonSound.clip = Resources.Load("Music/RibbonSound") as AudioClip;
        ribbonSound.volume = 1f;
        ribbonSound.playOnAwake = false;

        action1 = GameObject.Find("Action1");
        action1Option1 = GameObject.Find("Action1/Option1");
        action1.SetActive(false);
        action2 = GameObject.Find("Action2");
        action2Option1 = GameObject.Find("Action2/Option1");
        action2Option2 = GameObject.Find("Action2/Option2");
        action2.SetActive(false);

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
        board.transform.parent = GlobalData.worldObject.transform;

        for (int i = 0; i < numCells; i++)
        {
            BoardCell b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
            b.text = b.root.transform.FindChild("Text").gameObject;
            b.text.SetActive(false);
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

                string currentLegend = "barbarian";
                if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[0]) { currentLegend = "barbarian"; }
                else if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[1]) { currentLegend = "pilumantic"; }
                else if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[2]) { currentLegend = "dreamwalker"; }
                else if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[3]) { currentLegend = "henmancer"; }
                else if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[4]) { currentLegend = "disembodied"; }
                else if (GlobalData.agents[i].getOwnClass() == GlobalData.Classes[5]) { currentLegend = "buckler"; }

                g.transform.FindChild("Legend").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Legends/"+currentLegend);

                UIAgents[i] = g;
            }
        }

        for (int num = 0; num < auxSanctuaryCells.Length; num++)
        {
            BoardCell b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
            b.text = b.root.transform.FindChild("Text").gameObject;
            b.text.SetActive(false);
            b.ring = 4;
            b.changeBiome(Biome.Sanctuary);
            b.root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + GlobalData.biomeNames[(int)Biome.Sanctuary]);
            b.root.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
            b.root.name = "Cell_Aux" + 4 + "_" + num;

            auxSanctuaryCells[num] = b;

            if (num == 0)
            {
                positionCell(b, 2, 3);
            }
            else if (num == 1)
            {
                positionCell(b, 4, 0);
            }
            else if (num == 2)
            {
                positionCell(b, 2, -3);
            }
            else if (num == 3)
            {
                positionCell(b, -2, -3);
            }
            else if (num == 4)
            {
                positionCell(b, -4, 0);
            }
            else if (num == 5)
            {
                positionCell(b, -2, 3);
            }
        }


	}

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Application.LoadLevel("Menu");
    }

    [RPC]
    void moveRequestRPC(NetworkPlayer player, int i)
    {

        if (GlobalData.agents[GlobalData.currentAgentTurn].player == player) {

            List<int> path = DijkstraTarget(i, GlobalData.agents[GlobalData.currentAgentTurn]);

            if (path != null)
            {
                // HA SIDO UN REQUEST VALIDO Y LEGAL
                GetComponent<NetworkView>().RPC("moveAgentRPC", RPCMode.All, GlobalData.currentAgentTurn, path[path.Count-1]);
            }

        }

    }

    [RPC]
    void moveAgentRPC(int agent, int  endOfPath)
    {
        moveAgent(agent, endOfPath);
    }

    void moveAgent(int agent, int endOfPath) {

        List<int> path = DijkstraTarget(endOfPath, GlobalData.agents[agent]);
        // ESTO LUEGO SE CAMBIA OBVIAMENTE

        if (path != null) {

            movingAgent = agent;
            movingDelta = 0f;
            movingList = path;
            GlobalData.agents[agent].setCurrentSteps(GlobalData.agents[agent].getCurrentSteps() -lastDijkstraTargetSteps);

        }

        
    }

    [RPC]
    void nextTurnRequestRPC(NetworkPlayer player)
    {

        if (GlobalData.agents[GlobalData.currentAgentTurn].player == player) {

            GetComponent<NetworkView>().RPC("nextTurnRPC", RPCMode.All);

        }

    }

    [RPC]
    void nextTurnRPC()
    {
        nextTurn();
    }

    void nextTurn()
    {
        int auxOrder = 0;
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.currentAgentTurn == GlobalData.order[i]) { auxOrder = i; }
        }
        auxOrder++;
        if (auxOrder > GlobalData.activeAgents - 1) { auxOrder = 0; }
        GlobalData.currentAgentTurn = GlobalData.order[auxOrder];
        GlobalData.agents[GlobalData.currentAgentTurn].setCurrentSteps(GlobalData.agents[GlobalData.currentAgentTurn].getMaxSteps());
        usedDijkstra = false;
        usedAction = false;
        usedTurn = false;
        clickedCell = -1;
    }

    void previousTurn()
    {
        int auxOrder = 0;
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.currentAgentTurn == GlobalData.order[i]) { auxOrder = i; }
        }
        auxOrder--;
        if (auxOrder < 0) { auxOrder = GlobalData.activeAgents - 1; }
        GlobalData.currentAgentTurn = GlobalData.order[auxOrder];
        GlobalData.agents[GlobalData.currentAgentTurn].setCurrentSteps(GlobalData.agents[GlobalData.currentAgentTurn].getMaxSteps());
        usedDijkstra = false;
        usedAction = false;
        usedTurn = false;
        clickedCell = -1;
    }

    [RPC]
    void sanctuaryRequestRPC(NetworkPlayer player, int num)
    {

        if (phase == 1 && GlobalData.agents[GlobalData.currentAgentTurn].player == player && GlobalData.agents[GlobalData.currentAgentTurn].cellChampion == null)
        {

            // HA SIDO UN REQUEST VALIDO Y LEGAL
            GetComponent<NetworkView>().RPC("sanctuaryRPC", RPCMode.All, GlobalData.currentAgentTurn, num);

        }

    }

    [RPC]
    void sanctuaryRPC(int agent, int num)
    {
        AddSanctuary(agent, num);
    }

    private void RandomizeTurns()
    {
        GlobalData.order = new int[GlobalData.activeAgents];

        List<int> positions = new List<int>();

        for (int i = 0; i < GlobalData.order.Length; i++)
        {
            positions.Add(i);
        }

        float startingPerlin = 34.785625f;

        for (int i = 0; i < GlobalData.order.Length; i++)
        {
            int j = (int) Mathf.Ceil(Hacks.BinaryPerlin(0, positions.Count-1, 3, 0.23425f + startingPerlin, GlobalData.boardSeed));
            GlobalData.order[i] = positions[j];
            positions.RemoveAt(j);
            startingPerlin += 2.5662845f;
        }


        GlobalData.currentAgentTurn = (int) Mathf.Ceil(Hacks.BinaryPerlin(0, GlobalData.order.Length-1, 3, 0.23425f + startingPerlin, GlobalData.boardSeed));
        //Debug.Log(GlobalData.currentAgentTurn);

    }

    private List<int> Dijkstra()
    {
        usedDijkstra = true;

        distances = new int[boardCells.Length];
        unvisited = new List<BoardCell>();
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            unvisited.Add(boardCells[i]);
        }

        distances[GlobalData.agents[GlobalData.currentAgentTurn].currentCell] = 0;

        visitCell(boardCells[GlobalData.agents[GlobalData.currentAgentTurn].currentCell]);

        List <int> reachables = new List<int>();

        for (int i = 0; i < boardCells.Length; i++)
        {
            if (distances[i] <= GlobalData.agents[GlobalData.currentAgentTurn].getCurrentSteps())
            {
                reachables.Add(boardCells[i].positionInArray);
            }
        }

        reachables.Remove(GlobalData.agents[GlobalData.currentAgentTurn].currentCell);
        return reachables;
    }

    private void visitCell(BoardCell b)
    {

        if (unvisited.Contains(b)) {
            unvisited.Remove(b);
        }

        // CHANGE DISTANCES
        if (b.northWest != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.northWest.biome, b.northWest.positionInArray, GlobalData.currentAgentTurn) < distances[b.northWest.positionInArray]) { distances[b.northWest.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.northWest.biome, b.northWest.positionInArray, GlobalData.currentAgentTurn); visitCell(b.northWest);  }
        if (b.north != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.north.biome, b.north.positionInArray, GlobalData.currentAgentTurn) < distances[b.north.positionInArray]) { distances[b.north.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.north.biome, b.north.positionInArray, GlobalData.currentAgentTurn); visitCell(b.north);  }
        if (b.northEast != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.northEast.biome, b.northEast.positionInArray, GlobalData.currentAgentTurn) < distances[b.northEast.positionInArray]) { distances[b.northEast.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.northEast.biome, b.northEast.positionInArray, GlobalData.currentAgentTurn); visitCell(b.northEast);  }

        if (b.southWest != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.southWest.biome, b.southWest.positionInArray, GlobalData.currentAgentTurn) < distances[b.southWest.positionInArray]) { distances[b.southWest.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.southWest.biome, b.southWest.positionInArray, GlobalData.currentAgentTurn); visitCell(b.southWest);  }
        if (b.south != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.south.biome, b.south.positionInArray, GlobalData.currentAgentTurn) < distances[b.south.positionInArray]) { distances[b.south.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.south.biome, b.south.positionInArray, GlobalData.currentAgentTurn); visitCell(b.south);  }
        if (b.southEast != null && distances[b.positionInArray] + GlobalData.getBiomeCost(b.southEast.biome, b.southEast.positionInArray, GlobalData.currentAgentTurn) < distances[b.southEast.positionInArray]) { distances[b.southEast.positionInArray] = distances[b.positionInArray] + GlobalData.getBiomeCost(b.southEast.biome, b.southEast.positionInArray, GlobalData.currentAgentTurn); visitCell(b.southEast);  }
        

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

    private List<int> DijkstraTarget(int numCell, MainCharacter agent)
    {

        candidates = new List<List<int>>();

        List<int> list = new List<int>();
        list.Add(agent.currentCell);

        visitCellTarget(list, boardCells[agent.currentCell], numCell, agent.getCurrentSteps());

        List<int> bestList = null;
        int bestSteps = int.MaxValue;

        //Debug.Log("CANDIDATES: " + candidates.Count);

        for (int i = 0; i < candidates.Count; i++)
        {
            int currentSteps = 0;
            for (int j = 1; j < candidates[i].Count; j++)
            {
                currentSteps += GlobalData.getBiomeCost(boardCells[candidates[i][j]].biome, boardCells[candidates[i][j]].positionInArray, GlobalData.currentAgentTurn);
            }
            if (currentSteps < bestSteps)
            {
                bestSteps = currentSteps;
                bestList = candidates[i];
            }
        }

        lastDijkstraTargetSteps = bestSteps;

        return bestList;
    }

    private void visitCellTarget(List<int> list, BoardCell b, int numTarget, int currentSteps)
    {

        int auxSteps = currentSteps;

        // NORTH
        if (b.northWest != null)
        {
            if (GlobalData.getBiomeCost(b.northWest.biome, b.northWest.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.northWest.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.northWest.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.northWest.biome, b.northWest.positionInArray, GlobalData.currentAgentTurn);
                if (b.northWest.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.northWest, numTarget, auxSteps);
            }
        }

        if (b.north != null)
        {
            if (GlobalData.getBiomeCost(b.north.biome, b.north.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.north.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.north.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.north.biome, b.north.positionInArray, GlobalData.currentAgentTurn);
                if (b.north.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.north, numTarget, auxSteps);
            }
        }

        if (b.northEast != null)
        {
            if (GlobalData.getBiomeCost(b.northEast.biome, b.northEast.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.northEast.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.northEast.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.northEast.biome, b.northEast.positionInArray, GlobalData.currentAgentTurn);
                if (b.northEast.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.northEast, numTarget, auxSteps);
            }
        }

        // SOUTH
        if (b.southWest != null)
        {
            if (GlobalData.getBiomeCost(b.southWest.biome, b.southWest.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.southWest.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.southWest.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.southWest.biome, b.southWest.positionInArray, GlobalData.currentAgentTurn);
                if (b.southWest.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.southWest, numTarget, auxSteps);
            }
        }

        if (b.south != null)
        {
            if (GlobalData.getBiomeCost(b.south.biome, b.south.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.south.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.south.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.south.biome, b.south.positionInArray, GlobalData.currentAgentTurn);
                if (b.south.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.south, numTarget, auxSteps);
            }
        }

        if (b.southEast != null)
        {
            if (GlobalData.getBiomeCost(b.southEast.biome, b.southEast.positionInArray, GlobalData.currentAgentTurn) <= currentSteps && !list.Contains(b.southEast.positionInArray))
            {
                auxSteps = currentSteps;
                List<int> newList = copyList(list);
                newList.Add(b.southEast.positionInArray);
                auxSteps -= GlobalData.getBiomeCost(b.southEast.biome, b.southEast.positionInArray, GlobalData.currentAgentTurn);
                if (b.southEast.positionInArray == numTarget) { candidates.Add(newList); }
                visitCellTarget(newList, b.southEast, numTarget, auxSteps);
            }
        }


        // ESTO LIBERA LA MEMORIA DE ESA VARIABLE
        list = null;

    }

    private int CalculateDijkstraTarget(int origin, int end)
    {

        distances = new int[boardCells.Length];
        unvisited = new List<BoardCell>();
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            unvisited.Add(boardCells[i]);
        }

        distances[origin] = 0;

        visitCell(boardCells[origin]);

        return distances[end];
    }

    private List<int> copyList(List<int> source)
    {
        List<int> newList = new List<int>();

        for (int i = 0; i < source.Count; i++)
        {
            newList.Add(source[i]);
        }

        return newList;
    }


    void useTurnIA() {

        List<int> allBoard = new List<int>();
        for (int i = 0; i < boardCells.Length; i++) {
            allBoard.Add(i);
        }
        int longTermObjective = bestObjective(allBoard, GlobalData.currentAgentTurn);

        reachables = Dijkstra();
        int shortTermObjective = bestObjective(reachables, GlobalData.currentAgentTurn);



        while (CalculateDijkstraTarget(shortTermObjective, longTermObjective) > CalculateDijkstraTarget(GlobalData.agents[GlobalData.currentAgentTurn].currentCell, longTermObjective))
        {
            //REVISAR
            reachables.Remove(shortTermObjective);
            shortTermObjective = bestObjective(reachables, GlobalData.currentAgentTurn);
        }

        if (GlobalData.online) {
            GetComponent<NetworkView>().RPC("moveAgentRPC", RPCMode.All, GlobalData.currentAgentTurn, shortTermObjective);
        }
        else {
            moveAgent(GlobalData.currentAgentTurn, shortTermObjective);
        }

        usedTurn = true;
    }

    int bestObjective(List<int> availableCells, int agent) {

        int bestAux = -1;
        int bestValue = -1;

        for (int i = 0; i < availableCells.Count; i++) {
            if (cellValue(availableCells[i], agent) > bestValue) {
                bestAux = i;
                bestValue = cellValue(availableCells[i], agent);
            }
        }

        if (bestAux < 0 || bestAux >= availableCells.Count)
        {
            Debug.Log("BEST AUX:" + bestAux);
            Debug.Log("BEST VALUE:" + bestValue);
            Debug.Log("LOLASO:" + availableCells.Count);
        }

        return availableCells[bestAux];
    }

    int cellValue(int cell, int agent) {

        // SI TIENE LA FATIGA MUY ALTA
        if (GlobalData.agents[agent].getCurrentFatigue() > 0.5f && cell == GlobalData.agents[agent].sanctuary) {
            return 100;
        }
        // SI TIENE NIVEL 10 o MAS
        if (GlobalData.agents[agent].getCurrentLevel() >= 6 && boardCells[cell].biome == Biome.TheEvil) {
            // DECIDIR SI VA A POR EL BOSS BASANDOSE EN SU NIVEL Y EN LA FATIGA DEL BOSS
            float aux = Mathf.Pow(2, Mathf.Clamp(GlobalData.agents[agent].getCurrentLevel(), 0, 10) - 4);
            aux += GlobalData.bossFatigue * 30f;
            // AUX VA DESDE 4 a 64
            if (Random.Range(0f, 100f) < aux)
            {
                // DECIDE IR A POR EL BOSS
                return 99;
            }
        }

        // SI EN ESA CASILLA HAY UN JUGADOR ENEMIGO CON FATIGA ALTA Y NIVEL BAJO
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.agents[i].currentCell == cell && i != agent)
            {
                // HAY UN JUGADOR ENEMIGO
                if (false)
                {
                    // PUEDO ACABAR CON EL
                    if (false)
                    {
                        // TIENE UNA MISION POR ENTREGAR
                        return 98;
                    }
                    if (false)
                    {
                        // ESTA POR ENCIMA DE LA MEDIA DE NIVELES (Y POR ENCIMA DE TI EN EL RANKING O JUSTO DEBAJO DE TI)
                        // POR TANTO ES UN COMPETIDOR DIRECTO
                        return 98;
                    }
                    if (false)
                    {
                        // ESTA EN TU LISTA NEGRA Y TE CAE MAL
                        return 98;
                    }
                }
            }
        }
        


        // SI ESA CASILLA TIENE MISION
        if (false) {
            return 97;
        }

        // SI ESA CASILLA NO ESTA EXHAUSTA
        if (true) {
            return GlobalData.getBiomeCost(boardCells[cell].biome, cell, GlobalData.currentAgentTurn);
        }

        return 0;
    }


    // Update is called once per frame
    void Update()
    {

		if (!fadingBattle && fading.GetComponent<SpriteRenderer> ().color.a > 0f) {

			fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, fading.GetComponent<SpriteRenderer> ().color.a -Time.deltaTime);

		}

        if (phase == 2 && GlobalData.currentAgentTurn == GlobalData.myAgent && !usedDijkstra && movingList == null)
        {
            reachables = Dijkstra();
        }

        if (lastTurn != GlobalData.currentAgentTurn && GlobalData.currentAgentTurn == GlobalData.myAgent)
        {
            ribbon.transform.position = new Vector3(0f, 8f, -4f);
            ribbon.SetActive(true);
            ribbonOver.SetActive(true);
            Hacks.SpriteRendererAlpha(ribbon, 0f);
            Hacks.SpriteRendererAlpha(ribbonOver, 0f);
            ribbonAppearing = true;
            ribbonSound.Play();
        }
        lastTurn = GlobalData.currentAgentTurn;

        if (ribbon.activeInHierarchy) {
            if (ribbonAppearing) {
                Hacks.SpriteRendererAlpha(ribbon, Mathf.Lerp(ribbon.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime*5f));
                Hacks.SpriteRendererAlpha(ribbonOver, Mathf.Lerp(ribbon.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime*5f));
                ribbon.transform.position = new Vector3(0f, Mathf.Lerp(ribbon.transform.position.y, 6.3f, Time.deltaTime*5f), -4f); 
                if (ribbon.GetComponent<SpriteRenderer>().color.a >= 0.99f) {
                    ribbonAppearing = false;
                }
            }
            else {
                Hacks.SpriteRendererAlpha(ribbon, ribbon.GetComponent<SpriteRenderer>().color.a - Time.deltaTime*2f);
                Hacks.SpriteRendererAlpha(ribbonOver, ribbon.GetComponent<SpriteRenderer>().color.a - Time.deltaTime*2f);
                if (ribbon.GetComponent<SpriteRenderer>().color.a <= 0f) {
                    ribbon.SetActive(false);
                    ribbonOver.SetActive(false);
                }
            }
        }

        // BRIGHTNESS CELLS
        if (phase == 2)
        {
            if (usedDijkstra && GlobalData.currentAgentTurn == GlobalData.myAgent && reachables != null)
            {
                for (int i = 0; i < boardCells.Length; i++)
                {
                    if (!reachables.Contains(i) && i != GlobalData.agents[GlobalData.currentAgentTurn].currentCell)
                    {
                        float aux = Mathf.Lerp(boardCells[i].root.GetComponent<SpriteRenderer>().color.r, 0.3f, Time.deltaTime * 5f);
                        boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                    }
                }
                float aux2 = Mathf.Lerp(boardCells[GlobalData.agents[GlobalData.currentAgentTurn].currentCell].root.GetComponent<SpriteRenderer>().color.r, 1f, Time.deltaTime * 5f);
                boardCells[GlobalData.agents[GlobalData.currentAgentTurn].currentCell].root.GetComponent<SpriteRenderer>().color = new Color(aux2, aux2, aux2, 1f);
            }
            else
            {
                for (int i = 0; i < boardCells.Length; i++)
                {
                    float aux = Mathf.Lerp(boardCells[i].root.GetComponent<SpriteRenderer>().color.r, 1f, Time.deltaTime * 5f);
                    boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                }
            }
        }

        


        

        if (Input.GetKey(KeyCode.Return))
        {
            GlobalData.boardSeed = Random.Range(0f, 100f);
            GenerateBoard();
        }

        if (Input.GetKeyDown(KeyCode.Space) && GlobalData.currentAgentTurn == GlobalData.myAgent)
        {

            usedTurn = true;
            
        }

        // Place Champions
        if (movingList != null) {

            movingDelta += Time.deltaTime;
            float timeToMove = 0.25f;

            int auxPosition =  (int) Mathf.Floor(Mathf.Clamp((movingDelta+timeToMove)*(1f/timeToMove), 0, movingList.Count-1));

            if (auxPosition != previousAuxPosition) 
            { 
                int sound = Random.Range(1, 9);
                pieceSound.clip = Resources.Load("Music/Pieces/Piece_"+sound.ToString("00")) as AudioClip;
                pieceSound.Play();
            }
            previousAuxPosition = auxPosition;

            GlobalData.agents[movingAgent].currentCell = movingList[auxPosition];

            if (auxPosition == movingList.Count-1) {

                movingDelay -= Time.deltaTime;

                if (movingDelay <= 0f)
                {
                    movingAgent = -1;
                    movingDelta = 0f;
                    movingList = null;
                    previousAuxPosition = -1;
                    movingDelay = timeToMove;
                    usedDijkstra = false;
                }
            }

        }

        if (phase == 2)
        {
            for (int i = 0; i < GlobalData.activeAgents; i++)
            {
                GlobalData.agents[i].cellChampion.transform.position = new Vector3(Mathf.Lerp(GlobalData.agents[i].cellChampion.transform.position.x, boardCells[GlobalData.agents[i].currentCell].root.transform.position.x, Time.deltaTime * 10f), Mathf.Lerp(GlobalData.agents[i].cellChampion.transform.position.y, boardCells[GlobalData.agents[i].currentCell].root.transform.position.y, Time.deltaTime * 10f), GlobalData.agents[i].cellChampion.transform.position.z);
            }
        }

        // Place UI Agents
        if (movingList == null) {

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

        }
        

        if (phase == 2 && movingList == null && GlobalData.agents[GlobalData.currentAgentTurn].IA && (int.Parse(Network.player.ToString()) == 0 || !GlobalData.online))
        {
            // LE TOCA A LA IA
            thinkingIA -= Time.deltaTime;

            if (thinkingIA <= 0f) {

                useTurnIA();
                thinkingIA = 0.5f + Random.Range(0f, 0.5f);

            }
        }

        if (phase == 0)
        {
            // QUITANDO EL FADING
            transition += Time.deltaTime;
            if (transition >= 1f)
            {
                transition = 1f;
                phase = 1;
            }

            musicWorld.volume = transition*0.7f;
            fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, 1f - transition);

        }
        else if (phase == 1)
        {
            // TIME TO SELECT YOUR SANCTUARY

            if (selectedSanctuaries == GlobalData.activeAgents)
            {
                int bla = auxSanctuaryAvailable.Count;
                for (int i = 0; i < bla; i++)
                {
                    int aux = auxSanctuaryAvailable[0];
                    auxSanctuaryAvailable.Remove(aux);
                    Destroy(auxSanctuaryCells[aux].root);
                    auxSanctuaryCells[aux] = null;
                }

                GlobalData.agents[GlobalData.currentAgentTurn].setCurrentSteps(GlobalData.agents[GlobalData.currentAgentTurn].getMaxSteps());
                usedDijkstra = false;
                usedAction = false;
                usedTurn = false;
                clickedCell = -1;

                for (int i = 0; i < boardCells.Length; i++)
                {
                    float aux = Mathf.Lerp(boardCells[i].root.GetComponent<SpriteRenderer>().color.r, 1f, Time.deltaTime * 5f);
                    boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                }

                if (boardCells[0].root.GetComponent<SpriteRenderer>().color.r >= 0.99f) {
                    phase = 2;
                }
                
            }
            else
            {

                for (int i = 0; i < boardCells.Length; i++)
                {
                    float aux = Mathf.Lerp(boardCells[i].root.GetComponent<SpriteRenderer>().color.r, 0.3f, Time.deltaTime * 5f);
                    boardCells[i].root.GetComponent<SpriteRenderer>().color = new Color(aux, aux, aux, 1f);
                }

                if (GlobalData.online)
                {
                    if (int.Parse(Network.player.ToString()) == 0)
                    {
                        // ES EL SERVER
                        if (GlobalData.myAgent == GlobalData.currentAgentTurn)
                        {
                            // ES EL SERVER Y LE TOCA A EL
                            selectedSprite.SetActive(false);

                            for (int i = 0; i < auxSanctuaryAvailable.Count; i++)
                            {

                                if (isOver(auxSanctuaryCells[auxSanctuaryAvailable[i]].root))
                                {
                                    selectedSprite.SetActive(true);
                                    selectedSprite.transform.position = new Vector3(auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.x, auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.y, selectedSprite.transform.position.z);
                                }

                                if (ClickedOn(auxSanctuaryCells[auxSanctuaryAvailable[i]].root))
                                {

                                    int aux = auxSanctuaryAvailable[i];
                                    GetComponent<NetworkView>().RPC("sanctuaryRPC", RPCMode.All, GlobalData.myAgent, aux);
                                    break;

                                }

                            }
                        }
                        else if (GlobalData.agents[GlobalData.currentAgentTurn].IA)
                        {
                            // ES EL SERVER Y LE TOCA A LA IA
                            thinkingIA -= Time.deltaTime;

                            if (thinkingIA <= 0f)
                            {

                                int aux = auxSanctuaryAvailable[Random.Range(0, auxSanctuaryAvailable.Count)];
                                GetComponent<NetworkView>().RPC("sanctuaryRPC", RPCMode.All, GlobalData.currentAgentTurn, aux);

                                thinkingIA = 0.5f + Random.Range(0f, 0.5f);
                            }
                        }

                    }
                    else if (GlobalData.myAgent == GlobalData.currentAgentTurn)
                    {
                        // ES UN CLIENTE
                        selectedSprite.SetActive(false);

                        for (int i = 0; i < auxSanctuaryAvailable.Count; i++)
                        {

                            if (isOver(auxSanctuaryCells[auxSanctuaryAvailable[i]].root))
                            {
                                selectedSprite.SetActive(true);
                                selectedSprite.transform.position = new Vector3(auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.x, auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.y, selectedSprite.transform.position.z);
                            }

                            if (ClickedOn(auxSanctuaryCells[auxSanctuaryAvailable[i]].root))
                            {

                                int aux = auxSanctuaryAvailable[i];
                                GetComponent<NetworkView>().RPC("sanctuaryRequestRPC", RPCMode.All, Network.player, aux);
                                break;

                            }

                        }

                    }
                }
                else
                {
                    if (GlobalData.agents[GlobalData.currentAgentTurn].IA && GlobalData.agents[GlobalData.currentAgentTurn].cellChampion == null)
                    {
                        // ES LA IA
                        thinkingIA -= Time.deltaTime;

                        if (thinkingIA <= 0f) {

                            int aux = auxSanctuaryAvailable[Random.Range(0, auxSanctuaryAvailable.Count)];
                            AddSanctuary(GlobalData.currentAgentTurn, aux);

                            thinkingIA = 0.5f +Random.Range(0f, 0.5f);
                        }

                    }
                    else
                    {
                        // ES EL JUGADOR
                        selectedSprite.SetActive(false);

                        for (int i = 0; i < auxSanctuaryAvailable.Count; i++) {

                            if (isOver(auxSanctuaryCells[auxSanctuaryAvailable[i]].root)) {
                                selectedSprite.SetActive(true);
                                selectedSprite.transform.position = new Vector3(auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.x, auxSanctuaryCells[auxSanctuaryAvailable[i]].root.transform.position.y, selectedSprite.transform.position.z);
                            }

                            if (ClickedOn(auxSanctuaryCells[auxSanctuaryAvailable[i]].root)) {

                                int aux = auxSanctuaryAvailable[i];
                                AddSanctuary(GlobalData.currentAgentTurn, aux);
                                break;

                            }

                        }

                    }
                }
            }

        }
        else if (movingList == null)
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

            }
            else
            {
                // CONTROLLER NOT PLUGGED

                selectedSprite.SetActive(false);

                if (action1.activeInHierarchy && ClickedOn(action1Option1))
                {
                    clickedCell = -1;
                    usedAction = true;
                }
                else if (action2.activeInHierarchy && ClickedOn(action2Option1))
                {
                    moveTo(clickedCell);
                    clickedCell = -1;
                    usedAction = true;
                }
                else if (action2.activeInHierarchy && ClickedOn(action2Option2))
                {
                    moveTo(clickedCell);
                    clickedCell = -1;
                }
                else
                {

                    if (Input.GetMouseButtonUp(0))
                    {
                        clickedCell = -1;
                    }

                    for (int i = 0; i < boardCells.Length; i++)
                    {
                        if (isOver(boardCells[i].root))
                        {
                            selectedSprite.SetActive(true);
                            selected = boardCells[i];
                        }

                        if (GlobalData.currentAgentTurn == GlobalData.myAgent && ClickedOn(boardCells[i].root) && (reachables.Contains(i) || i == GlobalData.agents[GlobalData.currentAgentTurn].currentCell))
                        {

                            clickedCell = i;

                            Hacks.SpriteRendererAlpha(action1, 0f);
                            Hacks.SpriteRendererAlpha(action1Option1, 0f);

                            Hacks.SpriteRendererAlpha(action2, 0f);
                            Hacks.SpriteRendererAlpha(action2Option1, 0f);
                            Hacks.SpriteRendererAlpha(action2Option2, 0f);

                        }
                    }
                }


            }

            if (selected != null)
            {
                selectedSprite.transform.position = new Vector3(selected.root.transform.position.x, selected.root.transform.position.y, selectedSprite.transform.position.z);
            }
            if (clickedCell == -1)
            {

                if (action1.activeInHierarchy)
                {
                    Hacks.SpriteRendererAlpha(action1, Mathf.Lerp(action1.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime*10f));
                    Hacks.SpriteRendererAlpha(action1Option1, action1.GetComponent<SpriteRenderer>().color.a);
                    if (action1.GetComponent<SpriteRenderer>().color.a < 0.01f)
                    {
                        action1.SetActive(false);
                    }
                }

                if (action2.activeInHierarchy)
                {
                    Hacks.SpriteRendererAlpha(action2, Mathf.Lerp(action2.GetComponent<SpriteRenderer>().color.a, 0f, Time.deltaTime * 10f));
                    Hacks.SpriteRendererAlpha(action2Option1, action2.GetComponent<SpriteRenderer>().color.a);
                    Hacks.SpriteRendererAlpha(action2Option2, action2.GetComponent<SpriteRenderer>().color.a);
                    if (action2.GetComponent<SpriteRenderer>().color.a < 0.01f)
                    {
                        action2.SetActive(false);
                    }
                }
                
            }
            else
            {

                if (clickedCell == GlobalData.agents[GlobalData.myAgent].currentCell)
                {
                    if (action2.activeInHierarchy)
                    {
                        Hacks.SpriteRendererAlpha(action2, 0f);
                        Hacks.SpriteRendererAlpha(action2Option1, 0f);
                        Hacks.SpriteRendererAlpha(action2Option2, 0f);
                        action2.SetActive(false);
                    }

                    action1.SetActive(true);
                    Hacks.SpriteRendererAlpha(action1, (float)Mathf.Lerp(action1.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 5f));
                    Hacks.SpriteRendererAlpha(action1Option1, action1.GetComponent<SpriteRenderer>().color.a);
                    action1.transform.position = new Vector3(boardCells[clickedCell].root.transform.position.x, boardCells[clickedCell].root.transform.position.y + 0.7f, action1.transform.position.z);
                
                    if (isOver(action1Option1)) {
                        action1Option1.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, action1Option1.GetComponent<SpriteRenderer>().color.a);
                    }
                    else {
                        action1Option1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, action1Option1.GetComponent<SpriteRenderer>().color.a);
                    }

                }
                else
                {

                    if (action1.activeInHierarchy)
                    {
                        Hacks.SpriteRendererAlpha(action1, 0f);
                        Hacks.SpriteRendererAlpha(action1Option1, 0f);
                        action1.SetActive(false);
                    }

                    action2.SetActive(true);
                    Hacks.SpriteRendererAlpha(action2, (float)Mathf.Lerp(action2.GetComponent<SpriteRenderer>().color.a, 1f, Time.deltaTime * 5f));
                    Hacks.SpriteRendererAlpha(action2Option1, action2.GetComponent<SpriteRenderer>().color.a);
                    Hacks.SpriteRendererAlpha(action2Option2, action2.GetComponent<SpriteRenderer>().color.a);
                    action2.transform.position = new Vector3(boardCells[clickedCell].root.transform.position.x, boardCells[clickedCell].root.transform.position.y + 0.7f, action2.transform.position.z);

                    if (isOver(action2Option1))
                    {
                        action2Option1.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, action2Option1.GetComponent<SpriteRenderer>().color.a);
                    }
                    else
                    {
                        action2Option1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, action2Option1.GetComponent<SpriteRenderer>().color.a);
                    }


                    if (isOver(action2Option2))
                    {
                        action2Option2.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, action2Option2.GetComponent<SpriteRenderer>().color.a);
                    }
                    else
                    {
                        action2Option2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, action2Option2.GetComponent<SpriteRenderer>().color.a);
                    }

                }

                
            }

        }


        
        if (movingList == null)
        {
            if (usedTurn)
            {
				if (fadingBattle) {
					fading.GetComponent<SpriteRenderer>().color = new Color(fading.GetComponent<SpriteRenderer>().color.r, fading.GetComponent<SpriteRenderer>().color.g, fading.GetComponent<SpriteRenderer>().color.b, fading.GetComponent<SpriteRenderer>().color.a +Time.deltaTime);
					if (fading.GetComponent<SpriteRenderer>().color.a >= 1f) {
						fadingBattle = false;
						GlobalData.Battle();
					}
				}
				else {
					if (GlobalData.online)
					{
						if (int.Parse(Network.player.ToString()) == 0)
						{
							// ES EL SERVER
							GetComponent<NetworkView>().RPC("nextTurnRPC", RPCMode.All);
						}
						else
						{
							// ES UN CLIENTE
							GetComponent<NetworkView>().RPC("nextTurnRequestRPC", RPCMode.Server, Network.player);
						}
					}
					else
					{
						nextTurn();
					}
				}
            }
            else if (usedAction)
            {
                usedTurn = true;
                GlobalData.positionCharacterCombat = new int[] {GlobalData.myAgent, -1};
                GlobalData.currentBiome = boardCells[GlobalData.agents[GlobalData.currentAgentTurn].currentCell].biome;
				fadingBattle = true;
            }
        }
        

    }

    void moveTo(int i)
    {

        if (GlobalData.online)
        {
            if (int.Parse(Network.player.ToString()) == 0)
            {
                // ES EL SERVER
                List<int> path = DijkstraTarget(i, GlobalData.agents[GlobalData.currentAgentTurn]);

                if (path != null) {
                    GetComponent<NetworkView>().RPC("moveAgentRPC", RPCMode.All, GlobalData.myAgent, path[path.Count-1]);
                    reachables = null;
                }

            }
            else
            {
                // ES UN CLIENTE
                GetComponent<NetworkView>().RPC("moveRequestRPC", RPCMode.Server, Network.player, i);
                reachables = null;
            }
        }
        else
        {
            List<int> path = DijkstraTarget(i, GlobalData.agents[GlobalData.currentAgentTurn]);

            if (path != null) {
                moveAgent(GlobalData.myAgent, path[path.Count-1]);
                //usedTurn = true;
                reachables = null;
            }
                            
        }
        
    }

    void AddSanctuary(int agent, int num)
    {
        BoardCell b = boardCells[currentCell];
        b.ring = 4;
        b.changeBiome(Biome.Sanctuary);
        b.root.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BoardCells/" + GlobalData.biomeNames[(int)Biome.Sanctuary] + "_" +(agent+1).ToString("00"));
        b.root.GetComponent<SpriteRenderer>().color = GlobalData.colorCharacters[agent];
        b.root.transform.parent = board.transform;
        b.root.name = "Cell_" + 4 + "_" + num;

        GlobalData.agents[agent].currentCell = currentCell;
        GlobalData.agents[agent].cellChampion = Instantiate(Resources.Load("Prefabs/Champion")) as GameObject;
        GlobalData.agents[agent].cellChampion.name = "Champion_" + agent;
        GlobalData.agents[agent].cellChampion.transform.parent = GameObject.Find("Champions").transform;
        GlobalData.agents[agent].sanctuary = currentCell;

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

        auxSanctuaryAvailable.Remove(num);
        Destroy(auxSanctuaryCells[num].root);
        auxSanctuaryCells[num] = null;
        selectedSanctuaries++;

        if (selectedSanctuaries < GlobalData.activeAgents)
        {
            previousTurn();
        }

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
        goalMountains = 1 + (int) Mathf.Floor((Mathf.Clamp(Mathf.PerlinNoise(0.1f, GlobalData.boardSeed), 0f, 1f) * (GlobalData.activeAgents / 2 )));
        // 2-4 LAKES
        goalLakes = 2 + (int)Mathf.Floor((Mathf.Clamp(Mathf.PerlinNoise(3.1f, GlobalData.boardSeed), 0f, 1f) * 3f));
        // 5-6 SWAMPS
        goalSwamps = 5 + (int)Mathf.Floor((Mathf.Clamp(Mathf.PerlinNoise(6.1f, GlobalData.boardSeed), 0f, 1f) * 2f));

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

                    int target = 1 + (int)Mathf.Floor((Mathf.Clamp(Mathf.PerlinNoise(startingX, GlobalData.boardSeed), 0f, 1f) * 6f));
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

                    int target = 7 + (int)Mathf.Floor((Mathf.Clamp(Mathf.PerlinNoise(startingX, GlobalData.boardSeed), 0f, 1f) * 12f));
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
