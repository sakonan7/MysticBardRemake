using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//After breaking barrier, you can interrupt his attacks
//After a time limit, he will recreate a barrier
public class Witch : MonoBehaviour
{
    public GameObject barrier;
    public GameObject bomb;
    private GameObject bombFlare;
    private Animator animator;
    private Enemy enemyScript;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //StartCoroutine(IdleAnimation());
        enemyScript = GetComponent<Enemy>();
        enemyScript.SetHP(60);
        enemyScript.SetIdleStart(); //This doesn't work. May need an awake
        enemyScript.SetIdleTime(5);
        enemyScript.SetNormal();
        Barrier();
        enemyScript.SetArmor();
        bombFlare = transform.Find("Bomb Light").transform.Find("Lens").gameObject;
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
            else
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
        yield return new WaitForSeconds(1);
    }
    public void Barrier()
    {
        Instantiate(barrier, new Vector3(transform.position.x, transform.position.y, barrier.transform.position.z), barrier.transform.rotation);
    }
    public void BombCross()
    {
        Instantiate(bomb, new Vector3(transform.position.x + 2+1, transform.position.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x - 2-1, transform.position.y, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x, transform.position.y + 2+1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x, transform.position.y - 2 - 1, -7.59f), bomb.transform.rotation);
        enemyScript.AttackReadyOff();
    }
    public void BombCube()
    {
        Instantiate(bomb, new Vector3(transform.position.x + 1.5f +1, transform.position.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x - 1.5f - 1, transform.position.y + 1.5f + 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x + 1.5f + 1, transform.position.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
        Instantiate(bomb, new Vector3(transform.position.x - 1.5f - 1, transform.position.y - 1.5f - 1, -7.59f), bomb.transform.rotation);
        enemyScript.AttackReadyOff();
    }
    public void Attack()
    {
        //animator.SetBool("Idle",false);
        //animator.SetTrigger("Attack");
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
    }
}
