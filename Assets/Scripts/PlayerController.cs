using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

//Will instantiate the hit effects, such as Interrupt
//04/23/24
//I eyeballed a lot of flute
//04/30/24
//A lot of this has to be in Enemy. It makes sense becuase MouseDrag and MouseUp only work on the target directly
//It also make sense because the enemy needs to detect the hitbox. Like in Beast Dominion
public class PlayerController : MonoBehaviour
{
    //I made these with the idea of sticking stuff together. Shield stuff with shield stuff, violin stuff with violin stuff, IE
    public GameObject damageText;
    public AudioClip shieldTune;
    private GameObject camera;
    private CinemachineBasicMultiChannelPerlin camShake;
    private float originalCameraY;
    private Image HPBar;
    private GameObject damageFlash;
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem violinHitEffect;
    public ParticleSystem trumpetHitEffect;
    public bool attack = false;
    public bool lag = false;
    public int hitCount = 0;
    public GameObject allAttackEffect;
    private Image violinGauge;
    private Image trumpetGauge;
    private Image fluteGauge;
    private Image shieldGauge;
    private Image allAttackGauge;
    private GameObject weaponImages;
    private AudioSource audio;
    private GameObject damageFilter;
    private GameObject specialFilter;
    private GameObject shieldFilter;
    private GameObject weaponSelected;
    private TextMeshProUGUI numPotions;
    private int numPotionsInt = 4;
    private GameObject potionUsedIcon;

    //public objects?
    public GameObject shield;
    public GameObject shieldSpecial;
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
    public bool specialInvincibility = false;
    public bool trumpetOn = false;
    private bool trumpetDrained = false;
    public bool fluteDrained = false;

    //Private bools
    private bool violin = true;
    private bool trumpet = false;
    public bool flute = false;
    public bool wind = false;
    private bool potionUsed = false;
    private bool paused = false;
    
    private Coroutine cancelDamageText;
    private Coroutine cancelDamageShake;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        camShake = GameObject.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>().GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
        damageFlash = GameObject.Find("Damage Object").transform.Find("Damage").gameObject;
        violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
        trumpetGauge = GameObject.Find("Trumpet Gauge").GetComponent<Image>();
        fluteGauge = GameObject.Find("Flute Gauge").GetComponent<Image>();
        shieldGauge = GameObject.Find("Shield Gauge").GetComponent<Image>();
        allAttackGauge = GameObject.Find("All Attack Gauge").GetComponent<Image>();
        toolIcon = GameObject.Find("Tool Icons");
        weaponImages = GameObject.Find("Weapon Images");
        audio = GetComponent<AudioSource>();
        specialFilter = GameObject.Find("Filter").transform.Find("Special Filter").gameObject;
        shieldFilter = GameObject.Find("Filter").transform.Find("Shield Filter").gameObject;
        weaponSelected = GameObject.Find("Weapons");
        numPotions = GameObject.Find("Number of Potions").GetComponent<TextMeshProUGUI>();
        numPotions.text = "X " + numPotionsInt;
        potionUsedIcon = GameObject.Find("Potions").transform.Find("Use Potion").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (paused == false)
            {
                paused = true;
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                Time.timeScale = 1;
                Debug.Log("Pause Undone");
            }
        }
        if (paused ==false) {
            if (violinDrained == true)
            {
                violinGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
                if (violinGauge.fillAmount >= 1)
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
            if (shieldDrained == true)
            {
                shieldGauge.fillAmount += (float)1 / 10 * Time.deltaTime;
                if (shieldGauge.fillAmount >= 1)
                {
                    shieldDrained = false;
                }
            }

            //It's really interesting that I did shieldOn first
            if (shieldOn == false)
            {
                if (shieldDrained== false) {
                    if (Input.GetKeyDown(KeyCode.A)) {
                        StartCoroutine(ShieldOn());
                        audio.PlayOneShot(shieldTune, 1);
                    }
                }
            }
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //trumpetOn = true;
            //trumpetRange.SetActive(true);
            //}
            if (Input.GetMouseButtonDown(1) &&wind==false)
            {
                if (violin == true)
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
                WeaponSelect();
            }
            if (Input.GetMouseButtonDown(2)&&wind==false)
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
                WeaponSelect();
            }
            if (lag == false) {
                if (trumpet == true)
                {
                    if (trumpetDrained == false) {
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

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (hitCount >= 25)
                {
                    AllAttack();
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Potion();
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
    public void WeaponSelect()
    {
        if (violin == true)
        {
            weaponSelected.transform.Find("Violin Selected").gameObject.SetActive(true);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(false);
        }
        if (trumpet == true)
        {
            weaponSelected.transform.Find("Violin Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(true);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(false);
        }
        if (flute == true)
        {
            weaponSelected.transform.Find("Violin Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(true);
        }
    }
    public void ViolinAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        Instantiate(violinHitbox, newPosition, violinHitbox.transform.rotation);
        //Instantiate(violinSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), violinSoundwave.transform.rotation);
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
        //Instantiate(trumpetSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), trumpetSoundwave.transform.rotation);
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
        weaponImages.transform.Find("Flute Image").gameObject.SetActive(true);
    }
    public void WindEnd()
    {
        wind = false;
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
        //for (int i =0; i < enemies.Length; i++)
        //{
            //enemies[i].GetComponent<Enemy>().AnalyzeTeamAttackCapability();
        //}
        fluteWind.SetActive(false);
        weaponImages.transform.Find("Flute Image").gameObject.SetActive(false);
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
            //ViolinHitEffect(enemies[i].transform.position);
        }
        StartCoroutine(Lag());
        //Debug.Log("Player Attack");
        hitCount = 0;
        allAttackGauge.fillAmount=0;
        StartCoroutine(SpecialInvincibility());
        StartCoroutine(CameraShakeSpec(0));
        //allAttackEffect.SetActive(true);
        StartCoroutine(AllAttackDisappear1());
    }
    IEnumerator AllAttackDisappear1()
    {
        allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(0.25f*2);
        allAttackEffect.SetActive(false);
        StartCoroutine(AllAttackAgain());
    }
    IEnumerator AllAttackAgain()
    {
        //allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(0.25f *2);
        allAttackEffect.SetActive(true);
        StartCoroutine(AllAttackDisappear2());
    }
    IEnumerator AllAttackDisappear2()
    {
        allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        allAttackEffect.SetActive(false);
    }
    public void Potion()
    {
        if (HP < 20) {
            HP += 4;
            HPBar.fillAmount += (float)4 / HP;
            numPotionsInt--;
            numPotions.text = "X " + numPotionsInt;
            if (potionUsed == false) {
                StartCoroutine(PotionUse());
            }
        }
        if (numPotionsInt <= 0)
        {
            numPotions.text = "";
        }
    }
    IEnumerator PotionUse()
    {
        potionUsed = true;
        potionUsedIcon.SetActive(true);
        //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x*8/7, HPBar.transform.localScale.y * 8 / 7, HPBar.transform.localScale.z);
        yield return new WaitForSeconds(1.5f);
        potionUsed = false;
        potionUsedIcon.SetActive(false);
        //HPBar.transform.localScale = new Vector3(HPBar.transform.localScale.x * 7 / 8, HPBar.transform.localScale.y * 7 / 8, HPBar.transform.localScale.z);
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.5f);
    }
    IEnumerator Lag()
    {
        lag = true;
        //toolIcon.SetActive(true);
        if (violin ==true) {
            weaponImages.transform.Find("Violin Image").gameObject.SetActive(true);
        }
        else if (trumpet == true)
        {
            weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(true);
            trumpetRange.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        lag = false;
        //toolIcon.SetActive(false);
        if (violin == true)
        {
            weaponImages.transform.Find("Violin Image").gameObject.SetActive(false);
        }
        else if (trumpet == true)
        {
            weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(false);
            trumpetRange.SetActive(true);
        }
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
        Instantiate(violinHitEffect, position, violinHitEffect.transform.rotation);
    }
    public void TrumpetHitEffect(Vector3 position)
    {
        Instantiate(trumpetHitEffect, position, trumpetHitEffect.transform.rotation);
    }
    IEnumerator ShieldOn()
    {
        shieldOn = true;
        weaponImages.transform.Find("Shield Image").gameObject.SetActive(true);
        shieldFilter.SetActive(true);
        yield return new WaitForSeconds(2);
        shieldOn = false;
        weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
        shieldFilter.SetActive(false);
    }
    IEnumerator SpecialInvincibility()
    {
        specialInvincibility = true;
        //weaponImages.transform.Find("Shield Image").gameObject.SetActive(true);
        specialFilter.SetActive(true);
        yield return new WaitForSeconds(2);
        specialInvincibility = false;
        //weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
        specialFilter.SetActive(false);
    }
    public void GenerateShield(Vector3 position)
    {
        GameObject newShield =shield;
        if (shieldOn == true) {
            newShield = shield;
        }
        else if(specialInvincibility == true)
        {
            newShield = shieldSpecial;
        }
        Instantiate(newShield, new Vector3(position.x, position.y, position.z+2), newShield.transform.rotation);
        //Vector3 pos = camera.GetComponent<Camera>().WorldToScreenPoint(position + new Vector3(0, 1f, 0.5f));
        //DisplayHP();
        //if (transform.position != pos)
        //{
            //newShield.transform.position = pos;
        //}
        //Instantiate(shield,position,shield.transform.rotation);
    }
    public void ShieldGaugeDown(float damage)
    {
        shieldGauge.fillAmount -= (float)damage / 10;
        if (shieldGauge.fillAmount <= 0)
        {
            shieldDrained = true;
        }
    }
    public void GeneralDamageCode(float damage, float shakeAmount)
    {
        //StartCoroutine(CameraShake(1/10));
        HP -= damage;
        HPBar.fillAmount -= (float)damage / HP;
        PlayHurtEffect();

        //I'm thinking about not cancelling the Coroutine and just changing the value of damage
        //Camera shake for giant should be double
        cancelDamageShake = StartCoroutine(CameraShake(0));
        cancelDamageText =StartCoroutine(DamageText(damage));
    }
    IEnumerator DamageText(float damage)
    {
        damageText.GetComponent<TextMesh>().text = "" + damage;
        yield return new WaitForSeconds(1);
        damageText.GetComponent<TextMesh>().text = "";
    }
    IEnumerator OldCameraShake(float power)
    {
        camera.transform.Translate(0, -power,0);
        yield return new WaitForSeconds(1);
        camera.transform.position = new Vector3(camera.transform.position.x, originalCameraY, camera.transform.position.z);
    }
    IEnumerator CameraShake(float shakeAmount)
    {
        camShake.m_AmplitudeGain = 3f;
        camShake.m_FrequencyGain = 0.5f;
        yield return new WaitForSeconds(1);
        camShake.m_AmplitudeGain = 0;
        camShake.m_FrequencyGain = 0.5f;
    }
    IEnumerator CameraShakeSpec(float shakeAmount)
    {
        camShake.m_AmplitudeGain = 3f;
        camShake.m_FrequencyGain = 0.5f;
        yield return new WaitForSeconds(0.5f);
        camShake.m_AmplitudeGain = 0;
        camShake.m_FrequencyGain = 0.5f;
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
    public void OnMouseUp()
    {
        if(wind==true)
        {
            WindEnd();
        }
    }

}
