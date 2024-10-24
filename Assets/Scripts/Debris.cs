using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For destruction, try instantiating a prefab instead. But still can't destroy right away, because I need to use the hitbox. Will need to turn off the 
//trigger
public class Debris : MonoBehaviour
{
    private PlayerController playerScript;

    public bool windCaptured = false;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -5.49f)
        {
            transform.position = new Vector3(-5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.x >= 5.49f)
        {
            transform.position = new Vector3(5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.y <= -2f)
        {
            transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.y >= 3.47f)
        {
            transform.position = new Vector3(transform.position.x, 3.47f, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
    }

    private void OnMouseDown()
    {
        if (playerScript.wind == false)
        {
            if (playerScript.flute == true)
            {
                if (playerScript.fluteDrained == false)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        playerScript.FluteAttack();
                        playerScript.WindOn();
                    }
                }
            }
        }
    }
    private void OnMouseDrag()
    {
        //Debug.Log("Dragged");
        if (playerScript.wind == true)
        {

            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 7.59f));
        }
    }
    private void OnMouseUp()
    {
        if (playerScript.wind == true)
        {
            playerScript.WindEnd();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bomb")) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
