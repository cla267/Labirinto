using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsManager : MonoBehaviour
{
    Text fpsCount;

    // Start is called before the first frame update
    void Start()
    {
        fpsCount = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        fpsCount.text = ((int)current).ToString();

        if(MazeGenerator.isGenerated == false)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 23;
        } else
        {
            Application.targetFrameRate = -1;
        }
    }
}
