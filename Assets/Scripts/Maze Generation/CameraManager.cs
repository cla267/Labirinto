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

    void Start()
    {
        cam = GetComponent<Camera>();
        isMapping = false;
    }

    void Update()
    {
        cam.orthographicSize = ((float) MazeGenerator.mazeSize * 40f) / 25f;
        cam.transform.position = new Vector3(MazeGenerator.mazeSize * 3 / 2 -1.5f, cam.transform.position.y, MazeGenerator.mazeSize * 3 / 2 -1.5f);

        if(Input.GetKeyDown(KeyCode.M) && isMapping == false)
        {
            map.SetActive(true);
            moovement.canJump = false;
            moovement.canMove = false;
            isMapping = true;
        }else if(Input.GetKeyDown(KeyCode.M) && isMapping == true)
        {
            map.SetActive(false);
            moovement.canJump = true;
            moovement.canMove = true;
            isMapping = false;
        }

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
        }
    }
}
