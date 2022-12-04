using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MazeGenerator : MonoBehaviour
{
    public static GameObject currentBlock;
    int currentBlockIndex;
    int nextBlockIndex;
    int currentLine;

    public GameObject block;
    public GameObject player;
    public static int mazeSize = 15;
    public int falseReycasts = 0;

    public static bool canBeGenerated = false;
    public static bool isGenerated = false;

    float positionX, positionZ, startX;
    float seconds = 0f;
    float avgFrameRate;
    float delayTime;

    public string nonVisitedTag;
    public string visitedTag;
    public string visited2Tag;
    public string nonDestrucatbleTag;

    public List<GameObject> blocksList;

    public GameObject _currentBlock;

    void Awake()
    {
        
    }
    
    void Start()
    {
        GenerateGrid();
        blocksList.AddRange(GameObject.FindGameObjectsWithTag(nonVisitedTag));
        GameObject startBlock = blocksList[0];
        transform.position = startBlock.transform.position;
        blocksList[0].tag = visitedTag;
    }

    void Update()
    {
        _currentBlock = currentBlock;
        
        if(Input.GetKeyDown(KeyCode.E)) {GenerateMaze(); //print("Current block index: " + currentBlockIndex);
        }

        // if(blocksList.Contains(GameObject.FindGameObjectWithTag(nonVisitedTag)) && seconds >= 4)
        // {
        //     GenerateMaze();
        //     seconds = 0;
        // }
        // seconds += 1;

        //spawns the player on the network if the maze is finished (On the old method)
        if(!blocksList.Contains(GameObject.FindGameObjectWithTag(nonVisitedTag)) && isGenerated == false)
        {
            Destroy(gameObject);
            PhotonNetwork.Instantiate(player.name, blocksList[0].transform.position, Quaternion.identity);
            isGenerated = true;
        }
    }

    void GenerateMaze()
    {
        //indexes: 0=back 1=right 2=front 3=left
        int[] neighborhood = new int[]{currentBlockIndex-mazeSize, currentBlockIndex+1, currentBlockIndex+mazeSize, currentBlockIndex-1};
        int[] isNeighborhoodVisited = new int[4];

        for (int i = 0; i < isNeighborhoodVisited.Length; i++)
        {
            //putting the neighbors in the array
            if(neighborhood[i] >= 0 && neighborhood[i] < blocksList.Count)
            {
                if(blocksList[neighborhood[i]].tag == nonVisitedTag) isNeighborhoodVisited[i] = 0;
                else if(blocksList[neighborhood[i]].tag == visitedTag) isNeighborhoodVisited[i] = 1;
                else if(blocksList[neighborhood[i]].tag == visited2Tag) isNeighborhoodVisited[i] = 2;
            }
            else isNeighborhoodVisited[i] = 5;
        }

        List<int> visitedIndexes = new List<int>();
        List<int> nonVisitedIndexes = new List<int>();
        //adding the non visited neighbors in the nonVisitedIndexes list
        for (int i = 0; i < isNeighborhoodVisited.Length; i++)
        {
            if(isNeighborhoodVisited[i] == 0) nonVisitedIndexes.Add(i);
        }
        
        //removing the possibility to go left if the current block is all to the left
        if(currentBlockIndex == currentLine * mazeSize && nonVisitedIndexes.Contains(3)) nonVisitedIndexes.Remove(3);
        
        //removing the possibility to go right if the current block is all to the right
        if(currentBlockIndex == ((currentLine + 1) * mazeSize) - 1 && nonVisitedIndexes.Contains(1)) nonVisitedIndexes.Remove(1);

        if (nonVisitedIndexes.Count != 0)
        {
            //get the direction to go to
            int nextDirection = nonVisitedIndexes[Random.Range(0, nonVisitedIndexes.Count)];

            //update the line the current block is in
            if(nextDirection == 2) currentLine += 1;
            else if(nextDirection == 0) currentLine -= 1;

            nextBlockIndex = neighborhood[nextDirection];
            
            //indexes: 0=back 1=right 2=front 3=left

            //deactivating the walls on the blocks
            if(nextDirection - 2 >= 0) blocksList[nextBlockIndex].transform.GetChild(nextDirection - 2).gameObject.SetActive(false);
            else blocksList[nextBlockIndex].transform.GetChild(nextDirection + 2).gameObject.SetActive(false);
            blocksList[currentBlockIndex].transform.GetChild(nextDirection).gameObject.SetActive(false);
    
            currentBlockIndex = nextBlockIndex;
    
            blocksList[currentBlockIndex].tag = visitedTag;
        }else
        {
            //adding the visited neighbors in the visitedIndexes list
            for (int i = 0; i < isNeighborhoodVisited.Length; i++)
            {
                if(isNeighborhoodVisited[i] == 1) visitedIndexes.Add(i);
            }
        }

        if(nonVisitedIndexes.Count == 0 && visitedIndexes.Count != 0)
        {
            blocksList[currentBlockIndex].tag = visited2Tag;

            for (int i = 0; i < visitedIndexes.Count; i++)
            {
                switch (visitedIndexes[i])
                {
                    //indexes: 0=back 1=right 2=front 3=left
                    case 0:
                        if(blocksList[neighborhood[0]].transform.GetChild(2).gameObject.activeSelf == true) visitedIndexes.Remove(visitedIndexes[i]);
                        break;
                    case 1:
                        if(blocksList[neighborhood[1]].transform.GetChild(3).gameObject.activeSelf == true) visitedIndexes.Remove(visitedIndexes[i]);
                        break;
                    case 2:
                        if(blocksList[neighborhood[2]].transform.GetChild(0).gameObject.activeSelf == true) visitedIndexes.Remove(visitedIndexes[i]);
                        break;
                    case 3:
                        if(blocksList[neighborhood[3]].transform.GetChild(1).gameObject.activeSelf == true) visitedIndexes.Remove(visitedIndexes[i]);
                        break;
                }
            }
            //removing the possibility to go left if the current block is all to the left
            if(currentBlockIndex == currentLine * mazeSize && visitedIndexes.Contains(3)) visitedIndexes.Remove(3);

            //removing the possibility to go right if the current block is all to the right
            if(currentBlockIndex == ((currentLine + 1) * mazeSize) - 1 && visitedIndexes.Contains(1)) visitedIndexes.Remove(1);

            int nextDirection = visitedIndexes[Random.Range(0, visitedIndexes.Count)];
    
            if(nextDirection == 2) currentLine += 1;
            else if(nextDirection == 0) currentLine -= 1;
    
            currentBlockIndex = neighborhood[nextDirection];
        }
        if(nonVisitedIndexes.Count == 0 && visitedIndexes.Count == 0)
        {
            print("This should not happen!!");

            List<int> visited2Indexes = new List<int>();

            for (int i = 0; i < isNeighborhoodVisited.Length; i++)
            {
                if(isNeighborhoodVisited[i] == 2) visited2Indexes.Add(i);
            }

            for (int i = 0; i < visited2Indexes.Count; i++)
            {
                switch (visited2Indexes[i])
                {
                    case 0:
                        if(blocksList[neighborhood[0]].transform.GetChild(2).gameObject.activeInHierarchy == true) visited2Indexes.Remove(visited2Indexes[i]);
                        break;
                    case 1:
                        if(blocksList[neighborhood[1]].transform.GetChild(3).gameObject.activeInHierarchy == true) visited2Indexes.Remove(visited2Indexes[i]);
                        break;
                    case 2:
                        if(blocksList[neighborhood[2]].transform.GetChild(0).gameObject.activeInHierarchy == true) visited2Indexes.Remove(visited2Indexes[i]);
                        break;
                    case 3:
                        if(blocksList[neighborhood[3]].transform.GetChild(1).gameObject.activeInHierarchy == true) visited2Indexes.Remove(visited2Indexes[i]);
                        break;
                }
            }
            //removing the possibility to go left if the current block is all to the left
            if(currentBlockIndex == currentLine * mazeSize && visited2Indexes.Contains(3)) visited2Indexes.Remove(3);

            //removing the possibility to go right if the current block is all to the right
            if(currentBlockIndex == ((currentLine + 1) * mazeSize) - 1 && visited2Indexes.Contains(1)) visited2Indexes.Remove(1);

            int nextDirection = visited2Indexes[Random.Range(0, visited2Indexes.Count)];

            if(nextDirection == 2) currentLine += 1;
            else if(nextDirection == 0) currentLine -= 1;
    
            currentBlockIndex = neighborhood[nextDirection];
        }

        
    }

#region VecchioMetodo

    // void GenerateMaze()
    // {
        // for (int i = 0; i < blocksList.Capacity; i++)
        // {
        //     if(blocksList[i].tag == nonVisitedTag)
        //     {
        //         blocksList[i].layer = LayerMask.NameToLayer(nonVisitedTag);
        //     } else if(blocksList[i].tag == visitedTag)
        //     {
        //         blocksList[i].layer = LayerMask.NameToLayer(visitedTag);
        //     }else if(blocksList[i].tag == visited2Tag)
        //     {
        //         blocksList[i].layer = LayerMask.NameToLayer(visited2Tag);
        //     }else
        //     {
        //         Debug.LogError("Block " + blocksList[i].name + " needs to be set as visited or non visited");
        //     }
        //     for (int t = 0; t < blocksList[i].transform.childCount; t++)
        //     {
        //         if (blocksList[i].transform.GetChild(t).tag != "Non Destructable")
        //         {
        //             blocksList[i].transform.GetChild(t).tag = blocksList[i].tag;
        //             blocksList[i].transform.GetChild(t).gameObject.layer = blocksList[i].layer;
        //         }
        //     }
        // }

        // bool[] raycasts = new bool[4]; // firs one is forward, then left, then right, then back

        // raycasts[0] = Physics.Raycast(transform.position, Vector3.forward, 4, LayerMask.GetMask(nonVisitedTag));
        // raycasts[1] = Physics.Raycast(transform.position, Vector3.left, 4, LayerMask.GetMask(nonVisitedTag));
        // raycasts[2] = Physics.Raycast(transform.position, Vector3.right, 4, LayerMask.GetMask(nonVisitedTag));
        // raycasts[3] = Physics.Raycast(transform.position, Vector3.back, 4, LayerMask.GetMask(nonVisitedTag));

        // for (int i = 0; i < raycasts.Length; i++)
        // {
        //     if (raycasts[i] == false)
        //     {
        //         falseReycasts++;
        //     }
        // }

        // if(falseReycasts == 4) 
        // {
        //     falseReycasts = 0;
        //     currentBlock.tag = visited2Tag;
        //     currentBlock.layer = LayerMask.NameToLayer(visited2Tag);
        //     GoBack();
        //     return;
        // }
        // else
        // {
        //     falseReycasts = 0;
        // }

        // int nextDirection = Random.Range(0, 4);

        // while(raycasts[nextDirection] == false)
        // {
        //     nextDirection = Random.Range(0, 4);
        // }
        // // print("forward: " + raycasts[0] + "; left: " + raycasts[1] + "; right: " + raycasts[2] + "; back: " + raycasts[3] + ";");

        // StartCoroutine(Move(nextDirection));

        // print(nextDirection);
    // }

    // void GoBack()
    // {
    //     bool[] raycasts = new bool[4];

    //     raycasts[0] = Physics.Raycast(transform.position, Vector3.forward, 1.7f);
    //     raycasts[1] = Physics.Raycast(transform.position, Vector3.left, 1.7f);
    //     raycasts[2] = Physics.Raycast(transform.position, Vector3.right, 1.7f);
    //     raycasts[3] = Physics.Raycast(transform.position, Vector3.back, 1.7f);

    //     for (int index = 0; index < raycasts.Length; index++)
    //     {
    //         if(raycasts[index] == false)
    //         {
    //             if(index == 0)
    //             {
    //                 Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 4);
    //                 if(hit.transform.parent.gameObject.tag == visited2Tag)
    //                 {
    //                     raycasts[index] = true;
    //                 }
    //             }else if(index == 1)
    //             {
    //                 Physics.Raycast(transform.position, Vector3.left, out RaycastHit hit, 4);
    //                 if(hit.transform.parent.gameObject.tag == visited2Tag)
    //                 {
    //                     raycasts[index] = true;
    //                 }
    //             }else if(index == 2)
    //             {
    //                 Physics.Raycast(transform.position, Vector3.right, out RaycastHit hit, 4);
    //                 if(hit.transform.parent.gameObject.tag == visited2Tag)
    //                 {
    //                     raycasts[index] = true;
    //                 }
    //             }else
    //             {
    //                 Physics.Raycast(transform.position, Vector3.back, out RaycastHit hit, 4);
    //                 if(hit.transform.parent.gameObject.tag == visited2Tag)
    //                 {
    //                     raycasts[index] = true;
    //                 }
    //             }
    //         }
    //     }

    //     int nextDirection = Random.Range(0, 4);

    //     while(raycasts[nextDirection] == true)
    //     {
    //         nextDirection = Random.Range(0, 4);
    //     }
    //     // print("forward: " + raycasts[0] + "; left: " + raycasts[1] + "; right: " + raycasts[2] + "; back: " + raycasts[3] + ";");

    //     StartCoroutine(Move(nextDirection));
    // }

    // IEnumerator MakeWalls()
    // {
    //     int i = 1;
    //     while(i < mazeSize)
    //     {
    //         wallMaker.transform.localPosition = new Vector3(wallMaker.transform.position.x + 3, wallMaker.transform.position.y, wallMaker.transform.position.z);
    //         i++;
    //         yield return null;
    //     }
    //     i = 1;
    //     wallMaker.transform.position = new Vector3(wallMaker.transform.position.x + 1.5f, wallMaker.transform.position.y, wallMaker.transform.position.z + 1.5f);
    //     yield return null;
    //     while (i < mazeSize)
    //     {
    //         wallMaker.transform.localPosition = new Vector3(wallMaker.transform.position.x, wallMaker.transform.position.y, wallMaker.transform.position.z + 3);
    //         i++;
    //         yield return null;
    //     }
    //     i = 1;
    //     wallMaker.transform.position = new Vector3(wallMaker.transform.position.x - 1.5f, wallMaker.transform.position.y, wallMaker.transform.position.z + 1.5f);
    //     yield return null;
    //     while (i < mazeSize)
    //     {
    //         wallMaker.transform.localPosition = new Vector3(wallMaker.transform.position.x - 3, wallMaker.transform.position.y, wallMaker.transform.position.z);
    //         i++;
    //         yield return null;
    //     }
    //     i = 1;
    //     wallMaker.transform.position = new Vector3(wallMaker.transform.position.x - 1.5f, wallMaker.transform.position.y, wallMaker.transform.position.z - 1.5f);
    //     yield return null;
    //     while (i < mazeSize)
    //     {
    //         wallMaker.transform.localPosition = new Vector3(wallMaker.transform.position.x, wallMaker.transform.position.y, wallMaker.transform.position.z - 3);
    //         i++;
    //         yield return null;
    //     }
    //     Destroy(wallMaker);
    //     canBeGenerated = true;
    // }

    // IEnumerator Move(int direction)
    // {
    //     if(direction == 0)
    //     {
    //         transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f);
    //         yield return null;
    //         transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f);
    //     } else if(direction == 1)
    //     {
    //         transform.position = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
    //         yield return null;
    //         transform.position = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
    //     } else if(direction == 2)
    //     {
    //         transform.position = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
    //         yield return null;
    //         transform.position = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
    //     } else
    //     {
    //         transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.5f);
    //         yield return null;
    //         transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.5f);
    //     }
    // }
#endregion

    void GenerateGrid()
    {
        positionX = -((float)(mazeSize - 1) * 3) * 0.5f;
        startX = positionX;
        positionZ = -((float)(mazeSize - 1) * 3) * 0.5f;

        for (int i = 0; i < mazeSize; i++)
        {
            for(int t = 0; t < mazeSize; t++)
            {
                Instantiate(block, new Vector3(positionX, 0, positionZ), Quaternion.identity);
                positionX += 3;
            }

            positionZ += 3;
            positionX = startX;
        }
    }

    // private void OnTriggerStay(Collider other) {
    //     if (!other.CompareTag(nonDestrucatbleTag))
    //     {
    //         Destroy(other.gameObject);
    //     }
    // }
}
