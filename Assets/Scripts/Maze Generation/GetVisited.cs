using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVisited : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Generator"))
        {
            transform.parent.tag = "Visited";
            transform.parent.gameObject.layer = LayerMask.NameToLayer("Visited");
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if(!transform.parent.GetChild(i).CompareTag("Non Destructable"))
                {
                    transform.parent.GetChild(i).tag = transform.parent.tag;
                    transform.parent.GetChild(i).gameObject.layer = transform.parent.gameObject.layer;
                }
            }

            MazeGenerator.currentBlock = transform.parent.gameObject;
        }
    }
}
