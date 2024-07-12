using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For destruction, try instantiating a prefab instead. But still can't destroy right away, because I need to use the hitbox. Will need to turn off the 
//trigger
public class Bomb : MonoBehaviour
{
    private PlayerController playerScript;
    private AudioSource audio;
    private GameManager gameScript;
    private Coroutine explodeCancel;
    private Coroutine explodeEffectCancel;
    private GameObject effectPosition;
    private ParticleSystem aboutToExplode;
    private GameObject timeIndicator;
    public GameObject explosion;
    public AudioClip attackImpact;
    public AudioClip explosionSound;
    public AudioClip fuse;
    public bool timedBomb = false;
    public float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        audio = GetComponent<AudioSource>();
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        effectPosition = transform.Find("Effect Position").gameObject;
        
        aboutToExplode = transform.Find("About To Explode").gameObject.GetComponent<ParticleSystem>();
        

        if (timedBomb == false)
        {
            explodeCancel = StartCoroutine(TimedExplosion());
            explodeEffectCancel = StartCoroutine(AboutToExplode());
        }
        else
        {
            explodeCancel = StartCoroutine(SpecificTimedExplosion());
            explodeEffectCancel = StartCoroutine(SpecificAboutToExplode());
            StartCoroutine(TimeIndicatorDisappear());
            timeIndicator = transform.Find("Timer").gameObject;
        }
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
        if (transform.position.y <= -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
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
    IEnumerator TimedExplosion()
    {
        yield return new WaitForSeconds(6);
        transform.Find("Appearance").gameObject.SetActive(false);
        if (gameScript.playEffects ==true) {
            Instantiate(explosion, transform.position, transform.rotation);
        }
        StartCoroutine(ActualDamage());
        audio.Stop();
        //audio.PlayOneShot(attackImpact, 1);
        audio.PlayOneShot(explosionSound, 1.5f);
        aboutToExplode.Stop();
    }
    IEnumerator ActualDamage()
    {
        yield return new WaitForSeconds(0.5f);
        //atm, I want boosh-BAM

        Destroy(gameObject, 2);

        //Instantiate explosion effect, then 0.5 seconds, do actual damage
        //Will need to do destroy later, because there won't be anything to play damage codethedamage
        if (playerScript.shieldOn == false && playerScript.specialInvincibility == false)
        {
            playerScript.GeneralDamageCode(2, 3, false);
            //playerScript.PlayHurtEffect(effectAppear.transform.position);
            //playerScript.DamageFlashOn();
            audio.PlayOneShot(attackImpact, 1);
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
        aboutToExplode.Play();
        audio.PlayOneShot(fuse, 0.5f);
    }
    IEnumerator TimeIndicatorDisappear()
    {
        yield return new WaitForSeconds(3);
        timeIndicator.SetActive(false);
    }
    IEnumerator SpecificTimedExplosion()
    {
        yield return new WaitForSeconds(time);
        transform.Find("Appearance").gameObject.SetActive(false);
        if (gameScript.playEffects == true) { 
            Instantiate(explosion, transform.position, transform.rotation);
    }
        StartCoroutine(ActualDamage());
        audio.PlayOneShot(explosionSound, 1.5f);
        aboutToExplode.Stop();
    }
    IEnumerator SpecificAboutToExplode()
    {
        yield return new WaitForSeconds(time - 2);
        aboutToExplode.Play();
        audio.PlayOneShot(fuse, 0.5f);
    }
    public void EnemyExplode()
    {
        if (explodeEffectCancel != null)
        {
            StopCoroutine(explodeEffectCancel);
        }
        if (explodeCancel != null)
        {
            StopCoroutine(explodeCancel);
        }
        transform.Find("Appearance").gameObject.SetActive(false);
        //transform.Find("Hitbox").gameObject.SetActive(true);
        if (gameScript.playEffects == true)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }
        Destroy(gameObject, 2);
        audio.Stop();
        audio.PlayOneShot(explosionSound, 1.5f);
        //Debug.Log("Blow Up");
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
                        aboutToExplode.Stop();
                        //Debug.Log("Grabbed.");
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
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Bomb") || collision.gameObject.CompareTag("Debris"))
            {
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                EnemyExplode();
                playerScript.HitCountUp();
            }
            if (collision.gameObject.CompareTag("Debris"))
            {

                playerScript.WindHitEffect(collision.GetContact(0).point);
                Destroy(collision.gameObject);
                gameScript.ReduceNumDebris();
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
        if (other.CompareTag("Harp"))
        {
            EnemyExplode();
            //StopCoroutine(explodeEffectCancel);
        }
    }
}
