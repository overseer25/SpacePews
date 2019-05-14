using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    WorldGrid worldGrid;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private void Start()
    {
        worldGrid = WorldGrid.instance;
    }


    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startingNode = worldGrid.NodeFromWorldPosition(startPos);
        Node targetNode = worldGrid.NodeFromWorldPosition(targetPos);
        if(startingNode == targetNode)
        {
            List<Node> final = new List<Node>(1);
            final.Add(targetNode);
            return final;
        }

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startingNode);

        while(openList.Count > 0)
        {
            Node currentNode = openList[0];
            foreach(Node node in openList)
            {
                if(node.FCost < currentNode.FCost || node.FCost == currentNode.FCost && node.hCost < currentNode.hCost)
                {
                    currentNode = node;
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode == targetNode)
            {
                return GetFinalPath(startingNode, targetNode);
            }

            foreach(Node neighbor in worldGrid.GetNeighboringNodes(currentNode))
            {
                if(neighbor.isObstacle || closedList.Contains(neighbor))
                {
                    continue;
                }
                int moveCost = currentNode.gCost + CalculateManhattanDistance(currentNode, neighbor);
                if(moveCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = moveCost;
                    neighbor.hCost = CalculateManhattanDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Node> GetFinalPath(Node startNode, Node endNode)
    {
        List<Node> finalPath = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            finalPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        finalPath.Reverse();
        return finalPath;
    }

    private int CalculateManhattanDistance(Node a, Node b)
    {
        //return (int)Mathf.Sqrt((a.gridX - b.gridX) * (a.gridX - b.gridX) + (a.gridY - b.gridY) * (a.gridY - b.gridY));
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridY - b.gridY);
        //return (int)Mathf.Min(Mathf.Abs(a.gridX - b.gridX), Mathf.Abs(a.gridY - b.gridY));
    }
}
