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
//08/04/24
//What I can do for flamethrower is make a flamethrower bool and if flinch ==true, cancel flamethrower bool and flamethrower
//Oops, forgot. Interrupting attack cancels flamethrower. I already have a way to cancel attack effects. Forgot what I was actually going to write lol.
//08/30/24
//Could use attackNum instead of blank phase reg
//09/03/24
//I'm rewriting the code because I think green and purplephase doesn't account for green and bombUser, so it keeps using counterattack. Logically, it should keep
//using counteratt. I think this is why after a counterattack, sometimes it does bomb instead of follow
//At the moment, i'm going to make it so that if Dragon is counterattacking, it won't phase change
//More simple that way
//I am also thinking about putting phase change in a method. Not forced phase
//Testing
//I forgot to do UseCounterAttack
//I forgot to check attackNum
//09/04/24 Mostof my bomb changes were made based on Witch
//09/06/24 I can fix Dragon if I used attackNums. Or rewrite Idle Animation, the end of . Rewriting IdleAnimation is too much of a hassle, so I will
//use attackNum inst
//09/09/24 didn't put attackNum in summonBomb, because summonBomb is when it'stoo late
public class GrandDragon : MonoBehaviour
{
    public Material red;
    public Material purple;
    public Material green;
    public Material final;

    public Material flashing;
    public GameObject flashingLight;
    public Material dying;
    public AudioClip flamethrower;
    public AudioClip shortRoar;
    public AudioClip longRoar;
    public AudioClip aura;
    public AudioClip counterattack;
    public AudioClip finalPhaseAttacks;
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
    private AudioSource audio;
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
    private int attackNum = 0;

    private bool repeat = false;

    private int maxHP = 75*6+150;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        audio = GetComponent<AudioSource>();
        skin = transform.Find("Dragon").GetComponent<SkinnedMeshRenderer>();
        skin.material = red;
        bombFlare = transform.Find("Bomb Light").transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
        barrierAnimation = transform.Find("Root").transform.Find("Personal Barrier Object").transform.Find("Barrier Animation").gameObject;
        enemyScript.SetHP(maxHP);
        enemyScript.SetEXP(1000);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetRed();
        enemyScript.SetBoss();
        enemyScript.SetArmorAmount(40);
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
                //audio.PlayOneShot(shortRoar,1);
                PhaseChange();
            }
            if (enemyScript.HP < maxHP - 150 && secondPhase == true)
            {
                secondPhase = false;
                thirdPhase = true;
                enemyScript.UnsetBombUser();
                enemyScript.SetGreen();
                enemyScript.SetCounterattack();
                skin.material = green;
                enemyScript.SetIdleTime(3);
                PhaseChange();
            }
            if (enemyScript.HP < maxHP - 225 && thirdPhase == true &&enemyScript.counterattacking==false)
            {
                //Debug.Log("Fourth Phase");
                thirdPhase = false;
                fourthPhase = true;
                enemyScript.UnsetGreen();
                enemyScript.UnsetCounterattack();
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
                PhaseChange();
            }
            if (enemyScript.HP < maxHP - 300 && fourthPhase == true)
            {
                fourthPhase = false;
                fifthPhase = true;
                enemyScript.UnsetRed();
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
                PhaseChange();
            }
            if (enemyScript.HP < maxHP - 375 && fifthPhase == true && enemyScript.counterattacking == false)
            {
                fifthPhase = false;
                sixthPhase = true;
                enemyScript.UnsetGreen();
                enemyScript.UnsetCounterattack();
                enemyScript.SetBombUser();
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
                PhaseChange();
            }
            //This works perfectly on its own
            if (enemyScript.HP <=120 && sixthPhase == true)
            {
                sixthPhase = false;
                seventhPhase = true;
                enemyScript.UnsetBombUser();
                enemyScript.SetGreen();
                enemyScript.SetRed();
                enemyScript.SetCounterattack();
                PurpleAuraOff();
                FinalAuraOn();
                skin.material = final;
                enemyScript.SetIdleTime(3);
                audio.PlayOneShot(aura, 2);
                PhaseChange();
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
                if (enemyScript.bombReady == true)
                {
                    StartBombSummon();
                }
                if (enemyScript.summonBombs == true)
                {
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
                    SummonBombs(18);
                }
            }
            else if (thirdPhase == true)
            {
                if (enemyScript.attackReady == true)
                {
                    GreenAttack();
                    //Debug.Log("Attack");
                }
            }
            else if (fourthPhase == true)
            {
                if (attackNum==1) {
                    if (enemyScript.attackReady == true)
                    {
                        RedAttack();
                        enemyScript.SetBombUser();
                        attackNum = 0;
                    }
                }
                if (attackNum==0) {
                    if (enemyScript.bombReady == true)
                    {
                        StartBombSummon();
                    }
                }
                if (enemyScript.summonBombs == true)
                {
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
                    SummonBombs(18);
                }
            }
            else if (fifthPhase == true)
            {
                if (attackNum==1) {
                    if (enemyScript.attackReady == true)
                    {
                        GreenAttack();
                        enemyScript.UnsetCounterattack();
                        enemyScript.SetBombUser();
                        attackNum = 0;
                    }
                }
                if (attackNum==0) {
                    if (enemyScript.bombReady == true)
                    {
                        StartBombSummon();
                    }
                }
                if (enemyScript.summonBombs == true)
                {
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
                    SummonBombs(18);
                }
            }
            else if (sixthPhase == true)
            {

                if (enemyScript.bombReady == true)
                {
                    StartBombSummon();
                }
                if (enemyScript.summonBombs == true)
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
                    SummonBombs(18);
                }
            }
            else if (seventhPhase == true)
            {
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
        if (enemyScript.counterattacking == true && repeat == false)
        {
            StartCoroutine(Flashing());
        }
    }
    private void LateUpdate()
    {
        if (thirdPhase ==true) {
            if (enemyScript.counterAttackTriggered == true)
            {
                CounterAttack();
            }
        }
        else if (fifthPhase == true)
        {
            if (enemyScript.counterAttackTriggered == true)
            {
                CounterAttack();
            }
        }
        else if (seventhPhase == true)
        {
            if (enemyScript.counterAttackTriggered == true)
            {
                SeventhCounterAttack();
            }
        }
    }
    public void ForcedPhaseChange()
    {
        //For simplicity, I am making a cancel IdleAnimation();
        //I could use this for a revenge value att
        enemyScript.IdleAnimationCancel();
        enemyScript.AttackReadyOff();
        enemyScript.BombReadyOff();
        enemyScript.SummonBombsOff();
        //The way this code is written, this should make Dragon go into an attack almost immediately
        //if (enemyScript.bombUser ==true) {
            //enemyScript.Interrupt(); //At the moment, he will just go into barrier animation
        //}
        enemyScript.FlinchCancel();
        //StartCoroutine(Flashing());
        Debug.Log("Forced Phase Change");
    }
    //I could put other PhaseChange stuff in 
    public void PhaseChange()
    {
        attackNum = 0;
    }
    public void OpeningBombUser ()
    {
        enemyScript.IdleAnimationCancel();
        enemyScript.BombReadyOff();
        //enemyScript.Interrupt();

        
    }
    IEnumerator ChangeToNonBomb()
    {
        yield return new WaitForSeconds(2);
        if (fourthPhase==true||fifthPhase==true) {
            enemyScript.UnsetBombUser();
        }
        if (fifthPhase == true)
        {
            enemyScript.SetCounterattack();
        }
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
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Bomb");
        
        audio.PlayOneShot(longRoar, 0.9f*2);
        audio.PlayOneShot(aura, 2);
    }
    IEnumerator RoarDuration()
    {
        enemyScript.SetCantFlinch();
        Debug.Log(enemyScript.cantFlinch);
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
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("StrongAttack");
        enemyScript.SetDamage(4.5f);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
        audio.PlayOneShot(flamethrower, 1);
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
        StartBombSummon();
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y + 1, barrier.transform.position.z), barrier.transform.rotation);
        enemyScript.PlayBarrierSound();
    }
    public void StartBombSummon()
    {
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Bomb");
        //enemyScript.SetDamage(1);

        //Gonna keep SetAttackLength
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartBombLength();
        enemyScript.BombReadyOff();
        enemyScript.StartFlinchWindow();
        //if (enemyScript.teamAttackOn == true)
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(0);
        //}

        BombFlare();
        enemyScript.PlayBombLensFlareSound();
        StartCoroutine(RoarTime());
        attackNum = 1;
    }
    IEnumerator RoarTime()
    {
        yield return new WaitForSeconds(0.75f);
        if (secondPhase == true || fourthPhase == true || fifthPhase == true)
        {
            audio.PlayOneShot(shortRoar, 0.9f);
        }
        else
        {
            audio.PlayOneShot(longRoar, 0.9f * 2);
        }
    }
    public void SummonBombs(float time)
    {
        enemyScript.SummonBombsOff();

        enemyScript.PlayerCantPause(time);
        enemyScript.PlayerTransparentUI(time);

    }
    public void RegularBombRing1()
    {
        //if (enemyScript.teamAttackOn == true)
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(0);
        //}
        //enemyScript.AttackReadyOff();
        //BombFlare();
        enemyScript.PlayBombLensFlareSound();
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
        
    }
    public void RegularBombRing2()
    {
        //if (enemyScript.teamAttackOn == true)
        //{
        //enemyScript.PlayAttackEffect(1);
        //}
        //else
        //{
        //enemyScript.PlayAttackEffect(0);
        //}
        //enemyScript.AttackReadyOff();
        //BombFlare();
        enemyScript.PlayBombLensFlareSound();
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
        //BombFlare();
        enemyScript.PlayBombLensFlareSound();
        StartCoroutine(BombRingAppear(3));
        enemyScript.SetIdleTime(20);
        
        //StartCoroutine(ChangeToNonBomb());
    }
    public void BombRing4()
    {
        //BombFlare();
        enemyScript.PlayBombLensFlareSound();
        StartCoroutine(BombRingAppear(4));
        enemyScript.SetIdleTime(20);
        
        //StartCoroutine(ChangeToNonBomb());
    }
    IEnumerator BombRingAppear(int number)
    {
        yield return new WaitForSeconds(1.5f);
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
            StartCoroutine(ChangeToNonBomb());
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
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(4);
        enemyScript.SetCounterattackTime(1.5f + 1.5f + 1);
        enemyScript.StartCounterattackTimeMethod();
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
        audio.PlayOneShot(counterattack,1);
    }
    public void GreenAttack()
    {
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        if (enemyScript.counterattacking == false) {
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
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(6);
        enemyScript.SetCounterattackTime(1.5f + 1.5f + 1);
        enemyScript.StartCounterattackTimeMethod();
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
        audio.PlayOneShot(finalPhaseAttacks, 2);
        audio.PlayOneShot(counterattack, 1f);
    }
    public void SeventhGreenAttack()
    {
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("Attack");
        animator.SetTrigger("Attack2");
        enemyScript.SetDamage(4.5f);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        if (enemyScript.counterattacking == false)
        {
            audio.PlayOneShot(finalPhaseAttacks,2);
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
