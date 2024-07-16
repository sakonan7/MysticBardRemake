using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

//Need to ensure skip to next phase if HP reaches another phase
//Maybe have the attack play at least
//Don't have to worry about attack damage because each attack sets the attack damage
//I'm thinking of if the boss is flinching, reset the pattern
//Maybe I should have a flash to reset everything
//1.) Make bombs not deal damage at start of bombs appear
//2.) Witch (Fix armor us)
//06/18/24
//I want Red Dragon's cycle to restart when a new phase starts
//I don't think this will stop bombs from being spawned, though, so thatis not a bad thing
//I will have to do this case by case. The main concern is enemy attack cycle getting interrupted between phases
//It looks like that's happening with Green
public class GrandDragon : MonoBehaviour
{
    public Material red;
    public Material purple;
    public Material green;
    public Material final;

    public Material flashing;
    public GameObject flashingLight;
    public Material dying;
    public ParticleSystem []redAura;
    public ParticleSystem []greenAura;
    public ParticleSystem[] purpleAura;
    public ParticleSystem[] greenTailAura;
    public ParticleSystem[] counterattackAura;
    public ParticleSystem[] finalAttackAura;

    public GameObject barrier;
    public GameObject regularBombRing1;
    public GameObject regularBombRing2;
    public GameObject bombRing3;
    public GameObject bombRing4;
    private bool regularBombRing1Used = false;
    private bool regularBombRing2Used = false;
    private GameObject barrierAnimation;
    private ParticleSystem bombFlare;
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private SkinnedMeshRenderer skin;
    private bool forcedPhaseChange = false;
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

    private bool repeat = false;

    private int maxHP = 75*6+150;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        skin = transform.Find("Dragon").GetComponent<SkinnedMeshRenderer>();
        skin.material = red;
        bombFlare = transform.Find("Bomb Light").transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
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
        //06/19/24
        //I will atm make an if case to trigger phase force because I need this for every phase
        //I think I was about to write that I don't need IdleAnimationCancel for everything
        //I think it's because I want bombs to start asap
        //At the moment, I won't
        //Everything boils down to the IdleAnimation
        //It has attackReady!
        //At the moment, I've written bomb phases to wait a little bit and then perform bombs
        //Logically, it makes sense because it's not flinching and I want it to perform bomb right after barrier
        //I have to use OpeningBombUser() to have my cake and eat it, too
        //The issue is that interrupt is causing multiple bombs to be spawned
        //I'm really going to have to have my cake and eat it, too
        if (enemyScript.cantMove == false)
        {
            if (forcedPhaseChange == true)
            {
                forcedPhaseChange = false;
                ForcedPhaseChange();
            }
            if (enemyScript.HP < maxHP - 75 && firstPhase == true)
            {
                firstPhase = false;
                secondPhase = true;
                enemyScript.UnsetRed();
                //SetBomb, but also set CantFlinch for barrier
                //enemyScript.SetGreen();
                //skin.material = green;
                //enemyScript.SetBomb();
                enemyScript.SetNoAttack();
                enemyScript.SetBombUser();
                skin.material = purple;
                //Forced Bomb Start

                if (enemyScript.flinching == true)
                {
                    forcedPhaseChange = true;
                }
                else
                {
                    OpeningBombUser();
                }
                StartCoroutine(BarrierAnimation(1));
                enemyScript.SetIdleTime(20);
            }
            if (enemyScript.HP < maxHP - 150 && secondPhase == true)
            {
                secondPhase = false;
                thirdPhase = true;
                enemyScript.UnsetBomb();
                enemyScript.UnsetNoAttack();
                enemyScript.UnsetBombUser();
                enemyScript.SetGreen();
                skin.material = green;
                enemyScript.SetIdleTime(3);
                //if (enemyScript.flinching == true)
                //{
                //forcedPhaseChange = true;
                //}
            }
            if (enemyScript.HP < maxHP - 225 && thirdPhase == true)
            {
                //Debug.Log("Fourth Phase");
                thirdPhase = false;
                fourthPhase = true;
                enemyScript.UnsetGreen();
                enemyScript.SetNoAttack();
                enemyScript.SetBombUser();
                enemyScript.SetRed();
                enemyScript.CounterAttackWholeCancel();
                skin.material = purple;
                //Roar
                //Just do an IEnumerator and an invincibility periodbaby
                if (enemyScript.flinching == true)
                {
                    forcedPhaseChange = true;
                }
                else
                {
                    OpeningBombUser();
                }
                Roar();
                StartCoroutine(RoarDuration());
                RedAuraOn();

                //StartCoroutine(BarrierAnimation());
                //Need opportunities to do an attack every 10 seconds
                //And after another 10 seconds, set out more mines
                //The issue is bomb. Making more than it should
                enemyScript.SetIdleTime(10);
                regularBombRing1Used = false;
                regularBombRing2Used = false;
            }
            if (enemyScript.HP < maxHP - 300 && fourthPhase == true)
            {
                fourthPhase = false;
                fifthPhase = true;
                enemyScript.UnsetRed();
                enemyScript.SetNoAttack();
                enemyScript.SetBombUser();
                enemyScript.SetGreen();
                skin.material = purple;
                if (enemyScript.flinching == true)
                {
                    forcedPhaseChange = true;
                }
                else
                {
                    OpeningBombUser();
                }
                Roar();
                StartCoroutine(RoarDuration());
                RedAuraOff();
                GreenAuraOn();
                //Need opportunities to do an attack every 10 seconds
                //And after another 10 seconds, set out more mines
                //The issue is bomb. Making more than it should
                enemyScript.SetIdleTime(6);
                regularBombRing1Used = false;
                regularBombRing2Used = false;
                
            }
            if (enemyScript.HP < maxHP - 375 && fifthPhase == true)
            {
                fifthPhase = false;
                sixthPhase = true;
                enemyScript.UnsetGreen();
                enemyScript.SetBombUser();
                enemyScript.SetNoAttack();
                enemyScript.CounterAttackWholeCancel();
                //SetBomb, but also set CantFlinch for barrier
                //enemyScript.SetGreen();
                //skin.material = green;
                //enemyScript.SetBomb();
                skin.material = purple;
                //For simplicity, I am making a cancel IdleAnimation();
                //I could use this for a revenge value att
                if (enemyScript.flinching == true)
                {
                    forcedPhaseChange = true;
                }
                else
                {
                    OpeningBombUser();
                }
                Roar();
                StartCoroutine(RoarDuration());
                GreenAuraOff();
                PurpleAuraOn();
                enemyScript.SetIdleTime(20);
                regularBombRing1Used = false;
                regularBombRing2Used = false;
            }
            //This works perfectly on its own
            if (enemyScript.HP <=120 && sixthPhase == true)
            {
                sixthPhase = false;
                seventhPhase = true;
                enemyScript.UnsetBomb();
                enemyScript.UnsetNoAttack();
                enemyScript.UnsetBombUser();
                enemyScript.SetGreen();
                enemyScript.SetRed();
                PurpleAuraOff();
                FinalAuraOn();
                skin.material = final;
                enemyScript.SetIdleTime(3);
            }
            if (enemyScript.HP <= 0)
            {
                skin.material = dying;
                enemyScript.CounterAttackWholeCancel();
            }
            //if (enemyScript.HP < 1000 - 50 && firstPhase == true)
            //{
            //firstPhase = false;
            //}
            //if(enemyScript.HP < 1000-40 && enemyScript.HP > 1000-80)
            //{
            if (firstPhase == true)
            {
                if (enemyScript.attackReady == true)
                {
                    //StartCoroutine(Flashing());

                    RedAttack();
                    //Debug.Log("Attack");
                }
            }
            else if (secondPhase == true)
            {
                if (enemyScript.attackReady == true)
                {
                    enemyScript.SetNoAttack();
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
            else if (thirdPhase == true)
            {
                if (enemyScript.counterAttackTriggered == true)
                {
                    StartCoroutine(Flashing());
                    CounterAttack();
                }
                if (enemyScript.attackReady == true)
                {
                    if (enemyScript.unflinchingFollow == true)
                    {
                        StartCoroutine(Flashing());
                    }
                    GreenAttack();
                    //Debug.Log("Attack");
                }
            }
            else if (fourthPhase == true)
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
                        enemyScript.SetNoAttack();
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
            else if (fifthPhase == true)
            {
                if (enemyScript.counterAttackTriggered == true)
                {
                    StartCoroutine(Flashing());
                    CounterAttack();
                    Debug.Log("Counterattack?");
                }
                if (enemyScript.attackReady == true)
                {
                    if (fifthPhaseRegular == false)
                    {

                        //if (enemyScript.attackReady == true)
                        //{
                        if (enemyScript.unflinchingFollow == true)
                        {
                            StartCoroutine(Flashing());
                        }
                        GreenAttack();
                        //Debug.Log("Attack");
                        fifthPhaseRegular = true;
                        //}
                        //Debug.Log("Regularatt");
                    }
                    else
                    {
                        enemyScript.SetNoAttack();
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
            else if (sixthPhase == true)
            {
                if (enemyScript.attackReady == true)
                {
                    enemyScript.SetNoAttack();
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
            else if (seventhPhase == true)
            {
                if (enemyScript.counterAttackTriggered == true)
                {
                    StartCoroutine(Flashing());
                    SeventhCounterAttack();
                }
                if (enemyScript.attackReady == true)
                {
                    if (enemyScript.unflinchingFollow == true)
                    {
                        StartCoroutine(Flashing());
                    }
                    SeventhGreenAttack();
                    //Debug.Log("Attack");
                }
            }

            //}
            //Last phase
            //< 500, secondPhase ==true
        }
        if (enemyScript.unflinchingFollow == true && repeat == false)
        {
            StartCoroutine(Flashing());
        }
    }
    public void ForcedPhaseChange()
    {
        //For simplicity, I am making a cancel IdleAnimation();
        //I could use this for a revenge value att
        enemyScript.IdleAnimationCancel();
        enemyScript.AttackReadyOff();
        //The way this code is written, this should make Dragon go into an attack almost immediately
        //if (enemyScript.bombUser ==true) {
            //enemyScript.Interrupt(); //At the moment, he will just go into barrier animation
        //}
        enemyScript.FlinchCancel();
        StartCoroutine(Flashing());
    }
    public void OpeningBombUser ()
    {
        enemyScript.IdleAnimationCancel();
        enemyScript.AttackReadyOff();
        //enemyScript.Interrupt();
    }
IEnumerator Flashing()
    {
            skin.material = flashing;
        //Debug.Log("White");
        repeat = true;
        flashingLight.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            //Depends on phase
            if (secondPhase == true)
            {
                skin.material = purple;
            }
            if (thirdPhase == true)
            {
                skin.material = green;
            }
            if (fourthPhase == true)
            {
                skin.material = purple;
            }
            if (fifthPhase == true)
            {
                skin.material = purple;
            }
            if (sixthPhase == true)
            {
                skin.material = purple;
            }
            if (seventhPhase == true)
            {
                skin.material = final;
            }
        repeat = false;
        flashingLight.SetActive(false);
    }
    public void Roar()
    {
        animator.SetTrigger("Bomb");
        animator.SetBool("Idle", false);
    }
    IEnumerator RoarDuration()
    {
        enemyScript.SetCantFlinch();
        yield return new WaitForSeconds(1.5f);
        animator.ResetTrigger("Bomb");
        animator.SetBool("Idle", true);
        StartCoroutine(BarrierAnimation(2));
    }
    public void RedAuraOn()
    {
        for (int i =0;i< redAura.Length;i++)
        {
            redAura[i].gameObject.SetActive(true);
        }
    }
    public void RedAuraOff()
    {
        for (int i = 0; i < redAura.Length; i++)
        {
            redAura[i].gameObject.SetActive(false);
        }
    }
    public void GreenAuraOn()
    {
        for (int i = 0; i < greenAura.Length; i++)
        {
            greenAura[i].gameObject.SetActive(true);
        }
    }
    public void GreenAuraOff()
    {
        for (int i = 0; i < greenAura.Length; i++)
        {
            greenAura[i].gameObject.SetActive(false);
        }
    }
    public void PurpleAuraOn()
    {
        for (int i = 0; i < purpleAura.Length; i++)
        {
            purpleAura[i].gameObject.SetActive(true);
        }
    }
    public void PurpleAuraOff()
    {
        for (int i = 0; i < purpleAura.Length; i++)
        {
            purpleAura[i].gameObject.SetActive(false);
        }
    }
    public void FinalAuraOn()
    {
        for (int i = 0; i < redAura.Length; i++)
        {
            redAura[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < greenTailAura.Length; i++)
        {
            greenTailAura[i].gameObject.SetActive(true);
        }
    }
    public void CounterattackAuraOn()
    {
        for (int i = 0; i < counterattackAura.Length; i++)
        {
            counterattackAura[i].gameObject.SetActive(true);
        }
        StartCoroutine(CounterattackAuraOff());
    }
    IEnumerator CounterattackAuraOff()
    {
        yield return new WaitForSeconds(1.5f*2);
        for (int i = 0; i < counterattackAura.Length; i++)
        {
            counterattackAura[i].gameObject.SetActive(false);
        }
    }
    public void FinalAttackAuraOn()
    {
        for (int i = 0; i < finalAttackAura.Length; i++)
        {
            finalAttackAura[i].gameObject.SetActive(true);
        }
        StartCoroutine(FinalAttackAuraOff());
    }
    IEnumerator FinalAttackAuraOff()
    {
        yield return new WaitForSeconds(1.5f * 2);
        for (int i = 0; i < finalAttackAura.Length; i++)
        {
            finalAttackAura[i].gameObject.SetActive(false);
        }
    }
    public void RedAttack()
    {
        enemyScript.UnsetNoAttack();
        //animator.SetBool("Idle",false);
        animator.SetTrigger("StrongAttack");
        enemyScript.SetDamage(4.5f);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
        
    }
    IEnumerator BarrierAnimation(int time)
    {
        barrierAnimation.SetActive(true);
        //animator.SetTrigger("Barrier");
        enemyScript.SetArmor();
        enemyScript.SetCantFlinch();
        yield return new WaitForSeconds(time);
        //animator.ResetTrigger("Barrier");

        barrierAnimation.SetActive(false);

        
        //enemyScript.SetIdleStart();
        //enemyScript.NonStandardIdleStart();
        Barrier();
        //06/24/24
        //I don't think I need this
        //Not supposed to flinch while armor is 
        //enemyScript.UnsetCantFlinch();
        if (secondPhase ==true ||fourthPhase ==true||fifthPhase==true) {
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
        else if(sixthPhase ==true)
        {
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
        }
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y + 1, barrier.transform.position.z), barrier.transform.rotation);
        enemyScript.PlayBarrierSound();
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
        BombFlare();
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(1));
        //SetIdleTime during Bombrings
        //SamIAm
        if (secondPhase == true)
        {
            enemyScript.SetIdleTime(20);
        }
        if (fourthPhase == true)
        {
            enemyScript.SetIdleTime(10);
        }
        if (fifthPhase == true)
        {
            enemyScript.SetIdleTime(6);
        }
        enemyScript.PlayerCantPause(18);
        enemyScript.PlayerTransparentUI(18);
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
        BombFlare();
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(2));
        if (secondPhase == true)
        {
            enemyScript.SetIdleTime(20);
        }
        if (fourthPhase == true)
        {
            enemyScript.SetIdleTime(10);
        }
        if (fifthPhase == true)
        {
            enemyScript.SetIdleTime(6);
        }
    }
    public void BombRing3()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Bomb");
        enemyScript.SetBomb();
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        BombFlare();
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(3));
        enemyScript.SetIdleTime(20);
    }
    public void BombRing4()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Bomb");
        enemyScript.SetBomb();
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        BombFlare();
        enemyScript.AttackReadyOff();
        StartCoroutine(BombRingAppear(4));
        enemyScript.SetIdleTime(20);
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
        enemyScript.PlayerCantPause(18);
        enemyScript.PlayerTransparentUI(18);
    }
    public void BombFlare()
    {
        //bombFlare.SetActive(true);
        //yield return new WaitForSeconds(1f);
        bombFlare.Play();
    }
    //Green Attack. 2.65 damage
    //Last Phase. 4 damage. For now, Green Attack Animations
    public void CounterAttack()
    {
        enemyScript.UnsetNoAttack();
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
    public void GreenAttack()
    {
        enemyScript.UnsetNoAttack();
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        if (enemyScript.unflinchingFollow == false) {
            enemyScript.StartFlinchWindow();
        }
        //if (enemyScript.teamAttackOn == true)
        //{
        enemyScript.PlayAttackEffect(1);
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
        enemyScript.SetDamage(6);
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
        CounterattackAuraOn();
    }
    public void SeventhGreenAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(4.5f);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        if (enemyScript.unflinchingFollow == false)
        {
            enemyScript.StartFlinchWindow();
        }
        //if (enemyScript.teamAttackOn == true)
        //{
        enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        enemyScript.AttackReadyOff();
        FinalAttackAuraOn();
    }
}
