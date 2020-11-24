using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TestEnum
{
    north = 0,
    east = 90,
    south = 180,
    west = 270
}

public class JaspersTests : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log((int)TestEnum.east);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
