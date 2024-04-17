using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Coroutine flinchCancel; //or flinchReset
    private Coroutine idleCancel;
    private Coroutine flinchOpportunityCancel;
    private Coroutine attackLengthCancel;

    private bool idleStart = true;
    private bool idle = false;
    private float idleTime = 1;
    public bool attackReady = false;

    private bool flinchInterrupt = false; //I may want to changethis to flincOpportuni
    private bool attack = false; //Putting this here for now. I want this code to be assimple as possible //Need this for now, because may not want to use idle (check for it
    //While attacking a foe
    private float attackLength = 1;

    public ParticleSystem[] attackEffects;
    private int effectNumber = 0;

    private int HP = 10;
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
            idleCancel = StartCoroutine(IdleAnimation(idleTime));
            Debug.Log("Id");
        }
        Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position -transform.position);
        transform.rotation = Quaternion.Slerp(new Quaternion(0,transform.rotation.y,transform.rotation.z,0), lookRotation,3);
    }

    // Update is called once per frame
    void Update()
    {
        //Quaternion lookRotation = Quaternion.LookRotation(transform.position, GameObject.Find("Main Camera").transform.position);
        //transform.rotation = Quaternion.Slerp(lookRotation, transform.rotation, 3);
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (playerScript.hitCount>=30)
            {
                playerScript.AllAttack();
            }
        }
        if (HP<=0)
        {
            Destroy(gameObject);
        }
    }
    //Setters
    public void SetHP(int newHP)
    {
        HP = newHP;
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
            playerScript.InterruptEffect(transform.position);
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

        //Don't know why I didn't put this here right away, because I successful flinch will always start a flinchdur
        flinchCancel = StartCoroutine(FlinchDuration());

        
    }
    IEnumerator FlinchDuration()
    {
        yield return new WaitForSeconds(1);
        if (animatorTrue==true)
        {
            animator.ResetTrigger("Flinch");
        }
        idleCancel = StartCoroutine(IdleAnimation(3));
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
        playerScript.PlayHurtEffect();
    }
    public void PlayAttackEffect(int attackEffect)
    {
        effectNumber = attackEffect;
        attackEffects[effectNumber].Play();
    }
    public void StopAttackEffect()
    {
        attackEffects[effectNumber].Stop();
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
    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    private void OnMouseOver()
    {
        if (playerScript.attackLagging == false)
        {
            if (Input.GetMouseButtonDown(0) &&playerScript.violinDrained==false)
            {
                //Debug.Log("Attacked!");
                playerScript.ViolinAttack(transform.position);
                HP--; 
                playerScript.HitCountUp();
                //Debug.Log("Attack!");
                if (attack == false)
                {
                    Flinch();

                    if (idleCancel != null)
                    {
                        StopCoroutine(idleCancel);
                    }

                    if (flinchCancel != null)
                    {
                        StopCoroutine(flinchCancel);
                    }

                }

                //Atm, I need Attack to go to flinch and for flinch to go to id

                if (attack == true)
                {
                    if (flinchInterrupt == true)
                    {
                        //I'm thinking of making attack ==false here, but I wasn't expecting my code to become this complicate
                        Flinch();

                    }
                }
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
                Destroy(other.gameObject);
                Flinch();
            }
        }
    }
}
