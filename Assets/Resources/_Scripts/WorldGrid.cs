using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public Transform startPos; //probably dont need

    public LayerMask obstacleMask;

    public Vector2 worldSize;
    public Vector2 gridCenter;

    /// <summary>
    /// Size of each node that will layout the grid.
    /// </summary>
    public float nodeRadius = 3f;
    /// <summary>
    /// How much spacing there is between grid cells when drawn on gizmos.
    /// </summary>
    public float distance = 0.2f;

    private GameObject[] objsToGetPath;
    public List<Node> finalPath;

    private Node[,] grid;
     
    private float nodeDiameter;
     
    private int gridSizeX;
    private int gridSizeY;

    public static WorldGrid instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        objsToGetPath = GameObject.FindGameObjectsWithTag("Enemy");
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

    public Node[,] GetWorldGrid()
    {
        return grid;
    }

    public Node NodeFromWorldPosition(Vector2 worldPos)
    {
        float xPoint = (worldPos.x + worldSize.x * 0.5f) / worldSize.x;
        float yPoint = (worldPos.y + worldSize.y * 0.5f) / worldSize.y;

        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);

        return grid[x, y];
    }

    public List<Node> GetNeighboringNodes(Node currentNode)
    {
        List<Node> neighboringNodes = new List<Node>(4);

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
                int neighborXSpot = currentNode.gridX + x;
                int neighborYSpot = currentNode.gridY + y;
                if (neighborXSpot >= 0 && neighborXSpot < gridSizeX && neighborYSpot >= 0 && neighborYSpot < gridSizeY)
                {
                    neighboringNodes.Add(grid[neighborXSpot, neighborYSpot]);
                }
            }
        }

        return neighboringNodes;
    }

    private void OnDrawGizmos() //just for visualizing grid, will be removed once confirmed works
    {
        Gizmos.DrawWireCube(gridCenter, worldSize);
        if(grid != null)
        {
            foreach(Node node in grid)
            {
                Gizmos.color = node.isObstacle ? Color.red : Color.green;
                foreach(GameObject obj in objsToGetPath)
                {
                    List<Node> thisObjPath = obj.GetComponent<RammingAIMovement>().GetPath();
                    if(thisObjPath != null)
                    {
                        if (thisObjPath.Contains(node))
                        {
                            Gizmos.color = Color.blue;
                        }
                    }
                }
                //if(finalPath != null)
                //{
                //    if (finalPath.Contains(node))
                //    {
                //        Gizmos.color = Color.blue;
                //    }
                //}
                Gizmos.DrawCube(node.position, Vector2.one * (nodeDiameter - distance));
            }
        }
    }
}
