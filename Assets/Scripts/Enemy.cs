using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using Cinemachine;
using System.Linq.Expressions;

//I want to do some things I didn't do right in Beast Dominion
//Flinch
//Might as well do more shared stuff like idle animation here
//Less confusingcode
//StartingwithFlinch
//Itmakes sense, becauseIdle Animation is shared amongst enemies

//This game is a testament to what I learned during Beast Dominion
//I didn't expect to be using exact code
//I want to make this fast and playable right away

//Remembering how complicated Beast Dominion Actually Was
//The Data Fights In Kingdom Hearts III Are Definitely Hard. I think they put a stagger window between certain attacks. A Corout

//06/03/24
//This game is a lesson that I can either put a lot of things in an individual script and be disorganized, or put most of things in a single script, but
//be limited to having to follow it (something strict). You always have to follow these rules and you can't break these rules.
//Maybe use white filter for enemy if their attack can't be interrupted

//06/12/24
//For stuff like phases and complex attack patterns, do them in the individual enemy script
//For attacks where I don't want a flinch, just use a mix of not using a flinchwindow and using cantflinch

//06/26/24
//Bomb users will give a number of sounds to play Based on how many bombs they'd make .The sound is fzz
//Bonds will draw from the prefabs for effects so I don't have to account for the bonds being destroyed 

//TaskList
//Make Foe Not Spazz Between Idle And Att
public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Animation animation;
    private AudioSource audio;
    private DifficultyModerator difficulty;
    private bool animatorTrue = false;
    private bool animationTrue = false;
    private PlayerController playerScript;
    private GameObject effectPosition;
    private GameManager gameScript;
    private GameObject counterAttackCloud;
    private ParticleSystem counterAttackStart;
    private ParticleSystem counterAttackOn;
    private GameObject bossHPBarObject;
    private GameObject bossHPBarBackground;
    private Image HPBarActual;
    private TextMeshProUGUI HPText;
    private Image damageBar;
    private GameObject specialAuraObject;
    private ParticleSystem specialAuraStart;
    private ParticleSystem specialAura1;
    private ParticleSystem specialAura2;
    private ParticleSystem specialAura3;
    private ParticleSystem specialAuraFinish;

    private Coroutine cancelDamageDisplay;
    private Coroutine flinchCancel; //or flinchReset
    private Coroutine idleCancel;
    private Coroutine flinchOpportunityCancel;
    private Coroutine attackLengthCancel;
    private Coroutine bombLengthCancel;
    private Coroutine counterAttackCancel;
    private Coroutine counterAttackWholeCancel;
    private Coroutine specialCancel;
    private Coroutine stayStillCancel;

    private GameObject[] enemies;
    private List<Collider> collidingEnemies;
    private int collisionCount = 0;

    private bool idleStart = false;
    private bool idle = false;
    private float idleTime = 1;
    public bool attackReady = false;
    public bool bombReady = false;
    public bool summonBombs = false;
    private bool playGrunt = true;
    private bool stayStill = false;

    private bool flinchInterrupt = false; //I may want to changethis to flincOpportuni
    private float flinchWindowTime = 1.5f;
    public bool attack = false; //Putting this here for now. I want this code to be assimple as possible //Need this for now, because may not want to use idle (check for it
    //While attacking a foe
    //07/19/24
    //This is necessary because I need it for foes that counter. //I can't repeatedly trigger their counteratt
    private float attackLength = 1;
    public bool windCaptured = false;
    private bool repeat = true;
    public bool flinching = false;
    private bool barrier = false;
    public bool counterAttackActive = false;
    public bool counterAttackTriggered = false;
    //public bool unflinchingFollow = false;
    //09/03/24 This is good for more complex code
    //Say I don't want my foe to use a counterAttack
    private bool useCounterattack = false;
    public bool counterattacking = false;
    private float counterattackTime = 0;

    public ParticleSystem[] attackEffects;
    private int effectNumber = 0;

    //public gameOb
    public GameObject teamAttackAura;
    public AudioClip teamAttackSizzle;
    public AudioClip attackImpact;
    public AudioClip grunt;
    public GameObject attackEffectObject;
    public AudioClip barrierSound;
    public AudioClip barrierBreak;
    public AudioClip bombLensFlare;
    public GameObject harpShield;
    public GameObject trumpetShield;
    public AudioClip guardOn;
    public AudioClip specialSound;
    public AudioClip specialCancelled;

    public float HP = 10;
    private float originalHP;
    private float damage = 0;
    public float EXP = 0;
    //Individual enemy abilit
    private bool teamAttack = false;
    private bool normal = false;
    public bool teamAttackOn = false;
    private bool red = false;
    private bool green = false;
    private bool armor = false;
    private bool noAttack = false;
    public bool bombUser = false;
    public bool boss = false;
    private bool firstSalvo = true;
    private float armorGauge = 30;
    private float fullArmorGauge = 30;
    private GameObject armorObj;
    //private Image armorBackground;
    private Image armorFill;
    private bool guard = false;
    private bool revengeValue = false;
    private int revengeValueLimit = 10;
    private int currentRevengeValue = 0;
    public bool revengeValueMove = false;
    private bool fusileer = false;
    private float specialGauge = 29;
    private float fullSpecialGauge = 29;
    private GameObject specialObj;
    private Image specialFill;
    private bool rage = false;
    private int rageValueLimit = 12;
    public int currentRageValue = 0;
    public bool rageValueMove = false;
    public bool rageValueMoveActive = false;
    public bool rageLevel1 = false;
    public bool rageLevel2 = false;
    public bool rageLevel3 = false;
    public bool rageStart = false;
    public bool buildRage = false;

    //Individual Attacks
    private bool unblockable = false;
    private bool special = false;
    public bool harpGuard = false;
    public bool trumpetGuard = false;
    public bool guardCounterattackTriggered = false;
    private int guardNumber = 0;
    private bool invincible = false;

    private bool cantFlinch = false;
    private bool gettingDamaged = false;

    public bool cantMove = false;
    // Start is called before the first frame update
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        animation = GetComponent<Animation>();
        difficulty = GameObject.Find("Difficulty Moderator").GetComponent<DifficultyModerator>();

        if (animator != null)
        {
            animatorTrue = true;
        }
        else if (animation != null)
        {
            animationTrue = true;
        }

        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (idleStart == true)
        {
            if (green == false && rage==false) {
                idleCancel = StartCoroutine(IdleAnimation(Random.Range(4, 10)));
            }
            else if(green==true)
            {
                idleCancel = StartCoroutine(IdleAnimation(Random.Range(1, 7)));
            }
            else if (rage == true)
            {
                idleCancel = StartCoroutine(IdleAnimation(10));
            }
            //Debug.Log("Id");
        }
        //Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        collidingEnemies = new List<Collider>();

        effectPosition = transform.Find("Effect Position").gameObject;

        //May need to do this in awake, either in this or in Green Thief
        if (green == true) {
            counterAttackCloud = transform.Find("Counterattack Objects").transform.Find("Counterattack Cloud New").gameObject;
            counterAttackStart = transform.Find("Counterattack Objects").transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
            counterAttackOn = transform.Find("Counterattack Objects").transform.Find("Beam").gameObject.GetComponent<ParticleSystem>();
        }
        if (boss == true)
        {
            originalHP = HP;
            bossHPBarObject = GameObject.Find("Boss HP Object");
            bossHPBarBackground = bossHPBarObject.transform.Find("Boss HP Bar Background").gameObject;
            bossHPBarBackground.SetActive(true);
            HPBarActual = bossHPBarBackground.transform.Find("Boss HP Bar").GetComponent<Image>();
            HPText = bossHPBarBackground.transform.Find("Boss Numeric").GetComponent<TextMeshProUGUI>();
            HPText.text = originalHP + "/" + originalHP;
            damageBar = GameObject.Find("Damage Taken").GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = new Quaternion(0, 180, 0, 0);
        if (windCaptured == false) {
            //Quaternion lookRotation = Quaternion.LookRotation(transform.position, GameObject.Find("Look At").transform.position);
            //transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);
            //AnalyzeTeamAttackCapability();
        }

        if (transform.position.x <= -5.49f)
        {
            transform.position = new Vector3(-5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //F();
        }
        if (transform.position.x >= 5.49f)
        {
            transform.position = new Vector3(5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.y <= -2.5f)
        {
            transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.y >= 3.47f)
        {
            transform.position = new Vector3(transform.position.x, 3.47f, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }

        if (windCaptured == true && playerScript.wind == true)
        {
            if (repeat == true) {
                Flinch(true);
                repeat = false;
                StartCoroutine(WindFlinch());
                StartCoroutine(WindDamage());
            }
            if (armor == false) {
                //transform.Rotate(Vector3.up * 180 * Time.deltaTime);
            }
        }
        if (armor == true)
        {
            armorObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        }
        if (special == true)
        {
            specialObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2.25f, 0));
            if(audio.isPlaying==false)
            {
                audio.PlayOneShot(specialSound, 0.75f / 2 * 1.75f);
            }
        }
        if (playerScript.flute == false)
        {
            WindCaptureImpossible();
        }
        if(stayStill == true)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
    //Setters
    public void SetHP(int newHP)
    {
        HP = newHP;
    }
    public void SetEXP(float newEXP)
    {
        EXP = newEXP;
    }
    //05/01/24
    //I can either set the damage at the start(), or check for team att
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
        if (teamAttackOn == true)
        {
            damage =2;
        }
    }
    public void SetIdleStart()
    {
        idleStart = true;
    }
    public void SetIdleTime(float newTime)
    {
        idleTime = newTime;
    }
    public void NonStandardIdleStart()
    {
        idleCancel = StartCoroutine(IdleAnimation(Random.Range(4, 10)));
    }
    public void SetAttackLength(float newLength)
    {
        attackLength = newLength;
    }
    public void SetNormal()
    {
        normal = true;
    }
    public void SetTeamAttack()
    {
        teamAttack = true;
    }
    public void SetRed()
    {
        red = true;
    }
    public void SetGreen()
    {
        green = true;
        SetCantFlinch();
        counterAttackCloud = transform.Find("Counterattack Objects").transform.Find("Counterattack Cloud New").gameObject;
        counterAttackStart = transform.Find("Counterattack Objects").transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
        counterAttackOn = transform.Find("Counterattack Objects").transform.Find("Beam").gameObject.GetComponent<ParticleSystem>();
    }
    public void UnsetRed()
    {
        red = false;
    }
    public void UnsetGreen()
    {
        green = false;
        UnsetCantFlinch();
    }
    public void SetBombUser()
    {
        bombUser = true;
    }
    public void UnsetBombUser()
    {
        bombUser = false;
    }
    public void PlayerCantPause(float time)
    {
        playerScript.CantPauseMethod(time);
    }
    public void PlayerTransparentUI(float time)
    {
        playerScript.TransparentUI(time);
    }
    public void SetNoAttack()
    {
        noAttack = true;
    }
    public void UnsetNoAttack()
    {
        noAttack = false;
    }
    public void SetArmorAmount(int newAmount)
    {
        fullArmorGauge = newAmount;
    }
    //This is less of an enemy type and more of a mode
    public void SetArmor()
    {
        armor = true;
        //06/19/24
        SetCantFlinch();
        armorObj = GameObject.Find("Armor Bar Object").transform.Find("Armor Bar").gameObject;
        armorObj.SetActive(true);
        armorFill = armorObj.transform.Find("Actual").GetComponent<Image>();
        armorFill.fillAmount = 1;
        armorGauge = fullArmorGauge;
        if (name == "Witch") {
            transform.Find("root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(true);
        }
        
    }
    public void PlayBarrierSound()
    {
        StartCoroutine(DelayBarrierSound());
    }
    IEnumerator DelayBarrierSound()
    {
        yield return new WaitForSeconds(0.25f);
        audio.PlayOneShot(barrierSound, 0.75f);
    }
    public void PlayBombLensFlareSound()
    {
        StartCoroutine(DelayBombLensSound());
    }
    IEnumerator DelayBombLensSound()
    {
        yield return new WaitForSeconds(0.35f);
        audio.PlayOneShot(bombLensFlare, 0.75f);
    }
    public void ArmorOff()
    {
        Destroy(GameObject.Find("Barrier(Clone)"));
        Destroy(GameObject.Find("Dragon Barrier(Clone)"));
        

        armorObj.SetActive(false);
        if (name == "Witch")
        {
            transform.Find("root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(false);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().BarrierOff();
        }
        //06/19/24
        //Almost did this without green ==false. I need this for Dragon
        if (green == false)
        {
            UnsetCantFlinch();
        }
        BarrierOff();
        armor = false;
        audio.PlayOneShot(barrierBreak,1.5f*0.75f);
    }
    //Fun fact, triggerExit doesn't count if the object is destroyed
    public void BarrierOff()
    {
        barrier = false;
    }
    public void SetBoss()
    {
        boss = true;
    }
    public void SetCantFlinch()
    {
        cantFlinch = true;
    }
    public void UnsetCantFlinch()
    {
        cantFlinch = false;
    }
    public void DestroyDebris()
    {
        gameScript.DestroyDebris();
    }
    public void DamageText(float damage)
    {
        gameObject.transform.Find("Damage Received").GetComponent<TextMesh>().text = "" + damage;
    }
    IEnumerator DamageDisplayDuration(float damage)
    {
        DamageText(damage);
        yield return new WaitForSeconds(0.5f);
        gameObject.transform.Find("Damage Received").GetComponent<TextMesh>().text = "";
    }
    //I think I'm going to need every enemy to call this. This is so I don't need to keep running this check
    //This will only be an issue if enemies can teleport
    public void AnalyzeTeamAttackCapability()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float distance;
        bool enemyNextToAnother = false;
        int i = 0;
        //for (int i =0; i < enemies.Length; i++)
        //{
        while (i < enemies.Length && enemyNextToAnother == false) {
            distance = Vector3.Distance(gameObject.transform.position, enemies[i].transform.position);
            //Debug.Log("Distanceequal to "+distance);
            i++;
            if (distance <= 1f)
            {
                enemyNextToAnother = true;
                //if (teamAttack == true && teamAttackOn == false)
                //{
                //teamAttackOn = true;
                //TeamAttackPositives();
                //}
            }
        }
        //}
        if (enemyNextToAnother == false)
        {
            teamAttackOn = false;
            TeamAttackOff();
            //Debug.Log("No enemies next to each");
        }
    }
    //So far so good. The only problem is that I can't do Idle"", false. I need to keep snapping back to Idle
    //This'llonlybe a problem if I want a recover anima
    //08/20/24 I will need to check for red here, now. Will also have to check for armorBreak
    //Or TakeDamage will deal with that
    //This is annoying because only red doesn't flinch from nonArmorBreak
    //Maybe if (red == true) if(armorBreak ==true) flinch
    //Coding is annoying in general like that
    public void Flinch(bool armorBreak)
    {
        //Code like flinchWork is going to be very useful in the future
        bool flinchWork = true;
        if (red == true && armorBreak ==false)
        {
            flinchWork = false;
        }
        if (cantFlinch ==true)
        {
            flinchWork = false;
        }
        if (counterattacking == true)
        {
            flinchWork = false;
        }
        //Putting this here instead of using cantFlinch code everywhere
        if (HP>0) {
            if (flinchWork == true) {
                //Debug.Log("Flinched!");
                attackReady = false;
                bombReady = false;
                summonBombs = false;
                if (flinchInterrupt == true)
                {
                    attack = false;
                    flinchInterrupt = false;

                    //I don't know why I don't think to put this allhere
                    //I think I'm just thinking in the moment
                    //Also, I have to cancel different Coroutines for different mo
                    StopCoroutine(flinchOpportunityCancel);
                    if (bombUser ==false) {
                        StopCoroutine(attackLengthCancel);
                    }
                    else
                    {
                        StopCoroutine(bombLengthCancel);
                    }
                    playerScript.InterruptEffect(effectPosition.transform.position);
                    StopAttackEffect();
                    audio.PlayOneShot(grunt, 2);
                }
                else
                {
                    if (playGrunt == true)
                    {
                        audio.PlayOneShot(grunt, 2);
                        playGrunt = false;
                    }
                    else
                    {
                        playGrunt = true;
                    }
                }

                if (animatorTrue == true)
                {
                    animator.SetBool("Idle", true);
                    animator.SetTrigger("Flinch");
                    //I can see this being a problem for the Red Dragon's last phase
                    //I will make an if case where green == true and red==true
                    if (red == true && green == true)
                    {
                        animator.ResetTrigger("Attack");
                        animator.ResetTrigger("Attack2");
                    }
                    else if (red == true)
                    {
                        animator.ResetTrigger("StrongAttack");
                    }
                    else if (green == true) {
                        animator.ResetTrigger("Attack");
                        animator.ResetTrigger("Attack2");
                    }
                    else if (bombUser == true)
                    {
                        animator.ResetTrigger("Bomb");
                    }
                    else
                    {
                        animator.ResetTrigger("Attack");
                    }
                }
                else if (animationTrue == true)
                {
                    //animator.SetTrigger("Flinch");
                }

                //Moved this from mouseOver 
                if (idleCancel != null)
                {
                    //Debug.Log("Flinch from Idle");
                    StopCoroutine(idleCancel);
                }

                if (flinchCancel != null)
                {
                    StopCoroutine(flinchCancel);
                }

                //Don't know why I didn't put this here right away, because I successful flinch will always start a flinchdur
                flinchCancel = StartCoroutine(FlinchDuration());

                if (green == true)
                {
                    SetCantFlinch();
                    //Debug.Log("Green ThiefFlinched " + cantFlinch);
                }
                if (fusileer == true)
                {
                    SetCantFlinch();
                }
                if (rage == true)
                {
                    SetCantFlinch();
                }
            }
        }
    }
    IEnumerator FlinchDuration()
    {
        flinching = true;
        yield return new WaitForSeconds(2);
        if (animatorTrue == true)
        {
            animator.ResetTrigger("Flinch");
        }
        flinching = false;
        //06/24/24
        //ATM, use idleTime. Most foes have idleTimes of 5. It wouldn't make sense for Witch
        //06/25/24 atm, I am doing this because I don't want Dragon to be not doing something for so long
        //I can do something like recoveryIdleTime
        //if (useRecoveryIdleTime), IdleAnimation(recoveryIdleTime)
        if (rage ==false)
        {
            idleCancel = StartCoroutine(IdleAnimation(5));
        }
        else
        {
            idleCancel = StartCoroutine(IdleAnimation(10));
        }
        //Debug.Log("IdleAnimation");

        currentRevengeValue = 0;
        if (guard ==true)
        {
            if (harpGuard==false&&trumpetGuard==false) {
                RestartGuard();
            }
        }
    }
    public void FlinchCancel()
    {
        if (flinchCancel != null)
        {
            StopCoroutine(flinchCancel);
        }
        if (animatorTrue == true)
        {
            animator.ResetTrigger("Flinch");
        }
        flinching = false;
    }
    //For Flashing (Revenge Value/ Boss interrupt)
    public void Interrupt()
    {
        StartCoroutine(IdleAnimation(2.5f));
        StartCoroutine(InterruptTime());
    }
    IEnumerator InterruptTime()
    {
        cantFlinch = true;
        yield return new WaitForSeconds(2.5f);
        cantFlinch = false;
    }
    IEnumerator WindFlinch()
    {
        yield return new WaitForSeconds(1.5f);
        repeat = true;
        playerScript.WindDamage();
    }
    IEnumerator WindDamage()
    {
        yield return new WaitForSeconds(1.75f);
        //Doesn't need to be changed, becauseonce windCaptured, foes can't do anything
        TakeDamage(1, true, false);
        
    }
    public void SetFlinchWindow(float newTime)
    {
        flinchWindowTime = newTime;
    }
    public void StartFlinchWindow()
    {
        flinchOpportunityCancel = StartCoroutine(FlinchWindow());
        //For simplicity, I'm putting this here. I could put this in Green Thief, but I need this for Dragon too
        //UnsetCantFlinch();
    }
    //If you hit the foe with the right attack during this window, their attack will be interr
    IEnumerator FlinchWindow()
    {
        flinchInterrupt = true;
        yield return new WaitForSeconds(flinchWindowTime);
        flinchInterrupt = false;

    }
    //I forgot to do AttackReadyOff() after an attack, to not repeat the attack. This lead to other problems 04/15/24
    public void AttackReadyOff()
    {
        attackReady = false;
    }
    public void BombReadyOff()
    {
        bombReady = false;
    }
    public void SummonBombsOff()
    {
        summonBombs = false;
    }
    public void CounterAttackReadyOff()
    {
        counterAttackTriggered = false;
    }
    //public void UnflinchingFollowOff()
    //{
        //unflinchingFollow = false;
    //}
    public void SetGuard()
    {
        guard = true;
    }
    public void UnsetGuard()
    {
        guard = false;
    }
    public void SetRevengeValue()
    {
        revengeValue = true;
    }
    public void SetRevengeValueNumber(int newValue)
    {
        revengeValueLimit = newValue;
    }
    public void RevengeValueUp()
    {
        if (revengeValue ==true) {
            currentRevengeValue++;
            if (currentRevengeValue >= revengeValueLimit)
            {
                IdleAnimationCancel();
                currentRevengeValue = 0;
                revengeValueMove = true;
                
            }
        }
    }
    public void RevengeValueMoveOff()
    {
        revengeValueMove = false;
    }
    //I can either use RestartIdle with no waitTime and use it with the waitTime of Guard,
    //or do this
    public void RestartIdleMethod(float time)
    {
        StartCoroutine(RestartIdle(time));
    }
    IEnumerator RestartIdle(float time)
    {
        yield return new WaitForSeconds(time);
        StartIdle();
    }
    public void SetRage()
    {
        rage = true;
        SetCantFlinch();
    }
    public void SetRageValueNumber(int newValue)
    {
        rageValueLimit = newValue;
    }
    public void RageValueUp()
    {
        if (rage == true)
        {
            rageStart = true;
            if (buildRage==true) {
                currentRageValue++;
                if (currentRageValue == 3)
                {
                    rageLevel1 = true;
                }
                if (currentRageValue == 6)
                {
                    rageLevel1 = false;
                    rageLevel2 = true;
                }
                if (currentRageValue == 9)
                {
                    rageLevel2 = false;
                    rageLevel3 = true;
                }
                if (currentRageValue > 9)
                {
                    IdleAnimationCancel();
                    currentRageValue = 0;
                    rageValueMove = true;
                    rageValueMoveActive = true;
                    buildRage =false;
                }
            }
        }
    }
    public void RageValueMoveOff()
    {
        rageValueMove = false;
    }
    public void RageValueMoveActiveOff()
    {
        rageValueMoveActive = false;
    }
    public void CooldownStart()
    {
        StartCoroutine(Cooldown());
    }
    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1);
        RageOff();
        RageValueMoveActiveOff();
    }
    public void RageOff()
    {
        rageStart = false;
        rageLevel1 = false;
        rageLevel2 = false;
        rageLevel3 = false;
        currentRageValue = 0;
        buildRage = false;
    }
    public void SetFusileer()
    {
        fusileer = true;
    }

    public void SetUnblockable()
    {
        unblockable = true;
    }
    public void UnsetUnblockable()
    {
        unblockable = false;
    }
    public void SetHarpGuard()
    {
        harpGuard = true;
        guardNumber = 0;
        StartCoroutine(GuardAnimation());
        harpShield.SetActive(true);
        audio.PlayOneShot(guardOn, 0.85f);
    }
    public void UnsetHarpGuard()
    {
        harpGuard = false;
        harpShield.SetActive(false);
    }
    public void SetTrumpetGuard()
    {
        trumpetGuard = true;
        guardNumber = 1;
        StartCoroutine(GuardAnimation());
        trumpetShield.SetActive(true);
        audio.PlayOneShot(guardOn, 0.85f);
    }
    public void UnsetTrumpetGuard()
    {
        trumpetGuard = false;
        trumpetShield.SetActive(false);
    }
    IEnumerator GuardAnimation()
    {
        animator.SetTrigger("GuardShift");
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("GuardShift");
    }
public void RestartGuard()
    {
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            SetHarpGuard();
        }
        else
        {
            SetTrumpetGuard();
        }
    }
    public void GuardCounterattack()
    {
        IdleAnimationCancel();
        counterAttackTriggered = true;
        //unflinchingFollow = true;
        attack = true;
        
    }
    public void SpecialObjects()
    {
        specialAuraObject = transform.Find("Special Object").gameObject;
        specialAuraStart = specialAuraObject.transform.Find("Start").gameObject.GetComponent<ParticleSystem>();
        specialAura1 = specialAuraObject.transform.Find("Middle").gameObject.GetComponent<ParticleSystem>();
        specialAura2 = specialAuraObject.transform.Find("Middle 2").gameObject.GetComponent<ParticleSystem>();
        specialAura3 = specialAuraObject.transform.Find("Middle 3").gameObject.GetComponent<ParticleSystem>();
        specialAuraFinish = specialAuraObject.transform.Find("End").gameObject.GetComponent<ParticleSystem>();
    }
    public void SetSpecial()
    {
        special = true;
        specialObj = GameObject.Find("Special Bar Object").transform.Find("Special Bar").gameObject;
        specialObj.SetActive(true);
        specialFill = specialObj.transform.Find("Actual").GetComponent<Image>();
        specialFill.fillAmount = 1;
        specialGauge = fullSpecialGauge;

    }
    public void UnsetSpecial()
    {
        special = false;
    }
    public void SpecialActivate()
    {
        special = false;
        //specialObj.SetActive(false);
    }
    public void SpecialCancel()
    {
        specialObj.SetActive(false);
        if (HP >0) {
            special = false;
            StopCoroutine(specialCancel);
            UnsetCantFlinch();
            Flinch(true);

            playerScript.InterruptEffect(effectPosition.transform.position);
            audio.Stop();
            audio.PlayOneShot(specialCancelled, 1);
            audio.PlayOneShot(grunt, 1);
            specialAura1.Stop();
            specialAura2.Stop();
            specialAura3.Stop();
            //specialAuraFinish.Stop();
        }
    }

    public void StartAttackLength()
    {
        attackLengthCancel = StartCoroutine(AttackLength());
    }
    public void StartBombLength()
    {
        bombLengthCancel = StartCoroutine(BombLength());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator AttackLength()
    {
        yield return new WaitForSeconds(attackLength);
        //animator.ResetTrigger("Attack");
        //StartCoroutine(IdleAnimation());
        //06/20/24
        //The problem is that I'm using StartIdle no matter what. For counterattack, I'm not supposed to do that
        if (special==false) {
            StartIdle();
            //Debug.Log("StartIdleNormal");
        }
        if (noAttack==false) {
            DealDamage(damage);
        }
        if (guard == true)
        {
            if (guardNumber == 0)
            {
                SetHarpGuard();
            }
            else
            {
                SetTrumpetGuard();
            }
        }
    }
    IEnumerator BombLength()
    {
        yield return new WaitForSeconds(attackLength);
        summonBombs = true;
        StartIdle();
    }
    public void StartAttackLengthAlternate()
    {
        attackLengthCancel = StartCoroutine(AttackLengthAlternate());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator AttackLengthAlternate()
    {
        yield return new WaitForSeconds(attackLength);
            DealDamage(damage);
    }
    public void SetCounterattackTime(float time)
    {
        counterattackTime = time;
    }
    public void StartCounterattackTimeMethod()
    {
        StartCoroutine(StartCounterattackTime());
    }
    IEnumerator StartCounterattackTime()
    {
        counterattacking = true;
        yield return new WaitForSeconds(counterattackTime);
        counterattacking = false;
    }
    public void StartCounterAttackLength()
    {
        //Took this out because this can't get cancelled
        //Maybe this is causing the glitch after a counterattack
        StartCoroutine(CounterAttackLength());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator CounterAttackLength()
    {
        yield return new WaitForSeconds(attackLength);
        animator.ResetTrigger("Counterattack");
        animator.SetBool("Idle",true);
        //StartCoroutine(IdleAnimation());
        //StartIdle();
        StartCoroutine(FollowUpAttack(1));
        DealDamage(damage);
    }
    public void SetCounterattack()
    {
        useCounterattack = true;
    }
    public void UnsetCounterattack()
    {
        useCounterattack = false;
    }
    public void CounterAttackTriggered()
    {
        counterAttackActive = false;
        StopCoroutine(counterAttackCancel);
        counterAttackTriggered = true;
        //unflinchingFollow = true;

        //StartCoroutine(FollowUpAttack(4));
        animator.SetBool("Idle", true);
        counterAttackOn.Stop();
        counterAttackCloud.SetActive(false);
        //Debug.Log("Counterattack triggered " + counterAttackTriggered);
    }
    public void StartGuardAttackLength()
    {
        attackLengthCancel = StartCoroutine(GuardAttackLength());
    }
    IEnumerator GuardAttackLength()
    {
        yield return new WaitForSeconds(attackLength);
        animator.ResetTrigger("Counterattack");
        animator.SetBool("Idle", true);
        //StartCoroutine(IdleAnimation());
        //StartIdle();
        StartCoroutine(FollowUpAttack(1));
        DealDamage(damage);
    }
    //06/21/24
    //This triggered an error somehow
    //But I loaded effectNumber;
    //Oh I know why. Because I don't PlayAttackEffect(). I deactivated it to stop crashing the editor.
    public void PlayAttackEffect(int attackEffect)
    {
        effectNumber = attackEffect;
        if (gameScript.playEffects== true) {
            if (attackEffects.Length>0) {
                attackEffects[effectNumber].Play();
            }
        }
    }
    public void StopAttackEffect()
    {
        if (gameScript.playEffects ==true) {
            if (attackEffects.Length > 0)
                attackEffects[effectNumber].Stop();
        }
    }
    public void DealDamage(float newDamage)
    {
        if (difficulty.hardNonStatic==true)
        {
            newDamage *= 2;
        }
        if (HP >0) {
            if (unblockable == true)
            {
                if (playerScript.specialInvincibility == true)
                {
                    playerScript.GenerateShield(effectPosition.transform.position);
                    playerScript.PlayGuardSound();
                }
                else
                {
                    if (gameScript.gameOver == false)
                    {
                        playerScript.GeneralDamageCode(newDamage, 8, unblockable);
                    }
                    audio.PlayOneShot(attackImpact, 1.5f);
                }
            }
            else if (playerScript.shieldOn == false && playerScript.specialInvincibility == false)
            {
                //playerScript.GeneralDamageCode(newDamage, newDamage);
                //playerScript.PlayHurtEffect(effectAppear.transform.position);
                //playerScript.DamageFlashOn();
                if (newDamage < 3)
                {
                    playerScript.GeneralDamageCode(newDamage, 3, unblockable);
                    if (gameScript.gameOver == false) {
                        audio.PlayOneShot(attackImpact, 1);
                    }
                }
                else
                {
                    if (gameScript.gameOver == false)
                    {
                        playerScript.GeneralDamageCode(newDamage, 8, unblockable);
                    }
                    audio.PlayOneShot(attackImpact, 1.5f);
                }
            }
            else if (playerScript.shieldOn == true || playerScript.specialInvincibility == true)
            {
                playerScript.GenerateShield(effectPosition.transform.position);
                if (playerScript.specialInvincibility == false) {
                    if (playerScript.shieldOn == true)
                    {
                        //if (newDamage <3) {
                        //playerScript.ShieldGaugeDown(newDamage);
                        //}
                        //else
                        //{
                        playerScript.ShieldGaugeDown(newDamage);
                        //}
                    }
                }
                else
                {
                    playerScript.PlayGuardSound();
                }
            }
        }
    }
    public void StartIdle()
    {
        idleCancel = StartCoroutine(IdleAnimation(idleTime));
        //Debug.Log("Idle Now");
    }
    //This makes sense because not all foes have the same idletime
    IEnumerator IdleAnimation(float idleTime)
    {
        idle = true;
        attack = false;
        attackReady = false;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle", true);
            //animator.ResetTrigger("Attack");

            //06/03/24
            //I just realized I don't even reset the triggers after an attack if I don't interrupt them
            if (red == true && green == true)
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Attack2");
            }
            else if (red == true)
            {
                animator.ResetTrigger("StrongAttack");
            }
            else if (bombUser == true)
            {
                animator.ResetTrigger("Bomb");
            }
            else if (green == true)
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Attack2");
            }
            else if (unblockable == true)
            {
                animator.ResetTrigger("UnblockableAttack");
            }
            else
            {
                animator.ResetTrigger("Attack");
            }
        }
        //UnflinchingFollowOff();

        buildRage = true;
        yield return new WaitForSeconds(idleTime);
        if (cantMove == false)
        {
            idle = false;
            //09/03/24
            //I need to rearrange 
            if (bombUser == true)
            {
                attack = true;
                bombReady = true;
            }
            else if (fusileer == true)
            {
                PauseBeforeSpecialStart(20);
            }
            //This needs to be simplified
            //Maybe regularAttack
            else if (green == false &&fusileer==false &&bombUser==false)
            {
                attack = true;
                attackReady = true;
            }
            else if(green ==true)
            {
                if (useCounterattack== true) {
                    counterAttackWholeCancel = StartCoroutine(CounterattackCloud());
                }
                    //Debug.Log("CounterattackCloud");
            }

            if(rage ==true)
            {

            }
        }
    }
    //Used more for forcing the boss back into its attack pattern
    //Like with Revenge Value
    //I may need to merge this with FlinchCancel();
    //Maybe make a RevengeValue Method
    //It's annoying, because both IdleAnimation and Flinch cause IdleAnimation
    //08/29/24 This is in case cancelling attackReady isn't enough. The stuff I added. Sometimes a regular attack still happens
    //while Guard is follow up attacking or Rager is doing its rage att
    //I also moved IdleAnimationCancel up for methods that use it, so that idle, true and idle, false don't conflict. This should work, because the
    //MoveOn lines play after
    public void IdleAnimationCancel()
    {
        if (idleCancel != null)
        {
            StopCoroutine(idleCancel);
        }
        if (attackLengthCancel != null)
        {
            StopCoroutine(attackLengthCancel);
        }
        if (flinchOpportunityCancel != null)
        {
            StopCoroutine(flinchOpportunityCancel);
        }
        animator.SetBool("Idle",true);
        animator.ResetTrigger("Attack");
        attack = false;
        attackReady = false;
    }
    public void IdleBoolAnimatorCancel()
    {
        if (animatorTrue == true)
        {
            animator.SetBool("Idle", false);
        }
    }
    public void PauseBeforeSpecialStart(float time)
    {
        //Special Effect
        StartCoroutine(PauseOver1(time));
        //Debug.Log("Special Start");
        StartCoroutine(PlaySpecialSound(0.75f*1.5f));
        SpecialObjects();
        specialAuraStart.Play();
    }
    IEnumerator PauseOver1(float time)
    {
        yield return new WaitForSeconds(3);
        specialCancel = StartCoroutine(SetSpecialTime(time));
        SetSpecial();
        audio.Stop();
        audio.PlayOneShot(specialSound, 0.75f/2 * 1.75f);
        specialAuraStart.Stop();
        specialAura1.Play();
        StartCoroutine(SpecialAuras2());
    }
    IEnumerator SetSpecialTime(float time)
    {
        yield return new WaitForSeconds(time);
        PauseBeforeSpecial();
    }
    public void PauseBeforeSpecial()
    {
        //Special Effect
        StartCoroutine(PauseOver2());
        audio.Stop();
        StartCoroutine(PlaySpecialSound(0.75f*1/2*3 * 2));
        specialObj.SetActive(false);
        specialAura1.Stop();
        specialAura2.Stop();
        specialAura3.Stop();
        specialAuraFinish.Play();
    }
    IEnumerator PauseOver2()
    {
        yield return new WaitForSeconds(3);
        audio.Stop();
        SpecialActivate();
        attackReady = true;
        specialAuraFinish.Stop();
    }
    IEnumerator PlaySpecialSound(float volume)
    {
        audio.PlayOneShot(specialSound,volume);
        yield return new WaitForSeconds(3);
    }
    IEnumerator SpecialAuras2()
    {
        yield return new WaitForSeconds(6.5f);
        specialAura2.Play();
        StartCoroutine(SpecialAuras3());
    }
    IEnumerator SpecialAuras3()
    {
        yield return new WaitForSeconds(6.5f);
        specialAura3.Play();
    }

    IEnumerator CounterattackCloud()
    {
        counterAttackCloud.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CounterAttackStart());

    }
    IEnumerator CounterAttackStart()
    {
        counterAttackStart.Play();
        //counterAttackOn.Play();
        yield return new WaitForSeconds(1);
        counterAttackCancel = StartCoroutine(CounterAttack());
        //counterAttackStart.SetActive(false);
        counterAttackOn.Play();
        counterAttackActive = true;
    }
    IEnumerator CounterAttack()
    {
        yield return new WaitForSeconds(3);
        counterAttackActive = false;
        StartCoroutine(FollowUpAttack(3));
        animator.SetBool("Idle", true);
        counterAttackOn.Stop();
        counterAttackCloud.SetActive(false);
        Debug.Log("Counterattack over");
    }
    IEnumerator FollowUpAttack(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        attack = true;
        attackReady = true;
        if (animatorTrue == true)
        {
            animator.SetBool("Idle", false);
        }
        if(counterattacking == false)
        {
            //06/19/24forgot to dothis, which may be why Green Dragon isn't getting Unflinched
            if (armor ==false) {
                UnsetCantFlinch();
            }
        }
        //Debug.Log("Followup Attack");
    }
    public void CounterAttackWholeCancel()
    {
        if (counterAttackWholeCancel !=null) {
            StopCoroutine(counterAttackWholeCancel);
        }
        if (counterAttackCancel != null)
        {
            StopCoroutine(counterAttackCancel);
        }
        //counterAttackStart.SetActive(false);
        counterAttackOn.Stop();
        counterAttackCloud.SetActive(false);
        counterAttackActive = false;
        counterAttackTriggered = false;
    }
    public void TakeDamage(float damage, bool armorBreak, bool playerSpecial)
    {


                HP -= damage;
                if (boss == true)
                {
                    HPBarActual.fillAmount -= (float)damage / originalHP;
                    HPText.text = HP + "/" + originalHP;
                    if (gettingDamaged == false)
                    {
                        StartCoroutine(DamageBar());
                    }
                }

        DamageDisplay(damage);


            if (HP <= 0)
            {
                WindCaptureEnd();

            if (special==true)
            {
                SpecialCancel();
            }
                if (boss == false)
                {
                    for(int i=0;i <collidingEnemies.Count;i++)
                    {
                        if (collidingEnemies.Count > 0) {
                            if (collidingEnemies[i].GetComponent<Enemy>().teamAttack == true) {
                                collidingEnemies[i].GetComponent<Enemy>().CollisionCountDown();
                            }
                        }
                    }
                    gameObject.SetActive(false);
                    Destroy(gameObject, 2);

                }
                else
                {
                    animator.SetBool("Dying", true);
                    Destroy(gameObject, 5);
                    tag = "Untagged";
                }
                playerScript.GainEXP(EXP); //Putting this after ReduceNumEnemies is making it so that the last 70 doesn't add 
                gameScript.ReduceNumEnemies();

            }
    }
    //This will decide if damage is taken, a flinch happens or a counterattack happens
    //Harp ==0 and trumpet ==1 anddebrisandbomb == 2
    //playerSpecial will either ignore guard or pick the right guard (code wise)
    //Or maybe I don't need this
    //I'm just gonna simplify everything
    //I will still do some smaller methods
    //The most important thing is that this method will determine if Flinch() happens
    public void GeneralDamageCode(float damage, bool armorBreak, int harpOrTrumpet, bool playerSpecial)
    {

        //damage *= 4;
        //I want to do this, but I gotta make sure that counterattacker isn't damaged
        //I may have to write a lot of conditionals for TakeDamage, then (for it to happen)
        //I don't think I can simplify this, because I need to account for the cases that aren't a counterattacker, guard
        //or barrier user
        //So fucking annoying
        //I may need to rewrite this from scratch
        //For Guard, I will have CantFlinch turn on when guard is triggered, because I'm pretty sure I could still Flinch
        //Guard while it's performing a follow up attack
        //08/26/24 I think this is going to need to handle damage display, too, because not all enemies will TakeDam
        //Maybe use a DisplayDamage Method so I don'thave to keep copy and pasting
        //I only have to do this for ArmorBarDown and TakeDam
        //Debug.Log(name + " GeneralDamage Triggered");
        if (invincible ==false) {
            if (counterAttackActive == true)
            {
                CounterAttackTriggered();
            }
            else if (guard == true)
            {
                //I may need to turn off Guard if Guard is flinching
                //Both guards turn off when successfully flinched
                //...Except for wind flinch
                //08/22/24 I really am resourceful and good at what I do, because I accounted for counterattack being triggered multiple times
                //Was gonna write that I'm good at catching mistakes
                //windCaptured == false, so I don't have to deal with counter attacks while moving Guards into debris and bombs
                //I'm very dang sure unflinchingFollow doesn't work
                //I'm thinking of having guard unset when it is staggered, and then its revenge value goes up
                //I had to do this, because damage wasn't happening properly either
                //Originally, Guard would just take damage from Special. No Guard down. No Revenge Val
                //09/03/24 Really complicated, but it makes sense, because unlike Kingdom Hearts Guard, this doesn't guard just one type of attack or all
                //Also, because player hasno flinch
                //My middle ground is to have Guard quickly reestablish its guard after followup att
                //But this brings up the possibility of destroying its barrier while it's summoning
                //Just checked, This won't be a problem,because Guard only gets summoned at the end of the IEnum
                //The current downcode won't be a proble either, because Guard cancels Guard when it does a nonfollow upattack (regular attack when guardcounterattack isn't triggered)
                //Final, I need to make sure flinch can't happen when counterattack is happening. Less importantly,follow
                //I'm thinking of having a counterattack bool that plays as a counterattack and followup attack are play
                //Afterwards, I need to apply this to green
                //I think I should set counterAttacking length, like I set attacklength
                //Because all attack lengths are vari
                //I need to make sure even specials can't flinch counter
                if (windCaptured == false) {
                    //Player Special will cause damage and flinch and will interrupt guards
                    if (playerSpecial==false) {
                        if (harpGuard == true)
                        {
                            if (harpOrTrumpet != 0)
                            {
                                if (attack == false)
                                {
                                    GuardCounterattack();
                                }
                            }
                            else
                            {
                                //Flinch(false);
                                if (counterattacking ==false) {
                                    UnsetCantFlinch();
                                    RevengeValueUp();
                                }
                                TakeDamage(damage, false, false);
                                
                                UnsetHarpGuard();
                                UnsetGuard();
                            }
                        }
                        if (trumpetGuard == true)
                        {
                            if (harpOrTrumpet != 1)
                            {
                                if (attack == false)
                                {
                                    GuardCounterattack();
                                }
                            }
                            else
                            {
                                //Flinch(false);
                                if (counterattacking == false)
                                {
                                    UnsetCantFlinch();
                                    RevengeValueUp();
                                }
                                TakeDamage(damage, true, false);
                                UnsetTrumpetGuard();
                                UnsetGuard();
                            }
                        }
                    }
                    else
                    {
                        if (counterattacking == false)
                        {
                            UnsetCantFlinch();
                        }
                        TakeDamage(damage, true, true);
                        //RevengeValueUp();
                        UnsetHarpGuard();
                        UnsetTrumpetGuard();
                        UnsetGuard();
                    }
                }
            }
            else if (armor == true)
            {
                //Create another method for damaging armor
                //Need to account for Witch not getting armor and barrier
                ArmorBarDown(damage, armorBreak);
            }
            else
            {
                //Will also have code for damaging Special
                //08/26/24 Need bombs to deal usual damage even if they don't flinch Red 
                if(barrier ==true)
                {
                    if (armorBreak ==false)
                    {
                        damage /= 2;
                    }
                    
                }
                TakeDamage(damage, armorBreak, playerSpecial);
                RevengeValueUp();
            }

            if (special == true)
            {
                SpecialBarDown(playerSpecial);
            }

            //Flinch() should behere, because Flinch() decides if flinch even happens
            Flinch(armorBreak);
            //08/22/24 I can't put this in TakeDamage, because some stuff like windCapture is not supposed to add Revenge
            //Also, I can't put this here, because RevengeValue is supposed to go up only when the foetakes damage
            //RevengeValueUp(); //Gonna put this here, because it only matters if revengeValue ==true
            //I could always rewrite this to be like RevengeValueUp()
            if (rageValueMoveActive == false)
            {
                RageValueUp();
            }
            playerScript.HitCountUp();
        }
    }
    IEnumerator DamageBar()
    {
        gettingDamaged = true;
        yield return new WaitForSeconds(3);
        gettingDamaged = false;
        damageBar.fillAmount = HPBarActual.fillAmount;
    }
    public void ArmorBarDown(float damage, bool armorBreak)
    {
        if (armorBreak == false)
        {
            damage /= 2;
        }
        armorGauge -= damage;
        armorFill.fillAmount -= (float)damage / fullArmorGauge;
        if (armorGauge <= 0)
        {
            ArmorOff();
            //Easy way to deal damage after armor is brok
            if (armorGauge < 0) {
                DamageDisplay(-armorGauge);
            }
        }
        else
        {
            DamageDisplay(damage);
        }
    }
    public void SpecialBarDown(bool playerSpecial)
    {
        //Debug.Log("Armor damaged ");
        //I don't need specialDamage anymor
        int specialDamage = 0;
        if (playerSpecial == false)
        {
            specialDamage = 1;
        }
        else
        {
            specialDamage = 15;
        }
        specialGauge -= specialDamage;
        specialFill.fillAmount -= (float)specialDamage / fullSpecialGauge;
        if (specialGauge <= 0)
        {
            SpecialCancel();
        }
    }
    //I could make it better by making the damage for a second become a little bigger and have a light outline and then become small
    public void DamageDisplay(float amount)
    {
        //DamageText(damage);
        if (cancelDamageDisplay != null)
        {
            StopCoroutine(cancelDamageDisplay);
        }
        cancelDamageDisplay = StartCoroutine(DamageDisplayDuration(amount));
    }
    //08/20/24 I could make WindCaptureEnd() happen if player.wind ==false
    //I think I'll just have WindEnd search every object and if windCapture is ==true for it,I end windCapture for it
    public void WindCaptureEnd()
    {
        windCaptured = false;
        //Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);
        //transform.rotation = new Quaternion(0, 180, 0,0);
        //Debug.Log("Wind " + windCaptured);
        if (teamAttack==true) {
            AnalyzeTeamAttackCapability();
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().WindCaptureEnd();
        }
    }
    public void WindCaptureImpossible()
    {
        GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
    }
    IEnumerator Vector3Zero()
    {
        stayStill = true;
        yield return new WaitForSeconds(2);
        stayStill = false;
    }
    public void TeamAttackPositives()
    {
        teamAttackAura.SetActive(true);
        damage = 2;
        audio.PlayOneShot(teamAttackSizzle,0.25f);
    }
    public void TeamAttackOff()
    {
        teamAttackAura.SetActive(false);
        damage =1;
    }
    public void CollisionCountDown()
    {
        collisionCount--;
    }
    public void BossDying()
    {
        damage = 0;
        attackReady = false;
        noAttack = true;
        animator.SetBool("Idle", false);
    }

    private void OnMouseOver()
    {

        if (normal == false)
        {
            if (playerScript.flute ==true)
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
            }
        }
        else
        {
            GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
        }
    }
    private void OnMouseExit()
    {
        if (normal == false)
        {
            if (playerScript.flute == true)
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
            }
        }
    }
    private void OnMouseDown()
    {
        if (playerScript.wind == false)
        {
            if (playerScript.flute == true)
            {
                if (playerScript.fluteDrained == false)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //TrumpetAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                        if (normal == true) {
                            playerScript.FluteAttack();
                            playerScript.WindOn();
                            windCaptured = true;
                            if (stayStillCancel !=null)
                            {
                                StopCoroutine(stayStillCancel);
                                stayStill = false;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnKeyDown()
    {
        if (playerScript.wind == false)
        {
            if (playerScript.flute == true)
            {
                if (playerScript.fluteDrained == false)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        //TrumpetAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                        if (normal == true)
                        {
                            playerScript.FluteAttack();
                            playerScript.WindOn();
                            windCaptured = true;
                        }
                    }
                }
            }
        }
    }
    private void OnMouseDrag()
    {
        //Debug.Log("Dragged");
        if (playerScript.wind==true)
        {
            
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y-3, -Camera.main.transform.position.z - 7.59f));
        }
    }
    private void OnMouseUp()
    {
        if (playerScript.wind == true)
        {
            playerScript.WindEnd();
            WindCaptureEnd();
            //Debug.Log("End");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Need this for boundary
        if (collision.gameObject.CompareTag("Enemy") ||collision.gameObject.CompareTag("Bomb")|| collision.gameObject.CompareTag("Debris")) {
            
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //08/26/24 Something tells me that this should only be used for wind
            if(stayStill ==false)
            {
                stayStillCancel = StartCoroutine(Vector3Zero());
                //Debug.Log("StayStill");
            }
        }
        if (playerScript.wind==true)
        {
        //Wind off. Need wind variable for enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                playerScript.HitBox(collision.GetContact(0).point);
                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                //I need to cancel out a bunch of coroutines
                //if (windCaptured==true) {
                    WindCaptureEnd();
                //}

                    playerScript.WindHitEffect(collision.GetContact(0).point);
                if (teamAttackOn == false && teamAttack == true)
                {
                    teamAttackOn = true;
                    TeamAttackPositives();
                }
            }
            if (collision.gameObject.CompareTag("Debris"))
            {

                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                //I need to cancel out a bunch of coroutines
                //if (windCaptured==true) {
                WindCaptureEnd();
                //}
                //08/26/24 Need to ensure that this doesn't spawn more than one hitbox
                playerScript.DebrisHitBox(collision.GetContact(0).point);
                playerScript.DebrisHitEffect(collision.GetContact(0).point);
                Destroy(collision.gameObject);
                gameScript.ReduceNumDebris();
            }
            if (collision.gameObject.CompareTag("Bomb"))
            {

                playerScript.WindEnd();
                WindCaptureEnd();
                //playerScript.HitCountUp();
                collision.gameObject.GetComponent<Bomb>().EnemyExplode();
            }
        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (teamAttackOn == false && teamAttack == true)
            {
                teamAttackOn = true;
                TeamAttackPositives();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        //if (teamAttackOn == true && teamAttack == true)
        //{
            //teamAttackOn = false;
            //TeamAttackOff();
        //}
    }
    private void OnTriggerEnter(Collider other)
    {
        bool armorBreak = false;
        if (other.CompareTag("Trumpet"))
        {
            bool damaged = false;
            if (damaged ==false)
            {
                damaged = true;
                GeneralDamageCode(2, true, 1, false);
                playerScript.TrumpetHitEffect(effectPosition.transform.position);
            }
        }
        if (other.CompareTag("Harp"))
        {
            //Debug.Log("Hit twice?");
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                GeneralDamageCode(1,false,0, false);
                //For some reason this causes multiple hits
                //08/22/24
                //I think this was handled
                playerScript.HarpHitEffect(effectPosition.transform.position);
            }
        }
        //08/22/24 I can't tell if I want bombs to be armor piercing
        //To simplify, I could make a hitbox script and use a public inspector to set damage
        //Or have code call the hitbox script to set the damage
        //08/26/24
        //For simplicity, I am making it so that Red flinches from bombs
        if (other.CompareTag("Bomb Hitbox"))
        {
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                GeneralDamageCode(3, true, 2, false);
            }
        }
        if (other.CompareTag("Debris Hitbox"))
        {
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                GeneralDamageCode(2, false, 2, false);
            }
        }
        if (other.CompareTag("Hitbox"))
        {
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                GeneralDamageCode(other.GetComponent<Hitbox>().damage, other.GetComponent<Hitbox>().armorBreak, 2, false);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            if (GameObject.Find("Barrier(Clone)") !=null) {
                barrier = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            barrier = false;
        }
    }
}
