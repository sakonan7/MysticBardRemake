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
    private SkinnedMeshRenderer skin1;
    private SkinnedMeshRenderer skin2;
    private SkinnedMeshRenderer skin3;
    public Material skin1Original;
    public Material skin2Original;
    public Material skin3Original;
    public Material skin1Rage1;
    public Material skin2Rage1;
    public Material skin3Rage1;
    public Material skin1Rage2;
    public Material skin2Rage2;
    public Material skin3Rage2;
    public Material skin1Rage3;
    public Material skin2Rage3;
    public Material skin3Rage3;
    public ParticleSystem rageEffect;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        skin1 = transform.Find("SkinnedMeshes").transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        skin2 = transform.Find("SkinnedMeshes").transform.Find("Head").GetComponent<SkinnedMeshRenderer>();
        skin3 = transform.Find("SkinnedMeshes").transform.Find("Jaw").GetComponent<SkinnedMeshRenderer>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(100);
        enemyScript.SetEXP(100);

        enemyScript.SetIdleTime(10);
        enemyScript.SetRage();
        enemyScript.SetRageValueNumber(9);
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
        if(enemyScript.rageLevel1==true)
        {
            skin1.material = skin1Rage1;
            skin2.material = skin2Rage1;
            skin3.material = skin3Rage1;
        }
        if (enemyScript.rageLevel2 == true)
        {
            skin1.material = skin1Rage2;
            skin2.material = skin2Rage2;
            skin3.material = skin3Rage2;
        }
        if (enemyScript.rageLevel3 == true)
        {
            skin1.material = skin1Rage3;
            skin2.material = skin2Rage3;
            skin3.material = skin3Rage3;
        }
        if (enemyScript.rageStart == false)
        {
            skin1.material = skin1Original;
            skin2.material = skin2Original;
            skin3.material = skin3Original;
        }
        if (enemyScript.rageValueMove == true)
        {
            
            RageAttack1();
        }
    }
    public void Attack()
    {

        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        enemyScript.AttackReadyOff();

        enemyScript.UnsetCantFlinch();
        enemyScript.RageOff();
    }
    //I could instead use switches and lags
    //But this game is more of a puzzle 
    public void RageAttack1()
    {
        rageEffect.Play();
        enemyScript.RageValueMoveOff();
        
        animator.SetTrigger("RageAttack1");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1f);
        enemyScript.StartAttackLengthAlternate();
        StartCoroutine(RageAttack2());
    }
    IEnumerator RageAttack2()
    {
        yield return new WaitForSeconds(1.5f);
        animator.ResetTrigger("RageAttack1");
        animator.SetTrigger("RageAttack2");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1f);
        enemyScript.StartAttackLengthAlternate();
        StartCoroutine(RageAttack3());
    }
    IEnumerator RageAttack3()
    {
        yield return new WaitForSeconds(1.25f);
        animator.ResetTrigger("RageAttack2");
        animator.SetTrigger("RageAttack3");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1f);
        enemyScript.StartAttackLength();
        enemyScript.RestartIdleMethod(2);
        enemyScript.CooldownStart();
        StartCoroutine(RageOff());
    }
    IEnumerator RageOff()
    {
        yield return new WaitForSeconds(1);
        rageEffect.Stop();
        animator.SetTrigger("RageAttack3");
    }
}
