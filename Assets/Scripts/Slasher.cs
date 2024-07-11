using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATM, Goblin attacks every few seconds
public class Slasher : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private bool turnOnUnblockable = false;
    private int numAttacks = 2;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(60);
        enemyScript.SetEXP(60);

        enemyScript.SetIdleTime(5);
        enemyScript.SetUnblockable();
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
    IEnumerator IdleAnimation()
    {
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(2);
        //idle = false;
        //Attack();
    }
    public void Attack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
    }

}
