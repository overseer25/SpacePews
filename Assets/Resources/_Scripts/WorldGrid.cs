using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public Transform startPos; //probably dont need

    public LayerMask obstacleMask;

    public Vector2 worldSize;
    public Vector2 gridCenter;

    public float nodeRadius;
    public float distance;

    public List<Node> finalPath;

    private Node[,] grid;
     
    private float nodeDiameter;
     
    private int gridSizeX;
    private int gridSizeY;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = gridCenter - Vector2.right * worldSize.x * 0.5f - Vector2.up * worldSize.y * 0.5f;

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool obstacle = Physics2D.OverlapCircle(worldPoint, nodeRadius, obstacleMask) != null;
                grid[x, y] = new Node(obstacle, worldPoint, x, y);
            }
        }
    }

    private void OnDrawGizmos() //just for visualizing grid, will be removed once confirmed works
    {
        Gizmos.DrawWireCube(gridCenter, worldSize);
        if(grid != null)
        {
            foreach(Node node in grid)
            {
                Gizmos.color = node.isObstacle ? Color.red : Color.green;
                if(finalPath != null)
                {
                    if (finalPath.Contains(node))
                    {
                        Gizmos.color = Color.blue;
                    }
                }
                Gizmos.DrawCube(node.position, Vector2.one * (nodeDiameter - distance));
            }
        }
    }
}
