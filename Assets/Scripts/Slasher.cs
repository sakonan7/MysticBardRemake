using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//There is a flaw in the programming
//AttackNum doesn't change if foe is staggered. 
//The coding relies on the foe doing the long combo
//For an immediate fix, I need to either have attackNum change right away and make the IEnumerator play longer, or
//I forgot the second. I think it's if the foe is staggered, the attackNum still changes
//This is going to be tough, because I didn't make this game with multiple moves in mind
//Will now to need to implement attack
//I will still have unblockable effect play after a few seconds. This matches with the unblockable effect playing when a second away from idle animation stop
//08/29/24
//2 seconds and then sword flash. Around 2-1.5 seconds, and then slash
//May want a RestartCycle()
public class Slasher : MonoBehaviour
{
    private bool idle = true;
    private Animator animator;
    private Enemy enemyScript;
    private AudioSource audio;
    private bool turnOnUnblockable = false;
    private int numAttacks = 2;
    private int attackNum = 0;
    private bool unblockableAttackOn = false;
    public SkinnedMeshRenderer swordMaterial;
    public Material unblockableAttack;
    public Material regular;
    public ParticleSystem swordGlow;
    public AudioClip unblockableAttackOnSound;
    private void Awake()
    {
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        //StartCoroutine(IdleAnimation());

        enemyScript.SetHP(75 + 25);
        enemyScript.SetEXP(90);

        enemyScript.SetIdleTime(5);
        enemyScript.SetNormal();
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
                if (attackNum == 0)
                {
                    ComboAttack1();
                }
                //For some reason not using else causes a glitch, even thought attackReady isoff
                else if (attackNum ==1) {
                    UnblockableAttackOn();
                }
                //Debug.Log("Attack");
            }
        }
        //if(attackNum ==1 && unblockableAttackOn ==false)
        //{
            //StartCoroutine(UnblockableAttackOn());
        //}
        if(enemyScript.flinching==true)
        {
            //For consistency, I may want to dounblockableAttackOn/unblockableAttack in Flinch(
            if (unblockableAttackOn==true) {
                StartCoroutine(UnblockableAttackOff());
            }
        }
    }

    public void ComboAttack1()
    {
        //animator.SetBool("Idle",false);
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("ComboAttack1");
        enemyScript.SetDamage(1);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLengthAlternate();
        enemyScript.SetFlinchWindow(1.5f);
        enemyScript.StartFlinchWindow();
        enemyScript.PlayAttackEffect(1);
        enemyScript.AttackReadyOff();
        StartCoroutine(ComboAttack2());

        attackNum = 1;
        enemyScript.SetIdleTime(2); //This is okay, because if you don't flinch Slasher, he will quickly move onto his next att
    }
    IEnumerator ComboAttack2()
    {
        yield return new WaitForSeconds(1);
        
        if(enemyScript.flinching ==false)
        {
            animator.SetTrigger("ComboAttack2");
            enemyScript.SetDamage(1);
            enemyScript.SetAttackLength(1.5f);
            enemyScript.StartAttackLengthAlternate();
            enemyScript.PlayAttackEffect(2);
            StartCoroutine(ComboAttack3());
        }
        else
        {
            animator.ResetTrigger("ComboAttack1");
        }
    }
    IEnumerator ComboAttack3()
    {
        yield return new WaitForSeconds(1);
        //animator.ResetTrigger("ComboAttack2");
        if (enemyScript.flinching == false)
        {
            animator.SetTrigger("ComboAttack3");
            enemyScript.SetDamage(1);
            enemyScript.SetAttackLength(1.5f);
            enemyScript.StartAttackLengthAlternate();
            enemyScript.PlayAttackEffect(1);
            StartCoroutine(ComboAttack4());
        }
    }
    IEnumerator ComboAttack4()
    {
        yield return new WaitForSeconds(1);
        //animator.ResetTrigger("ComboAttack3");
        if (enemyScript.flinching == false)
        {
            animator.SetTrigger("ComboAttack4");
            enemyScript.SetDamage(1);
            enemyScript.SetAttackLength(1.5f);
            enemyScript.StartAttackLength();
            enemyScript.PlayAttackEffect(3);
            StartCoroutine(ResetTrigger());
            //attackNum = 1;
        }
    }
    IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(2);
        animator.ResetTrigger("ComboAttack1");
        animator.ResetTrigger("ComboAttack2");
        animator.ResetTrigger("ComboAttack3");
        animator.ResetTrigger("ComboAttack4");
    }
    //Basically switchedfunctionalit
    //UnblockableAttackOff is more of a command now
    //2 Issues. I usually play AttackLength and animation at the same time. Also, I never pay attention to how the character goes back to IdleAnimation
    //After
    //I need the damage to be dealt around the time and the animation is fin
    //Because I play the animation at the same time, that means I need at least a 1.5f second difference between attackLength and the anima
    public void UnblockableAttackOn()
    {
        

        StartCoroutine(UnblockableAttack());
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(4);
        enemyScript.StartAttackLength();
        enemyScript.SetFlinchWindow(4);
        enemyScript.StartFlinchWindow();

        enemyScript.AttackReadyOff();
        attackNum = 0;
        enemyScript.SetIdleTime(5);
        swordMaterial.material = unblockableAttack;
        swordGlow.Play();
        enemyScript.SetUnblockable(); //The issue with this is that I'm gonna need this to preempt the end of IdleAnimation
        audio.PlayOneShot(unblockableAttackOnSound, 1);
        unblockableAttackOn = true;
        
    }
    //For the animation
    //And sword flash off, if flinch doesn't happ
    IEnumerator UnblockableAttack()
    {
        //unblockableAttackOn = true;
        yield return new WaitForSeconds(2.5f);
        enemyScript.IdleBoolAnimatorCancel();
        animator.SetTrigger("UnblockableAttack");
        enemyScript.PlayAttackEffect(0);
        StartCoroutine(UnblockableAttackOff());
        Debug.Log("Play?");
    }
    IEnumerator UnblockableAttackOff()
    {
        unblockableAttackOn = false;
        yield return new WaitForSeconds(1f);
        swordMaterial.material = regular;
        
        enemyScript.UnsetUnblockable();
    }
}
