using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicOcclusionCulling : MonoBehaviour
{
    int radius = 6;
    int newRadius = 2;
    public LayerMask mask;
    List<GameObject> walls = new List<GameObject>();
    [SerializeField]
    GameObject basicCell;
    [SerializeField]
    GameObject leavesCell;
    Mesh newMesh;
    const float Striscia = 2f;

    public List<string> allWalls = new List<string>(){"bottom", "right", "front", "left"};

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkWalls());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator checkWalls()
    {
        while (true)
        {
            CheckWallsFunction();
            yield return new WaitForSeconds(Striscia);
        }
    }

    public void CheckWallsFunction()
    {
        foreach(GameObject wall in walls)
        {
            wall.GetComponent<MeshRenderer>().enabled = false;
        }
        walls = new List<GameObject>();
        int currentCell = Mathf.FloorToInt((transform.position.x + 1.5f) / 3);
        
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int _y = currentCell + y;
                _y = (_y >= 0 && _y < MazeGenerator.mazeSize)? _y: currentCell;
                int _x = Mathf.FloorToInt((transform.position.x + 1.5f) / 3) + x;
                _x = (_x >= 0 && _x < MazeGenerator.mazeSize)? _x: Mathf.FloorToInt((transform.position.x + 1.5f) / 3);
                GameObject cell = MazeGenerator.grid[_y, _x].gameObject;
                for (int t = 0; t < cell.transform.childCount; t++)
                {
                    GameObject wall = cell.transform.GetChild(t).gameObject;
                    if(x == 0 && y == 0)
                    {
                        GameObject basicWall = basicCell.transform.GetChild(allWalls.IndexOf(MazeGenerator.grid[_y,_x].walls[t])).gameObject;
                        wall.GetComponent<MeshFilter>().sharedMesh = basicWall.GetComponent<MeshFilter>().sharedMesh;
                        wall.transform.localPosition = basicWall.transform.localPosition;
                        wall.transform.localRotation = basicWall.transform.localRotation;
                        wall.transform.localScale = basicWall.transform.localScale;
                        wall.GetComponent<BoxCollider>().size = basicWall.GetComponent<BoxCollider>().size;
                        wall.GetComponent<BoxCollider>().center = basicWall.GetComponent<BoxCollider>().center;
                        wall.GetComponent<MeshRenderer>().material = null;
                    }
                    wall.GetComponent<MeshRenderer>().enabled = true;
                    walls.Add(cell.transform.GetChild(t).gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        
    }
}
