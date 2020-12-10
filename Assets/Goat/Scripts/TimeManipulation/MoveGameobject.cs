using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameobject : MonoBehaviour
{
    private float movementSpeed = 1f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(movementSpeed, 0, 0) * Time.deltaTime);

        if (transform.position.x > 10f || transform.position.x < -10f)
        {
            movementSpeed = -movementSpeed;
        }
    }
}