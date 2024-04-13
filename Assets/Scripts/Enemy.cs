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
        if (idleStart==true)
        {
            idleCancel = StartCoroutine(IdleAnimation(idleTime));
            Debug.Log("Id");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Attack!");
            if (attack==false)
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

            if (attack==true)
            {
                if (flinchInterrupt==true)
                {
                    //I'm thinking of making attack ==false here, but I wasn't expecting my code to become this complicate
                    Flinch();
                    
                }
            }
        }
    }
    //Setters
    public void SetIdleStart()
    {
        idleStart = true;     
    }
    public void SetIdleTime(float newTime)
    {
        idleTime = newTime;
    }
    //So far so good. The only problem is that I can't do Idle"", false. I need to keep snapping back to Idle
    //This'llonlybe a problem if I want a recover anima
    public void Flinch()
    {
        if (flinchInterrupt==true)
        {
            attack = false;
            flinchInterrupt = false;

            //I don't know why I don't think to put this allhere
            //I think I'm just thinking in the moment
            //Also, I have to cancel different Coroutines for different mo
            StopCoroutine(flinchOpportunityCancel);
            StopCoroutine(attackLengthCancel);
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
        yield return new WaitForSeconds(1);
        flinchInterrupt = false;
    }
    public void StartAttackLength()
    {
        attackLengthCancel =StartCoroutine(AttackLength());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator AttackLength()
    {
        yield return new WaitForSeconds(4f);
        //animator.ResetTrigger("Attack");
        //StartCoroutine(IdleAnimation());
        StartIdle();
    }
    public void StartIdle()
    {
        idleCancel = StartCoroutine(IdleAnimation(idleTime));
    }
    //This makes sense because not all foes have the same idletime
    IEnumerator IdleAnimation(float idleTime)
    {
        idle = true;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle",true);
            //animator.ResetTrigger("Attack");
        }
        yield return new WaitForSeconds(idleTime);
        idle = false;
        attackReady = true;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle", true);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attacked!");
        }
    }
}
