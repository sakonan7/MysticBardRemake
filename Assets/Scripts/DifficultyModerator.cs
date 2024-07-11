using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyModerator : MonoBehaviour
{
    public static bool normal = true;
    public bool normalNonStatic = true;
    public static bool hard = false;
    public bool hardNonStatic = false;
    // Start is called before the first frame update
    void Start()
    {
        if (hard == true)
        {
            normalNonStatic = false;
            hardNonStatic = true;
        }
        if (normal == true)
        {
            normalNonStatic = true;
            hardNonStatic = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetNormal()
    {
        normal = true;
        hard = false;
        normalNonStatic = true;
        hardNonStatic = false;
    }
    public void SetHard()
    {
        normal = false;
        hard = true;
        normalNonStatic = false;
        hardNonStatic = true;
    }
}
