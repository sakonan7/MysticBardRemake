using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Will instantiate the hit effects, such as Interrupt
public class PlayerController : MonoBehaviour
{
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem violinHitEffect;
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
            ViolinHitEffect(enemies[i].transform.position);
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
        Instantiate(interruptEffect, new Vector3(position.x, interruptEffect.transform.position.y, interruptEffect.transform.position.z), interruptEffect.transform.rotation);
    }
    public void PlayHurtEffect()
    {
        hurtEffect.Play();
    }
    public void ViolinHitEffect(Vector3 position)
    {
        Instantiate(violinHitEffect, new Vector3(position.x, violinHitEffect.transform.position.y,violinHitEffect.transform.position.z), violinHitEffect.transform.rotation);
    }
}
