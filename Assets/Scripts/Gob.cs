using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATM, Goblin attacks every few seconds
public class Gob : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart();
        enemyScript.SetIdleTime(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.attackReady==true)
        {
            Attack();
            Debug.Log("Attack");
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
        animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        StartCoroutine(AttackLength());
    }
    IEnumerator AttackLength()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("Attack");
        //StartCoroutine(IdleAnimation());
    }
}
