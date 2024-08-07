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

        enemyScript.SetHP(75);
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
                if (attackNum ==1) {
                    UnblockableAttack();
                }
                //Debug.Log("Attack");
            }

            if (enemyScript.attack == true && enemyScript.flinching == true)
            {
                Debug.Log("Attack Change");
                if (attackNum == 0)
                {
                    attackNum = 1;
                }
                if (attackNum == 1)
                {
                    attackNum = 0;
                }
            }
        }
        if(attackNum ==1 && unblockableAttackOn ==false)
        {
            StartCoroutine(UnblockableAttackOn());
        }
    }
    IEnumerator UnblockableAttackOn()
    {
        unblockableAttackOn = true;
        yield return new WaitForSeconds(3);
        swordMaterial.material = unblockableAttack;
        swordGlow.Play();
        enemyScript.SetUnblockable();
        audio.PlayOneShot(unblockableAttackOnSound,1);
    }
    IEnumerator UnblockableAttackOff()
    {
        yield return new WaitForSeconds(2f);
        swordMaterial.material = regular;
        unblockableAttackOn = false;
        enemyScript.UnsetUnblockable();
    }
    public void ComboAttack1()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("ComboAttack1");
        enemyScript.SetDamage(1);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLengthAlternate();
        enemyScript.StartFlinchWindow();
        enemyScript.PlayAttackEffect(1);
        enemyScript.AttackReadyOff();
        StartCoroutine(ComboAttack2());
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
            enemyScript.StartFlinchWindow();
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
            enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(3);
            StartCoroutine(ResetTrigger());
            attackNum = 1;
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
    public void UnblockableAttack()
    {
        //animator.SetBool("Idle",false);
        animator.SetTrigger("UnblockableAttack");
        enemyScript.SetDamage(3);
        enemyScript.SetAttackLength(1.5f);
        enemyScript.StartAttackLength();
        enemyScript.StartFlinchWindow();
            enemyScript.PlayAttackEffect(0);
        enemyScript.AttackReadyOff();
        StartCoroutine(UnblockableAttackOff());
        attackNum = 0;
    }

}
