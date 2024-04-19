using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Will instantiate the hit effects, such as Interrupt
public class PlayerController : MonoBehaviour
{
    //I made these with the idea of sticking stuff together. Shield stuff with shield stuff, violin stuff with violin stuff, IE
    private GameObject camera;
    private float originalCameraY;
    private Image HPBar;
    private GameObject damageFlash;
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem violinHitEffect;
    public bool attack = false;
    public bool lag = false;
    public int hitCount = 0;
    private Image violinGauge;
    private Image allAttackGauge;

    //public objects?
    public GameObject shield;
    private GameObject toolIcon;
    public GameObject trumpetRange;

    public bool violinDrained = false;
    private float HP = 20;

    public bool shieldOn = false;
    public bool shieldDrained = false;
    public bool trumpetOn = false;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
        damageFlash = GameObject.Find("Damage Object").transform.Find("Damage").gameObject;
        violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
        allAttackGauge = GameObject.Find("All Attack Gauge").GetComponent<Image>();
        toolIcon = GameObject.Find("Tool Icons");
    }

    // Update is called once per frame
    void Update()
    {
        if (violinDrained==true)
        {
            violinGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
            if (violinGauge.fillAmount>=1)
            {
                violinDrained = false;
            }
        }
        if (shieldOn==false)
        {
            if (Input.GetKeyDown(KeyCode.A)) {
                StartCoroutine(ShieldOn());
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            trumpetOn = true;
            trumpetRange.SetActive(true);
        }
    }
    private void FixedUpdate()
    {
        if (trumpetOn==true)
        {
            trumpetRange.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z-8.264f));
        }
    }
    public void ViolinAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
            ViolinHitEffect(newPosition);
            StartCoroutine(Lag());
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
        StartCoroutine(Lag());
        //Debug.Log("Player Attack");
        hitCount = 0;
        allAttackGauge.fillAmount=0;
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Lag()
    {
        lag = true;
        toolIcon.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        lag = false;
        toolIcon.SetActive(false);
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
    IEnumerator ShieldOn()
    {
        shieldOn = true;
        yield return new WaitForSeconds(2);
        shieldOn = false;
    }
    public void GenerateShield(Vector3 position)
    {
        GameObject newShield = shield;
        Instantiate(newShield, GameObject.Find("Canvas General").transform);
        Vector3 pos = camera.GetComponent<Camera>().WorldToScreenPoint(position + new Vector3(0, 1f, 0.5f));
        //DisplayHP();
        //if (transform.position != pos)
        //{
            newShield.transform.position = pos;
        //}
        //Instantiate(shield,position,shield.transform.rotation);
    }
    public void GeneralDamageCode(float damage, float shakeAmount)
    {
        //StartCoroutine(CameraShake(1/10));
        HP -= damage;
        HPBar.fillAmount -= (float)damage / HP;
    }
    IEnumerator CameraShake(float power)
    {
        camera.transform.Translate(0, -power,0);
        yield return new WaitForSeconds(1);
        camera.transform.position = new Vector3(camera.transform.position.x, originalCameraY, camera.transform.position.z);
    }
    public void DamageFlashOn()
    {
        StartCoroutine(DamageFlash());
    }
    IEnumerator DamageFlash()
    {
        damageFlash.SetActive(true);
        yield return new WaitForSeconds(1);
        damageFlash.SetActive(false);
    }
}
