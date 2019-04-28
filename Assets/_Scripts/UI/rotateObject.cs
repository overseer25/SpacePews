using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObject : MonoBehaviour {

    public float rotationSpeed = 0.0f;

    void Start()
    {
        if (rotationSpeed == 0.0f) { randomRotationSpeed(); }
    }
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0.0f, 0.0f, rotationSpeed);
	}

    public void randomRotationSpeed()
    {
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
        rotationSpeed = (float) rand.NextDouble() * (0.05f - (-0.05f)) + (-0.05f);

    }
}
