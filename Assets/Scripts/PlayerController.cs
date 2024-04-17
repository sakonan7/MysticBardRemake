using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Will instantiate the hit effects, such as Interrupt
public class PlayerController : MonoBehaviour
{
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem violinHitEffect;
    public bool attack = false;
    public bool attackLagging = false;
    public int hitCount = 0;
    private Image violinGauge;
    private Image allAttackGauge;

    public bool violinDrained = false;
    // Start is called before the first frame update
    void Start()
    {
        violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
        allAttackGauge = GameObject.Find("All Attack Gauge").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackLagging==false) {
            //if (Input.GetMouseButtonDown(0))
            //{
                //AllAttack();
            //}
        }
        if (violinDrained==true)
        {
            violinGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
            if (violinGauge.fillAmount>=1)
            {
                violinDrained = false;
            }
        }
    }
    public void ViolinAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
            ViolinHitEffect(newPosition);
            StartCoroutine(AttackLag());
            violinGauge.fillAmount -= (float)1 / 15;
            if (violinGauge.fillAmount <= 0)
            {
                violinDrained = true;
            }
        //}
}
    public void HitCountUp()
    {
        hitCount++;
        allAttackGauge.fillAmount += (float)1 / 30;
    }
    public void AllAttack()
    {
        //Produce a hurt and a interrupt effect on every enemy
        GameObject [] enemies =GameObject.FindGameObjectsWithTag("Enemy");
        for (int i= 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().TakeDamage(20);
            //Instantiate(hurt, enemies[i].transform.position, hurt.transform.rotation);
            ViolinHitEffect(enemies[i].transform.position);
        }
        StartCoroutine(AttackLag());
        //Debug.Log("Player Attack");
        hitCount = 0;
        allAttackGauge.fillAmount=0;
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator AttackLag()
    {
        attackLagging = true;
        yield return new WaitForSeconds(0.5f);
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
