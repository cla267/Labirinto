using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicOcclusionCulling : MonoBehaviour
{
    int radius = 6;
    int newRadius = 2;
    List<GameObject> walls;
    List<int> basicWallsIndices = new List<int>();
    [SerializeField]
    GameObject basicCell;
    [SerializeField]
    GameObject leavesCell;
    const float Striscia = 2f;

    List<string> allWalls = new List<string>(){"bottom", "right", "front", "left"};

    // Start is called before the first frame update
    void Start()
    {
        walls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Wall"));
        for (int i = 0; i < walls.Count; i++) basicWallsIndices.Add(-1);
        StartCoroutine(checkWalls());
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
        for (int t = 0; t < walls.Count; t++)
        {
            GameObject wall = walls[t];
            print(walls.Count + " ----- " + basicWallsIndices.Count);
            int index = basicWallsIndices[t];
            if(index != -1)
            {
                GameObject leavesWall = leavesCell.transform.GetChild(index).gameObject;
                wall.GetComponent<MeshFilter>().sharedMesh = leavesWall.GetComponent<MeshFilter>().sharedMesh;
                wall.transform.localPosition = leavesWall.transform.localPosition;
                wall.transform.localRotation = leavesWall.transform.localRotation;
                wall.transform.localScale = leavesWall.transform.localScale;
                wall.GetComponent<BoxCollider>().size = leavesWall.GetComponent<BoxCollider>().size;
                wall.GetComponent<BoxCollider>().center = leavesWall.GetComponent<BoxCollider>().center;
                wall.GetComponent<MeshRenderer>().material = leavesWall.GetComponent<MeshRenderer>().sharedMaterial;
            }
            wall.GetComponent<MeshRenderer>().enabled = false;
        }
        walls = new List<GameObject>();
        basicWallsIndices = new List<int>();
        
        Vector2Int currentCell = new Vector2Int(Mathf.FloorToInt((transform.position.x + 1.5f) / 3), Mathf.FloorToInt((transform.position.z + 1.5f) / 3));
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int _y = currentCell.y + y;
                _y = (_y >= 0 && _y < MazeGenerator.mazeSize)? _y: currentCell.y;
                int _x = currentCell.x + x;
                _x = (_x >= 0 && _x < MazeGenerator.mazeSize)? _x: currentCell.x;
                GameObject cell = MazeGenerator.grid[_y, _x].gameObject;
                for (int t = 0; t < cell.transform.childCount; t++)
                {
                    GameObject wall = cell.transform.GetChild(t).gameObject;
                    if(_y < currentCell.y - newRadius || _y > currentCell.y + newRadius || _x < currentCell.x - newRadius || _x > currentCell.x + newRadius)
                    {
                        GameObject basicWall = basicCell.transform.GetChild(allWalls.IndexOf(MazeGenerator.grid[_y,_x].walls[t])).gameObject;
                        wall.GetComponent<MeshFilter>().sharedMesh = basicWall.GetComponent<MeshFilter>().sharedMesh;
                        wall.transform.localPosition = basicWall.transform.localPosition;
                        wall.transform.localRotation = basicWall.transform.localRotation;
                        wall.transform.localScale = basicWall.transform.localScale;
                        wall.GetComponent<BoxCollider>().size = basicWall.GetComponent<BoxCollider>().size;
                        wall.GetComponent<BoxCollider>().center = basicWall.GetComponent<BoxCollider>().center;
                        wall.GetComponent<MeshRenderer>().material = basicWall.GetComponent<MeshRenderer>().sharedMaterial;
                        wall.GetComponent<MeshRenderer>().material.color = Color.green;
                        basicWallsIndices.Add(allWalls.IndexOf(MazeGenerator.grid[_y,_x].walls[t]));
                    }
                    else
                    {
                        basicWallsIndices.Add(-1);
                    }
                    wall.GetComponent<MeshRenderer>().enabled = true;
                    walls.Add(wall);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        
    }
}
