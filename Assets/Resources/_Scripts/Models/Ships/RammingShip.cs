using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingShip : Ship
{
    [Header("Ramming Ship Movement Variables")]
    public float thrustAcceleration = 20f;
    public float thrustDecaleration = 0.05f;
    public float emergencyStopSpeed = 2.3f;
    public float boostModifier = 2;
    public float topSpeed = 15f;
    [Header("Ramming Ship Combat Parameters")]
    [Tooltip("How long in seconds until the ship is able to be controlled again.")]
    public float recoveryTime = 1.5f;
    public float crashSpinModifier = 15f;

}
