using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphinx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localRotation =new Quaternion(0, 45*Time.deltaTime,0,0);
        transform.Rotate(0, 90 * Time.deltaTime, 0, 0);
    }
}
