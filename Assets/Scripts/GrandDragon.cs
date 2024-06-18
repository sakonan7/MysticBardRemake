using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//Need to ensure skip to next phase if HP reaches another phase
//Maybe have the attack play at least
//Don't have to worry about attack damage because each attack sets the attack damage
//I'm thinking of if the boss is flinching, reset the pattern
//Maybe I should have a flash to reset everything
//1.) Make bombs not deal damage at start of bombs appear
//2.) Witch (Fix armor us)
public class GrandDragon : MonoBehaviour
{
    public Material red;
    public Material purple;
    public Material green;
    public Material flashing;
    public Material dying;
    public GameObject barrier;
    public GameObject regularBombRing1;
    public GameObject regularBombRing2;
    public GameObject bombRing3;
    public GameObject bombRing4;
    private bool regularBombRing1Used = false;
    private bool regularBombRing2Used = false;
    private GameObject barrierAnimation;
    private GameObject bombFlare;
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private SkinnedMeshRenderer skin;
    private bool firstPhase = true;
    private bool secondPhase = false;
    private bool thirdPhase = false;
    private bool fourthPhase = false;
    private bool fifthPhase = false;
    private bool sixthPhase = false;
    private bool seventhPhase = false;

    //After a bomb attack, trigger this
    private bool fourthPhaseRegular = false;
    private bool fifthPhaseRegular = false;

    private int maxHP = 350;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        skin = transform.Find("Dragon").GetComponent<SkinnedMeshRenderer>();
        bombFlare = transform.Find("Bomb Light").transform.Find("Lens").gameObject;
        barrierAnimation = transform.Find("Root").transform.Find("Personal Barrier Object").transform.Find("Barrier Animation").gameObject;
        enemyScript.SetHP(maxHP);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetRed();
        enemyScript.SetBoss();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyScript.HP < 350 -10 &&firstPhase==true)
        {
            firstPhase = false;
            secondPhase = true;
            enemyScript.UnsetRed();
            //SetBomb, but also set CantFlinch for barrier
            //enemyScript.SetGreen();
            //skin.material = green;
            //enemyScript.SetBomb();
            skin.material = purple;
            //For simplicity, I am making a cancel IdleAnimation();
            //I could use this for a revenge value att
            enemyScript.IdleAnimationCancel();
            enemyScript.FlinchCancel();
            enemyScript.AttackReadyOff();
            StartCoroutine(BarrierAnimation());
            enemyScript.SetIdleTime(20);
        }
        if (enemyScript.HP < 350 - 20 && secondPhase == true)
        {
            secondPhase = false;
            thirdPhase = true;
            enemyScript.UnsetBomb();
            enemyScript.SetGreen();
            skin.material = green;
        }
        if (enemyScript.HP < 350 - 30 && thirdPhase == true)
        {
            thirdPhase = false;
            fourthPhase = true;
            enemyScript.UnsetGreen();
            enemyScript.SetRed();
            skin.material = purple;
            enemyScript.IdleAnimationCancel();
            enemyScript.FlinchCancel();
            StartCoroutine(BarrierAnimation());
            //Need opportunities to do an attack every 10 seconds
            //And after another 10 seconds, set out more mines
            //The issue is bomb. Making more than it should
            enemyScript.SetIdleTime(10);
            regularBombRing1Used = false;
            regularBombRing2Used = false;
        }
        if (enemyScript.HP < 350 - 40 && fourthPhase == true)
        {
            fourthPhase = false;
            fifthPhase = true;
            enemyScript.UnsetRed();
            enemyScript.SetGreen();
            skin.material = purple;
            enemyScript.IdleAnimationCancel();
            enemyScript.FlinchCancel();
            StartCoroutine(BarrierAnimation());
            //Need opportunities to do an attack every 10 seconds
            //And after another 10 seconds, set out more mines
            //The issue is bomb. Making more than it should
            enemyScript.SetIdleTime(10);
            regularBombRing1Used = false;
            regularBombRing2Used = false;
        }
        if (enemyScript.HP < 350 - 50 && fifthPhase == true)
        {
            fifthPhase = false;
            sixthPhase = true;
            enemyScript.UnsetGreen();
            //SetBomb, but also set CantFlinch for barrier
            //enemyScript.SetGreen();
            //skin.material = green;
            //enemyScript.SetBomb();
            skin.material = purple;
            //For simplicity, I am making a cancel IdleAnimation();
            //I could use this for a revenge value att
            enemyScript.IdleAnimationCancel();
            enemyScript.FlinchCancel();
            enemyScript.AttackReadyOff();
            StartCoroutine(BarrierAnimation());
            enemyScript.SetIdleTime(30);
            regularBombRing1Used = false;
            regularBombRing2Used = false;
        }
        if (enemyScript.HP < 350 - 60 && sixthPhase == true)
        {
            sixthPhase = false;
            seventhPhase = true;
            enemyScript.UnsetBomb();
            enemyScript.SetGreen();
            enemyScript.SetRed();
            skin.material = red;
        }
        if(enemyScript.HP<=0)
        {
            skin.material = dying;
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
                StartCoroutine(Flashing());

                RedAttack();
                //Debug.Log("Attack");
            }
        }
        if (secondPhase == true)
        {
            if (enemyScript.attackReady == true)
            {
                //RegularBombRing1();
                Debug.Log("Bomb Attack Regular");
                //Randomize
                //Make first ring appear right away
                //At least 20 seconds between salvos
                if (regularBombRing1Used == false && regularBombRing2Used == false)
                {
                    int random = Random.Range(0, 1);
                    if (random == 0)
                    {
                        RegularBombRing1();
                        regularBombRing1Used = true;
                    }
                    else
                    {
                        RegularBombRing2();
                        regularBombRing2Used = true;
                    }
                }
                else if (regularBombRing1Used ==false && regularBombRing2Used ==true)
                {
                  RegularBombRing1();
                     regularBombRing1Used = true;
                }
                else if (regularBombRing1Used == true && regularBombRing2Used == false)
                {
                    RegularBombRing2();
                    regularBombRing2Used = true;
                }

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
        if (fourthPhase == true)
        {
            if (enemyScript.attackReady == true)
            {
                if (fourthPhaseRegular == false)
                {
                    RedAttack();
                    fourthPhaseRegular = true;
                }
                else
                {
                    //RegularBombRing1();
                    Debug.Log("Bomb Attack Regular");
                    //Randomize
                    //Make first ring appear right away
                    //At least 20 seconds between salvos
                    if (regularBombRing1Used == false && regularBombRing2Used == false)
                    {
                        int random = Random.Range(0, 1);
                        if (random == 0)
                        {
                            RegularBombRing1();
                            regularBombRing1Used = true;
                        }
                        else
                        {
                            RegularBombRing2();
                            regularBombRing2Used = true;
                        }
                    }
                    else if (regularBombRing1Used == false && regularBombRing2Used == true)
                    {
                        RegularBombRing1();
                        regularBombRing1Used = true;
                    }
                    else if (regularBombRing1Used == true && regularBombRing2Used == false)
                    {
                        RegularBombRing2();
                        regularBombRing2Used = true;
                    }
                }

            }
        if (fifthPhase == true)
        {
            if (enemyScript.attackReady == true)
            {
                if (fifthPhaseRegular == false)
                {
                        if (enemyScript.counterAttackTriggered == true)
                        {
                            CounterAttack();
                        }
                        if (enemyScript.attackReady == true)
                        {
                            GreenAttack();
                            //Debug.Log("Attack");
                            fifthPhaseRegular = true;
                        }
                }
                else
                {
                    //RegularBombRing1();
                    Debug.Log("Bomb Attack Regular");
                    //Randomize
                    //Make first ring appear right away
                    //At least 20 seconds between salvos
                    if (regularBombRing1Used == false && regularBombRing2Used == false)
                    {
                        int random = Random.Range(0, 1);
                        if (random == 0)
                        {
                            RegularBombRing1();
                            regularBombRing1Used = true;
                        }
                        else
                        {
                            RegularBombRing2();
                            regularBombRing2Used = true;
                        }
                    }
                    else if (regularBombRing1Used == false && regularBombRing2Used == true)
                    {
                        RegularBombRing1();
                        regularBombRing1Used = true;
                    }
                    else if (regularBombRing1Used == true && regularBombRing2Used == false)
                    {
                        RegularBombRing2();
                        regularBombRing2Used = true;
                    }
                }

            }
        }
        }
        if (sixthPhase == true)
        {
            if (enemyScript.attackReady == true)
            {
                //RegularBombRing1();
                Debug.Log("Bomb Attack Regular");
                //Randomize
                //Make first ring appear right away
                //At least 20 seconds between salvos
                if (regularBombRing1Used == false && regularBombRing2Used == false)
                {
                    int random = Random.Range(0, 1);
                    if (random == 0)
                    {
                        BombRing3();
                        regularBombRing1Used = true;
                    }
                    else
                    {
                        BombRing4();
                        regularBombRing2Used = true;
                    }
                }
                else if (regularBombRing1Used == false && regularBombRing2Used == true)
                {
                    BombRing3();
                    regularBombRing1Used = true;
                }
                else if (regularBombRing1Used == true && regularBombRing2Used == false)
                {
                    BombRing4();
                    regularBombRing2Used = true;
                }

            }
        }
        if (seventhPhase == true)
        {
            if (enemyScript.counterAttackTriggered == true)
            {
                SeventhCounterAttack();
            }
            if (enemyScript.attackReady == true)
            {
                SeventhGreenAttack();
                //Debug.Log("Attack");
            }
        }

        //}
        //Last phase
        //< 500, secondPhase ==true
    }
IEnumerator Flashing()
    {
        int numFlash = 0;
        while(numFlash < 2)
        {
            skin.material = flashing;
            Debug.Log("White");
            yield return new WaitForSeconds(0.5f);
            //Depends on phase
            skin.material = red;
            numFlash++;

            Debug.Log("Red");
        }
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

        
        //enemyScript.SetIdleStart();
        //enemyScript.NonStandardIdleStart();
        Barrier();
        enemyScript.UnsetCantFlinch();
        if (regularBombRing1Used == false && regularBombRing2Used == false)
        {
            int random = Random.Range(0, 1);
            if (random == 0)
            {
                RegularBombRing1();
                regularBombRing1Used = true;
            }
            else
            {
                RegularBombRing2();
                regularBombRing2Used = true;
            }
        }
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
    public void RegularBombRing2()
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
        StartCoroutine(BombRingAppear(2));
    }
    public void BombRing3()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Bomb");
        enemyScript.SetBomb();
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        StartCoroutine(BombFlare());
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(3));
    }
    public void BombRing4()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Bomb");
        enemyScript.SetBomb();
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        StartCoroutine(BombFlare());
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(4));
    }
    IEnumerator BombRingAppear(int number)
    {
        yield return new WaitForSeconds(1);
        if (number == 1)
        {
            Instantiate(regularBombRing1, regularBombRing1.transform.position, regularBombRing1.transform.rotation);
        }
        if (number == 2)
        {
            Instantiate(regularBombRing2, regularBombRing2.transform.position, regularBombRing2.transform.rotation);
        }
        if (number == 3)
        {
            Instantiate(bombRing3, bombRing3.transform.position, bombRing3.transform.rotation);
        }
        if (number == 4)
        {
            Instantiate(bombRing4, bombRing4.transform.position, bombRing4.transform.rotation);
        }
        if (regularBombRing1Used == true && regularBombRing2Used == true)
        {
            regularBombRing1Used = false;
            regularBombRing2Used = false;
        }
        if (fourthPhase == true)
        {
            if (fourthPhaseRegular == true)
            {
                fourthPhaseRegular = false;
            }
        }
        if (fifthPhase ==true)
        {
            if (fifthPhaseRegular ==true)
            {
                fifthPhaseRegular = false;
            }
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
    public void SeventhCounterAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(4);
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
    public void SeventhGreenAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(4);
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
