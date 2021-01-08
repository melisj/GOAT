using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMovement : MonoBehaviour
{
    [SerializeField]
    GameObject explosion;
    void Update()
    {
       this.transform.position -= transform.up /10;
        Explode();
    }

    void Explode()
    {
        if (transform.position.y < -3)
        {
            Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        //doe particle
        //replace tiles met resources
    }
}
