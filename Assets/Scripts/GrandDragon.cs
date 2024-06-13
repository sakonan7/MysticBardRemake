using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandDragon : MonoBehaviour
{
    public Material red;
    public Material green;
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private SkinnedMeshRenderer skin;
    private bool firstPhase = true;
    private bool secondPhase = false;
    private bool thirdPhase = false;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        skin = transform.Find("Dragon").GetComponent<SkinnedMeshRenderer>();
        enemyScript.SetHP(1000);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetRed();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyScript.HP < 1000 -20 &&firstPhase==true)
        {
            firstPhase = false;
            thirdPhase = true;
            enemyScript.SetGreen();
            skin.material = green;
        }
        //if (enemyScript.HP < 1000 - 50 && firstPhase == true)
        //{
            //firstPhase = false;
        //}
        //if(enemyScript.HP < 1000-40 && enemyScript.HP > 1000-80)
        //{
        if (firstPhase ==true) {
            if (enemyScript.attackReady == true)
            {
                RedAttack();
                //Debug.Log("Attack");
            }
        }
        if (thirdPhase == true)
        {
            if (enemyScript.counterAttackTriggered == true)
            {
                CounterAttack();
            }
            if (enemyScript.attackReady == true)
            {
                GreenAttack();
                //Debug.Log("Attack");
            }
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
            //enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
    }
    //Green Attack. 2.65 damage
    //Last Phase. 4 damage. For now, Green Attack Animations
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
        enemyScript.PlayAttackEffect(0);
        //}
        enemyScript.CounterAttackReadyOff();
    }
    public void GreenAttack()
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
