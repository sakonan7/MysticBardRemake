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
        if (playerScript.wind == true)
        {
            if (transform.position.x <= -4.32f)
            {
                transform.position = new Vector3(-4.31f, transform.position.y, transform.position.z);
                //playerScript.WindEnd();
            }
            if (transform.position.x >= 4.32f)
            {
                transform.position = new Vector3(4.31f, transform.position.y, transform.position.z);
                //playerScript.WindEnd();
            }
            if (transform.position.y <= -3f)
            {
                transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
                //playerScript.WindEnd();
            }
            if (transform.position.y >= 3f)
            {
                transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
                //playerScript.WindEnd();
            }
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
        if (playerScript.wind == true)
        {
            //Wind off. Need wind variable for enemy
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Bomb"))
            {
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
            }
        }
    }
}
