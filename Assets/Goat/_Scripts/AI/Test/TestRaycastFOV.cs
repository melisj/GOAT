using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycastFOV : MonoBehaviour
{
    public LayerMask targetMask, obstacleMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 10, targetMask))
        {
            Debug.LogFormat("Hit target: {0}", hit.transform.name);
        }
    }
}
