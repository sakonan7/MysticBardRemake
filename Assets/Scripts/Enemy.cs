using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I want to do some things I didn't do right in Beast Dominion
//Flinch
//Might as well do more shared stuff like idle animation here
//Less confusingcode
//StartingwithFlinch
//Itmakes sense, becauseIdle Animation is shared amongst enemies
public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Animation animation;
    private bool animatorTrue = false;
    private bool animationTrue = false;
    private Coroutine flinchCancel; //or flinchReset
    private Coroutine idleCancel;

    private bool idleStart = false;
    private bool idle = false;
    private float idleTime = 1;
    public bool attackReady = false;
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
            Flinch();

            if (idleCancel!=null)
            {
                StopCoroutine(idleCancel);
            }

            if (flinchCancel!=null) {
                StopCoroutine(flinchCancel);
            }
            flinchCancel = StartCoroutine(FlinchDuration());
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

    public void Flinch()
    {
        if (animatorTrue==true)
        {
            //animator.SetBool("Idle",false);
            animator.SetTrigger("Flinch");
        }
        else if (animationTrue == true)
        {
            //animator.SetTrigger("Flinch");
        }
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
    public void StartIdle()
    {
        idleCancel = StartCoroutine(IdleAnimation(idleTime));
    }
    //This makes sense because not all foes have the same idletime
    IEnumerator IdleAnimation(float idleTime)
    {
        idle = true;
        //if (animatorTrue == true)
        //{
            //animator.SetBool("Idle",true);
        //}
        yield return new WaitForSeconds(idleTime);
        idle = false;
        attackReady = true;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Attack!");
        }
    }
}
