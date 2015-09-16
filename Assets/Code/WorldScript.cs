using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour {

    public GameObject board;
    public BoardCell[] boardCells = new BoardCell[1];

	// Use this for initialization
	void Start () {
        // GlobalData.Start();
        board = new GameObject();
        board.name = "Board";
        board.transform.position = new Vector3(0f, 0f, 0f);

        GenerateBoard();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void GenerateBoard()
    {
        BoardCell b = new BoardCell("desert");
        b.root.transform.parent = board.transform;
        b.root.name = "Cell_1_1";
    }
}
