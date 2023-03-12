using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicOcclusionCulling : MonoBehaviour
{
    float radius = 20;
    public LayerMask mask;
    Collider[] walls = new Collider[]{};
    const float Striscia = 2f;

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
        foreach(Collider wall in walls)
            {
                wall.GetComponent<MeshRenderer>().enabled = false;
            }
    
            walls = Physics.OverlapSphere(transform.position, radius, mask);
    
            foreach(Collider wall in walls)
            {
                wall.GetComponent<MeshRenderer>().enabled = true;
            }
    }

    private void OnDrawGizmos() {
        
    }
}
