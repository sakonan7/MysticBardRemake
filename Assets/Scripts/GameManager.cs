using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerController player;
    private AudioSource audio;
    public bool playEffects = false;
    public GameObject background;
    public AudioClip turnPageSound;
    public AudioClip prelude;
    public AudioClip story;
    public AudioClip regularBattle;
    public AudioClip regularBattle2;
    public AudioClip regularBattle3;
    public AudioClip bossMusic;
    public AudioClip victoryMusic;
    private GameObject gameOverObject;
    private GameObject gameOverText;
    private GameObject gameOverFilter;
    private GameObject gameOverButtons;
    public GameObject[] debris;
    private int numDebris = 4;
    private Vector3[] debrisLocation =new Vector3[4];
    public bool gameOver = false;

    private int numEnemies = 0;

    public static bool levelUpStatic = false;
    public bool levelUp = false;

    public static int level1 = 2;
    public static int currentLevel = 2;

    public bool victory = false;
    public bool boss = false;

    //06/05/24
    //This should be okay
    //Also, because I am not using bools, but using scene names
    public static bool nonGame = false;
    public bool nonGameNonStatic = false;
    public static bool levelSelect = false;
    public bool levelSelectNonStatic = false;
    // Start is called before the first frame update
    private void Awake()
    {
        audio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "Cover" ||SceneManager.GetActiveScene().name == "Instructions" || SceneManager.GetActiveScene().name == "Credits")
        {
            nonGame = true;
            nonGameNonStatic = true;
        }
        else
        {
            nonGame = false;
            nonGameNonStatic = false;
        }
        if (SceneManager.GetActiveScene().name=="Level Select")
        {
            levelSelect = true;
            levelSelectNonStatic = true;
        }
        else
        {
            levelSelect = false;
            levelSelectNonStatic = false;
        }
        if (levelUpStatic == true)
        {
            levelUp = true;
        }

        SetLevel(SceneManager.GetActiveScene().buildIndex);
        if (nonGame == false)
        {
            if (currentLevel >= 3 && boss == false)
            {
                int random = 0;
                random = Random.Range(0, 2);
                if (random == 0)
                {
                    audio.clip = regularBattle;
                    audio.Play();
                }
                else
                {
                    audio.clip = regularBattle2;
                    audio.Play();
                }
            }
            if (SceneManager.GetActiveScene().name == "Level 9")
            {
                    audio.clip = regularBattle3;
                    audio.Play();
            }
            if (SceneManager.GetActiveScene().name == "Level 10" || SceneManager.GetActiveScene().name == "Level 11")
            {
                int random = 0;
                random = Random.Range(0, 3);
                if (random == 0)
                {
                    audio.clip = regularBattle;
                    audio.Play();
                }
                else if (random == 1)
                {
                    audio.clip = regularBattle2;
                    audio.Play();
                }
                else
                {
                    audio.clip = regularBattle3;
                    audio.Play();
                }
            }
        }
        if (SceneManager.GetActiveScene().name == "Instructions")
        {
            int random = 0;
            random = Random.Range(0, 2);
            if (random == 0)
            {
                audio.clip = regularBattle;
                audio.Play();
            }
            else
            {
                audio.clip = regularBattle2;
                audio.Play();
            }
        }
        if (SceneManager.GetActiveScene().name == "Cover")
        {
            audio.clip = prelude;
            audio.Play();
        }
        if (SceneManager.GetActiveScene().name=="Dragon")
        {
            boss = true;
            audio.clip = bossMusic;
            audio.Play();
        }
    }
    void Start()
    {
        
        //Use bool nonGame
        if (nonGame ==false) {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            gameOverObject = GameObject.Find("Game Over Object");
            gameOverText = gameOverObject.transform.Find("Game Over Text").gameObject;
            gameOverFilter = gameOverObject.transform.Find("Red Filter").gameObject;
            gameOverButtons = gameOverObject.transform.Find("Buttons").gameObject;
            numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        GameObject[] currentDebris = GameObject.FindGameObjectsWithTag("Debris");
        //debrisLocation = GameObject.FindGameObjectsWithTag("Debris").transform.position;
        for(int i =0;i < currentDebris.Length;i++)
        {
            debrisLocation[i] = currentDebris[i].transform.position;
        }

        //HPZero();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void TitleScreen()
    {
        SceneManager.LoadScene(level1);
    }
    public void StartGame()
    {
        //SceneManager.LoadScene(level1);
        GameObject.Find("Cover").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(true);
        GameObject.Find("Orange Background").gameObject.SetActive(false);
        GameObject.Find("Start Menu Canvas").transform.Find("Forest Background").gameObject.SetActive(true);
        audio.clip = story;
        audio.Play();
    }
    public void Instructions()
    {
        SceneManager.LoadScene("Instructions");
    }
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void StoryPage1()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(false);
    }
    public void StoryPage2()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Page 3").gameObject.SetActive(false);
    }
    public void StoryPage3()
    {
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 3").gameObject.SetActive(true);
    }
    public void Page1()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(false);
    }
    public void Page2()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Page 3").gameObject.SetActive(false);
    }
    public void Page3()
    {
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 3").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Harp Page").gameObject.SetActive(false);
    }
    public void HarpPage()
    {
        GameObject.Find("Story").transform.Find("Page 3").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Harp Page").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Trumpet Page").gameObject.SetActive(false);
    }
    public void TrumpetPage()
    {
        GameObject.Find("Story").transform.Find("Harp Page").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Trumpet Page").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Flute Page").gameObject.SetActive(false);
    }
    public void FlutePage()
    {
        GameObject.Find("Story").transform.Find("Trumpet Page").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Flute Page").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Shield Page").gameObject.SetActive(false);
    }
    public void ShieldPage()
    {
        GameObject.Find("Story").transform.Find("Flute Page").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Shield Page").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Special Page").gameObject.SetActive(false);
    }
    public void SpecialPage()
    {
        GameObject.Find("Story").transform.Find("Shield Page").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Special Page").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Flinching Foes").gameObject.SetActive(false);
    }
    public void FlinchingPage()
    {
        GameObject.Find("Story").transform.Find("Special Page").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Flinching Foes").gameObject.SetActive(true);
        //GameObject.Find("Story").transform.Find("Flinching Page").gameObject.SetActive(false);
    }
    public void CreditsPage1()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(true);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(false);
    }
    public void CreditsPage2()
    {
        GameObject.Find("Story").transform.Find("Page 1").gameObject.SetActive(false);
        GameObject.Find("Story").transform.Find("Page 2").gameObject.SetActive(true);
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }
    public void ReduceNumEnemies()
    {
        numEnemies--;
        if (numEnemies <=0)
        {
            if (boss ==false) {
                if (player.noEXPNonStatic ==false)
                {
                    if (player.levelNonStatic < 11)
                    {
                        EXP();
                    }
                    //This code didn't account for what would happen if noEXP ==false, but the player has maxed out their lev
                    //No, I think the real problem is if level >=11
                    else
                    {
                        GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
                        if (SceneManager.GetActiveScene().name == "Level 11")
                        {
                            GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("Next Level Button").gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
                    if(SceneManager.GetActiveScene().name=="Level 11")
                    {
                        GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("Next Level Button").gameObject.SetActive(false);
                    }
                }
                player.WeaponReset();
                PlayVictoryMusic();
            }
            else
            {
                StartCoroutine(DefeatBoss());
            }
            victory = true;
        }
    }
    public void PlayVictoryMusic()
    {
        audio.clip = victoryMusic;
        audio.Play();
    }
    public void PlayTurnPage()
    {
        audio.PlayOneShot(turnPageSound,1);
    }
    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOverFilter.SetActive(true);
        gameOverButtons.SetActive(true);
        gameOver = true;
    }
    IEnumerator DefeatBoss()
    {
        yield return new WaitForSeconds(6);
        if (player.noEXPNonStatic == false)
        {
            if (player.levelNonStatic < 11)
            {
                EXP();
            }
            else
            {
                GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
            }
        }
        else
        {
            GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
        }
        player.WeaponReset();
        PlayVictoryMusic();
    }
    public void EXP()
    {
        GameObject.Find("Level Done Object").transform.Find("EXP").gameObject.SetActive(true);
        //yield return new WaitForSeconds(2);
        //ProgressLevel();
        player.StartEXPUp();
    }
    public void StatIncrease()
    {
        GameObject.Find("Level Done Object").transform.Find("Increase Stat").gameObject.SetActive(true);
        //yield return new WaitForSeconds(2);
        //ProgressLevel();
        player.StatUpValues();
    }
    //I'm going to ask players if they want to progress or to go back to the menu
    public void ContinueOrQuit()
    {
        if (player.levelUpStock > 0)
        {
            StartCoroutine(BlankPage());
        }
        else
        {
            GameObject.Find("Level Done Object").transform.Find("Increase Stat").gameObject.SetActive(false);
            GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
            if (SceneManager.GetActiveScene().name == "Level 11")
            {
                GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").transform.Find("Level Up Object").transform.Find("Buttons").transform.Find("Next Level Button").gameObject.SetActive(false);
            }
        }

    }
    IEnumerator BlankPage()
    {
        GameObject.Find("Level Done Object").transform.Find("Increase Stat").gameObject.SetActive(false);
        GameObject.Find("Level Done Object").transform.Find("Blank Page").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        StatIncrease();
        GameObject.Find("Level Done Object").transform.Find("Blank Page").gameObject.SetActive(false);
    }
    public void Continue()
    {
        ProgressLevel();
    }
    public void ProgressLevel()
    {
        //player.LevelUp();
        levelUpStatic = true;
        //This is going to need to be reset if I go back to level select
        //!Just look at the scene number and add on 
        //Now it's less gimmicky
        currentLevel++;
        SceneManager.LoadScene(currentLevel);
    }
    public void LevelUpOff()
    {
        levelUpStatic = false;
    }
    public void RestartLevel()
    {
        player.FullRestore();
        SceneManager.LoadScene(currentLevel);
    }
    public void SetLevel(int level)
    {
        currentLevel = level;
    }
    public void DestroyDebris()
    {
        GameObject[] debris = GameObject.FindGameObjectsWithTag("Debris"); 
        for (int i = 0; i< debris.Length; i++)
        {
            if (debris[i].GetComponent<Debris>().windCaptured == false)
            {
                Destroy(debris[i]);
            }
        }
    }
    public void ReduceNumDebris()
    {
        numDebris--;
        if(numDebris <=0)
        {
            StartCoroutine(StartGenerateDebris());
        }
    }
    IEnumerator StartGenerateDebris()
    {
        yield return new WaitForSeconds(5);
        for (int i =0; i<debrisLocation.Length;i++)
        {
            GameObject currentDebris = debris[Random.Range(0, 4)];
            Quaternion currentRotation = currentDebris.transform.rotation;
            Instantiate(currentDebris, debrisLocation[i], currentRotation);
        }
        numDebris = 4;
    }
    public void HPZero()
    {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                    enemies[i].GetComponent<Enemy>().SetHP(1);
            }
    }
}
