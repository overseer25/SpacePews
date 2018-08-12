using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour {

    void FixedUpdate()
    {
        var pos = Input.mousePosition;
        pos.z = transform.position.z - Camera.main.transform.position.z;
        pos = Camera.main.ScreenToWorldPoint(pos);

        transform.rotation = Quaternion.FromToRotation(Vector3.up, pos - transform.position);
    }
}
