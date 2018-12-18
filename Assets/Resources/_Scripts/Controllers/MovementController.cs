using System.Linq;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    private Vector3 velocity;
    private Rigidbody2D rigidBody;
    private float desiredRotation;
    private ShipMountController mountController;
    private float acceleration;
    private float deceleration;
    private float maxSpeed;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;
    private Ship _ship;

    void Start()
    {
        velocity = Vector3.zero;
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        mountController = gameObject.GetComponent<ShipMountController>();
        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer D:");
        else
        {
            ship = shipRenderer.gameObject;
            _ship = ship.GetComponent<Ship>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position,
                                        new Vector3(ship.transform.position.x, ship.transform.position.y, Camera.main.transform.position.z), rigidBody.velocity.magnitude * Time.deltaTime);
    }

    /// <summary>
    /// Move the ship forward.
    /// </summary>
    public void MoveForward()
    {
        foreach(var thruster in mountController.GetThrusterMounts())
        {
            acceleration = mountController.GetThrusterMounts().Sum(t => (t.GetShipComponent() as ThrusterComponent).acceleration);
            maxSpeed = mountController.GetThrusterMounts().Sum(t => (t.GetShipComponent() as ThrusterComponent).maxSpeed);
        }
        if (!_ship.engine.isPlaying)
            _ship.engine.Play();
        rigidBody.AddForce(ship.transform.up * acceleration * 10.0f * Time.deltaTime);
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
    }
    /// <summary>
    /// Rotates the ship to the left.
    /// </summary>
    public void RotateLeft()
    {
        if (ship == null) { return; }

        desiredRotation += _ship.rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, _ship.rotationSpeed * Time.deltaTime);
        rigidBody.velocity = ship.transform.up * rigidBody.velocity.magnitude;
    }

    /// <summary>
    /// Rotates the ship to the right.
    /// </summary>
    public void RotateRight()
    {
        if (ship == null) { return; }

        desiredRotation -= _ship.rotationSpeed * Time.deltaTime;
        var rotationQuaternion = Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, desiredRotation);
        ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation, rotationQuaternion, _ship.rotationSpeed * Time.deltaTime);
        rigidBody.velocity = ship.transform.up * rigidBody.velocity.magnitude;
    }

    /// <summary>
    /// Slow down the ship when no button is being pressed.
    /// </summary>
    public void Decelerate()
    {
        foreach (var thruster in mountController.GetThrusterMounts())
        {
            deceleration = mountController.GetThrusterMounts().Sum(t => (t.GetShipComponent() as ThrusterComponent).deceleration);
        }
        if (_ship.engine.isPlaying)
            _ship.engine.Stop();
        if (rigidBody.velocity.magnitude > 0)
            rigidBody.velocity *= (1 - deceleration);
    }
}
