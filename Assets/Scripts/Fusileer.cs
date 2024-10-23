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
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(100 + 25);
        enemyScript.SetEXP(100);

        
        enemyScript.SetCantFlinch();
        enemyScript.SetFusileer();
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyScript.PauseBeforeSpecialStart(20);
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
    public void Attack()
    {
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetBool("SpecialLoop", true);
        //enemyScript.SetDamage(12);
        //enemyScript.SetAttackLength(1.5f);
        //enemyScript.StartAttackLength();
        //enemyScript.StartFlinchWindow();
        enemyScript.AttackReadyOff();
        StartCoroutine(Special());
        audio.PlayOneShot(gun, 1);
        enemyScript.PlayAttackEffect(0);
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
            enemyScript.StartAttackLengthAlternate();
            yield return new WaitForSeconds(time);
            hitCount++;
        }
        //This works because the whole loop is over
        hitCount = 0;
        audio.Stop();
        enemyScript.StopAttackEffect();
        enemyScript.RestartIdleMethod(5);
        animator.SetBool("SpecialLoop",false);
        animator.SetBool("Idle", true);
    }

}
