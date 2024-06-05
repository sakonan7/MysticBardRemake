using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandDragon : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetHP(1000);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetRed();
    }

    // Update is called once per frame
    void Update()
    {

        //if(enemyScript.HP < 1000-40 && enemyScript.HP > 1000-80)
        //{
            if (enemyScript.attackReady == true)
           {
                RedAttack();
                //Debug.Log("Attack");
            }
        //}
        //Last phase
        //< 500, secondPhase ==true
    }
    IEnumerator IdleAnimation()
    {
        animator.SetBool("Idle", true);
        yield return new WaitForSeconds(2);
        //idle = false;
        //Attack();
    }
    public void RedAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("StrongAttack");
        enemyScript.SetDamage(4);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
    }
    //Green Attack. 2.65 damage
    //Last Phase. 4 damage. For now, Green Attack Animations
}
