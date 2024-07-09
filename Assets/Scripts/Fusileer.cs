using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusileer : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private AudioSource audio;
    private int hitCount = 0;
    public AudioClip gun;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(60);
        enemyScript.SetEXP(60);

        enemyScript.SetIdleTime(15);
        enemyScript.SetCantFlinch();
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
        //animator.SetTrigger("Attack");
        //enemyScript.SetDamage(12);
        //enemyScript.SetAttackLength(1.5f);
        //enemyScript.StartAttackLength();
        //enemyScript.StartFlinchWindow();
        enemyScript.AttackReadyOff();
        StartCoroutine(Special());
        enemyScript.SetSpecial();
        audio.PlayOneShot(gun, 1);
    }
    IEnumerator Special()
    {
        while (enemyScript.HP>0&& hitCount< 12)
        {
            int random = Random.Range(0, 1);
            float time;
            if (random == 0)
            {
                time = 0.5f; 
            }
            else
            {
                time = 1.5f;
            }
            enemyScript.SetDamage(1);
            enemyScript.SetAttackLength(time);
            enemyScript.StartAttackLength();
            yield return new WaitForSeconds(time);
            hitCount++;
            if(hitCount >= 12)
            {
                enemyScript.UnsetSpecial();
                
            }
        }
        //This works because the whole loop is over
        hitCount = 0;
        audio.Stop();
    }

}
