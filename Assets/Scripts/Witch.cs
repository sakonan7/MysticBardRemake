using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//After breaking barrier, you can interrupt his attacks
//After a time limit, he will recreate a barrier
//05/16/24 After this, put this in Enemy because multiplenemies use this
public class Witch : MonoBehaviour
{
    public GameObject barrier;
    public GameObject bomb;
    private GameObject barrierAnimation;
    private ParticleSystem bombFlare;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetHP(60);
        enemyScript.SetEXP(90);

        enemyScript.SetNormal();
        enemyScript.SetNoAttack();
        enemyScript.SetBombUser();
        //Barrier();

        //bombFlare = transform.Find("Bomb Light").transform.Find("Lens").gameObject;
        bombFlare = transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
        barrierAnimation= transform.Find("root").transform.Find("Personal Barrier Object").transform.Find("Barrier Animation").gameObject;

        
    }
    void Start()
    {
            StartCoroutine(BarrierAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.attackReady == true)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                BombCross();
            }
            else if(random == 1)
            {
                BombCube();
            }
            //Debug.Log("Attack");
            //Only doing this because I need to
            //I intend for the foe to not be able to be staggered while using a barrier
            Attack();
        }
        //if (enemyScript.armor=)
        //{

        //}
    }
    IEnumerator BarrierAnimation()
    {
        barrierAnimation.SetActive(true);
        animator.SetTrigger("Barrier");
        enemyScript.SetArmor();
        enemyScript.SetCantFlinch();
        yield return new WaitForSeconds(1);
        animator.ResetTrigger("Barrier");
        
        barrierAnimation.SetActive(false);
        
        enemyScript.SetIdleTime(12);
        //enemyScript.SetIdleStart();
        enemyScript.NonStandardIdleStart();
        Barrier();
        enemyScript.UnsetCantFlinch();
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y, barrier.transform.position.z), barrier.transform.rotation);
        enemyScript.PlayBarrierSound();
    }
    public void BombCross()
    {
        Vector3 bombPosition = new Vector3(0, 0.7f,-7.59f);
        Instantiate(bomb, new Vector3(bombPosition.x + 2+1, bombPosition.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 2-1, bombPosition.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x, bombPosition.y + 2+1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x, bombPosition.y - 2 - 1, -7.59f), bomb.transform.rotation);
        enemyScript.AttackReadyOff();
    }
    public void BombCube()
    {
        Vector3 bombPosition = new Vector3(0, 0.7f, -7.59f);
        Instantiate(bomb, new Vector3(bombPosition.x + 1.5f +1, bombPosition.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 1.5f - 1, bombPosition.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x + 1.5f + 1, bombPosition.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 1.5f - 1, bombPosition.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
        enemyScript.AttackReadyOff();
    }
    public void Attack()
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
    }
    IEnumerator BombFlare()
    {
        //bombFlare.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        bombFlare.Play();
    }
}
