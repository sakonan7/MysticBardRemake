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
        enemyScript.SetHP(60 + 10);
        enemyScript.SetEXP(90);

        enemyScript.SetNormal();
        
        enemyScript.SetBombUser();
        //Barrier();

        //bombFlare = transform.Find("Bomb Light").transform.Find("Lens").gameObject;
        bombFlare = transform.Find("Lens Flare").gameObject.GetComponent<ParticleSystem>();
        barrierAnimation= transform.Find("root").transform.Find("Personal Barrier Object").transform.Find("Barrier Animation").gameObject;

        //enemyScript.SetIdleTime(5);
    }
    void Start()
    {
            StartCoroutine(BarrierAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyScript.bombReady==true)
        {
            StartBombSummon();
        }
        if (enemyScript.summonBombs == true)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                StartCoroutine(BombCross());
            }
            else if(random == 1)
            {
                StartCoroutine(BombCube());
            }
            //Debug.Log("Attack");
            //Only doing this because I need to
            //I intend for the foe to not be able to be staggered while using a barrier
            //Attack();
            SummonBombs();
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
        //enemyScript.SetCantFlinch();
        yield return new WaitForSeconds(1);
        animator.ResetTrigger("Barrier");
        
        barrierAnimation.SetActive(false);
        
        enemyScript.SetIdleTime(12);
        //enemyScript.SetIdleStart();
        enemyScript.NonStandardIdleStart();
        Barrier();
        //08/27/24, Oh my god, a glitch that wasn't triggered until now. Unity does do this. UnsetCantFlinch never happ
        //enemyScript.UnsetCantFlinch();
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y, barrier.transform.position.z), barrier.transform.rotation);
        enemyScript.PlayBarrierSound();
    }
    IEnumerator BombCross()
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 bombPosition = new Vector3(0, 0.7f,-7.59f);
        Instantiate(bomb, new Vector3(bombPosition.x + 2+1, bombPosition.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 2-1, bombPosition.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x, bombPosition.y + 2+1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x, bombPosition.y - 2 - 1, -7.59f), bomb.transform.rotation);
        
    }
    IEnumerator BombCube()
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 bombPosition = new Vector3(0, 0.7f, -7.59f);
        Instantiate(bomb, new Vector3(bombPosition.x + 1.5f +1, bombPosition.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 1.5f - 1, bombPosition.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x + 1.5f + 1, bombPosition.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(bombPosition.x - 1.5f - 1, bombPosition.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
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
    }
    public void SummonBombs()
    {
        enemyScript.SummonBombsOff();

        enemyScript.PlayerCantPause(6);
        enemyScript.PlayerTransparentUI(6);
    }
    public void BombFlare()
    {
        //bombFlare.SetActive(true);
        //yield return new WaitForSeconds(0.5f);
        bombFlare.Play();
    }
}
