using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Will instantiate the hit effects, such as Interrupt
//04/23/24
//I eyeballed a lot of flute
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
    private Image trumpetGauge;
    private Image fluteGauge;
    private Image allAttackGauge;

    //public objects?
    public GameObject shield;
    private GameObject toolIcon;
    public GameObject trumpetRange;
    public GameObject trumpetHitbox;
    public GameObject violinHitbox;
    public GameObject trumpetSoundwave;
    public GameObject violinSoundwave;
    public GameObject fluteWind;

    public bool violinDrained = false;
    private float HP = 20;

    public bool shieldOn = false;
    public bool shieldDrained = false;
    public bool trumpetOn = false;
    private bool trumpetDrained = false;
    private bool fluteDrained = false;

    //Private bools
    private bool violin = true;
    private bool trumpet = false;
    public bool flute = false;
    public bool wind = false;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
        damageFlash = GameObject.Find("Damage Object").transform.Find("Damage").gameObject;
        violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
        trumpetGauge = GameObject.Find("Trumpet Gauge").GetComponent<Image>();
        fluteGauge = GameObject.Find("Flute Gauge").GetComponent<Image>();
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
        if (trumpetDrained == true)
        {
            trumpetGauge.fillAmount += (float)1 / 15 * Time.deltaTime;
            if (trumpetGauge.fillAmount >= 1)
            {
                trumpetDrained = false;
            }
        }
        if (fluteDrained == true)
        {
            fluteGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
            if (fluteGauge.fillAmount >= 1)
            {
                fluteDrained = false;
            }
        }
        if (shieldOn==false)
        {
            if (Input.GetKeyDown(KeyCode.A)) {
                StartCoroutine(ShieldOn());
            }
        }
        //if (Input.GetKeyDown(KeyCode.S))
        //{
            //trumpetOn = true;
            //trumpetRange.SetActive(true);
        //}
        if (Input.GetMouseButtonDown(1))
        {
            if (violin==true)
            {
                violin = false;
                trumpet = true;
                trumpetRange.SetActive(true);
            }
            else
            {
                violin = true;
                trumpet = false;
                trumpetRange.SetActive(false);
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            if (violin == true)
            {
                violin = false;
                flute = true;
                //trumpetRange.SetActive(true);
            }
            else
            {
                violin = true;
                flute = false;
                //trumpetRange.SetActive(false);
            }
            //wind = true;
        }
        if (lag==false) {
            if (trumpet == true)
            {
                if (trumpetDrained ==false) {
                    if (Input.GetMouseButtonDown(0))
                    {
                        TrumpetAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                    }
                }
            }
            if (violin == true)
            {
                if (violinDrained == false)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ViolinAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                    }
                }
            }
        }
        if (wind==false)
        {
            if (flute == true)
            {
                if (fluteDrained == false)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                    //TrumpetAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
                    FluteAttack();
                    WindOn();
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (trumpet==true)
        {
            trumpetRange.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z-8.264f));
        }
        if (wind ==true)
        {
            fluteWind.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f));
        }
    }
    public void ViolinAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        Instantiate(violinHitbox, newPosition, violinHitbox.transform.rotation);
        Instantiate(violinSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), violinSoundwave.transform.rotation);
        //ViolinHitEffect(newPosition);
        StartCoroutine(Lag());
            violinGauge.fillAmount -= (float)1 / 15;
            if (violinGauge.fillAmount <= 0)
            {
                violinDrained = true;
            }
        //}
}
    public void TrumpetAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        //ViolinHitEffect(newPosition);
        Instantiate(trumpetHitbox, newPosition, trumpetHitbox.transform.rotation);
        Instantiate(trumpetSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), trumpetSoundwave.transform.rotation);
        StartCoroutine(Lag());
        trumpetGauge.fillAmount -= (float)1 / 10;
        if (trumpetGauge.fillAmount <= 0)
        {
            trumpetDrained = true;
        }
        //}
    }
    public void FluteAttack()
    {
        fluteGauge.fillAmount -= (float)1 / 3;
        if (fluteGauge.fillAmount <= 0)
        {
            fluteDrained = true;
        }
    }
    public void WindOn()
    {
        wind = true;
        fluteWind.SetActive(true);
    }
    public void WindEnd()
    {
        wind = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        for (int i =0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().AnalyzeTeamAttackCapability();
        }
        fluteWind.SetActive(false);
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
        //toolIcon.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        lag = false;
        //toolIcon.SetActive(false);
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
