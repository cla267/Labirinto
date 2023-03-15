using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    Camera cam;

    public static bool isMapping = false;
    public static bool isLoading = true;

    public GameObject loadingPanel;
    public GameObject map;
    public Camera loadingCamera;
    public GameObject mazeManager;

    private List<MeshRenderer> wallsRenderer = new List<MeshRenderer>();

    void Start()
    {
        cam = GetComponent<Camera>();
        isMapping = false;
    }

    void Update()
    {
        cam.orthographicSize = ((float) MazeGenerator.mazeSize * 40f) / 25f;
        cam.transform.position = new Vector3(MazeGenerator.mazeSize * 3 / 2 -1.5f, cam.transform.position.y, MazeGenerator.mazeSize * 3 / 2 -1.5f);

        // if (!isLoading)
        // {
            if(Input.GetKeyDown(KeyCode.M) && isMapping == false)
            {
                map.SetActive(true);
                moovement.canJump = false;
                moovement.canMove = false;
                foreach(MeshRenderer wallRenderer in wallsRenderer) wallRenderer.enabled = true;
                isMapping = true;
            }else if(Input.GetKeyDown(KeyCode.M) && isMapping == true)
            {
                map.SetActive(false);
                moovement.canJump = true;
                moovement.canMove = true;
                foreach(MeshRenderer wallRenderer in wallsRenderer) wallRenderer.enabled = false;
                mazeManager.GetComponent<MazeGenerator>().player.transform.GetChild(1).GetComponent<DinamicOcclusionCulling>().CheckWallsFunction();
                isMapping = false;
            }
        // }

        isLoading = !MazeGenerator.isGenerated;

        if (isLoading)
        {
            loadingPanel.SetActive(true);
            loadingCamera.enabled = true;
        }
        else
        {
            loadingPanel.SetActive(false);
            loadingCamera.enabled = false;
            foreach(GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
            {
                wallsRenderer.Add(wall.GetComponent<MeshRenderer>());
            }
        }
    }
}
