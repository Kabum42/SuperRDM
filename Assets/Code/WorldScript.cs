using UnityEngine;
using System.Collections;

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

    private float lastDPadX = 0f;
    private float lastDPadY = 0f;
    private string lastHorizontal = "down";

    private AudioSource selectCellEffect;


    // Use this for initialization
    void Start () {

        if (!GlobalData.started)
        {
            GlobalData.Start();
        }
       
        for (int i = 1; i < GlobalData.activeAgents; i++)
        {
            if (GlobalData.agents[i] != null)
            {
                Debug.Log(i);
                GameObject g = Instantiate(Resources.Load("Prefabs/UIAgent") as GameObject);
                g.name = "UIAgent" + i;
                g.transform.parent = GameObject.Find("UIAgents").transform;
                g.transform.localPosition = new Vector3(-8.7f, 5.8f -i*2f, 0f);
                g.transform.FindChild("PictureHolder").gameObject.GetComponent<SpriteRenderer>().color = GlobalData.colorCharacters[i];
            }
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
        

        int numCells = 1;
        
        cellsPerRing = new int[numRings+1];
        cellsPerRing[0] = 1;

        for (int i = 1; i <= numRings; i++)
        {
            cellsPerRing[i] = 6 * i;
            numCells += 6 * i;
        }

        boardCells = new BoardCell[numCells];
        //boardCells = new BoardCell[numCells + GlobalData.activeAgents];

        board = new GameObject();
        board.name = "Board";
        board.transform.position = new Vector3(0f, 0f, 0f);

        GenerateBoard();
        
        for (int i = 0; i < GlobalData.activeAgents; i++)
        {
            AddSanctuary(i, i);
        }

	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(boardCells[1].south);
        //boardCells[1].south.root.transform.position = boardCells[1].south.root.transform.position + new Vector3(0f, 0.02f, 0f);

        if (phase == 0)
        {
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
            }

            if (ClickedOn(selected.root))
            {
                // MOVE TO THE CELL??
            }

        }


        if (selected != null)
        {
            selectedSprite.transform.position = new Vector3(selected.root.transform.position.x, selected.root.transform.position.y, selectedSprite.transform.position.z);
        }

    }

    void AddSanctuary(int agent, int num)
    {
        BoardCell b = new BoardCell();
        b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
        b.ring = 4;
        b.changeBiome("sanctuary");
        b.root.GetComponent<SpriteRenderer>().color = GlobalData.colorCharacters[agent];
        b.root.transform.parent = board.transform;
        b.root.name = "Cell_" + 4 + "_" + num;

        if (num == 0) {
            positionCell(b, 2, 3);
        }
        else if (num == 1) {
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

    void GenerateBoard()
    {

        // RINGS
        for (int i = 0; i <= numRings; i++)
        {
            makeRing(i);
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
        b = new BoardCell();
        b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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

            b = new BoardCell();
            b.root = Instantiate(Resources.Load("Prefabs/BoardCell")) as GameObject;
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
