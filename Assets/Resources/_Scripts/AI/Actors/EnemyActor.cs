using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("AI/Actors/Enemy Actor"))]
public class EnemyActor : ActorBase
{
	public float maxSpeed;

	public float acceleration;

	public float minRotationSpeed;

	public float maxRotationSpeed;

	public float minDistance;

	public float maxDistance;

	public float dashSpeed;

	public float dashCooldown;
}
