using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeMaker
{
    private float width;
    private float height;
    private float halfWidth;
    private float halfHeight;

    private Vector2 center;
    private List<Vector2> points;

    private int capacity;

    private QuadTreeMaker quadrantOne;
    private QuadTreeMaker quadrantTwo;
    private QuadTreeMaker quadrantThree;
    private QuadTreeMaker quadrantFour;

    private bool subdivided;

    public QuadTreeMaker(float width, float height, Vector2 center, int capacity)
    {
        this.width = width;
        this.height = height;
        this.center = center;
        this.capacity = capacity;
        points = new List<Vector2>(this.capacity);
        subdivided = false;
        halfWidth = this.width * 0.5f;
        halfHeight = this.height * 0.5f;
    }

    public void InsertPoint(Vector2 newPoint)
    {
        if (!InsideArea(newPoint))
        {
            return;
        }
        if(!subdivided && points.Count < capacity)
        {
            points.Add(newPoint);
        }
        else
        {
            if (!subdivided)
            {
                Subdivide();
                points.Clear();
            }
            quadrantOne.InsertPoint(newPoint);
            quadrantTwo.InsertPoint(newPoint);
            quadrantThree.InsertPoint(newPoint);
            quadrantFour.InsertPoint(newPoint);
        }
    }

    public List<Vector2> Query(float x, float y, float w, float h)
    {
        if(!Intersects(x, y, w, h))
        {
            return new List<Vector2>();
        }
        else
        {
            List<Vector2> foundPoints = new List<Vector2>();
            if (subdivided)
            {
                foundPoints.AddRange(quadrantOne.Query(x, y, w, h));
                foundPoints.AddRange(quadrantTwo.Query(x, y, w, h));
                foundPoints.AddRange(quadrantThree.Query(x, y, w, h));
                foundPoints.AddRange(quadrantFour.Query(x, y, w, h));
            }
            else
            {
                foreach(Vector2 p in points)
                {
                    if (InsideArea(p))
                    {
                        foundPoints.Add(p);
                    }
                }
            }
            return foundPoints;
        }
    }

    private bool Intersects(float x, float y, float w, float h)
    {
        return !(x - w > center.x + halfWidth || x + w < center.x - halfWidth || y - h > center.y + halfHeight || y + h < center.y - halfHeight);
    }

    private bool InsideArea(Vector2 point)
    {
        return point.x >= center.x - halfWidth && point.x < center.x + halfWidth && point.y >= center.y - halfHeight && point.y < center.y + halfHeight;
    }

    private void Subdivide()
    {
        float newHalfWidth = halfWidth * 0.5f;
        float newHalfHeight = halfHeight * 0.5f;
        quadrantOne = new QuadTreeMaker(halfWidth, halfHeight, new Vector2(center.x - newHalfWidth, center.y + newHalfHeight), capacity);
        quadrantTwo = new QuadTreeMaker(halfWidth, halfHeight, new Vector2(center.x + newHalfWidth, center.y + newHalfHeight), capacity);
        quadrantThree = new QuadTreeMaker(halfWidth, halfHeight, new Vector2(center.x - newHalfWidth, center.y - newHalfHeight), capacity);
        quadrantFour = new QuadTreeMaker(halfWidth, halfHeight, new Vector2(center.x + newHalfWidth, center.y - newHalfHeight), capacity);
        subdivided = true;
    }
}
