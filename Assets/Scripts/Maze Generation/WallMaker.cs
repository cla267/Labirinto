using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMaker : MonoBehaviour
{
    public string wallTag;

    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        other.gameObject.tag = wallTag;
        other.gameObject.layer = LayerMask.NameToLayer(wallTag);
    }
}
