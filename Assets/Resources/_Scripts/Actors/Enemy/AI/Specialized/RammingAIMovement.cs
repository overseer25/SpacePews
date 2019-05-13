using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingAIMovement : BaseAIMovement
{
    public float StoppingDistance = 3f;
    public float AcceptableAngleMarginOfError = 1f;

    // DEBUG
    private static bool following = false;
    // DEBUG
    public GameObject targetObj;

    private AStarPathfinding pathfinding;
    private List<Node> path = null;

    private void Start()
    {
        pathfinding = GetComponent<AStarPathfinding>();
    }

    /// <summary>
    /// A function to cause a body to rotate towards a target until
    /// fully pointing at the target, and move towards the target until
    /// it is within StoppingDistance range.
    /// </summary>
    /// <param name="target">Target the body is trying to follow.</param>
    /// <param name="speed">How fast should the body move to follow the target.</param>
    /// <param name="rotationSpeed">How fast should the body rotate to point towards target.</param>
    public void FollowMove(Vector2 target, float speed = 1, float rotationSpeed = 1)
    {
        bool doneTurning = PointAtTarget(target, rotationSpeed);
        if (doneTurning)
        {
            MoveTowardsTarget(target, speed);
        }
        else
        {
            MoveTowardsTarget(target, speed / 3);
        }
    }

    /// <summary>
    /// A function to cause a body to rotate towards a target until
    /// fully pointing at the target, and move towards the target until
    /// it is within StoppingDistance range.
    /// </summary>
    /// <param name="targetObj">Target object the body is trying to follow.</param>
    /// <param name="speed">How fast should the body move to follow the target object.</param>
    /// <param name="rotationSpeed">How fast should the body rotate to point towards target object.</param>
    public void FollowMove(GameObject targetObj, float speed = 1, float rotationSpeed = 1)
    {
        Vector2 target = targetObj.transform.position;
        FollowMove(target, speed, rotationSpeed);
    }

    /// <summary>
    /// A function to cause a body to rotate towards a target until
    /// fully pointing at the target, and move towards the target until
    /// it is within StoppingDistance range.
    /// </summary>
    /// <param name="targetX">An X coordinate in space the body is trying to follow.</param>
    /// <param name="targetY">A Y coordinate in space the body is trying to follow.</param>
    /// <param name="speed">How fast should the body move to follow the point in space.</param>
    /// <param name="rotationSpeed">How fast should the body rotate to point towards point in space.</param>
    public void FollowMove(float targetX, float targetY, float speed = 1, float rotationSpeed = 1)
    {
        Vector2 target = new Vector2(targetX, targetY);
        FollowMove(target, speed, rotationSpeed);
    }

    /// <summary>
    /// Rotates body to face target with forward vector by rotating clockwise or
    /// counterclockwise the shortest distance.
    /// </summary>
    /// <param name="target">The target vector point that is being oriented towards.</param>
    /// <param name="rotationSpeed">How quickly does the body rotate towards the target.</param>
    /// <returns>Returns true if the heading is within the acceptable error angle.</returns>
    public bool PointAtTarget(Vector2 target, float rotationSpeed = 1)
    {
        Vector2 currentVec = GetCurrentForwardDirection();
        Vector2 targetDir = target - (Vector2)rigidbody.transform.position;
        if(Vector2.Angle(currentVec, targetDir) < AcceptableAngleMarginOfError)
        {
            return true;
        }
        bool dir = GetRotationDirection(currentVec, targetDir);
        if (dir)
        {
            RotateClockwise(rotationSpeed);
        }
        else
        {
            RotateCounterClockwise(rotationSpeed);
        }
        currentVec = GetCurrentForwardDirection();
        return Vector2.Angle(currentVec, targetDir) < AcceptableAngleMarginOfError;
    }

    /// <summary>
    /// Rotates body to face target with forward vector by rotating clockwise or
    /// counterclockwise the shortest distance.
    /// </summary>
    /// <param name="targetObj">The target object in the scene that is being oriented towards.</param>
    /// <param name="rotationSpeed">How quickly does the body rotate towards the target.</param>
    /// <returns>Returns true if the heading is within the acceptable error angle.</returns>
    public bool PointAtTarget(GameObject targetObj, float rotationSpeed = 1)
    {
        Vector2 target = targetObj.transform.position;
        return PointAtTarget(target, rotationSpeed);
    }

    /// <summary>
    /// Rotates body to face target with forward vector by rotating clockwise or
    /// counterclockwise the shortest distance.
    /// </summary>
    /// <param name="targetX">The target X coordinate in space that the body is trying to point towards.</param>
    /// <param name="targetY">The target Y coordinate in space that the body is trying to point towards.</param>
    /// <param name="rotationSpeed">How quickly does the body rotate towards the target.</param>
    /// <returns>Returns true if the heading is within the acceptable error angle.</returns>
    public bool PointAtTarget(float targetX, float targetY, float rotationSpeed = 1)
    {
        Vector2 target = new Vector2(targetX, targetY);
        return PointAtTarget(target, rotationSpeed);
    }

    /// <summary>
    /// Move the body towards the target, if not within the stopping distance.
    /// </summary>
    /// <param name="target">Target that the body is moving towards.</param>
    /// <param name="speed">How quickly the body moves toward the target.</param>
    /// <returns>Returns the remaining distance between the body and the target.</returns>
    public float MoveTowardsTarget(Vector2 target, float speed = 1)
    {
        Vector2 currentPos = rigidbody.transform.position;
        if(Vector2.Distance(currentPos, target) <= StoppingDistance)
        {
            return 0;
        }
        MoveForward(speed);
        currentPos = rigidbody.transform.position;
        return Vector2.Distance(currentPos, target);
    }

    /// <summary>
    /// Move the body towards the target, if not within the stopping distance.
    /// </summary>
    /// <param name="targetObj">Target object that the body is moving towards.</param>
    /// <param name="speed">How quickly the body moves toward the target.</param>
    /// <returns>Returns the remaining distance between the body and the target.</returns>
    public float MoveTowardsTarget(GameObject targetObj, float speed = 1)
    {
        Vector2 target = targetObj.transform.position;
        return MoveTowardsTarget(target, speed);
    }

    /// <summary>
    /// Move the body towards the target, if not within the stopping distance.
    /// </summary>
    /// <param name="targetX">X coordinate that the body is moving towards.</param>
    /// <param name="targetY">Y coordinate that the body is moving towards.</param>
    /// <param name="speed">How quickly the body moves toward the target.</param>
    /// <returns>Returns the remaining distance between the body and the target.</returns>
    public float MoveTowardsTarget(float targetX, float targetY, float speed = 1)
    {
        Vector2 target = new Vector2(targetX, targetY);
        return MoveTowardsTarget(target, speed);
    }

    public void TurnOnFollow()
    {
        following = true;
    }

    public void TurnOffFollow()
    {
        following = false;
    }

    public List<Node> GetPath()
    {
        return path;
    }

    protected override void Update()
    {
        base.Update();
        path = pathfinding.FindPath(rigidbody.transform.position, targetObj.transform.position);
        if (following && path != null)
        {
            FollowMove(path[0].position, Speed, RotationSpeed);
        }
    }

}
