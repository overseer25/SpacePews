using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    public int FCost { get { return gCost + hCost; } }

    public bool isObstacle;

    public Vector2 position;

    public Node parent;

    public Node(bool isObstacle, Vector2 pos, int gridX, int gridY)
    {
        this.isObstacle = isObstacle;
        this.position = pos;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}
