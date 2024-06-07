using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;

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
    public AudioClip shieldBreak;
    private GameObject camera;
    private CinemachineBasicMultiChannelPerlin camShake;
    private GameManager gameScript;
    private float originalCameraY;
    private Image HPBar;
    private TextMeshProUGUI levelText;
    private GameObject damageFlash;
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem violinHitEffect;
    public ParticleSystem trumpetHitEffect;
    public ParticleSystem windCrash;
    public bool attack = false;
    public bool lag = false;
    public int hitCount = 0;
    private bool hitCountReached = false;
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
    private GameObject shieldShatter;
    private GameObject weaponSelected;
    private TextMeshProUGUI numPotions;
    private int numPotionsInt = 4;
    private GameObject potionUsedIcon;

    //Private Numerics
    //It's very hard to be organized. Either you put all the HP stuff together and the private and statics are mixed up,
    //or you put all the privates together, but the HP stuff is all disorganized
    private TextMeshProUGUI HPText;
    private TextMeshProUGUI violinText;
    private TextMeshProUGUI trumpetText;
    private TextMeshProUGUI fluteText;
    private TextMeshProUGUI shieldText;

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

    public bool violinDrained = false; //I don't think I have to make this public anymore
    private float originalHP = 20;
    private float originalViolin = 15;
    private float originalTrumpet = 10;
    private float originalFlute = 3;
    public static float currentHP = 20;
    public static float HPTotal = 20;
    private static float currentViolin = 15;
    public static float violinTotal = 15;
    private static float currentTrumpet = 10;
    public static float trumpetTotal = 10;
    private static float currentFlute = 3;
    public static float fluteTotal = 3;
    private static float currentShield = 10;
    public static float shieldTotal = 10;
    public static int level = 6;
    //EXPCounter will be the number that goes down
    //currentEXP is for holding the amount
    //subtract if you levelled up
    private static float currentEXP = 0;
    private static float EXPCounter = 0;
    private static float EXPToLevel = 0;
    private static float EXPToLevelMax = 200;
    private static float EXPGained = 0;

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


    //Statics for the most part
    public static int EXP = 0;
    private static bool noEXP = false;
    
    private Coroutine cancelDamageText;
    private Coroutine cancelDamageShake;
    private Coroutine cancelDamageFlash;
    private Coroutine cancelShield;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        camShake = GameObject.Find("FreeLook Camera").GetComponent<CinemachineFreeLook>().GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //Load different things based on levelSelectNonStatic
        if (gameScript.titleNonStatic ==false) {
            if (gameScript.levelSelectNonStatic == true)
            {
                HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
                levelText = GameObject.Find("Mugshot").transform.Find("Level Text").GetComponent<TextMeshProUGUI>();
                violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
                trumpetGauge = GameObject.Find("Trumpet Gauge").GetComponent<Image>();
                fluteGauge = GameObject.Find("Flute Gauge").GetComponent<Image>();
                shieldGauge = GameObject.Find("Shield Gauge").GetComponent<Image>();
                toolIcon = GameObject.Find("Tool Icons");
                weaponImages = GameObject.Find("Weapon Images");
            }
            else
            {
                HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
                levelText = GameObject.Find("Mugshot").transform.Find("Level Text").GetComponent<TextMeshProUGUI>();
                damageFlash = GameObject.Find("Damage Object").transform.Find("Damage").gameObject;
                violinGauge = GameObject.Find("Violin Gauge").GetComponent<Image>();
                trumpetGauge = GameObject.Find("Trumpet Gauge").GetComponent<Image>();
                fluteGauge = GameObject.Find("Flute Gauge").GetComponent<Image>();
                shieldGauge = GameObject.Find("Shield Gauge").GetComponent<Image>();
                allAttackGauge = GameObject.Find("All Attack Gauge").GetComponent<Image>();
                toolIcon = GameObject.Find("Tool Icons");
                weaponImages = GameObject.Find("Weapon Images");

                specialFilter = GameObject.Find("Filter").transform.Find("Special Filter").gameObject;
                shieldFilter = GameObject.Find("Filter").transform.Find("Shield Filter").gameObject;
                shieldShatter = GameObject.Find("Filter").transform.Find("Shield Shatter").gameObject;
                weaponSelected = GameObject.Find("Weapons");
                numPotions = GameObject.Find("Number of Potions").GetComponent<TextMeshProUGUI>();
                numPotions.text = "X " + numPotionsInt;
                potionUsedIcon = GameObject.Find("Potions").transform.Find("Use Potion").gameObject;
            }
        }
        audio = GetComponent<AudioSource>();

        //I think I should use numeric values for the bar length and HP num
        if (gameScript.levelUp==true)
        {
            LevelUp();
            gameScript.LevelUpOff();
        }
        levelText.text = "Lv. " + level;

        //This is necessary in case I didn't increase any of these values
        currentHP = HPTotal;
        currentViolin = violinTotal;
        currentTrumpet = trumpetTotal;
        currentFlute = fluteTotal;
        currentShield = shieldTotal;
        HPText = GameObject.Find("HP Bar Object").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        HPText.text = HPTotal + "/" + HPTotal;
        violinText = GameObject.Find("Violin").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        violinText.text = violinTotal + "/" + violinTotal;
        trumpetText = GameObject.Find("Trumpet").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        trumpetText.text = trumpetTotal + "/" + trumpetTotal;
        fluteText = GameObject.Find("Flute").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        fluteText.text = fluteTotal + "/" + fluteTotal;
        shieldText = GameObject.Find("Shield").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        shieldText.text = shieldTotal + "/" + shieldTotal;
    }

    // Update is called once per frame
    void Update()
    {
        //I had titleNonStatic here before
        if (gameScript.levelSelectNonStatic == false)
        {
            if (gameScript.victory ==false) {
                if (currentHP > 0)
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
                    if (paused == false)
                    {
                        //I decided not to increase the time it needs to reload when drained.
                        if (violinDrained == true)
                        {
                            violinGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
                            if (violinGauge.fillAmount >= 1)
                            {
                                violinDrained = false;
                                violinGauge.color = new Color(0.9503901f, 1, 0, 1);
                                currentViolin = violinTotal;
                                violinText.text = currentViolin + "/" + violinTotal;
                            }
                        }
                        if (trumpetDrained == true)
                        {
                            trumpetGauge.fillAmount += (float)1 / 15 * Time.deltaTime;
                            if (trumpetGauge.fillAmount >= 1)
                            {
                                trumpetDrained = false;
                                trumpetGauge.color = new Color(0.9503901f, 1, 0, 1);
                                currentTrumpet = trumpetTotal;
                                trumpetText.text = currentTrumpet + "/" + trumpetTotal;
                            }
                        }
                        if (fluteDrained == true)
                        {
                            fluteGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
                            if (fluteGauge.fillAmount >= 1)
                            {
                                fluteDrained = false;
                                fluteGauge.color = new Color(0.9503901f, 1, 0, 1);
                                currentFlute = fluteTotal;
                                fluteText.text = currentFlute + "/" + fluteTotal;
                            }
                        }
                        if (shieldDrained == true)
                        {
                            shieldGauge.fillAmount += (float)1 / 10 * Time.deltaTime;
                            if (shieldGauge.fillAmount >= 1)
                            {
                                shieldDrained = false;
                                shieldGauge.color = new Color(0.9503901f, 1, 0, 1);
                                currentShield = shieldTotal;
                                shieldText.text = currentShield + "/" + shieldTotal;
                            }
                        }

                        //It's really interesting that I did shieldOn first
                        if (shieldOn == false)
                        {
                            if (shieldDrained == false)
                            {
                                if (Input.GetKeyDown(KeyCode.A))
                                {
                                    cancelShield = StartCoroutine(ShieldOn());
                                    audio.PlayOneShot(shieldTune, 1.5f);
                                }
                            }
                        }
                        //if (Input.GetKeyDown(KeyCode.S))
                        //{
                        //trumpetOn = true;
                        //trumpetRange.SetActive(true);
                        //}
                        if (Input.GetMouseButtonDown(1) && wind == false)
                        {
                            if (trumpet == false)
                            {
                                violin = false;
                                trumpet = true;
                                flute = false;
                                trumpetRange.SetActive(true);
                            }
                            else
                            {
                                violin = true;
                                trumpet = false;
                                flute = false;
                                trumpetRange.SetActive(false);
                            }
                            WeaponSelect();
                        }
                        if (Input.GetMouseButtonDown(2) && wind == false)
                        {

                            if (flute == false)
                            {
                                violin = false;
                                trumpet = false;
                                flute = true;
                                trumpetRange.SetActive(false);
                            }
                            else
                            {
                                violin = true;
                                trumpet = false;
                                flute = false;
                                trumpetRange.SetActive(false);
                            }
                            WeaponSelect();
                        }
                        if (lag == false)
                        {
                            if (trumpet == true)
                            {
                                if (trumpetDrained == false)
                                {
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

                        if (Input.GetKeyDown(KeyCode.S))
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
                        if (Input.GetKeyDown(KeyCode.D))
                        {
                            violin = true;
                            trumpet = false;
                            flute = false;
                            trumpetRange.SetActive(false);
                            WeaponSelect();
                        }
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
    public void WeaponReset ()
    {
        violin = true;
        trumpet = false;
        flute = false;
        trumpetRange.SetActive(false);
        WeaponSelect();
        WindEnd();
    }
    public void ViolinAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        Instantiate(violinHitbox, newPosition, violinHitbox.transform.rotation);
        //Instantiate(violinSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), violinSoundwave.transform.rotation);
        //ViolinHitEffect(newPosition);
        StartCoroutine(Lag());
            violinGauge.fillAmount -= (float)1 / violinTotal;
        currentViolin--;
        violinText.text = currentViolin + "/" + violinTotal;
        if (currentViolin <= 0)
            {
                violinDrained = true;
            violinGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
            
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
        trumpetGauge.fillAmount -= (float)1 / trumpetTotal;
        currentTrumpet--;
        trumpetText.text = currentTrumpet + "/" + trumpetTotal;
        if (currentTrumpet <= 0)
        {
            trumpetDrained = true;
            trumpetGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
        }
        //}
    }
    public void FluteAttack()
    {
        fluteGauge.fillAmount -= (float)1 / fluteTotal;
        currentFlute--;
        fluteText.text = currentFlute + "/" + fluteTotal;
        if (currentFlute <= 0) 
        { 
            fluteDrained = true;
            fluteGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); 
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
        if(hitCount >=30)
        {
            hitCountReached = true;
            StartCoroutine(AllAttackBarFlash());
        }
    }
    IEnumerator AllAttackBarFlash()
    {
        allAttackGauge.transform.localScale += new Vector3(0, allAttackGauge.transform.localScale.y*0.1f, 0);
        yield return new WaitForSeconds(1);
        allAttackGauge.transform.localScale -= new Vector3(0, allAttackGauge.transform.localScale.y * 0.1f, 0);
    }
    public void AllAttack()
    {
        //Produce a hurt and a interrupt effect on every enemy
        GameObject [] enemies =GameObject.FindGameObjectsWithTag("Enemy");
        for (int i= 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<Enemy>().TakeDamage(20, true);
            //Instantiate(hurt, enemies[i].transform.position, hurt.transform.rotation);
            //ViolinHitEffect(enemies[i].transform.position);
        }
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        for (int i = 0; i < bombs.Length; i++)
        {
            bombs[i].GetComponent<Bomb>().EnemyExplode();
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
        hitCountReached = false;
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
        if (currentHP < 20) {
            currentHP += 4;
            HPBar.fillAmount += (float)4 / HPTotal;
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
    public void WindHitEffect(Vector3 position)
    {
        Instantiate(windCrash, new Vector3(position.x, position.y, -7.59f), windCrash.transform.rotation);
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
    IEnumerator ShieldBreakAnimation()
    {
        shieldFilter.SetActive(true);
        shieldShatter.SetActive(true);
        yield return new WaitForSeconds(1);
        shieldFilter.SetActive(false);
        shieldShatter.SetActive(false);
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
    public void ShieldGaugeDown(float damage, bool red)
    {
        //Want a deep pianonote to play when this happens
        shieldGauge.fillAmount -= (float)damage / shieldTotal;
        currentShield-=damage;
        shieldText.text = currentShield + "/" + shieldTotal;
        if (damage > currentShield)
        {
            GeneralDamageCode(damage - currentShield, 3);
            audio.PlayOneShot(shieldBreak, 2f);
        }
        if (currentShield <= 0)
        {
            currentShield = 0; //For Some Reason This Solved A Problem Where ShieldGauge Would Say 1/10
            //This isn't run by an animation so this isn't an excuse
            shieldDrained = true;
            shieldGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
            shieldText.text = currentShield + "/" + shieldTotal;
            if (cancelShield!=null)
            {
                StopCoroutine(cancelShield);
                shieldOn = false;
                weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
                shieldFilter.SetActive(false);
            }
        }
        if (red == true)
        {
            cancelDamageShake = StartCoroutine(CameraShake(3));
        }
    }
    public void GeneralDamageCode(float damage, float shakeAmount)
    {
        //StartCoroutine(CameraShake(1/10));
        if (currentHP >0) {
            currentHP -= damage;
            HPBar.fillAmount -= (float)damage / HPTotal;
            PlayHurtEffect();
            HPText.text = currentHP + "/" + HPTotal;

            //I'm thinking about not cancelling the Coroutine and just changing the value of damage
            //Camera shake for giant should be double
            if (cancelDamageShake != null)
            {
                StopCoroutine(cancelDamageShake);
            }
            if (cancelDamageText != null)
            {
                StopCoroutine(cancelDamageText);
            }
            if (cancelDamageFlash != null)
            {
                StopCoroutine(cancelDamageFlash);
            }
            cancelDamageShake = StartCoroutine(CameraShake(shakeAmount));
            cancelDamageText = StartCoroutine(DamageText(damage));
            cancelDamageFlash = StartCoroutine(DamageFlash());
            if (currentHP <= 0)
            {
                gameScript.GameOver();
            }
        }
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
        camShake.m_AmplitudeGain = shakeAmount; //06/07/24, forgotto make parameter factor
        camShake.m_FrequencyGain = 0.5f;
        yield return new WaitForSeconds(1);
        camShake.m_AmplitudeGain = 0;
        camShake.m_FrequencyGain = 0.5f;
    }
    IEnumerator CameraShakeSpec(float shakeAmount)
    {
        //3, for regular attack, 5 for Red Giant, 3 for Red Giant block
        camShake.m_AmplitudeGain = shakeAmount;
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
    public void FullRestore()
    {
        currentHP = HPTotal;
        currentViolin = violinTotal;
        currentTrumpet = trumpetTotal;
        currentFlute = fluteTotal;
        currentShield = shieldTotal;
    }
    public void GainEXP(float newEXP)
    {
        EXPGained += newEXP;
        //Debug.Log(EXPGained + " equals to");
    }
    public void StartEXPUp()
    {
        //I need to find some way to know how much EXP is left to level
        //That's the amount I need to decrease besides EXPgained
        EXPCounter = EXPGained;
        //06/04/24
        //For now, I have to use this. I know it won't work because currentEXP isn't always equal to EXPToLevel. But I need something for the start of the
        //game
        //I could use EXPToLevelUp;
        //Maybe I should use EXPToLevel itself. The only problem is increasing it by 3/2 when I level 
        //I don't likw this
        //I tried it. I do need a "max" value for EXPToLevel
        //I really do need something besides EXPToLevel. Now I will just use EXPToLevel
        //I'm lierally stuck in a fucking infinite loop with currentEXP and EXPToLevel
        //The main issue is that I need to account for the start of the game. That is the only time EXPToLevel will equal its max
        //At the moment, I will use currentEXP =EXPToLevelMax, because there is no start right now
        //Or EXPToLevel = EXPToLevelMax and then,currentEXP = EXPToLevel;
        //I need some way to not have to use a startGame bool because it feels cheap. And I need to prepare for more statics. startGame is too gimmicky
        //Also, the player can start at any level (Actually, they'll still have to go to the start screen)
        currentEXP = EXPToLevelMax; 
        StartCoroutine(EXPUp(EXPGained));
        //Debug.Log("EXP Gained equal to " +EXPGained);
    }
    //The parameter is EXPGained
    IEnumerator EXPUp(float exp)
    {
        bool levelUp = false;
        while (exp >0)
        {
            exp--;
            //EXPCounter--;
            currentEXP--;
            //EXPToLevel--;
            GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("EXP Gained").GetComponent<TextMeshProUGUI>().text = "EXP Gained: " +exp;
            GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("EXP To Level").GetComponent<TextMeshProUGUI>().text = "EXP To Level: " + currentEXP;
            yield return new WaitForSeconds(0.05f);
            //if (currentEXP <=0)
            //{
            //level++;
            //}
            if (exp > 0 && currentEXP <= 0)
            {
                //EXPToLevel = EXPToLevelMax 3 / 2;
                //currentEXP = EXPToLevel;
                LevelUp();
                levelUp = true;
            }
            if (exp <= 0)
            {
                if (levelUp == true)
                {
                    StartCoroutine(PauseBeforeLevelUp());
                    StartCoroutine(StatUpMenuButton());
                }
                else
                {
                    StartCoroutine(ContinueButton());
                }
            }
        }
    }
    IEnumerator ContinueButton()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("Continue").gameObject.SetActive(true);
    }
    IEnumerator StatUpMenuButton()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("Continue With Level Up").gameObject.SetActive(true);
    }
    IEnumerator PauseBeforeLevelUp()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + level;
        //Continue Button
    }
    public void LevelUp()
    {
        //GameObject.Find("HP Bar Background").transform.localScale += new Vector3(20, 0, 0);
        //currentHP = originalHP;
        //currentHP += 3;
        level += 1;
        EXPToLevelMax *= 3 / 2;
        currentEXP = EXPToLevelMax;
        //Debug.Log(EXPToLevel);
    }
    public void HPUp()
    {
        //currentHP = originalHP;
        //currentHP += 3;
        HPTotal += 3;
        currentHP = HPTotal;
        //gameScript.ProgressLevel();
        gameScript.ContinueOrQuit();
    }
    public void ViolinUp()
    {
        violinTotal += 3;
        currentViolin = violinTotal;
        gameScript.ContinueOrQuit();
    }
    public void TrumpetUp()
    {
        trumpetTotal += 2;
        currentTrumpet = trumpetTotal;
        gameScript.ContinueOrQuit();
    }
    public void FluteUp()
    {
        fluteTotal += 3;
        currentFlute = fluteTotal;
        gameScript.ContinueOrQuit();
    }
    public void ShieldUp()
    {
        shieldTotal += 3;
        currentShield = shieldTotal;
        gameScript.ContinueOrQuit();
    }
    public void OnMouseUp()
    {
        if(wind==true)
        {
            WindEnd();
        }
    }

}
