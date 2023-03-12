using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering;

public class MazeGenerator : MonoBehaviourPunCallbacks
{
    public GameObject block;
    public GameObject floor;

    public static int mazeSize = 40;
    public static bool isGenerated = false;

    Cell[,] grid = new Cell[mazeSize, mazeSize];
    Cell currentCell;
    Stack<Cell> stack = new Stack<Cell>();
    [HideInInspector]
    public GameObject player;
    
    bool isPlayerSpawned = false;
    bool master;

    int[] newX = new int[mazeSize*mazeSize];
    int[] newY = new int[mazeSize*mazeSize];
    string[] newWalls = new string[mazeSize*mazeSize];

    private List<MeshFilter> sourceMeshFilters = new List<MeshFilter>();

    //back, right, front, left

    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for(int y = 0; y < mazeSize; y++)
            {
                for (int x = 0; x < mazeSize; x++)
                {
                    grid[y,x] = new Cell(x,y);
                    // grid[y,x].gameObject = Instantiate(block, new Vector3(grid[y,x].x * 3, 0, grid[y,x].y * 3), Quaternion.identity);
                }
            }
    
            currentCell = grid[0,0];
            master = true;
        }

        floor.transform.position = new Vector3(mazeSize/2*3-1.5f, 0, mazeSize/2*3-1.5f);
        floor.transform.localScale = new Vector3(mazeSize*3/10,1,mazeSize*3/10);
        floor.layer = 3;
    }

    void Update()
    {
        if (master)
        {
            currentCell.visited = true;
            
            List<Cell> neighbours = GetNonVisitedNeighbours(currentCell);
            if(neighbours.Count != 0)
            {
                Cell nextCell = neighbours[Random.Range(0,neighbours.Count)];
                stack.Push(currentCell);
                DestroyWalls(currentCell, nextCell);
                currentCell = nextCell;
            }
            else if(stack.Count != 0)
            {
                currentCell = stack.Pop();
            }
            else
            {
                isGenerated = true;
                if (!isPlayerSpawned)
                {
                    player = PhotonNetwork.Instantiate("Player", new Vector3(0,1,0), Quaternion.identity);
                    isPlayerSpawned = true;
                    for (int y = 0; y < mazeSize; y++)
                    {
                        for (int x = 0; x < mazeSize; x++)
                        {
                            grid[y,x].gameObject = Instantiate(block, new Vector3(grid[y,x].x * 3, 0, grid[y,x].y * 3), Quaternion.identity);
                            List<string> wallsToRemove = new List<string>(){"bottom", "right", "front", "left"};
                            for (int t = 0; t < grid[y,x].walls.Count; t++)
                            {
                                wallsToRemove.Remove(grid[y,x].walls[t]);
                            }
                            for (int t = 0; t < wallsToRemove.Count; t++)
                            {
                                List<string> allWalls = new List<string>(){"bottom", "right", "front", "left"};
                                switch(wallsToRemove[t])
                                {
                                    case "bottom":
                                        Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("bottom"))).gameObject);
                                        allWalls.Remove("bottom");
                                        break;
                                    case "right":
                                        Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("right"))).gameObject);
                                        allWalls.Remove("right");
                                        break;
                                    case "front":
                                        Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("front"))).gameObject);
                                        allWalls.Remove("front");
                                        break;
                                    case "left":
                                        Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("left"))).gameObject);
                                        allWalls.Remove("left");
                                        break;
                                }
                            }
                            int i = y * mazeSize + x;
                            newX[i] = grid[y,x].x;
                            newY[i] = grid[y,x].y;
                            string _walls = "";
                            for (int t = 0; t < grid[y,x].walls.Count; t++)
                            {
                                if(t == grid[y,x].walls.Count - 1) _walls += grid[y,x].walls[t];
                                else _walls += grid[y,x].walls[t] + ",";
                            }
                            newWalls[i] = _walls;
                        }
                    }
                    photonView.RPC("Ready", RpcTarget.OthersBuffered, newX, newY, newWalls);
                }
            }
            // Debug.Log($"X: {currentCell.x} - Y: {currentCell.y}");
        }
        else if(isPlayerSpawned)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                for (int x = 0; x < mazeSize; x++)
                {
                    int index = y * mazeSize + x;
                    grid[y,x] = new Cell(newX[index], newY[index]);
                    List<string> walls =  new List<string>(newWalls[index].Split(','));
                    grid[y,x].walls = walls;
                }
            }
            for (int y = 0; y < mazeSize; y++)
            {
                for (int x = 0; x < mazeSize; x++)
                {
                    Debug.Log(y * mazeSize + x);
                    grid[y,x].gameObject = Instantiate(block, new Vector3(grid[y,x].x * 3, 0, grid[y,x].y * 3), Quaternion.identity);
                    List<string> wallsToRemove = new List<string>(){"bottom", "right", "front", "left"};
                    for (int i = 0; i < grid[y,x].walls.Count; i++)
                    {
                        Debug.Log(grid[y,x].walls[i]);
                        wallsToRemove.Remove(grid[y,x].walls[i]);
                    }
                    Debug.Log("-----------------------------------------");
                    for (int i = 0; i < wallsToRemove.Count; i++)
                    {
                        List<string> allWalls = new List<string>(){"bottom", "right", "front", "left"};
                        Debug.Log(wallsToRemove[i]);
                        switch(wallsToRemove[i])
                        {
                            case "bottom":
                                Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("bottom"))).gameObject);
                                allWalls.Remove("bottom");
                                break;
                            case "right":
                                Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("right"))).gameObject);
                                allWalls.Remove("right");
                                break;
                            case "front":
                                Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("front"))).gameObject);
                                allWalls.Remove("front");
                                break;
                            case "left":
                                Destroy(grid[y,x].gameObject.transform.GetChild(allWalls.FindIndex(a => a.Contains("left"))).gameObject);
                                allWalls.Remove("left");
                                break;
                        }
                    }
                }
            }
            isGenerated = true;
            player = PhotonNetwork.Instantiate("Player", new Vector3(0,1,0), Quaternion.identity);
            isPlayerSpawned = false;
        }
    }
    
    void DestroyWalls(Cell startCell, Cell endCell)
    {
        int x = startCell.x - endCell.x;
        switch(x){
            case -1:
                // Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("right"))).gameObject);
                startCell.walls.Remove("right");
                // Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("left"))).gameObject);
                endCell.walls.Remove("left");
                break;
            case 1:
                // Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("left"))).gameObject);
                startCell.walls.Remove("left");
                // Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("right"))).gameObject);
                endCell.walls.Remove("right");
                break;
        }
        int y = startCell.y - endCell.y;
        switch(y){
            case -1:
                // Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("front"))).gameObject);
                startCell.walls.Remove("front");
                // Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("bottom"))).gameObject);
                endCell.walls.Remove("bottom");
                break;
            case 1:
                // Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("bottom"))).gameObject);
                startCell.walls.Remove("bottom");
                // Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("front"))).gameObject);
                endCell.walls.Remove("front");
                break;
        }
    }

    List<Cell> GetNonVisitedNeighbours(Cell cell)
    {
        List<Cell> toReturn = new List<Cell>();
        //bottom
        if(cell.y > 0 && grid[cell.y - 1, cell.x].visited == false) toReturn.Add(grid[currentCell.y - 1, currentCell.x]);
        //front
        if(cell.y < mazeSize - 1 && grid[cell.y + 1, cell.x].visited == false) toReturn.Add(grid[currentCell.y + 1, currentCell.x]);
        //left
        if(cell.x > 0 && grid[cell.y, cell.x - 1].visited == false) toReturn.Add(grid[currentCell.y, currentCell.x - 1]);
        //right
        if(cell.x < mazeSize - 1 && grid[cell.y, cell.x + 1].visited == false) toReturn.Add(grid[currentCell.y, currentCell.x + 1]);
        return toReturn;
    }

    [PunRPC]
    void Ready(int[] _x, int[] _y, string[] _walls)
    {
        isPlayerSpawned = true;
        newX = _x;
        newY = _y;
        newWalls = _walls;
    }
}
