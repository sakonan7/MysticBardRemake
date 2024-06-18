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

//TaskList
//Make Foe Not Spazz Between Idle And Att
public class Enemy : MonoBehaviour
{
    private Animator animator;
    private Animation animation;
    private bool animatorTrue = false;
    private bool animationTrue = false;
    private PlayerController playerScript;
    private GameObject effectPosition;
    private GameManager gameScript;
    private GameObject counterAttackCloud;
    private GameObject counterAttackStart;
    private GameObject counterAttackOn;

    private Coroutine cancelDamageDisplay;
    private Coroutine flinchCancel; //or flinchReset
    private Coroutine idleCancel;
    private Coroutine flinchOpportunityCancel;
    private Coroutine attackLengthCancel;
    private Coroutine counterAttackCancel;

    private GameObject[] enemies;

    private bool idleStart = false;
    private bool idle = false;
    private float idleTime = 1;
    public bool attackReady = false;

    private bool flinchInterrupt = false; //I may want to changethis to flincOpportuni
    private bool attack = false; //Putting this here for now. I want this code to be assimple as possible //Need this for now, because may not want to use idle (check for it
    //While attacking a foe
    private float attackLength = 1;
    private bool windCaptured = false;
    private bool repeat = true;
    private bool flinching = false;
    private bool barrier = false;
    private bool counterAttackActive = false;
    public bool counterAttackTriggered = false;

    public ParticleSystem[] attackEffects;
    private int effectNumber = 0;

    //public gameOb
    public GameObject teamAttackAura;

    public float HP = 10;
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
    private bool bomb = false;
    private bool boss = false;
    private bool firstSalvo = true;
    private float armorGauge = 30;
    private float fullArmorGauge = 30;
    private GameObject armorObj;
    //private Image armorBackground;
    private Image armorFill;

    private bool cantFlinch = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animation = GetComponent<Animation>();
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
            idleCancel = StartCoroutine(IdleAnimation(Random.Range(4, 10)));
            //Debug.Log("Id");
        }
        //Quaternion lookRotation = Quaternion.LookRotation(GameObject.Find("Look At").transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(new Quaternion(0, transform.rotation.y, transform.rotation.z, 0), lookRotation, 3);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        effectPosition = transform.Find("Effect Position").gameObject;

        //May need to do this in awake, either in this or in Green Thief
        if (green ==true) {
            counterAttackCloud = transform.Find("Counterattack Objects").transform.Find("Counterattack Cloud 2").gameObject;
            counterAttackStart = transform.Find("Counterattack Objects").transform.Find("Counterattack Start").gameObject;
            counterAttackOn = transform.Find("Counterattack Objects").transform.Find("Counterattack On").gameObject;
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
        if (HP <= 0)
        {
            WindCaptureEnd();
            if(boss ==false)
            {
                Destroy(gameObject);

            }
            else
            {
                animator.SetBool("Dying",true);
                Destroy(gameObject, 4);

            }
            playerScript.GainEXP(EXP); //Putting this after ReduceNumEnemies is making it so that the last 70 doesn't add 
            gameScript.ReduceNumEnemies();
            
        }
        if (transform.position.x <= -5.49f)
        {
            transform.position = new Vector3(-5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.x >= 5.49f)
        {
            transform.position = new Vector3(5.49f, transform.position.y, transform.position.z);
            //playerScript.WindEnd();
            //WindCaptureEnd();
        }
        if (transform.position.y <= -3f)
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
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
                Flinch();
                repeat = false;
                StartCoroutine(WindFlinch());
                StartCoroutine(WindDamage());
            }
            if (armor ==false) {
                //transform.Rotate(Vector3.up * 180 * Time.deltaTime);
            }
        }
        if (armor == true)
        {
            armorObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
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
            damage++;
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
        counterAttackCloud = transform.Find("Counterattack Objects").transform.Find("Counterattack Cloud 2").gameObject;
        counterAttackStart = transform.Find("Counterattack Objects").transform.Find("Counterattack Start").gameObject;
        counterAttackOn = transform.Find("Counterattack Objects").transform.Find("Counterattack On").gameObject;
    }
    public void UnsetRed()
    {
        red = false;
    }
    public void UnsetGreen()
    {
        red = false;
        UnsetCantFlinch();
    }
    public void SetBomb()
    {
        bomb = true;
    }
    public void UnsetBomb()
    {
        bomb = false;
    }
    public void SetNoAttack()
    {
        noAttack = !noAttack;
    }
    //This is less of an enemy type and more of a mode
    public void SetArmor()
    {
        armor = true;
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
    public void ArmorOff()
    {
        armor = false;
        BarrierOff();
        armorObj.SetActive(false);
        if (name == "Witch")
        {
            transform.Find("root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Root").Find("Personal Barrier Object").transform.Find("Personal Barrier").gameObject.SetActive(false);
        }
        GameObject []enemies=GameObject.FindGameObjectsWithTag("Enemy");
        for(int i =0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().BarrierOff();
        }
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
            Debug.Log("No enemies next to each");
        }
    }
    //So far so good. The only problem is that I can't do Idle"", false. I need to keep snapping back to Idle
    //This'llonlybe a problem if I want a recover anima
    public void Flinch()
    {
        //Putting this here instead of using cantFlinch code everywhere
        if (cantFlinch ==false) {
            //Debug.Log("Flinched!");
            attackReady = false;
            if (flinchInterrupt == true)
            {
                attack = false;
                flinchInterrupt = false;

                //I don't know why I don't think to put this allhere
                //I think I'm just thinking in the moment
                //Also, I have to cancel different Coroutines for different mo
                StopCoroutine(flinchOpportunityCancel);
                StopCoroutine(attackLengthCancel);
                //playerScript.InterruptEffect(effectPosition.transform.position);
                StopAttackEffect();
            }

            if (animatorTrue == true)
            {
                animator.SetBool("Idle", true);
                animator.SetTrigger("Flinch");
                //I can see this being a problem for the Red Dragon's last phase
                //I will make an if case where green == true and red==true
                if (red == true)
                {
                    animator.ResetTrigger("StrongAttack");
                }
                else if (green == true) {
                    animator.ResetTrigger("Attack");
                    animator.ResetTrigger("Attack2");
                }
                else if (bomb == true)
                {
                    animator.ResetTrigger("Bomb");
                    UnsetBomb();
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
                StopCoroutine(idleCancel);
            }

            if (flinchCancel != null)
            {
                StopCoroutine(flinchCancel);
            }

            //Don't know why I didn't put this here right away, because I successful flinch will always start a flinchdur
            flinchCancel = StartCoroutine(FlinchDuration());

            if(green == true)
            {
                SetCantFlinch();
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
        idleCancel = StartCoroutine(IdleAnimation(3));
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
    IEnumerator WindFlinch()
    {
        yield return new WaitForSeconds(1);
        repeat = true;
    }
    IEnumerator WindDamage()
    {
        yield return new WaitForSeconds(1.25f);
        TakeDamage(1, true);
    }
    public void StartFlinchWindow()
    {
        flinchOpportunityCancel = StartCoroutine(FlinchWindow());
        //For simplicity, I'm putting this here. I could put this in Green Thief, but I need this for Dragon too
        UnsetCantFlinch();
    }
    //If you hit the foe with the right attack during this window, their attack will be interr
    IEnumerator FlinchWindow()
    {
        flinchInterrupt = true;
        yield return new WaitForSeconds(0.5f);
        flinchInterrupt = false;
        if (green == true)
        {
            SetCantFlinch();
        }
    }
    //I forgot to do AttackReadyOff() after an attack, to not repeat the attack. This lead to other problems 04/15/24
    public void AttackReadyOff()
    {
        attackReady = false;
    }
    public void CounterAttackReadyOff()
    {
        counterAttackTriggered = false;
    }
    public void StartAttackLength()
    {
        attackLengthCancel = StartCoroutine(AttackLength());
    }
    //I'm gonna need to put this in Enem
    //I'm going to need to cancelthisif I stagger foe
    IEnumerator AttackLength()
    {
        yield return new WaitForSeconds(attackLength);
        //animator.ResetTrigger("Attack");
        //StartCoroutine(IdleAnimation());
        StartIdle();
        if (noAttack==false) {
            DealDamage(damage);
        }
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
    public void PlayAttackEffect(int attackEffect)
    {
        effectNumber = attackEffect;
        attackEffects[effectNumber].Play();
    }
    public void StopAttackEffect()
    {
        attackEffects[effectNumber].Stop();
    }
    public void DealDamage(float newDamage)
    {
        if (playerScript.shieldOn == false && playerScript.specialInvincibility == false)
        {
            //playerScript.GeneralDamageCode(newDamage, newDamage);
            //playerScript.PlayHurtEffect(effectAppear.transform.position);
            //playerScript.DamageFlashOn();
            if(red ==false)
            {
                playerScript.GeneralDamageCode(newDamage, 3);
            }
            else
            {
                playerScript.GeneralDamageCode(newDamage, 8);
            }
        }
        else if (playerScript.shieldOn == true || playerScript.specialInvincibility == true)
        {
            playerScript.GenerateShield(effectPosition.transform.position);
            if (playerScript.shieldOn == true)
            {
                if (red ==false) {
                    playerScript.ShieldGaugeDown(newDamage,false);
                }
                else
                {
                    playerScript.ShieldGaugeDown(newDamage, true);
                }
            }
        }
    }
    public void StartIdle()
    {
        idleCancel = StartCoroutine(IdleAnimation(idleTime));
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
            if (red == true)
            {
                animator.ResetTrigger("StrongAttack");
            }
            else if (green == true)
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Attack2");
            }
            else
            {
                animator.ResetTrigger("Attack");
            }
        }
        yield return new WaitForSeconds(idleTime);
        idle = false;
        if (green == false) {
            attack = true;
            attackReady = true;
            if (animatorTrue == true)
            {
                animator.SetBool("Idle", false);
            }
        }
        else
        {
            StartCoroutine(CounterattackCloud());
            
        }
    }
    //Used more for forcing the boss back into its attack pattern
    //Like with Revenge Value
    //I may need to merge this with FlinchCancel();
    //Maybe make a RevengeValue Method
    //It's annoying, because both IdleAnimation and Flinch cause IdleAnimation
    public void IdleAnimationCancel()
    {
        if (idleCancel != null)
        {
            StopCoroutine(idleCancel);
        }
    }
    IEnumerator CounterattackCloud()
    {
        counterAttackCloud.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(CounterAttackStart());

    }
    IEnumerator CounterAttackStart()
    {
        counterAttackStart.SetActive(true);
        yield return new WaitForSeconds(1);
        counterAttackCancel = StartCoroutine(CounterAttack());
        counterAttackStart.SetActive(false);
        counterAttackOn.SetActive(true);
        counterAttackActive = true;
    }
    IEnumerator CounterAttack()
    {
        yield return new WaitForSeconds(3);
        counterAttackActive = false;
        StartCoroutine(FollowUpAttack(4));
        animator.SetBool("Idle", true);
        counterAttackOn.SetActive(false);
        counterAttackCloud.SetActive(false);

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
    }
    public void TakeDamage(float damage, bool armorBreak)
    {
        if (counterAttackActive==false) {
            if ((barrier == true || armor == true))
            {
                //Debug.Log("Reduced Damage");
                if (armorBreak ==false) {
                    damage /= 2;
                    //Debug.Log("Not armorBreak");
                }
            }


            if (armor == true)
            {
                //Debug.Log("Armor damaged ");
                armorGauge -= damage;
                armorFill.fillAmount -= (float)damage / fullArmorGauge;
                if (armorGauge <= 0)
                {
                    ArmorOff();
                    Destroy(GameObject.Find("Barrier(Clone)"));
                }
            }
            else
            {
                HP -= damage;
            }
            //DamageText(damage);
            if (cancelDamageDisplay != null)
            {
                StopCoroutine(cancelDamageDisplay);
            }
            cancelDamageDisplay = StartCoroutine(DamageDisplayDuration(damage));
        }
        else
        {
            StopCoroutine(counterAttackCancel);
            counterAttackTriggered = true;
            counterAttackActive = false;
            //StartCoroutine(FollowUpAttack(4));
            animator.SetBool("Idle", true);
            counterAttackOn.SetActive(false);
            counterAttackCloud.SetActive(false);

        }
    }
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
    }
    public void TeamAttackPositives()
    {
        teamAttackAura.SetActive(true);
        damage++;
    }
    public void TeamAttackOff()
    {
        teamAttackAura.SetActive(false);
        damage--;
    }

    private void OnMouseOver()
    {

        if (normal == false || cantFlinch ==false)
        {
            if (playerScript.flute ==true &&playerScript.wind==false)
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find("Capture Impossible").transform.Find("Cursor").gameObject.SetActive(false);
            }
        }
    }
    private void OnMouseExit()
    {
        if (normal == false || cantFlinch == false)
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
            
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 7.59f));
        }
    }
    private void OnMouseUp()
    {
        if (playerScript.wind == true)
        {
            playerScript.WindEnd();
            WindCaptureEnd();
            Debug.Log("End");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Need this for boundary
        if (collision.gameObject.CompareTag("Enemy") ||collision.gameObject.CompareTag("Bomb")|| collision.gameObject.CompareTag("Debris")) {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        if (playerScript.wind==true)
        {
        //Wind off. Need wind variable for enemy
        if (collision.gameObject.CompareTag("Enemy"))
            {
                bool damaged = false;
                if (damaged == false)
                {
                    damaged = true;
                    TakeDamage(3, true);
                    //Destroy(other.gameObject);
                    Flinch();
                }
                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                //I need to cancel out a bunch of coroutines
                //if (windCaptured==true) {
                    WindCaptureEnd();
                //}
                    playerScript.WindHitEffect(collision.GetContact(0).point);
            if (teamAttack == true && teamAttackOn == false)
            {
            teamAttackOn = true;
            TeamAttackPositives();
            //Debug.Log("Team Attack On");
            }
            //Debug.Log("Crash!");
        }
            if (collision.gameObject.CompareTag("Debris"))
            {
                bool damaged = false;
                if (damaged == false)
                {
                    damaged = true;
                    TakeDamage(2, true);
                    //Destroy(other.gameObject);
                    Flinch();
                }
                //gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                //collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerScript.WindEnd();
                //I need to cancel out a bunch of coroutines
                //if (windCaptured==true) {
                WindCaptureEnd();
                //}
                playerScript.WindHitEffect(collision.GetContact(0).point);
                Destroy(collision.gameObject, 1);
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (teamAttack == true && teamAttackOn == false)
            {
                teamAttackOn = true;
                TeamAttackPositives();
                //Debug.Log("Team Attack On");
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (teamAttack == true)
            {
                TeamAttackOff();
            }
        }
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
                if(armor ==false)
                {
                    Flinch();
                }
                TakeDamage(2, true);
                //Destroy(other.gameObject);
                
                playerScript.HitCountUp();
                //playerScript.TrumpetHitEffect(effectPosition.transform.position);
            }
        }
        if (other.CompareTag("Harp"))
        {
            //Debug.Log("Hit twice?");
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                TakeDamage(1, false);
                //Damage(1);
                //Destroy(other.gameObject);
                if (red==false && armor==false) {
                    Flinch();
                }
                playerScript.HitCountUp();
                //For some reason this causes multiple hits
                //playerScript.HarpHitEffect(effectPosition.transform.position);
            }
        }
        if (other.CompareTag("Hitbox"))
        {
            bool damaged = false;
            if (damaged == false)
            {
                damaged = true;
                TakeDamage(3, true);
                if (armor ==false) {
                    Flinch();
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Barrier") && armor==false)
        {
            barrier = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrier") &&armor==false)
        {
            barrier = false;
        }
    }
}
