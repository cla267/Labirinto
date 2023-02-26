using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    public new GameObject gameObject;
    public List<string> walls = new List<string>(){"bottom", "right", "front", "left"};
    public bool visited = false;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
