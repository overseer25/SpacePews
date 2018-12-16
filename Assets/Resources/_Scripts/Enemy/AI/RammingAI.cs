using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingAI : MonoBehaviour
{
    public GameObject target = null;

    public float range = 75f;

    private Vector2 avoidancePoint;
    private EnemyMotor motor;
    private RammingShip ship;
    private static System.Random rand;
    private float checkTimer;
    private float checkTime;
    private float stunnedTimer;
    private bool stunned;
    private bool avoiding;
    private int stunnedDir;

    // Start is called before the first frame update
    void Start()
    {        
        checkTimer = 0;
        checkTime = 1f;
        avoiding = false;
        motor = this.GetComponent<EnemyMotor>();
        if (motor == null)
        {
            Debug.LogError("Motor does not exist on this enemy.");
        }
        ship = this.GetComponentInChildren<RammingShip>();
        if (ship == null)
        {
            Debug.LogError("Ship does not exist on this enemy.");
        }
        avoidancePoint = ship.transform.position;
        rand = new System.Random();
        stunned = false;
        stunnedTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
            Debug.Log("I am stunned");
            StunnedMotion();
            stunnedTimer += Time.deltaTime;
            if (stunnedTimer >= ship.recoveryTime)
            {
                stunnedTimer = 0;
                stunned = false;
            }
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (target == null && checkTimer > checkTime)
            {
                checkTimer = 0;
                CheckForTarget();
            }
            if (target == null)
            {
                motor.Decelerate(ship.thrustDecaleration);
                return;
            }
            else if (Vector2.Distance(ship.transform.position, target.transform.position) > range * 2)
            {
                SetTarget(null);
            }
            else
            {
                ExecuteAttack();
            }
        }
    }

    /// <summary>
    /// Begin the ramming attack of the enemy ship.
    /// </summary>
    private void ExecuteAttack()
    {
        Vector2 targetVec = ship.transform.position - target.transform.position;
        float targetAngle = Vector2.SignedAngle(ship.transform.up, targetVec);
        if (Math.Abs(targetAngle) < 170)
        {
            motor.Decelerate(ship.thrustDecaleration);
            if (targetAngle > 0)
            {
                motor.RotateRight();
            }
            else
            {
                motor.RotateLeft();
            }
        }
        else
        {
            float distFromTarget = Vector2.Distance(this.transform.position, target.transform.position);
            bool canBoost = WithinBoostDistance(distFromTarget, range * 0.4f);
            float speed = canBoost ? ship.thrustAcceleration * ship.boostModifier : ship.thrustAcceleration;
            float topSpeed = canBoost ? ship.topSpeed * ship.boostModifier : ship.topSpeed;
            // when the ramming ai is within boost distance, it disregards obstacle avoidance in favor of hitting the player
            if (!canBoost)
            {
                ObstacleAvoidance(targetVec, distFromTarget);
            }
            motor.MoveForward(speed, topSpeed);
        }
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, target.transform.position - transform.position);
    }

    private bool WithinBoostDistance(float distAway, float cutoffDist)
    {
        return distAway <= cutoffDist;
    }

    /// <summary>
    /// Check in the range of this enemy ship and see if there is a target
    /// that could be attacked.
    /// </summary>
    private void CheckForTarget()
    {
        //Debug.DrawLine(ship.transform.position, ship.transform.position + ship.transform.up * range, Color.red, 4f);
        LayerMask mask = LayerMask.GetMask("Player");
        Collider2D[] cols = Physics2D.OverlapCircleAll(ship.transform.position, range, mask);
        if (cols.Length == 0)
        {
            return;
        }
        else
        {
            target = cols[0].gameObject;
        }
    }

    /// <summary>
    /// When the ship is in a stunned state, spin it based on given impact direction.
    /// </summary>
    private void StunnedMotion()
    {
        motor.Decelerate(ship.emergencyStopSpeed);
        if (stunnedDir == 1)
        {
            motor.RotateRight(ship.crashSpinModifier, false);
        }
        else if (stunnedDir == -1)
        {
            motor.RotateLeft(ship.crashSpinModifier, false);
        }
    }

    private void ObstacleAvoidance(Vector2 targetVec, float distAway)
    {
        LayerMask mask = LayerMask.GetMask("Obstacle");
        //may want to change this to RaycastNonAlloc
        RaycastHit2D hit = Physics2D.Raycast(ship.transform.position, targetVec, distAway, mask);
        if (hit.collider != null)
        {
            //there is an obstacle, so place two points to either side of the ship
            //from those two points raycast again to the target. pick which one doesn't have an
            //obstacle in front of it. then fly to that avoidance point before recentering on the target.
            //if both avoidance points have obstacles, then pick one at random, and recenter on target
            //check avoidance will be called again and may find an answer this time
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.tag);
        switch (collider.gameObject.tag)
        {
            case "Ship":
            case "Player":
                stunnedDir = rand.Next(10) > 5 ? 1 : -1;
                stunned = true;
                Vector2 driftDir = new Vector2(ship.transform.right.x + rand.Next(-2,2), ship.transform.right.y + rand.Next(-2, 2)) * stunnedDir;
                motor.ShipImpact(10, stunnedDir, driftDir);
                ship.health -= 1;
                break;
            case "Mineable":
            case "Immovable":
            case "Asteroid":
                stunnedDir = rand.Next(10) > 5 ? 1 : -1;
                stunned = true;
                Vector2 driftDir2 = new Vector2(ship.transform.right.x + rand.Next(-2, 2), ship.transform.right.y + rand.Next(-2, 2)) * stunnedDir;
                motor.ShipImpact(30, stunnedDir, driftDir2);
                ship.health -= 3;
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Manually set which target the this enemy should go for.
    /// </summary>
    public void SetTarget(GameObject _target)
    {
        target = _target;
    }
}
