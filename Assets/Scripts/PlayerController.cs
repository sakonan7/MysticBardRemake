using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will instantiate the hit effects, such as Interrupt
public class PlayerController : MonoBehaviour
{
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public bool attack = false;
    public bool attackLagging = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackLagging==false) {
            if (Input.GetMouseButtonDown(0))
            {
                AllAttack();
            }
        }
    }
    public void AllAttack()
    {
        //Produce a hurt and a interrupt effect on every enemy
        GameObject [] enemies =GameObject.FindGameObjectsWithTag("Enemy");
        for (int i= 0; i < enemies.Length; i++)
        {
            //enemies[i].GetComponent<Enemy>().TakeDamage();
            Instantiate(hurt, enemies[i].transform.position, hurt.transform.rotation);
        }
        StartCoroutine(AttackLag());
        Debug.Log("Player Attack");
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator AttackLag()
    {
        attackLagging = true;
        yield return new WaitForSeconds(1);
        attackLagging = false;
    }
    public void InterruptEffect(Vector3 position)
    {
        Instantiate(interruptEffect, position, interruptEffect.transform.rotation);
    }
}
