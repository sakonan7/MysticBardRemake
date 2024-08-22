using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Need to cancel IdleAnimation, then use a counterattack that then follows up with the regular attack
//Guard code makes no sense. I'm gonna need like to hit like atm
//It makes moresense to use unlike and unlike, rather than like and like
//But I'll do this for now
//08/20/24 I need to have CantFlinch be on when a guard is finished being evoked
public class Guard
    : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private PlayerController player;
    private SkinnedMeshRenderer skin;
    private AudioSource audio;
    public Material originalSkin;
    public Material flashSkin;
    public GameObject flashing;
    public Material harpGuard;
    
    public Material trumpetGuard;
    public AudioClip explosive;
    public AudioClip flame;

    private bool repeat = false;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        skin = transform.Find("DragonSoulEater").GetComponent<SkinnedMeshRenderer>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(100);
        enemyScript.SetEXP(100);

        enemyScript.SetIdleTime(5);
        enemyScript.SetGuard();
        enemyScript.SetNormal();
        enemyScript.SetRevengeValue();
        enemyScript.SetRevengeValueNumber(10);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Start by Evoking a Guard
        StartCoroutine(EvokeGuard());
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
        if (enemyScript.unflinchingFollow == true && repeat == false)
        {
            StartCoroutine(Flashing());
        }
        if(enemyScript.harpGuard ==true)
        {
            skin.material = harpGuard;
        }
        if (enemyScript.trumpetGuard == true)
        {
            skin.material = trumpetGuard;
        }
    }
    private void LateUpdate()
    {
        if (enemyScript.counterAttackTriggered == true)
        {
            StartCoroutine(GuardCounterAttack());
            //Debug.Log("Attack");
        }
        if (enemyScript.revengeValueMove == true)
        {
            StartCoroutine(EvokeGuard());
            enemyScript.RestartIdleMethod(2.5f);
        }
    }
    IEnumerator Flashing()
    {
        skin.material = flashSkin;
        flashing.SetActive(true);
        repeat = true;
        yield return new WaitForSeconds(0.5f);
        skin.material = originalSkin;


        //numFlash++;
        flashing.SetActive(false);
        //Debug.Log(numFlash);
        repeat = false;
        //}
    }
    IEnumerator EvokeGuard()
    {
        enemyScript.SetGuard();
        enemyScript.RevengeValueMoveOff();
        StartCoroutine(Flashing());
        if (enemyScript.windCaptured == false) {
            enemyScript.SetCantFlinch();
        }
        else
        {
            enemyScript.UnsetCantFlinch();
        }
        yield return new WaitForSeconds(1);
        int random = Random.Range(0, 2);
        if(random==0)
        {
            enemyScript.SetHarpGuard();
        }
        else
        {
            enemyScript.SetTrumpetGuard();
        }
        //enemyScript.UnsetCantFlinch();
    }
    public void Attack()
    {
        if(enemyScript.unflinchingFollow ==false)
        {
            //Drop guard and attack
            enemyScript.UnsetHarpGuard();
            enemyScript.UnsetTrumpetGuard();
            skin.material = originalSkin;
        }

        //animator.SetBool("Idle",false);
        animator.SetTrigger("Attack");
        enemyScript.SetDamage(2);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
        enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
    }
    IEnumerator GuardCounterAttack()
    {
        enemyScript.IdleAnimationCancel();
        enemyScript.AttackReadyOff();
        enemyScript.CounterAttackReadyOff();
        yield return new WaitForSeconds(0.25f);


        //animator.SetBool("Idle",false);
        animator.SetTrigger("Counterattack");
        enemyScript.SetDamage(4);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartCounterAttackLength();
        enemyScript.PlayAttackEffect(1);
        
        StartCoroutine(CounterAttackSoundDelay());
    }
    IEnumerator CounterAttackSoundDelay()
    {
        yield return new WaitForSeconds(0.35f);
        audio.PlayOneShot(explosive, 1);
        audio.PlayOneShot(flame, 1);
    }

}
