using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MazeGenerator : MonoBehaviour
{
    public GameObject block;

    public static int mazeSize = 40;
    public static bool isGenerated = false;

    Cell[,] grid = new Cell[mazeSize, mazeSize];
    Cell currentCell;
    Stack<Cell> stack = new Stack<Cell>();
    
    bool isPlayerSpawned = false;

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
                    grid[y,x].gameObject = PhotonNetwork.Instantiate("block", new Vector3(grid[y,x].x * 3, 0, grid[y,x].y * 3), Quaternion.identity);
                }
            }
    
            currentCell = grid[0,0];
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.Escape))
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
                    GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0,1,0), Quaternion.identity);
                    isPlayerSpawned = true;
                    // List<string>[,] newWalls = new List<string>[mazeSize, mazeSize];
                    // int[,] newX = new int[mazeSize, mazeSize];
                    // int[,] newY = new int[mazeSize, mazeSize];
                    // for (int y = 0; y < mazeSize; y++)
                    // {
                    //     for (int x = 0; x < mazeSize; x++)
                    //     {
                    //         newWalls[y,x] = grid[y,x].walls;
                    //         newX[y,x] = grid[y,x].x;
                    //         newY[y,x] = grid[y,x].y;
                    //     }
                    // }
                    // player.GetComponent<PhotonView>().RPC("Ready", RpcTarget.AllBuffered, newWalls, newX, newY);
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
                    grid[y,x].gameObject = Instantiate(block, new Vector3(grid[y,x].x * 3, 0, grid[y,x].y * 3), Quaternion.identity);
                    List<string> allWalls = new List<string>(){"bottom", "right", "front", "left"};
                    List<string> wallsToRemove = allWalls;
                    for (int i = 0; i < grid[y,x].walls.Count; i++)
                    {
                        wallsToRemove.Remove(grid[y,x].walls[i]);
                    }
                    for (int i = 0; i < wallsToRemove.Count; i++)
                    {
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
            isPlayerSpawned = false;
        }
    }
    
    void DestroyWalls(Cell startCell, Cell endCell)
    {
        int x = startCell.x - endCell.x;
        switch(x){
            case -1:
                PhotonNetwork.Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("right"))).gameObject);
                startCell.walls.Remove("right");
                PhotonNetwork.Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("left"))).gameObject);
                endCell.walls.Remove("left");
                break;
            case 1:
                PhotonNetwork.Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("left"))).gameObject);
                startCell.walls.Remove("left");
                PhotonNetwork.Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("right"))).gameObject);
                endCell.walls.Remove("right");
                break;
        }
        int y = startCell.y - endCell.y;
        switch(y){
            case -1:
                PhotonNetwork.Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("front"))).gameObject);
                startCell.walls.Remove("front");
                PhotonNetwork.Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("bottom"))).gameObject);
                endCell.walls.Remove("bottom");
                break;
            case 1:
                PhotonNetwork.Destroy(startCell.gameObject.transform.GetChild(startCell.walls.FindIndex(a => a.Contains("bottom"))).gameObject);
                startCell.walls.Remove("bottom");
                PhotonNetwork.Destroy(endCell.gameObject.transform.GetChild(endCell.walls.FindIndex(a => a.Contains("front"))).gameObject);
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
    void Ready(List<string>[,] walls, int[,] _x, int[,] _y)
    {
        isPlayerSpawned = true;
        for (int y = 0; y < mazeSize; y++)
        {
            for (int x = 0; x < mazeSize; x++)
            {
                grid[y,x] = new Cell(_x[y,x],_y[y,x]);
                grid[y,x].walls = walls[y,x];
            }
        }
    }
}
