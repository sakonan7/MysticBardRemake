using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenThief : MonoBehaviour
{
    public GameObject flashing;
    public SkinnedMeshRenderer armor1;
    public SkinnedMeshRenderer armor2;
    public SkinnedMeshRenderer body;
    public SkinnedMeshRenderer head;
    public SkinnedMeshRenderer helmet;
    public SkinnedMeshRenderer legs;
    public Material originalArmor1;
    public Material originalArmor2;
    public Material originalBody;
    public Material originalHead;
    public Material originalHelmet;
    public Material originalLegs;


    public Material flashArmor1;
    public Material flashArmor2;
    public Material flashBody;
    public Material flashHead;
    public Material flashHelmet;
    public Material flashLegs;
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetHP(90+10);
        enemyScript.SetEXP(90);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(3);
        enemyScript.SetGreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.cantMove == false)
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
                Debug.Log("Attack");
            }
        }
    }
    IEnumerator Flashing()
    {
        int numFlash = 0;
        while (numFlash < 2)
        {
    armor1.material=flashArmor1;
        armor2.material=flashArmor1;
        body.material=flashBody;
        head.material=flashHead;
        helmet.material=flashHelmet;
        legs.material=flashLegs;

    yield return new WaitForSeconds(0.5f);
            armor1.material = originalArmor1;
            armor2.material = originalArmor1;
            body.material = originalBody;
            head.material = originalHead;
            helmet.material = originalHelmet;
            legs.material = originalLegs;


            numFlash++;

            Debug.Log(numFlash);
        }
    }
    //I need a revenge value
    public void CounterAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(3);
        //Below isn't the problem because both counterattacklength and attacklength uses attacklength set by
        //SetAttackLength()
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartCounterAttackLength();
        //enemyScript.StartFlinchWindow();
        //if (enemyScript.teamAttackOn == true)
        //{
            //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
            enemyScript.PlayAttackEffect(1);
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
        if (enemyScript.unflinchingFollow == false) {
            enemyScript.StartFlinchWindow();
        }
        //if (enemyScript.teamAttackOn == true)
        //{
        enemyScript.PlayAttackEffect(0);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        enemyScript.AttackReadyOff();
    }
}
