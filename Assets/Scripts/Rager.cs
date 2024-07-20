using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Rager
    : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(100);
        enemyScript.SetEXP(100);

        enemyScript.SetIdleTime(5);
        enemyScript.SetGuard();
        enemyScript.SetNormal();
        enemyScript.SetRevengeValue();
        enemyScript.SetRevengeValueNumber(10);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.cantMove == false)
        {
            if (enemyScript.attackReady == true)
            {
                Attack();
                //Debug.Log("Attack");
            }
        }
    }
    private void LateUpdate()
    {
        if (enemyScript.counterAttackTriggered == true)
        {
        }
        if (enemyScript.revengeValueMove == true)
        {
            enemyScript.RestartIdleMethod(1);
        }
    }
    public void Attack()
    {

        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        enemyScript.SetDamage(2);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
    }

}
