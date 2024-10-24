using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATM, Goblin attacks every few seconds
public class Gob : MonoBehaviour
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

        enemyScript.SetHP(60+ 10);
        enemyScript.SetEXP(60);

        enemyScript.SetIdleTime(5);
        enemyScript.SetNormal();
        enemyScript.SetTeamAttack();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.cantMove ==false) {
            if (enemyScript.attackReady == true)
            {
                Attack();
                //Debug.Log("Attack");
            }
        }
    }
    IEnumerator IdleAnimation()
    {
        animator.SetBool("Idle",true);
        yield return new WaitForSeconds(2);
        //idle = false;
        //Attack();
    }
    public void Attack()
    {
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Attack");
        enemyScript.SetDamage(1);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        if (enemyScript.teamAttackOn==true) {
            enemyScript.PlayAttackEffect(1);
        }
        else
        {
            enemyScript.PlayAttackEffect(0);
        }
        enemyScript.AttackReadyOff();
    }

}
