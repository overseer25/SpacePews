using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals with moving an enemy. The movement controlls within should be called from the enemy's state
/// machine, to ensure that the enemy is controlled by it's current state(s).
/// </summary>
public class EnemyMovementController : MonoBehaviour
{
	private EnemyActor enemyStats;
	private Rigidbody2D body;
	private float currentSpeed;
	private bool dashing;

    void Start()
    {
		currentSpeed = 0;
		body = GetComponent<Rigidbody2D>();
		enemyStats = gameObject.GetComponent<StateController>().actor as EnemyActor;
    }

	/// <summary>
	/// Move the enemy forward.
	/// </summary>
	public void MoveForward()
	{
		var pos = transform.up * enemyStats.acceleration * Time.deltaTime;
		body.AddForce(pos);
        if (body.velocity.magnitude > enemyStats.maxSpeed)
            Decelerate();
	}

    public void Decelerate()
    {
        if(body.velocity.magnitude > 0)
            body.velocity = Vector2.Lerp(body.velocity, Vector2.zero, Time.deltaTime * enemyStats.acceleration);
    }

    /// <summary>
    /// Move the enemy toward a point.
    /// </summary>
    /// <param name="point"></param>
    public void MoveTowardPoint(Vector2 point)
	{
		if(!dashing)
		{
			var dist = Vector2.Distance(point, body.position);
			float rotationSpeed;
			if (dist > enemyStats.maxDistance)
				rotationSpeed = enemyStats.minRotationSpeed;
			else if (dist < enemyStats.minDistance)
				rotationSpeed = enemyStats.maxRotationSpeed;
			else
			{
				var ratio = (dist - enemyStats.maxDistance) / (enemyStats.minDistance - enemyStats.maxDistance);
				rotationSpeed = (ratio * (enemyStats.maxRotationSpeed - enemyStats.minRotationSpeed)) + enemyStats.maxRotationSpeed;
			}

			var angle = MathUtils.AngleBetweenTwoVectors(body.position, point) + 90.0f;
			var result = Mathf.LerpAngle(body.rotation, angle, rotationSpeed * Time.deltaTime);
			body.MoveRotation(result);
		}
		MoveForward();
	}

	/// <summary>
	/// Dash forward.
	/// </summary>
	public void Dash()
	{
		currentSpeed = enemyStats.dashSpeed;
		var newPos = body.position + (Vector2)transform.up * currentSpeed * Time.deltaTime;
		MoveDirection(newPos);
		dashing = true;
	}

	public void MoveDirection(Vector2 direction)
	{
        body.velocity += direction;
        if (body.velocity.magnitude > enemyStats.maxSpeed)
            Decelerate();
    }

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("Collided");
		switch(collider.gameObject.tag)
		{
			default:
				var direction = (body.position - (Vector2)collider.transform.position).normalized * enemyStats.maxSpeed;
				MoveDirection(direction);
				break;
		}
	}

}
