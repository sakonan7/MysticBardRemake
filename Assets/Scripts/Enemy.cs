using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//I want to do some things I didn't do right in Beast Dominion
//Flinch
//Might as well do more shared stuff like idle animation here
//Less confusingcode
//StartingwithFlinch
//Itmakes sense, becauseIdle Animation is shared amongst enemies

//This game is a testament to what I learned during Beast Dominion
//I didn't expect to be using exact code
//I want to make this fast and playable right away

//Remembering how complicated Beast Dominion Actually Was
//The Data Fights In Kingdom Hearts III Are Definitely Hard. I think they put a stagger window between certain attacks. A Corout

//TaskList
//Make Foe Not Spazz Between Idle And Att
public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Animation animation;
    private bool animatorTrue = false;
    private bool animationTrue = false;
    private PlayerController playerScript;
    private GameObject effectPosition;

    private Coroutine cancelDamageDisplay;
    private Coroutine flinchCancel; //or flinchReset
    private Coroutine idleCancel;
    private Coroutine flinchOpportunityCancel;
    private Coroutine attackLengthCancel;

    private GameObject[] enemies;

    private bool idleStart = true;
    private bool idle = false;
    private float idleTime = 1;
    public bool attackReady = false;

    private bool flinchInterrupt = false; //I may want to changethis to flincOpportuni
    private bool attack = false; //Putting this here for now. I want this code to be assimple as possible //Need this for now, because may not want to use idle (check for it
    //While attacking a foe
    private float attackLength = 1;
    private bool windCaptured = false;
    private bool repeat = true;
    private bool flinching = false;
    private bool barrier = false;

    public GameObject effectAppear;
    public ParticleSystem[] attackEffects;
    private int effectNumber = 0;

    //public gameOb
    public GameObject teamAttackAura;

    private float HP = 10;
    private float damage = 0;
    //Individual enemy abilit
    private bool teamAttack = false;
    private bool normal = false;
    public bool teamAttackOn = false;
    private bool red = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animation = GetComponent<Animation>();
        if (animator!=null)
        {
            animatorTrue = true;
        }
        else if (animation != null)
        {
            animationTrue = true;
        }

        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        if (idleStart==true)
        {
            idleCancel = StartCoroutine(IdleAnimation(Random.Range(4, 10)));
            //Debug.Log("Id");
        }
        Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position -transform.position);
        transform.rotation = Quaternion.Slerp(new Quaternion(0,transform.rotation.y,transform.rotation.z,0), lookRotation,3);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        effectPosition=transform.Find("Effect Position").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (windCaptured==false) {
            //Quaternion lookRotation = Quaternion.LookRotation(transform.position, GameObject.Find("Look At").transform.position);
            //transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);
        }
        if (HP<=0)
        {
            WindCaptureEnd();
            Destroy(gameObject);
        }
        if (transform.position.x <= -4.32f)
        {
            transform.position = new Vector3(-4.31f, transform.position.y, transform.position.z);
            playerScript.WindEnd();
            WindCaptureEnd();
        }
        if (transform.position.x >= 4.32f)
        {
            transform.position = new Vector3(4.31f, transform.position.y, transform.position.z);
            playerScript.WindEnd();
            WindCaptureEnd();
        }
        if (transform.position.y <= -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
            playerScript.WindEnd();
            WindCaptureEnd();
        }
        if (transform.position.y >= 3f)
        {
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            playerScript.WindEnd();
            WindCaptureEnd();
        }

        if (windCaptured ==true && playerScript.wind==true)
        {
            if (repeat ==true) {
                Flinch();
                repeat = false;
                StartCoroutine(WindFlinch());
                StartCoroutine(WindDamage());
            }
            transform.Rotate(Vector3.up * 180 * Time.deltaTime);
        }
    }
    //Setters
    public void SetHP(int newHP)
    {
        HP = newHP;
    }
    //05/01/24
    //I can either set the damage at the start(), or check for team att
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
        if (teamAttackOn ==true)
        {
            damage++;
        }
    }
    public void SetIdleStart()
    {
        idleStart = true;     
    }
    public void SetIdleTime(float newTime)
    {
        idleTime = newTime;
    }
    public void SetAttackLength(float newLength)
    {
        attackLength = newLength;
    }
    public void SetNormal()
    {
        normal = true;
    }
    public void SetTeamAttack()
    {
        teamAttack = true;
    }
    public void SetRed()
    {
        red = true;
    }
    public void DamageText(float damage)
    {
        gameObject.transform.Find("Damage Received").GetComponent<TextMesh>().text= ""+damage;
    }
    IEnumerator DamageDisplayDuration(float damage)
    {
        DamageText(damage);
        yield return new WaitForSeconds(0.5f);
        gameObject.transform.Find("Damage Received").GetComponent<TextMesh>().text = "";
    }
    //I think I'm going to need every enemy to call this. This is so I don't need to keep running this check
    //This will only be an issue if enemies can teleport
    public void AnalyzeTeamAttackCapability()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float distance;
        bool enemyNextToAnother = false;
        int i = 0;
        //for (int i =0; i < enemies.Length; i++)
        //{
            while (i < enemies.Length &&enemyNextToAnother ==false) {
                distance = Vector3.Distance(gameObject.transform.position, enemies[i].transform.position);
                i++;
                if (distance <= 0.5f)
                {
                    enemyNextToAnother = true;
                    if (teamAttack == true && teamAttackOn == false)
                    {
                        teamAttackOn = true;
                        TeamAttackPositives();
                    }
                }
            }
        //}
        if (enemyNextToAnother==false)
        {
            teamAttackOn = false;
            TeamAttackOff();
            Debug.Log("No enemies next to each");
        }
    }
    //So far so good. The only problem is that I can't do Idle"", false. I need to keep snapping back to Idle
    //This'llonlybe a problem if I want a recover anima
    public void Flinch()
    {
        attackReady = false;
        if (flinchInterrupt==true)
        {
            attack = false;
            flinchInterrupt = false;

            //I don't know why I don't think to put this allhere
            //I think I'm just thinking in the moment
            //Also, I have to cancel different Coroutines for different mo
            StopCoroutine(flinchOpportunityCancel);
            StopCoroutine(attackLengthCancel);
            playerScript.InterruptEffect(effectAppear.transform.position);
            StopAttackEffect();
        }

        if (animatorTrue==true)
        {
            animator.SetBool("Idle",true);
            animator.SetTrigger("Flinch");
            animator.ResetTrigger("Attack");
        }
        else if (animationTrue == true)
        {
            //animator.SetTrigger("Flinch");
        }

        //Moved this from mouseOver 
        if (idleCancel != null)
        {
            StopCoroutine(idleCancel);
        }

        if (flinchCancel != null)
        {
            StopCoroutine(flinchCancel);
        }

        //Don't know why I didn't put this here right away, because I successful flinch will always start a flinchdur
        flinchCancel = StartCoroutine(FlinchDuration());

        
    }
    IEnumerator FlinchDuration()
    {
        flinching = true;
        yield return new WaitForSeconds(2);
        if (animatorTrue==true)
        {
            animator.ResetTrigger("Flinch");
        }
        flinching = false;
        idleCancel = StartCoroutine(IdleAnimation(3));
    }
    IEnumerator WindFlinch()
    {
        yield return new WaitForSeconds(1);
        repeat = true;
    }
    IEnumerator WindDamage()
    {
        yield return new WaitForSeconds(1.25f);
        TakeDamage(1);
    }
    public void StartFlinchWindow()
    {
        flinchOpportunityCancel =StartCoroutine(FlinchWindow());
    }
    //If you hit the foe with the right attack during this window, their attack will be interr
    IEnumerator FlinchWindow()
    {
        flinchInterrupt = true;
        yield return new WaitForSeconds(0.5f);
        flinchInterrupt = false;
    }
    //I forgot to do AttackReadyOff() after an attack, to not repeat the attack. This lead to other problems 04/15/24
    public void AttackReadyOff()
    {
        attackReady = false;
    }
    public void StartAttackLength()
    {
        attackLengthCancel =StartCoroutine(AttackLength());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator AttackLength()
    {
        yield return new WaitForSeconds(attackLength);
        //animator.ResetTrigger("Attack");
        //StartCoroutine(IdleAnimation());
        StartIdle();
        DealDamage(damage);
    }
    public void PlayAttackEffect(int attackEffect)
    {
        effectNumber = attackEffect;
        attackEffects[effectNumber].Play();
    }
    public void StopAttackEffect()
    {
        //attackEffects[effectNumber].Stop();
    }
    public void DealDamage(float newDamage)
    {
        if (playerScript.shieldOn==false && playerScript.specialInvincibility == false)
        {
            playerScript.GeneralDamageCode(newDamage, newDamage);
            //playerScript.PlayHurtEffect(effectAppear.transform.position);
            playerScript.DamageFlashOn();
        }
        else if(playerScript.shieldOn==true || playerScript.specialInvincibility ==true)
        {
            playerScript.GenerateShield(effectPosition.transform.position);
            if(playerScript.shieldOn==true)
            {
                playerScript.ShieldGaugeDown(newDamage);
            }
        }
    }
    public void StartIdle()
    {
        idleCancel = StartCoroutine(IdleAnimation(idleTime));
    }
    //This makes sense because not all foes have the same idletime
    IEnumerator IdleAnimation(float idleTime)
    {
        idle = true;
        attack = false;
        attackReady = false;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle",true);
            //animator.ResetTrigger("Attack");
        }
        yield return new WaitForSeconds(idleTime);
        idle = false;
        attack = true;
        attackReady = true;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle", false);
        }
    }
    public void TakeDamage(float damage)
    {
        if (barrier ==true&&windCaptured==false)
        {
            damage /= 2;
        }
        HP -= damage;
        //DamageText(damage);
        if (cancelDamageDisplay !=null)
        {
            StopCoroutine(cancelDamageDisplay);
        }
        cancelDamageDisplay = StartCoroutine(DamageDisplayDuration(damage));
    }
    public void WindCaptureEnd()
    {
        windCaptured = false;
        Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);
        Debug.Log("Wind " + windCaptured);
    }
    public void TeamAttackPositives()
    {
        teamAttackAura.SetActive(true);
        damage++;
    }
    public void TeamAttackOff()
    {
        teamAttackAura.SetActive(false);
        damage--;
    }

    private void OnMouseOver()
    {
        if (playerScript.lag == false)
        {
            if (Input.GetMouseButtonDown(0) &&playerScript.violinDrained==false)
            {
                //Debug.Log("Attacked!");
                //playerScript.ViolinAttack(transform.position);
                //HP--; 
                //playerScript.HitCountUp();
                //Debug.Log("Attack!");

                //I think I will always call Flinch(), but the method will determine if the foe will stagger or
                //if (attack == false)
                //{
                    //Flinch();



                //}

                //Atm, I need Attack to go to flinch and for flinch to go to id

                //if (attack == true)
                //{
                    //if (flinchInterrupt == true)
                    //{
                        //I'm thinking of making attack ==false here, but I wasn't expecting my code to become this complicate
                        //Flinch();

                    //}
                //}
            }
        }
        if (normal == false)
        {
            if (playerScript.flute ==true &&playerScript.wind==false)
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
            }
        }
    }
    private void OnMouseExit()
    {
        if (normal == false)
        {
            if (playerScript.flute == true)
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
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
                        //TrumpetAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                        if (normal == true) {
                            playerScript.FluteAttack();
                            playerScript.WindOn();
                            windCaptured = true;
                        }
                    }
                }
            }
        }
    }
    private void OnMouseDrag()
    {
        //Debug.Log("Dragged");
        if (playerScript.wind==true)
        {
            
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 7.59f));
        }
    }
    private void OnMouseUp()
    {
        if (playerScript.wind == true)
        {
            playerScript.WindEnd();
            WindCaptureEnd();
            Debug.Log("End");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (playerScript.wind==true)
        {
            //Wind off. Need wind variable for enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                bool damaged = false;
                if (damaged == false)
                {
                    damaged = true;
                    TakeDamage(3);
                    //Destroy(other.gameObject);
                    Flinch();
                }
                gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                //I need to cancel out a bunch of coroutines
                //if (windCaptured==true) {
                    WindCaptureEnd();
                //}
                    playerScript.WindHitEffect(collision.GetContact(0).point);

            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (teamAttack == true && teamAttackOn == false)
            {
                teamAttackOn = true;
                TeamAttackPositives();
                Debug.Log("Team Attack On");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (teamAttack == true)
            {
                TeamAttackOff();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trumpet"))
        {
            bool damaged = false;
            if (damaged ==false)
            {
                damaged = true;
                TakeDamage(2);
                //Destroy(other.gameObject);
                Flinch();
                playerScript.HitCountUp();
                playerScript.TrumpetHitEffect(effectPosition.transform.position);
                playerScript.TrumpetHitEffect(effectPosition.transform.position);
            }
        }
        if (other.CompareTag("Violin"))
        {
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                TakeDamage(1);
                //Damage(1);
                //Destroy(other.gameObject);
                if (red==false) {
                    Flinch();
                }
                playerScript.HitCountUp();
                playerScript.ViolinHitEffect(effectPosition.transform.position);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            barrier = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            barrier = false;
        }
    }
}
