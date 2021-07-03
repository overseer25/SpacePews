using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAICombat : MonoBehaviour
{
    public float Damage { get; set; } = 20f;
    public float ReloadTime { get; set; } = 2f;
    public float AcceptableAimingError { get; set; } = 5f;

    protected bool reloading = false;

    public virtual void Attack()
    {

    }

    public virtual void Reload()
    {

    }

    public virtual void Aim(Vector2 target)
    {

    }

    public virtual void Aim(GameObject targetObj)
    {
        Vector2 target = targetObj.transform.position;
        Aim(target);
    }

    public virtual void Aim(float targetX, float targetY)
    {
        Vector2 target = new Vector2(targetX, targetY);
        Aim(target);
    }
}
