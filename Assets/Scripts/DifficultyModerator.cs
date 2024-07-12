using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyModerator : MonoBehaviour
{
    private PlayerController player;
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
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetNormal()
    {
        if (hard == true)
        {
            player.PlayToggleOn();
        }
        normal = true;
        hard = false;
        normalNonStatic = true;
        hardNonStatic = false;
        GameObject.Find("Difficulty Buttons").transform.Find("Normal Selected").gameObject.SetActive(true);
        GameObject.Find("Difficulty Buttons").transform.Find("Hard Selected").gameObject.SetActive(false);
    }
    public void SetHard()
    {
        if (normal == true)
        {
            player.PlayToggleOn();
        }
        normal = false;
        hard = true;
        normalNonStatic = false;
        hardNonStatic = true;
        GameObject.Find("Difficulty Buttons").transform.Find("Normal Selected").gameObject.SetActive(false);
        GameObject.Find("Difficulty Buttons").transform.Find("Hard Selected").gameObject.SetActive(true);
    }
}
