using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenThief : MonoBehaviour
{
    public GameObject flashing;
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetHP(80);
        enemyScript.SetEXP(90);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(7);
        enemyScript.SetGreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.counterAttackTriggered == true)
        {
            StartCoroutine(Flashing());
            CounterAttack();
        }
        if (enemyScript.attackReady == true)
        {
            if (enemyScript.unflinchingFollow == true)
            {
                StartCoroutine(Flashing());
            }
            RegularAttack();
            //Debug.Log("Attack");
        }
    }
    IEnumerator Flashing()
    {
        int numFlash = 0;
        while (numFlash < 2)
        {
            flashing.SetActive(true);
            //Debug.Log("White");
            yield return new WaitForSeconds(1);
            flashing.SetActive(false);


            numFlash++;

            Debug.Log(numFlash);
        }
    }
    //I need a revenge value
    public void CounterAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(2);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartCounterAttackLength();
        //enemyScript.StartFlinchWindow();
        //if (enemyScript.teamAttackOn == true)
        //{
            //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
            //enemyScript.PlayAttackEffect(0);
        //}
        enemyScript.CounterAttackReadyOff();
    }
    public void RegularAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(2);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        //if (enemyScript.teamAttackOn == true)
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        enemyScript.AttackReadyOff();
    }
}
