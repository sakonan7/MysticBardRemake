using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For destruction, try instantiating a prefab instead. But still can't destroy right away, because I need to use the hitbox. Will need to turn off the 
//trigger
public class Bomb : MonoBehaviour
{
    private PlayerController playerScript;
    private Coroutine explodeCancel;
    private Coroutine explodeEffectCancel;
    private GameObject effectPosition;
    private GameObject aboutToExplode;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        effectPosition = transform.Find("Effect Position").gameObject;
        explodeCancel = StartCoroutine(TimedExplosion());
        aboutToExplode = transform.Find("About To Explode").gameObject;
        explodeEffectCancel = StartCoroutine(AboutToExplode());
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.wind==true) {
            if (transform.position.x <= -4.32f)
            {
                transform.position = new Vector3(-4.31f, transform.position.y, transform.position.z);
                playerScript.WindEnd();
                EnemyExplode();
            }
            if (transform.position.x >= 4.32f)
            {
                transform.position = new Vector3(4.31f, transform.position.y, transform.position.z);
                playerScript.WindEnd();
                EnemyExplode();
            }
            if (transform.position.y <= -3f)
            {
                transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
                playerScript.WindEnd();
                EnemyExplode();
            }
            if (transform.position.y >= 3f)
            {
                transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
                playerScript.WindEnd();
                EnemyExplode();
            }
        }
    }
    IEnumerator TimedExplosion()
    {
        yield return new WaitForSeconds(6);
        Destroy(gameObject);
        if (playerScript.shieldOn == false && playerScript.specialInvincibility == false)
        {
            playerScript.GeneralDamageCode(2, 0);
            //playerScript.PlayHurtEffect(effectAppear.transform.position);
            //playerScript.DamageFlashOn();
        }
        else if (playerScript.shieldOn == true || playerScript.specialInvincibility == true)
        {
            playerScript.GenerateShield(effectPosition.transform.position);
            if (playerScript.shieldOn == true)
            {
                playerScript.ShieldGaugeDown(2);
            }
        }
    }
    IEnumerator AboutToExplode()
    {
        yield return new WaitForSeconds(6-2);
        aboutToExplode.SetActive(true);
    }
    public void EnemyExplode()
    {
        if (explodeEffectCancel != null)
        {
            StopCoroutine(explodeEffectCancel);
        }
        transform.Find("Appearance").gameObject.SetActive(false);
        transform.Find("Hitbox").gameObject.SetActive(true);
        Destroy(gameObject, 2);
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
                        StopCoroutine(explodeCancel);
                        StopCoroutine(explodeEffectCancel);
                        aboutToExplode.SetActive(false);
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
            //WindCaptureEnd();
            //Debug.Log("End");
            EnemyExplode();
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
                EnemyExplode();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trumpet"))
        {
            EnemyExplode();
            //StopCoroutine(explodeEffectCancel);
        }
        if (other.CompareTag("Violin"))
        {
            EnemyExplode();
            //StopCoroutine(explodeEffectCancel);
        }
    }
}
