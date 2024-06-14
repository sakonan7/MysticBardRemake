using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GrandDragon : MonoBehaviour
{
    public Material red;
    public Material purple;
    public Material green;
    public GameObject barrier;
    public GameObject regularBombRing1;
    public GameObject regularBombRing2;
    private GameObject barrierAnimation;
    private GameObject bombFlare;
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
        bombFlare = transform.Find("Bomb Light").transform.Find("Lens").gameObject;
        barrierAnimation = transform.Find("Root").transform.Find("Personal Barrier Object").transform.Find("Barrier Animation").gameObject;
        enemyScript.SetHP(1000);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetRed();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyScript.HP < 1000 -10 &&firstPhase==true)
        {
            firstPhase = false;
            secondPhase = true;
            enemyScript.UnsetRed();
            //SetBomb, but also set CantFlinch for barrier
            //enemyScript.SetGreen();
            //skin.material = green;
            //enemyScript.SetBomb();
            skin.material = purple;
            StartCoroutine(BarrierAnimation());
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
        if (secondPhase == true)
        {
            if (enemyScript.attackReady == true)
            {
                RegularBombRing1();
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
    IEnumerator BarrierAnimation()
    {
        barrierAnimation.SetActive(true);
        //animator.SetTrigger("Barrier");
        enemyScript.SetArmor();
        enemyScript.SetCantFlinch();
        yield return new WaitForSeconds(1);
        //animator.ResetTrigger("Barrier");

        barrierAnimation.SetActive(false);

        enemyScript.SetIdleTime(3);
        //enemyScript.SetIdleStart();
        enemyScript.NonStandardIdleStart();
        Barrier();
        enemyScript.UnsetCantFlinch();
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y, barrier.transform.position.z), barrier.transform.rotation);
    }
    public void RegularBombRing1()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Bomb");
        enemyScript.SetBomb();
        //enemyScript.SetDamage(1);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        //if (enemyScript.teamAttackOn == true)
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(0);
        //}
        //enemyScript.AttackReadyOff();
        StartCoroutine(BombFlare());
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(1));
    }
    IEnumerator BombRingAppear(int number)
    {
        yield return new WaitForSeconds(1);
        if (number == 1)
        {
            Instantiate(regularBombRing1, regularBombRing1.transform.position, transform.rotation);
        }
    }
    IEnumerator BombFlare()
    {
        bombFlare.SetActive(true);
        yield return new WaitForSeconds(1f);
        bombFlare.SetActive(false);
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
        //enemyScript.PlayAttackEffect(0);
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
