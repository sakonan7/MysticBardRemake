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
//06/17/24
//To reload weapons while not using them, wait for a IEnumerator called harpReload
//If you use the weapon, you will cancel out
public class PlayerController : MonoBehaviour
{
    //I made these with the idea of sticking stuff together. Shield stuff with shield stuff, violin stuff with violin stuff, IE
    private GameObject damageText;
    public AudioClip harpSound;
    public AudioClip trumpet1;
    public AudioClip trumpet2;
    public AudioClip shieldTune;
    public AudioClip shieldBreak;
    private GameObject camera;
    private CinemachineBasicMultiChannelPerlin camShake;
    private GameManager gameScript;
    private float originalCameraY;
    private Image HPBar;
    private Image damageBar;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI EXPText;
    private GameObject damageFlash;
    public ParticleSystem interruptEffect;
    public GameObject hurt;
    public ParticleSystem hurtEffect;
    public ParticleSystem harpHitEffect;
    public ParticleSystem trumpetHitEffect;
    public ParticleSystem windCrash;
    public bool attack = false;
    public bool lag = false;
    public int hitCount = 0;
    private bool hitCountReached = false;
    public GameObject allAttackEffect;
    private Image harpGauge;
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
    //private int numPotionsInt = 4;
    private GameObject potionUsedIcon;

    //Private transparent


    //Private Numerics
    //It's very hard to be organized. Either you put all the HP stuff together and the private and statics are mixed up,
    //or you put all the privates together, but the HP stuff is all disorganized
    private TextMeshProUGUI HPText;
    private TextMeshProUGUI harpText;
    private TextMeshProUGUI trumpetText;
    private TextMeshProUGUI fluteText;
    private TextMeshProUGUI shieldText;

    //public objects?
    public GameObject shield;
    public GameObject shieldSpecial;
    private GameObject toolIcon;
    public GameObject trumpetRange;
    public GameObject trumpetHitbox;
    public GameObject harpHitbox;
    public GameObject trumpetSoundwave;
    public GameObject harpSoundwave;
    public GameObject fluteWind;

    public bool harpDrained = false; //I don't think I have to make this public anymore
    private float originalHP = 20;
    private float originalHarp = 15;
    private float originalTrumpet = 10;
    private float originalFlute = 4;
    public static float currentHP = 20;
    public static float HPTotal = 20;
    private static float currentHarp = 15;
    public static float harpTotal = 15;
    private static float currentTrumpet = 10;
    public static float trumpetTotal = 10;
    private static float currentFlute = 4;
    public static float fluteTotal = 4;
    private static float currentShield = 10;
    public static float shieldTotal = 10;
    private static float currentPotion = 4;
    public static float potionTotal = 4;
    public static int level = 5;
    public  int levelNonStatic = 5;
    //EXPCounter will be the number that goes down
    //currentEXP is for holding the amount
    //subtract if you levelled up
    //06/07/24
    //Each time you start the game, EXPToLevel will equal EXPToLevelMax
    //I can do this now that I have title and level select
    //Now I can rewrite based on this
    //I only have to worry about this if there is a save function
    private static float EXPToLevelLimit = 200;
    private static float EXPToLevel = EXPToLevelLimit;
    private static float EXPGained = 0;
    public int levelUpStock = 0;

    public bool shieldOn = false;
    public bool shieldDrained = false;
    public bool specialInvincibility = false;
    public bool trumpetOn = false;
    private bool trumpetDrained = false;
    public bool fluteDrained = false;
    private bool harpReloading = false;
    private Coroutine harpReloadCancel;
    private bool harpReloadStart = false;
    private bool trumpetReloading = false;
    private Coroutine trumpetReloadCancel;
    private bool trumpetReloadStart = false;
    private bool fluteReloading = false;
    private Coroutine fluteReloadCancel;
    private bool fluteReloadStart = false;

    private bool shieldReloading = false;
    private Coroutine shieldReloadCancel;

    //Private bools
    private bool harp = true;
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
    private bool gettingDamaged = false;
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
                EXPText = GameObject.Find("Mugshot").transform.Find("EXP To Level").GetComponent<TextMeshProUGUI>();
                EXPText.text = "EXP: " + EXPToLevel;
                harpGauge = GameObject.Find("Harp Gauge").GetComponent<Image>();
                trumpetGauge = GameObject.Find("Trumpet Gauge").GetComponent<Image>();
                fluteGauge = GameObject.Find("Flute Gauge").GetComponent<Image>();
                shieldGauge = GameObject.Find("Shield Gauge").GetComponent<Image>();
                toolIcon = GameObject.Find("Tool Icons");
                weaponImages = GameObject.Find("Weapon Images");
                numPotions = GameObject.Find("Number of Potions").GetComponent<TextMeshProUGUI>();
                numPotions.text = "X " + currentPotion;
            }
            else
            {
                HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
                damageBar = GameObject.Find("Damage Done").GetComponent<Image>();
                levelText = GameObject.Find("Mugshot").transform.Find("Level Text").GetComponent<TextMeshProUGUI>();
                damageFlash = GameObject.Find("Damage Object").transform.Find("Damage").gameObject;
                harpGauge = GameObject.Find("Harp Gauge").GetComponent<Image>();
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
                //numPotions.text = "X " + currentPotion;
                potionUsedIcon = GameObject.Find("Potions").transform.Find("Use Potion").gameObject;
                weaponImages.transform.Find("Harp Image").gameObject.SetActive(true);
                //TransparentUI(5);
                damageText = GameObject.Find("Player Damage Received");
            }
        }
        audio = GetComponent<AudioSource>();

        levelText.text = "Lv. " + level;
        levelNonStatic = level;

        //This is necessary in case I didn't increase any of these values
        currentHP = HPTotal;
        currentHarp = harpTotal;
        currentTrumpet = trumpetTotal;
        currentFlute = fluteTotal;
        currentShield = shieldTotal;
        currentPotion = potionTotal;
        HPText = GameObject.Find("HP Bar Object").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        HPText.text = HPTotal + "/" + HPTotal;
        harpText = GameObject.Find("Harp").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        harpText.text = harpTotal + "/" + harpTotal;
        trumpetText = GameObject.Find("Trumpet").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        trumpetText.text = trumpetTotal + "/" + trumpetTotal;
        fluteText = GameObject.Find("Flute").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        fluteText.text = fluteTotal + "/" + fluteTotal;
        shieldText = GameObject.Find("Shield").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        shieldText.text = shieldTotal + "/" + shieldTotal;
        numPotions.text = "X " + currentPotion;
        //AllAttack();
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
                        if (harpDrained == true)
                        {
                            harpGauge.fillAmount += (float)2 / 15 * Time.deltaTime;
                            if (harpGauge.fillAmount >= 1)
                            {
                                harpDrained = false;
                                harpGauge.color = new Color(0.9503901f, 1, 0, 1);
                                currentHarp = harpTotal;
                                harpText.text = harpTotal + "/" + harpTotal;
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
                        //I have to increase based on the actual gauge amount
                        //I think
                        //That'll be the nerf
                        //See if it causes problems
                        //It should, so I will
                        //06/20/24
                        //Lag will trigger this, but only if reloading isn't already on
                        //I think that's what I was missing
                        //I don't know why the bar is disappearing, though
                        if (harpReloading == true)
                        {
                            harpGauge.fillAmount += (float)1 / harpTotal * Time.deltaTime;
                            harpText.text = currentHarp + "/" + harpTotal;
                            if (harpGauge.fillAmount >= 1)
                            {
                                harpGauge.color = new Color(0.9503901f, 1, 0, 1);
                                HarpReloadCancel();
                                currentHarp = harpTotal;
                                harpText.text = harpTotal + "/" + harpTotal;
                                harpReloadStart = false;
                            }
                        }
                        if (trumpetReloading == true)
                        {
                            trumpetGauge.fillAmount += (float)1 / trumpetTotal * Time.deltaTime;
                            trumpetText.text = currentTrumpet + "/" + trumpetTotal;
                            if (trumpetGauge.fillAmount >= 1)
                            {
                                trumpetGauge.color = new Color(0.9503901f, 1, 0, 1);
                                TrumpetReloadCancel();
                                currentTrumpet = trumpetTotal;
                                trumpetText.text = trumpetTotal + "/" + trumpetTotal;
                                trumpetReloadStart = false;
                            }
                        }
                        if (fluteReloading == true)
                        {
                            fluteGauge.fillAmount += (float)1 /fluteTotal * Time.deltaTime;
                            fluteText.text = currentFlute + "/" + fluteTotal;
                            if (fluteGauge.fillAmount >= 1)
                            {
                                fluteGauge.color = new Color(0.9503901f, 1, 0, 1);
                                FluteReloadCancel();
                                currentFlute = fluteTotal;
                                fluteText.text = fluteTotal + "/" + fluteTotal;
                                fluteReloadStart = false;
                            }
                        }
                        if (shieldReloading == true)
                        {

                            if (shieldGauge.fillAmount >= 1)
                            {
                                shieldGauge.color = new Color(0.9503901f, 1, 0, 1);
                                ShieldReloadCancel();
                                currentShield = shieldTotal;
                                shieldText.text = shieldTotal + "/" + shieldTotal;
                                //shieldReloadStart = false;
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
                                harp = false;
                                trumpet = true;
                                flute = false;
                                trumpetRange.SetActive(true);
                            }
                            else
                            {
                                harp = true;
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
                                harp = false;
                                trumpet = false;
                                flute = true;
                                trumpetRange.SetActive(false);
                            }
                            else
                            {
                                harp = true;
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
                            if (harp == true)
                            {
                                if (harpDrained == false)
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        HarpAttack(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.264f)));
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
                            harp = true;
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
        if (harp == true)
        {
            weaponSelected.transform.Find("Harp Selected").gameObject.SetActive(true);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(false);
            weaponImages.transform.Find("Harp Image").gameObject.SetActive(true);
            weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(false);
            weaponImages.transform.Find("Flute Image").gameObject.SetActive(false);
        }
        if (trumpet == true)
        {
            weaponSelected.transform.Find("Harp Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(true);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(false);
            weaponImages.transform.Find("Harp Image").gameObject.SetActive(false);
            weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(true);
            weaponImages.transform.Find("Flute Image").gameObject.SetActive(false);
        }
        if (flute == true)
        {
            weaponSelected.transform.Find("Harp Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Trumpet Selected").gameObject.SetActive(false);
            weaponSelected.transform.Find("Flute Selected").gameObject.SetActive(true);
            weaponImages.transform.Find("Harp Image").gameObject.SetActive(false);
            weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(false);
            weaponImages.transform.Find("Flute Image").gameObject.SetActive(true);
        }
    }
    public void WeaponReset ()
    {
        harp = true;
        trumpet = false;
        flute = false;
        trumpetRange.SetActive(false);
        WeaponSelect();
        WindEnd();
    }
    public void HarpAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        Instantiate(harpHitbox, newPosition, harpHitbox.transform.rotation);
        if (gameScript.playEffects ==true) {
            Instantiate(harpSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), harpSoundwave.transform.rotation);
        }
        HarpHitEffect(newPosition);
        StartCoroutine(Lag(0.5f));
            harpGauge.fillAmount -= (float)1 / harpTotal;
        currentHarp--;
        harpText.text = currentHarp + "/" + harpTotal;
        if (currentHarp <= 0)
            {
                harpDrained = true;
            harpGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
            
            }
        //}
        audio.Stop();
        audio.PlayOneShot(harpSound, 0.75f);
    }
    public void TrumpetAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        //ViolinHitEffect(newPosition);
        Instantiate(trumpetHitbox, newPosition, trumpetHitbox.transform.rotation);
        if (gameScript.playEffects == true)
        {
            Instantiate(trumpetSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), trumpetSoundwave.transform.rotation);
        }
        TrumpetHitEffect(newPosition);
        StartCoroutine(Lag(0.5f));
        trumpetGauge.fillAmount -= (float)1 / trumpetTotal;
        currentTrumpet--;
        trumpetText.text = currentTrumpet + "/" + trumpetTotal;
        if (currentTrumpet <= 0)
        {
            trumpetDrained = true;
            trumpetGauge.color = new Color(0.9254902f, 0.3664465f, 0, 1);
        }
        //}
        int random = Random.Range(0, 2);
        if(random == 0)
        {
            audio.PlayOneShot(trumpet1, 0.75f);
        }
        else
        {
            audio.PlayOneShot(trumpet2, 0.75f);
        }
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
        if (flute == true)
        {
            fluteReloadStart = false;
            if (fluteReloadCancel != null)
            {
                FluteReloadCancel();
            }
        }
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
    }
    public void HitCountUp()
    {
        hitCount++;
        allAttackGauge.fillAmount += (float)1 / 25;
        if(hitCount >=25 && hitCountReached==false)
        {
            hitCountReached = true;
            StartCoroutine(AllAttackBarFlash());
        }
    }
    IEnumerator AllAttackBarFlash()
    {
        allAttackGauge.transform.localScale += new Vector3(allAttackGauge.transform.localScale.x * 0.05f, allAttackGauge.transform.localScale.y*0.2f, 0);
        allAttackGauge.color = new Color(0.9931557f, 0.8836478f,1,1);
        yield return new WaitForSeconds(1);
        allAttackGauge.transform.localScale -= new Vector3(allAttackGauge.transform.localScale.x * 0.05f, allAttackGauge.transform.localScale.y * 0.2f, 0);
        allAttackGauge.color = new Color(0.9411778f, 0, 1, 1);
    }
    public void AllAttack()
    {
        //Produce a hurt and a interrupt effect on every enemy
        GameObject [] enemies =GameObject.FindGameObjectsWithTag("Enemy");
        for (int i= 0; i < enemies.Length; i++)
        {
            //Nerf, destroying armor will not cause foe to flinch
            enemies[i].GetComponent<Enemy>().Flinch();
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
        StartCoroutine(Lag(0.5f));
        //Debug.Log("Player Attack");
        hitCount = 0;
        allAttackGauge.fillAmount=0;
        StartCoroutine(SpecialInvincibility());
        StartCoroutine(CameraShakeSpec(3));
        //allAttackEffect.SetActive(true);
        StartCoroutine(AllAttackDisappear1());
        hitCountReached = false;
    }
    IEnumerator AllAttackDisappear1()
    {
        allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        allAttackEffect.SetActive(false);
        StartCoroutine(AllAttackAgain());
    }
    IEnumerator AllAttackAgain()
    {
        //allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(0.50f);
        allAttackEffect.SetActive(true);
        StartCoroutine(AllAttackDisappear2());
        StartCoroutine(CameraShakeSpec(3));
    }
    IEnumerator AllAttackDisappear2()
    {
        allAttackEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        allAttackEffect.SetActive(false);
    }
    public void Potion()
    {
        if (currentHP < HPTotal) {
            currentHP += 4;
            HPBar.fillAmount += (float)4 / HPTotal;
            currentPotion--;
            numPotions.text = "X " + currentPotion;
            HPText.text = currentHP + "/" + HPTotal;
            if (potionUsed == false) {
                StartCoroutine(PotionUse());
            }
        }
        if (currentPotion <= 0)
        {
            numPotions.text = "";
        }
        damageBar.fillAmount = HPBar.fillAmount;
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
    IEnumerator Lag(float time)
    {
        lag = true;
        if(harp == true)
        {
            harpReloadStart = false;
            if (harpReloadCancel !=null)
            {
                HarpReloadCancel();
            }
        }
        if (trumpet == true)
        {
            trumpetReloadStart = false;
            if (trumpetReloadCancel != null)
            {
                TrumpetReloadCancel();
            }
        }
        if (trumpet == true)
        {
            trumpetRange.SetActive(false);
        }
            yield return new WaitForSeconds(time);
        lag = false;
        if (harp == true)
        {
            harpReloadStart = false;
        }
        if (trumpet == true)
        {
            trumpetReloadStart = false;
        }
        if (trumpet == true)
        {
            trumpetRange.SetActive(true);
        }
    }
    public void InterruptEffect(Vector3 position)
    {
        if (gameScript.playEffects== true) {
            Instantiate(interruptEffect, new Vector3(position.x, interruptEffect.transform.position.y, interruptEffect.transform.position.z), interruptEffect.transform.rotation);
        }
    }
    public void PlayHurtEffect()
    {
        if (gameScript.playEffects == true) {
            hurtEffect.Play();
        }
    }
    public void HarpHitEffect(Vector3 position)
    {
        if (gameScript.playEffects == true)
        Instantiate(harpHitEffect, position, harpHitbox.transform.rotation);
    }
    public void TrumpetHitEffect(Vector3 position)
    {
        if (gameScript.playEffects == true)
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
        ShieldReloadCancel();
        yield return new WaitForSeconds(2);
        shieldOn = false;
        weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
        shieldFilter.SetActive(false);

        //Maybe do shieldReloadCancel != null
        if (shieldReloading == false && currentShield < shieldTotal)
        {
            shieldReloadCancel = StartCoroutine(ShieldReload());
        }
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

        //06/19/24
        //Need to place this here, because it interprets this after the damage has been done. Aka, you have 2 left after 4 damage, it's interpreted
        //as 4 > 2, when it's supposed to be 4 > 2 before shield break

        //06/23/24
        //I will do shieldReloading == false, but I need some way to start this up again if stop using shield
        //Maybe do ShieldReload IEnumerator here, too
        ShieldReloadCancel();
        bool shieldBroken = false;
        if (damage > currentShield)
        {
            GeneralDamageCode(damage - currentShield, 3);
            audio.PlayOneShot(shieldBreak, 2f);
            shieldBroken = true;
        }
        shieldGauge.fillAmount -= (float)damage / shieldTotal;
        currentShield-=damage;
        shieldText.text = currentShield + "/" + shieldTotal;

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
        if(shieldBroken ==true)
        {
            StartCoroutine(ShieldBreakAnimation());
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
        if(gettingDamaged ==false)
        {
            StartCoroutine(DamageBar());
        }
    }
    IEnumerator DamageText(float damage)
    {
        damageText.GetComponent<TextMeshProUGUI>().text = "" + damage;
        yield return new WaitForSeconds(1);
        damageText.GetComponent<TextMeshProUGUI>().text = "";
    }
    IEnumerator DamageBar()
    {
        gettingDamaged = true;
        yield return new WaitForSeconds(3);
        gettingDamaged = false;
        damageBar.fillAmount = HPBar.fillAmount;
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
    //06/19/24
    //After 2 seconds pass, start reloading. Gauge turns Green and thick
    //Gonna use ReloadCancel Methods for simplicity
    IEnumerator HarpReload()
    {
        yield return new WaitForSeconds(2);
        harpReloading = true;
        //harpGauge.transform.localScale += new Vector3(harpGauge.transform.localScale.x * 0.05f, harpGauge.transform.localScale.y * 0.05f, 0);
        harpGauge.color = new Color(0.6997535f, 0, 0.5817609f, 0);
    }
    //06/20/24
    //Wait, why do I even need StopCoroutine in ReloadCancel()
    //I think it's for lag
    public void HarpReloadCancel()
    {
        if (harpReloadCancel != null)
        {
            StopCoroutine(harpReloadCancel);
        }
        harpReloading = false;
        //harpGauge.transform.localScale -= new Vector3(harpGauge.transform.localScale.x * 0.05f, harpGauge.transform.localScale.y * 0.05f, 0);
    }
    IEnumerator TrumpetReload()
    {
        yield return new WaitForSeconds(2);
        trumpetReloading = true;
        //trumpetGauge.transform.localScale += new Vector3(trumpetGauge.transform.localScale.x*0.05f, trumpetGauge.transform.localScale.y * 0.05f, 0);
        trumpetGauge.color = new Color(0.6997535f, 0, 0.5817609f, 0);
    }
    public void TrumpetReloadCancel()
    {
        if(trumpetReloadCancel!=null)
        {
            StopCoroutine(trumpetReloadCancel);
        }
        trumpetReloading = false;
        //trumpetGauge.transform.localScale -= new Vector3(trumpetGauge.transform.localScale.x * 0.05f, trumpetGauge.transform.localScale.y * 0.05f, 0);
    }
    IEnumerator FluteReload()
    {
        yield return new WaitForSeconds(2);
        fluteReloading = true;
        //fluteGauge.transform.localScale += new Vector3(fluteGauge.transform.localScale.x * 0.05f, fluteGauge.transform.localScale.y * 0.05f, 0);
        fluteGauge.color = new Color(0.6997535f, 0, 0.5817609f, 0);
    }
    public void FluteReloadCancel()
    {
        if (fluteReloadCancel != null)
        {
            StopCoroutine(fluteReloadCancel);
        }
        fluteReloading = false;
        //fluteGauge.transform.localScale -= new Vector3(fluteGauge.transform.localScale.x * 0.05f, fluteGauge.transform.localScale.y * 0.05f, 0);
    }
    IEnumerator ShieldReload()
    {
        yield return new WaitForSeconds(5 +3);
        shieldReloading = true;
        //fluteGauge.transform.localScale += new Vector3(fluteGauge.transform.localScale.x * 0.05f, fluteGauge.transform.localScale.y * 0.05f, 0);
        //shieldGauge.color = new Color(0.6997535f, 0, 0.5817609f, 0);
        StartCoroutine(ActualShieldReload());
    }
    public void ShieldReloadCancel()
    {
        if (shieldReloadCancel != null)
        {
            StopCoroutine(shieldReloadCancel);
        }
        shieldReloading = false;
        //fluteGauge.transform.localScale -= new Vector3(fluteGauge.transform.localScale.x * 0.05f, fluteGauge.transform.localScale.y * 0.05f, 0);
    }
    IEnumerator ActualShieldReload()
    {
        while (shieldReloading == true)
        {
            yield return new WaitForSeconds(1);
            currentShield++;
            //shieldGauge.color = new Color(0.6997535f, 0, 0.5817609f, 0);
            //shieldGauge.fillAmount += (float)1 / 10 * Time.deltaTime;
            shieldGauge.fillAmount += (float)(1 / shieldTotal);
            //I need to add to a quantity over time
            shieldText.text = currentShield + "/" + shieldTotal;
        }
        if (currentShield > shieldTotal)
        {
            currentShield = shieldTotal;
            shieldText.text = currentShield + "/" + shieldTotal;
        }
        Debug.Log("End");
    }
    public void TransparentUI(float time)
    {
        //4 + weapon images (use bool) + shield + weapons + potions
        //No text atm
        GameObject.Find("HP Bar Background").GetComponent<RawImage>().color = new Color(this.GetComponent<RawImage>().color.r, this.GetComponent<RawImage>().color.g, this.GetComponent<RawImage>().color.b, 0.5f);
        StartCoroutine(TransparentUITime(time));
    }
    IEnumerator TransparentUITime(float time)
    {
        yield return new WaitForSeconds(time);
    }
    public void FullRestore()
    {
        currentHP = HPTotal;
        currentHarp = harpTotal;
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
        //currentEXP = EXPToLevelMax; 

        //Debug.Log("EXP Gained equal to " +EXPGained);
        //06/10/24
        StartCoroutine(EXPUp(EXPGained));
    }
    //The parameter is EXPGained
    IEnumerator EXPUp(float exp)
    {
        bool levelUp = false;
        while (exp >0)
        {
            exp--;
            EXPGained--;
            EXPToLevel--;
            //EXPCounter--;
            //EXPToLevel--;

            GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("EXP Gained").GetComponent<TextMeshProUGUI>().text = "EXP Gained: " +EXPGained;
            GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("EXP To Level").GetComponent<TextMeshProUGUI>().text = "EXP To Level: " + EXPToLevel;
            yield return new WaitForSeconds(0.025f);
            //if (currentEXP <=0)
            //{
            //level++;
            //}
            if (exp > 0 &&EXPToLevel <= 0)
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
    public void ContinueInstead()
    {
        StartCoroutine(ContinueButton());
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
    IEnumerator PauseBeforeLevelUpAgainButton()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("More Level Up").gameObject.SetActive(true);
    }
    IEnumerator PauseBeforeLevelUp()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. " + level;
        levelText.text = "Lv. " + level;
        if(level >= 11)
        {
            GameObject.Find("EXP").transform.Find("Level Up Object").transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv. MAX";
            levelText.text = "Lv. MAX";
        }
        //Continue Button
    }
    public void LevelUp()
    {
        //GameObject.Find("HP Bar Background").transform.localScale += new Vector3(20, 0, 0);
        //currentHP = originalHP;
        //currentHP += 3;
        level += 1;
        EXPToLevelLimit *= 1.75f;
        EXPToLevel = EXPToLevelLimit;
        //Debug.Log(EXPToLevel);
        levelUpStock++;
        if (level >=11)
        {
            level = 11;
            levelUpStock = 1;
        }
    }
    public void HPUp()
    {
        //currentHP = originalHP;
        //currentHP += 3;
        HPTotal += 3;
        currentHP = HPTotal;
        levelUpStock--;
        //gameScript.ProgressLevel();
        gameScript.ContinueOrQuit();
    }
    public void HarpUp()
    {
        harpTotal += 3;
        currentHarp = harpTotal;
        levelUpStock--;
        gameScript.ContinueOrQuit();
    }
    public void TrumpetUp()
    {
        trumpetTotal += 2;
        currentTrumpet = trumpetTotal;
        levelUpStock--;
        gameScript.ContinueOrQuit();
    }
    public void FluteUp()
    {
        fluteTotal += 1;
        currentFlute = fluteTotal;
        levelUpStock--;
        gameScript.ContinueOrQuit();
    }
    public void ShieldUp()
    {
        shieldTotal += 2;
        currentShield = shieldTotal;
        levelUpStock--;
        gameScript.ContinueOrQuit();
    }
    public void PotionUp()
    {
        potionTotal += 1;
        currentPotion = potionTotal;
        levelUpStock--;
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
