using System;
using System.Collections;
using UnityEngine;

public class RammingAI : MonoBehaviour
{
    public GameObject target = null;
    public GameObject[] patrolPoints;
    public ParticleEffect explosion;
    public AudioClip explosionSound;
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
    private bool patrolling;
    private int stunnedDir;
    private int patrolIndex;

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
        if (patrolPoints.Length == 0)
        {
            Debug.Log("This ramming ship has no patrol points set.");
        }
        avoidancePoint = ship.transform.position;
        rand = new System.Random();
        stunned = false;
        patrolling = true;
        stunnedTimer = 0;
        patrolIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (stunned)
        {
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
            if ((target == null || patrolling) && checkTimer > checkTime)
            {
                checkTimer = 0;
                CheckForPlayerTarget();
            }
            if (target == null && patrolling)
            {
                if (patrolPoints.Length > 0)
                {
                    target = patrolPoints[patrolIndex];
                }
            }
            //need to check to see if the player is dead
            if(target != null && target.transform.parent.GetComponentInChildren<PlayerController>())
            {
                if (target.transform.parent.GetComponentInChildren<PlayerController>().CheckIfDead())
                {
                    target = null;
                    patrolling = true;
                }
            }
            if (target == null)
            {
                motor.Decelerate(ship.thrustDecaleration);
                return;
            }
            else if (!patrolling && Vector2.Distance(ship.transform.position, target.transform.position) > range * 2)
            {
                SetTarget(null);
            }
            else if (patrolling)
            {
                GoToPatrolPoint();
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
        Vector2 targetVec = target.transform.position - ship.transform.position;
        float targetAngle = Vector2.SignedAngle(ship.transform.up, targetVec);
        float distFromTarget = Vector2.Distance(ship.transform.position, target.transform.position);
        bool canBoost = WithinBoostDistance(distFromTarget, range * 0.4f);
        float speed = canBoost ? ship.thrustAcceleration * ship.boostModifier : ship.thrustAcceleration;
        float topSpeed = canBoost ? ship.topSpeed * ship.boostModifier : ship.topSpeed;

        // when the ramming ai is within boost distance, it disregards obstacle avoidance in favor of hitting the player
        if (!avoiding && !canBoost && Math.Abs(targetAngle) <= 10)
        {
            ObstacleAvoidance(targetVec, distFromTarget);
        }
        CheckIfDoneAvoiding();
        if (avoiding)
        {
            targetVec = avoidancePoint - (Vector2)ship.transform.position;
            targetAngle = Vector2.SignedAngle(ship.transform.up, targetVec);
            speed = ship.thrustAcceleration;
            topSpeed = ship.topSpeed;
        }
        // aiming the enemy ship attack
        if (Math.Abs(targetAngle) > 10)
        {
            motor.Decelerate(ship.thrustDecaleration);
            if (targetAngle < 0)
            {
                motor.RotateRight();
            }
            else
            {
                motor.RotateLeft();
            }
        }
        // moving the enemy ship towards its target
        else
        {
            motor.MoveForward(speed, topSpeed);
        }
    }

    /// <summary>
    /// Have the AI go to a set patrol point
    /// </summary>
    private void GoToPatrolPoint()
    {
        Vector2 targetVec = target.transform.position - ship.transform.position;
        float targetAngle = Vector2.SignedAngle(ship.transform.up, targetVec);
        float distFromTarget = Vector2.Distance(ship.transform.position, target.transform.position);
        float speed = ship.thrustAcceleration;
        float topSpeed = ship.topSpeed;

        // when the ramming ai is within boost distance, it disregards obstacle avoidance in favor of hitting the player
        if (!avoiding && Math.Abs(targetAngle) <= 10)
        {
            ObstacleAvoidance(targetVec, distFromTarget);
        }
        CheckIfDoneAvoiding();
        if (avoiding)
        {
            targetVec = avoidancePoint - (Vector2)ship.transform.position;
            targetAngle = Vector2.SignedAngle(ship.transform.up, targetVec);
            speed = ship.thrustAcceleration;
            topSpeed = ship.topSpeed;
        }
        // aiming the enemy ship move
        if (Math.Abs(targetAngle) > 10)
        {
            motor.Decelerate(ship.thrustDecaleration);
            if (targetAngle < 0)
            {
                motor.RotateRight();
            }
            else
            {
                motor.RotateLeft();
            }
        }
        // moving the enemy ship towards its target
        else
        {
            motor.MoveForward(speed, topSpeed);
        }
        if (Vector2.Distance(ship.transform.position, patrolPoints[patrolIndex].transform.position) < 2)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            target = patrolPoints[patrolIndex];
        }
    }

    /// <summary>
    /// If enemy ship is close enough to avoidance point, stop avoiding.
    /// </summary>
    private void CheckIfDoneAvoiding()
    {
        if (Vector2.Distance(ship.transform.position, avoidancePoint) <= 1)
        {
            avoiding = false;
        }
    }

    private bool WithinBoostDistance(float distAway, float cutoffDist)
    {
        return distAway <= cutoffDist;
    }

    /// <summary>
    /// Check in the range of this enemy ship and see if there is a target
    /// that could be attacked.
    /// </summary>
    private void CheckForPlayerTarget()
    {
        LayerMask mask = LayerMask.GetMask("Player");
        Collider2D[] cols = Physics2D.OverlapCircleAll(ship.transform.position, range, mask);
        if (cols.Length == 0)
        {
            patrolling = true;
        }
        else
        {
            patrolling = false;
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
            avoiding = true;
            Vector2 leftPoint = new Vector2(ship.transform.position.x, ship.transform.position.y) + ((Vector2)ship.transform.right * -2);
            Vector2 rightPoint = new Vector2(ship.transform.position.x, ship.transform.position.y) + ((Vector2)ship.transform.right * 2);
            Vector2 leftTargetVec = leftPoint - (Vector2)target.transform.position;
            Vector2 rightTargetVec = rightPoint - (Vector2)target.transform.position;
            RaycastHit2D leftHit = Physics2D.Raycast(leftPoint, leftTargetVec, distAway, mask);
            RaycastHit2D rightHit = Physics2D.Raycast(rightPoint, rightTargetVec, distAway, mask);
            /*while(leftHit.collider != null && rightHit.collider != null)
            {
                leftPoint += ((Vector2)ship.transform.right * -1);
                rightPoint += ((Vector2)ship.transform.right * 1);
                leftTargetVec = leftPoint - (Vector2)target.transform.position;
                rightTargetVec = rightPoint - (Vector2)target.transform.position;
                leftHit = Physics2D.Raycast(leftPoint, leftTargetVec, distAway, mask);
                rightHit = Physics2D.Raycast(rightPoint, rightTargetVec, distAway, mask);
            }*/
            //if this is true, we should avoid left
            if (leftHit.collider == null)
            {
                avoidancePoint = leftPoint;
            }
            else
            {
                // if we get here that means left was blocked, so we are going to try to avoid right
                avoidancePoint = rightPoint;
            }

            //there is an obstacle, so place two points to either side of the ship
            //from those two points raycast again to the target. pick which one doesn't have an
            //obstacle in front of it. then fly to that avoidance point before recentering on the target.
            //if both avoidance points have obstacles, then pick one at random, and recenter on target
            //check avoidance will be called again and may find an answer this time
        }
        else
        {
            avoiding = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Ship":
            case "Player":
                if (motor.GetCurrentVelocity().magnitude > ship.topSpeed)
                {
                    stunnedDir = rand.Next(10) > 5 ? 1 : -1;
                    stunned = true;
                    Vector2 driftDir = new Vector2(ship.transform.right.x + rand.Next(-2, 2), ship.transform.right.y + rand.Next(-2, 2)) * stunnedDir;
                    motor.ShipImpact(10, stunnedDir, driftDir);
                    ship.health -= 1;
                }
                break;
            case "Mineable":
            case "Immovable":
            case "Asteroid":
                if (motor.GetCurrentVelocity().magnitude > ship.topSpeed)
                {
                    stunnedDir = rand.Next(10) > 5 ? 1 : -1;
                    stunned = true;
                    Vector2 driftDir2 = new Vector2(ship.transform.right.x + rand.Next(-2, 2), ship.transform.right.y + rand.Next(-2, 2)) * stunnedDir;
                    motor.ShipImpact(30, stunnedDir, driftDir2);
                    ship.health -= 3;
                }
                else
                {
                    Vector2 impactVec = (ship.transform.position - collider.transform.position).normalized;
                    motor.SetCurrentVelocity(impactVec);
                }
                break;
            default:
                break;
        }
        if (ship.health <= 0)
        {
            ParticleManager.PlayParticle(explosion, gameObject);
            transform.parent.gameObject.SetActive(false);
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
