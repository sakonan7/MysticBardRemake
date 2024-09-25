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
    public AudioClip harpSound2;
    public AudioClip trumpet1;
    public AudioClip trumpet2;
    public AudioClip fluteSound;
    public AudioClip fluteSound2;
    public AudioClip windDamage1;
    public AudioClip windDamage2;
    public AudioClip shieldTune;
    public AudioClip shieldTune2;
    public AudioClip guard;
    public AudioClip pianoSlam;
    public AudioClip shieldBreak;
    public AudioClip allAttack;
    public AudioClip allAttackFilled;
    public AudioClip debrisHit;
    public AudioClip interrupt1;
    public AudioClip interrupt2;
    public AudioClip gulp;
    public AudioClip bell1;
    public AudioClip bell2;
    public AudioClip toggleOff;
    public AudioClip toggleOn;
    public AudioClip statUp;
    public AudioClip difficultyOn;
    public AudioClip difficultyOff;
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
    public ParticleSystem hurtEffect2;
    public ParticleSystem harpHitEffect;
    public ParticleSystem trumpetHitEffect;
    public ParticleSystem windCrash;
    public ParticleSystem debrisCrash;
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
    private GameObject pause;
    private GameObject cantPauseObject;

    //Private transparent
    private RawImage HPBackgroundT;
    private Image HPBarT;
    private Image damageBarT;
    private RawImage allAttackBackgroundT;
    private TextMeshProUGUI numericT;
    private RawImage mugShotT;
    private TextMeshProUGUI levelT;
    private RawImage bossHPBackgroundT;
    private Image bossHPBarT;
    private Image bossdamageBarT;
    //Weapon Images, Just Make Them All Transparent
    //I just have the weapon barsleft
    private Image harpBar1;
    private Image harpBar2;
    private Image harpBar3;
    private Image harpBar4;
    private Image trumpetBar1;
    private Image trumpetBar2;
    private Image trumpetBar3;
    private Image trumpetBar4;
    private Image fluteBar1;
    private Image fluteBar2;
    private Image fluteBar3;
    private Image fluteBar4;
    private Image shieldBar1;
    private Image shieldBar2;
    private Image shieldBar3;
    private Image shieldBar4;
    private RawImage potionT;

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
    public GameObject debrisHitbox;
    public GameObject hitbox;
    public GameObject trumpetSoundwave;
    public GameObject harpSoundwave;
    public GameObject fluteWind;
    private ParticleSystem windDamage;

    public bool harpDrained = false; //I don't think I have to make this public anymore
    private float originalHP = 20;
    private float originalHarp = 15;
    private float originalTrumpet = 10;
    private float originalFlute = 4;
    public static float currentHP = 20;
    public static float HPTotal = 20;
    private static float currentHarp = 15;
    public static float harpTotal = 15;
    private static float currentTrumpet = 8;
    public static float trumpetTotal = 8;
    private static float currentFlute = 4;
    public static float fluteTotal = 4;
    private static float currentShield = 10;
    public static float shieldTotal = 10;
    private static float currentPotion = 4;
    public static float potionTotal = 4;
    public static int level = 5;
    public  int levelNonStatic = 5;
    private bool HPMaxed = false;
    private bool harpMaxed = false;
    private bool trumpetMaxed = false;
    private bool fluteMaxed = false;
    private bool shieldMaxed = false;
    private bool potionMaxed = false;
    //EXPCounter will be the number that goes down
    //currentEXP is for holding the amount
    //subtract if you levelled up
    //06/07/24
    //Each time you start the game, EXPToLevel will equal EXPToLevelMax
    //I can do this now that I have title and level select
    //Now I can rewrite based on this
    //I only have to worry about this if there is a save function
    private static int EXPToLevelLimit = 200;
    private static int EXPToLevel = EXPToLevelLimit;
    private static int EXPGained = 0;
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
    private bool cantPause = false;
    private bool cantRepeat = false;
    private int numOfSoundEffects = 0;
    private bool uninterruptibleSound = false;

    //Statics for the most part
    public static int EXP = 0;
    private static bool noEXP = false;
    public bool noEXPNonStatic = false;

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
        if (gameScript.nonGameNonStatic ==false) {
            if (gameScript.levelSelectNonStatic == true)
            {
                HPBar = GameObject.Find("HP Bar").GetComponent<Image>();
                levelText = GameObject.Find("Mugshot").transform.Find("Level Text").GetComponent<TextMeshProUGUI>();
                //EXPText = GameObject.Find("Mugshot").transform.Find("EXP To Level").GetComponent<TextMeshProUGUI>();
                //EXPText.text = "EXP: " + EXPToLevel;
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
                HPBackgroundT = GameObject.Find("HP Bar Background").GetComponent<RawImage>();
                HPBarT = GameObject.Find("HP Bar Background").transform.Find("HP Bar").GetComponent<Image>();
                damageBarT = GameObject.Find("HP Bar Background").transform.Find("Damage Done").GetComponent<Image>();
                allAttackBackgroundT = GameObject.Find("All Attack Background").GetComponent<RawImage>();
                mugShotT = GameObject.Find("Mugshot").GetComponent<RawImage>();
                potionT = GameObject.Find("Potion Icon").GetComponent<RawImage>();
                if (gameScript.boss==true) {
                    bossHPBackgroundT = GameObject.Find("Boss HP Object").transform.Find("Boss HP Bar Background").GetComponent<RawImage>();
                    bossHPBarT = bossHPBackgroundT.transform.Find("Boss HP Bar").GetComponent<Image>();
                    bossdamageBarT = bossHPBackgroundT.transform.Find("Damage Taken").GetComponent<Image>();
                }
                harpBar1 =GameObject.Find("Harp").transform.Find("Image").GetComponent<Image>();
                harpBar2 = GameObject.Find("Harp").transform.Find("Image (1)").GetComponent<Image>();
                harpBar3 = GameObject.Find("Harp").transform.Find("Image (2)").GetComponent<Image>();
                harpBar4 = GameObject.Find("Harp").transform.Find("Image (3)").GetComponent<Image>();
                trumpetBar1 = GameObject.Find("Trumpet").transform.Find("Image").GetComponent<Image>();
                trumpetBar2 = GameObject.Find("Trumpet").transform.Find("Image (1)").GetComponent<Image>();
                trumpetBar3 = GameObject.Find("Trumpet").transform.Find("Image (2)").GetComponent<Image>();
                trumpetBar4 = GameObject.Find("Trumpet").transform.Find("Image (3)").GetComponent<Image>();
                fluteBar1 = GameObject.Find("Flute").transform.Find("Image").GetComponent<Image>();
                fluteBar2 = GameObject.Find("Flute").transform.Find("Image (1)").GetComponent<Image>();
                fluteBar3 = GameObject.Find("Flute").transform.Find("Image (2)").GetComponent<Image>();
                fluteBar4 = GameObject.Find("Flute").transform.Find("Image (3)").GetComponent<Image>();
                shieldBar1 = GameObject.Find("Shield").transform.Find("Image (4)").GetComponent<Image>();
                shieldBar2 = GameObject.Find("Shield").transform.Find("Image (5)").GetComponent<Image>();
                shieldBar3 = GameObject.Find("Shield").transform.Find("Image (6)").GetComponent<Image>();
                shieldBar4 = GameObject.Find("Shield").transform.Find("Image (7)").GetComponent<Image>();

                damageText = GameObject.Find("Player Damage Received");
                pause = GameObject.Find("Pause Object").transform.Find("Pause").gameObject;
                cantPauseObject = GameObject.Find("Pause Object").transform.Find("Can't Pause").gameObject;
            }
        }
        audio = GetComponent<AudioSource>();

        levelText.text = "Lv. " + level;
        levelNonStatic = level;
        if(level >=11)
        {
            levelText.text = "Lv. MAX";
        }

        //This is necessary in case I didn't increase any of these values
        currentHP = HPTotal;
        currentHarp = harpTotal;
        currentTrumpet = trumpetTotal;
        currentFlute = fluteTotal;
        currentShield = shieldTotal;
        currentPotion = potionTotal;
        HPText = GameObject.Find("HP Bar Object").transform.Find("Numeric").GetComponent<TextMeshProUGUI>();
        HPText.text = HPTotal + "/" + HPTotal;
        if(gameScript.levelSelectNonStatic == true)
        {
            HPText.text = HPTotal + "/" + HPTotal + "    EXP: " + EXPToLevel;
        }
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
        noEXPNonStatic = noEXP;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            TransparentUI(5);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //UndoTransparentUI();
        }
        //I had titleNonStatic here before
        if (gameScript.levelSelectNonStatic == false)
        {
            if (gameScript.victory ==false) {
                if (currentHP > 0)
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (cantPause==false) {
                            if (paused == false)
                            {
                                paused = true;
                                Time.timeScale = 0;
                                pause.SetActive(true);
                            }
                            else
                            {
                                paused = false;
                                Time.timeScale = 1;
                                //Debug.Log("Pause Undone");
                                pause.SetActive(false);
                            }
                        }
                        else
                        {
                            if (cantRepeat==false) {
                                StartCoroutine(CantPauseDisplay());
                            }
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
                                    int random = Random.Range(0, 2);
                                    if (random == 0)
                                    {
                                        audio.PlayOneShot(shieldTune, 1.5f);
                                    }
                                    else
                                    {
                                        audio.PlayOneShot(shieldTune2, 1);
                                    }
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
                            if (currentShield > shieldTotal)
                            {
                                currentShield = shieldTotal;
                                shieldText.text = currentShield + "/" + shieldTotal;
                                shieldReloading = false;
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
                                    else if (Input.GetKeyDown(KeyCode.E))
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
                                    else if (Input.GetKeyDown(KeyCode.E))
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
                            //09/05/24 Don't know why I didn't do this, but I must have been very bus
                            if (currentHP < HPTotal)
                            {
                                Potion();
                            }
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
        if (GameManager.playEffectsStatic ==true) {
            Instantiate(harpSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), harpSoundwave.transform.rotation);
        }
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
        if (uninterruptibleSound == false)
        {
            audio.Stop();
        }
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            audio.PlayOneShot(harpSound, 0.75f);
        }
        else
        {
            audio.PlayOneShot(harpSound2, 0.75f);
        }
        
    }
    public void TrumpetAttack(Vector3 newPosition)
    {
        //if (violinDrained ==false) {
        //ViolinHitEffect(newPosition);
        Instantiate(trumpetHitbox, newPosition, trumpetHitbox.transform.rotation);
        if (GameManager.playEffectsStatic == true)
        {
            Instantiate(trumpetSoundwave, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z - 8.6f)), trumpetSoundwave.transform.rotation);
        }
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
        if (uninterruptibleSound == false)
        {
            audio.Stop();
        }
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
    public void DebrisHitBox(Vector3 position)
    {
        //Always the same Z
        Instantiate(debrisHitbox, new Vector3(position.x,position.y,-7.59f),debrisHitbox.transform.rotation);
    }
    public void HitBox(Vector3 position)
    {
        //Always the same Z
        Instantiate(hitbox, new Vector3(position.x, position.y, -7.59f), hitbox.transform.rotation);
    }
    public void WindOn()
    {
        wind = true;
        fluteWind.SetActive(true);
        windDamage = fluteWind.transform.Find("Wind Damage").GetComponent<ParticleSystem>();
        windDamage.Stop();
        if (flute == true)
        {
            fluteReloadStart = false;
            if (fluteReloadCancel != null)
            {
                FluteReloadCancel();
            }
        }
        if (uninterruptibleSound == false)
        {
            audio.Stop();
        }
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            audio.PlayOneShot(fluteSound, 1);
        }
        else
        {
            audio.PlayOneShot(fluteSound2, 1);
        }
    }
    public void WindDamage()
    {
        windDamage.Play();
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            audio.PlayOneShot(windDamage1, 1);
        }
        else
        {
            audio.PlayOneShot(windDamage2, 1);
        }
    }
    public void WindEnd()
    {
        wind = false;
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
            //audio.Stop();
            if (uninterruptibleSound == false)
            {
                StartCoroutine(UninterruptibleSound());
            }
            audio.PlayOneShot(allAttackFilled,2);
            GameObject.Find("Press S").GetComponent<TextMeshProUGUI>().text ="Press S";
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
        GameObject [] enemiesTemp =GameObject.FindGameObjectsWithTag("Enemy");
        for (int i= 0; i < enemiesTemp.Length; i++)
        {
            //Nerf, destroying armor will not cause foe to flinch
            //if (enemies[i].GetComponent<Enemy>().counterAttackActive == false) {
                //enemies[i].GetComponent<Enemy>().Flinch(true);
                enemiesTemp[i].GetComponent<Enemy>().GeneralDamageCode(20, true, 3,true);
            //}
            //else
            //{
                //enemies[i].GetComponent<Enemy>().CounterAttackTriggered();
            //}
            //Instantiate(hurt, enemies[i].transform.position, hurt.transform.rotation);
            //ViolinHitEffect(enemies[i].transform.position);
        }
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        for (int i = 0; i < bombs.Length; i++)
        {
            if (bombs[i].GetComponent<Bomb>().exploded ==false) {
                bombs[i].GetComponent<Bomb>().EnemyExplode();
            }
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
        if (GameManager.playEffectsStatic           ) {
            StartCoroutine(AllAttackDisappear1());
        }
        hitCountReached = false;
        if (uninterruptibleSound == false)
        {
            StartCoroutine(UninterruptibleSound());
        }
        audio.PlayOneShot(allAttack,3);
        GameObject.Find("Press S").GetComponent<TextMeshProUGUI>().text = "";
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
        int random = Random.Range(0, 2);
        if (potionUsed == false)
        {
            if (random == 0)
            {
                audio.PlayOneShot(bell1, 0.25f);
            }
            else
            {
                audio.PlayOneShot(bell2, 0.25f);
            }
        }
        StartCoroutine(Gulp());
        if (currentHP < HPTotal) {
            currentHP += 4;
            HPBar.fillAmount += (float)4 / HPTotal;
            currentPotion--;
            numPotions.text = "X " + currentPotion;
            HPText.text = currentHP + "/" + HPTotal;
            if (potionUsed == false) {
                StartCoroutine(PotionUse());
            }
            if(currentHP>HPTotal)
            {
                currentHP = HPTotal;
                HPText.text = currentHP + "/" + HPTotal;
            }
        }
        if (currentPotion <= 0)
        {
            numPotions.text = "";
        }
        damageBar.fillAmount = HPBar.fillAmount;
        if (uninterruptibleSound == false)
        {
            StartCoroutine(UninterruptibleSound());
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
    IEnumerator Gulp()
    {
        yield return new WaitForSeconds(1);
        audio.PlayOneShot(gulp, 2);
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
        if (GameManager.playEffectsStatic == true) {
            Instantiate(interruptEffect, position, interruptEffect.transform.rotation);
        }
        if (uninterruptibleSound == false)
        {
            StartCoroutine(UninterruptibleSound());
        }
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            audio.PlayOneShot(interrupt1, 0.5f);
        }
        else
        {
            audio.PlayOneShot(interrupt2, 0.5f);
        }
    }
    public void PlayHurtEffect(bool unblockable)
    {
        if (GameManager.playEffectsStatic == true) {
            if (unblockable == false)
            {
                hurtEffect.Play();
            }
            else
            {
                hurtEffect2.Play();
            }
        }
    }
    public void HarpHitEffect(Vector3 position)
    {
        if (GameManager.playEffectsStatic == true)
        Instantiate(harpHitEffect, position, harpHitbox.transform.rotation);
    }
    public void TrumpetHitEffect(Vector3 position)
    {
        if (GameManager.playEffectsStatic == true)
        Instantiate(trumpetHitEffect, position, trumpetHitEffect.transform.rotation);
    }
    public void WindHitEffect(Vector3 position)
    {
        Instantiate(windCrash, new Vector3(position.x, position.y, -7.59f), windCrash.transform.rotation);
    }
    public void DebrisHitEffect(Vector3 position)
    {
        if (GameManager.playEffectsStatic == true)
        {
            Instantiate(debrisCrash, new Vector3(position.x, position.y, -7.59f), debrisCrash.transform.rotation);
        }
        audio.PlayOneShot(debrisHit, 0.6f);
    }
    IEnumerator ShieldOn()
    {
        shieldOn = true;
        weaponImages.transform.Find("Shield Image").gameObject.SetActive(true);
        shieldFilter.SetActive(true);
        ShieldReloadCancel();
        weaponImages.transform.Find("Harp Image").gameObject.SetActive(false);
        weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(false);
        weaponImages.transform.Find("Flute Image").gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        shieldOn = false;
        weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
        shieldFilter.SetActive(false);

        //Maybe do shieldReloadCancel != null
        if (shieldReloading == false && currentShield < shieldTotal)
        {
            shieldReloadCancel = StartCoroutine(ShieldReload());
        }

        numOfSoundEffects = 0;
        WeaponSelect();
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
        weaponImages.transform.Find("Harp Image").gameObject.SetActive(false);
        weaponImages.transform.Find("Trumpet Image").gameObject.SetActive(false);
        weaponImages.transform.Find("Flute Image").gameObject.SetActive(false);
        weaponImages.transform.Find("Special Image").gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        specialInvincibility = false;
        //weaponImages.transform.Find("Shield Image").gameObject.SetActive(false);
        specialFilter.SetActive(false);
        WeaponSelect();
        weaponImages.transform.Find("Special Image").gameObject.SetActive(false);
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
        //Want a deep pianonote to play when this happens

        //06/19/24
        //Need to place this here, because it interprets this after the damage has been done. Aka, you have 2 left after 4 damage, it's interpreted
        //as 4 > 2, when it's supposed to be 4 > 2 before shield break

        //06/23/24
        //I will do shieldReloading == false, but I need some way to start this up again if stop using shield
        //Maybe do ShieldReload IEnumerator here, too
        if (uninterruptibleSound==false)
        {
            audio.Stop();
        }
        ShieldReloadCancel();
        bool shieldBroken = false;
        if (damage > currentShield)
        {
            if (uninterruptibleSound == false)
            {
                StartCoroutine(UninterruptibleSound());
            }
            GeneralDamageCode(damage - currentShield, 3, false);
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
                numOfSoundEffects = 0;
                WeaponSelect();
            }
            

        }
        if (damage >=3)
        {
            if (uninterruptibleSound == false)
            {
                StartCoroutine(UninterruptibleSound());
            }
            cancelDamageShake = StartCoroutine(CameraShake(3));
            audio.PlayOneShot(pianoSlam, 1.5f);
        }
        else
        {
            numOfSoundEffects++;
            if (numOfSoundEffects < 4) {
                audio.PlayOneShot(guard, 0.9f);
            }
        }
        if(shieldBroken ==true)
        {
            StartCoroutine(ShieldBreakAnimation());
        }
    }
    public void PlayGuardSound()
    {
        audio.PlayOneShot(guard, 0.9f);
    }
    public void GeneralDamageCode(float damage, float shakeAmount, bool unblockable)
    {
        //StartCoroutine(CameraShake(1/10));
        if (currentHP >0) {
            currentHP -= damage;
            HPBar.fillAmount -= (float)damage / HPTotal;
            PlayHurtEffect(unblockable);
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
        }
        //Created a zombie situation
        if (currentHP <= 0)
        {
            gameScript.GameOver();
        }
        if (gettingDamaged ==false)
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
            if (currentShield > shieldTotal)
            {
                currentShield = shieldTotal;
                shieldText.text = currentShield + "/" + shieldTotal;
                shieldReloading = false;
            }
        }
        if (currentShield > shieldTotal)
        {
            currentShield = shieldTotal;
            shieldText.text = currentShield + "/" + shieldTotal;
        }
        //Debug.Log("End");
    }
    IEnumerator UninterruptibleSound()
    {
        uninterruptibleSound = true;
        yield return new WaitForSeconds(2);
        uninterruptibleSound = false;
    }
    public void TransparentUI(float time)
    {
        //4 + weapon images (use bool) + shield + weapons + potions
        //No text atm
        //GameObject.Find("HP Bar Background").GetComponent<RawImage>().color = new Color(this.GetComponent<RawImage>().color.r, this.GetComponent<RawImage>().color.g, this.GetComponent<RawImage>().color.b, 0.5f);
        HPBackgroundT.color = new Color(HPBackgroundT.color.r, HPBackgroundT.color.b, HPBackgroundT.color.g, 0.5f);
        //StartCoroutine(TransparentUITime(time));
        //harpGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //trumpetGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //fluteGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //shieldGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //HPBarT.color = new Color(0.3607843f, 1, 0, 0.5f);
        //damageBarT = GameObject.Find("HP Bar Background").transform.Find("Damage Done").GetComponent<Image>();
        mugShotT.color = new Color(mugShotT.color.r, mugShotT.color.b, mugShotT.color.g, 0.5f);
        potionT.color = new Color(potionT.color.r, potionT.color.b, potionT.color.g, 0.5f);
        allAttackBackgroundT.color = new Color(allAttackBackgroundT.color.r, allAttackBackgroundT.color.b, allAttackBackgroundT.color.g, 0.5f);
        if (gameScript.boss==true) {
            bossHPBackgroundT.color = new Color(bossHPBackgroundT.color.r, bossHPBackgroundT.color.b, bossHPBackgroundT.color.g, 0.5f);
            bossHPBarT.color = new Color(1, 0, 0.1378136f, 0.5f);
            //bossHPBarT = bossHPBackgroundT.transform.Find("HP Bar").GetComponent<Image>();
            //bossdamageBarT = bossHPBackgroundT.transform.Find("Damage Done").GetComponent<Image>();
        }
        harpBar1.color = new Color(harpBar1.color.r, harpBar1.color.b, harpBar1.color.g, 0.5f);
        harpBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 0.5f);
        harpBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 0.5f);
        harpBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 0.5f);
        trumpetBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 0.5f);
        trumpetBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 0.5f);
        trumpetBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 0.5f);
        trumpetBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 0.5f);
        fluteBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 0.5f);
        fluteBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 0.5f);
        fluteBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 0.5f);
        fluteBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 0.5f);
        shieldBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 0.5f);
        shieldBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 0.5f);
        shieldBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 0.5f);
        shieldBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 0.5f);
        StartCoroutine(TransparentUITime(time));
    }
    public void UndoTransparentUI()
    {
        HPBackgroundT.color = new Color(HPBackgroundT.color.r, HPBackgroundT.color.b, HPBackgroundT.color.g, 1);
        //StartCoroutine(TransparentUITime(time));
        //harpGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //trumpetGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //fluteGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //shieldGauge.color = new Color(harpGauge.color.r, harpGauge.color.b, harpGauge.color.g, 0.5f);
        //HPBarT.color = new Color(HPBarT.color.r, HPBarT.color.b, HPBarT.color.g, 0.5f);
        //damageBarT = GameObject.Find("HP Bar Background").transform.Find("Damage Done").GetComponent<Image>();
        mugShotT.color = new Color(mugShotT.color.r, mugShotT.color.b, mugShotT.color.g, 1);
        potionT.color = new Color(potionT.color.r, potionT.color.b, potionT.color.g, 1);
        allAttackBackgroundT.color = new Color(allAttackBackgroundT.color.r, allAttackBackgroundT.color.b, allAttackBackgroundT.color.g, 1);
        if (gameScript.boss == true)
        {
            bossHPBackgroundT.color = new Color(bossHPBackgroundT.color.r, bossHPBackgroundT.color.b, bossHPBackgroundT.color.g, 1);
            bossHPBarT.color = new Color(1, 0, 0.1378136f, 1);
            //bossdamageBarT = bossHPBackgroundT.transform.Find("Damage Done").GetComponent<Image>();
        }
        harpBar1.color = new Color(harpBar1.color.r, harpBar1.color.b, harpBar1.color.g, 1);
        harpBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 1);
        harpBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 1);
        harpBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 1);
        trumpetBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 1);
        trumpetBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 1);
        trumpetBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 1);
        trumpetBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 1);
        fluteBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 1);
        fluteBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 1);
        fluteBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 1);
        fluteBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 1);
        shieldBar1.color = new Color(trumpetBar1.color.r, trumpetBar1.color.b, trumpetBar1.color.g, 1);
        shieldBar2.color = new Color(harpBar2.color.r, harpBar2.color.b, harpBar2.color.g, 1);
        shieldBar3.color = new Color(harpBar3.color.r, harpBar3.color.b, harpBar3.color.g, 1);
        shieldBar4.color = new Color(harpBar4.color.r, harpBar4.color.b, harpBar4.color.g, 1);
    }
    IEnumerator TransparentUITime(float time)
    {
        //TransparentUI();
        yield return new WaitForSeconds(time);
        UndoTransparentUI();
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
        if (noEXP ==false) {
            EXPGained += (int)newEXP;
        }
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
            yield return new WaitForSeconds(2*Time.deltaTime);
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
        if (level < 11) {
            level += 1;
            EXPToLevelLimit *= (int)1.75f;
            EXPToLevel = EXPToLevelLimit;
            //Debug.Log(EXPToLevel);
            levelUpStock++;

        }
        else
        {
            //if (level >= 11)
            //{
                level = 11;
                //levelUpStock = 1; //This was written with the idea that the player was level 10
                //I need to either do this stock correctly, or make EXP process each level
                //The latter might be more appealing to people
                //I think the former was to make EXP stop calculating after reaching 11. like EXPToLevelLimit becomes 0 or becomes very high
                //Simply stop doing LevelUp after reaching 11
            //}
        }
    }
    public void NoEXP()
    {
        noEXP = !noEXP;
        if (noEXP ==false)
        {
            PlayToggleOff();
            PlayDifficultyOff();
            GameObject.Find("Difficulty Buttons").transform.Find("No EXP").gameObject.SetActive(false);
        }
        else
        {
            PlayToggleOn();
            PlayDifficultyOn();
            GameObject.Find("Difficulty Buttons").transform.Find("No EXP").gameObject.SetActive(true);
        }
    }
    public void PlayToggleOff()
    {
        audio.PlayOneShot(toggleOff,1);
    }
    public void PlayToggleOn()
    {
        audio.PlayOneShot(toggleOn, 1);
        StartCoroutine(ToggleOnTurnOff());
    }
    IEnumerator ToggleOnTurnOff()
    {
        yield return new WaitForSeconds(2);
        audio.Stop();
    }
    public void PlayDifficultyOn()
    {
        audio.PlayOneShot(difficultyOn, 1);
    }
    public void PlayDifficultyOff()
    {
        audio.PlayOneShot(difficultyOff, 1);
    }
    public void LevelSelectSound()
    {
        int random = Random.Range(0, 4);
        if (random == 0)
        {
            audio.PlayOneShot(harpSound, 0.75f);
        }
        else if (random == 1)
        {
            audio.PlayOneShot(harpSound2, 0.75f);
        }
        else if (random == 2)
        {
            audio.PlayOneShot(trumpet1, 0.75f);
        }
        else if (random == 3)
        {
            audio.PlayOneShot(trumpet2, 0.75f);
        }
    }
    public void StatUpSound()
    {
        audio.PlayOneShot(statUp,1);
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
    //Had to do this because the button stuff is confusing
    public void StatUpValues()
    {
        //GameObject.Find("Level Done Object").transform.Find("Increase Stat").transform.Find("Weapons").transform.Find("Harp");
        //GameObject.Find("Level Done Object").transform.Find("Increase Stat").transform.Find("Weapons").transform.Find("Harp").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = harpTotal + "/"+harpTotal;
        //GameObject.Find("Level Done Object").transform.Find("Increase Stat").transform.Find("Weapons").transform.Find("Trumpet").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = trumpetTotal + "/" + trumpetTotal;
        GameObject.Find("HP Bar Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = HPTotal + "/" + HPTotal;
        GameObject.Find("Harp Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = harpTotal + "/" + harpTotal;
        GameObject.Find("Trumpet Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = trumpetTotal + "/" + trumpetTotal;
        GameObject.Find("Flute Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = fluteTotal + "/" + fluteTotal;
        GameObject.Find("Shield Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = shieldTotal + "/" + shieldTotal;
        GameObject.Find("Potion Stat Up").transform.Find("Numeric").GetComponent<TextMeshProUGUI>().text = potionTotal + "/" + potionTotal;

        if (HPTotal >= 32)
        {
            if (HPMaxed ==false) {
                GameObject.Find("HP Bar Stat Up").transform.Find("Stat Increase (2)").gameObject.SetActive(false);
                GameObject.Find("HP Button").gameObject.SetActive(false);
            }
            HPMaxed = true;
        }
        if (harpTotal >= 24)
        {
            if (harpMaxed == false)
            {
                GameObject.Find("Harp Stat Up").transform.Find("Numeric (1)").gameObject.SetActive(false);
                GameObject.Find("Harp Button").gameObject.SetActive(false);
            }
            harpMaxed = true;
        }
        if (trumpetTotal >= 14)
        {
            if (trumpetMaxed == false)
            {
                GameObject.Find("Trumpet Stat Up").transform.Find("Numeric (1)").gameObject.SetActive(false);
                GameObject.Find("Trumpet Button").gameObject.SetActive(false);
            }
            trumpetMaxed = true;
        }
        if (fluteTotal >= 7)
        {
            if (fluteMaxed == false)
            {
                GameObject.Find("Flute Stat Up").transform.Find("Numeric (1)").gameObject.SetActive(false);
                GameObject.Find("Flute Button").gameObject.SetActive(false);
            }
            fluteMaxed = true;
        }
        if (shieldTotal >= 16)
        {
            if (shieldMaxed == false)
            {
                GameObject.Find("Shield Stat Up").transform.Find("Numeric (1)").gameObject.SetActive(false);
                GameObject.Find("Shield Button").gameObject.SetActive(false);
            }
            shieldMaxed = true;
        }
        if (potionTotal >= 8)
        {
            if (potionMaxed == false)
            {
                GameObject.Find("Potion Stat Up").transform.Find("Numeric (1)").gameObject.SetActive(false);
                GameObject.Find("Potion Button").gameObject.SetActive(false);
            }
            potionMaxed = true;
        }
    }
    public void CantPauseMethod(float time)
    {
        StartCoroutine(CantPause(time));
    }
    IEnumerator CantPause(float time)
    {
        cantPause = true;
        yield return new WaitForSeconds(time);
        cantPause = false;
    }
    IEnumerator CantPauseDisplay()
    {
        cantRepeat = true;
        cantPauseObject.SetActive(true);
        yield return new WaitForSeconds(2);
        cantRepeat = false;
        cantPauseObject.SetActive(false);
    }
    public void OnMouseUp()
    {
        if(wind==true)
        {
            WindEnd();
        }
    }

}
