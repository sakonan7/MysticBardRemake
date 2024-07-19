using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrivateButtonCaller : MonoBehaviour
{
    private GameManager gameScript;
    private Button button;
    private PlayerController player;
    private DifficultyModerator difficulty;
    private GameObject story;
    public bool title = false;
    public bool startGame = false;
    public bool levelSelect = false;
    public bool instructions = false;
    public bool credits = false;
    public bool retry = false;
    public bool quit = false;
    public bool statIncrease = false;
    public bool nextLevel = false;
    public bool continueQuit = false;
    public bool turnPageSound = false;

    [Header("Difficulty")]
    public bool normal = false;
    public bool hard = false;
    public bool noEXP = false;
    [Header("Story")]
    public bool storyPage1 = false;
    public bool storyPage2 = false;
    public bool storyPage3 = false;
    [Header("Instructions")]
    public bool page1 = false;
    public bool page2 = false;
    public bool page3 = false;
    public bool harpPage = false;
    public bool trumpetPage = false;
    public bool flutePage = false;
    public bool shieldPage = false;
    public bool specialPage = false;
    public bool interruptingPage = false;
    [Header("Instructions")]
    public bool creditsPage1 = false;
    public bool creditsPage2 = false;
    public bool creditsPage3 = false;

    [Header("Increase")]
    public bool statUpSound = false;
    public bool increaseHP = false;
    public bool increaseViolin = false;
    public bool increaseTrumpet = false;
    public bool increaseFlute = false;
    public bool increaseShield = false;
    public bool increasePotion = false;

    [Header("Level Select")]
    public bool levelSelectSound = false;
    public bool level1 = false;
    public bool level2 = false;
    public bool level3 = false;
    public bool level4 = false;
    public bool level5 = false;
    public bool level6 = false;
    public bool level7 = false;
    public bool level8 = false;
    public bool level9 = false;

    // Start is called before the first frame update
    void Start()
    {
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(EvokeButton);
        if (gameScript.nonGameNonStatic == false)
        {
            difficulty = GameObject.Find("Difficulty Moderator").GetComponent<DifficultyModerator>();
        }

        if(GameObject.Find("Player") !=null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        if (SceneManager.GetActiveScene().name =="Instructions")
        {
            story = GameObject.Find("Story");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EvokeButton()
    {
        if (startGame == true)
        {
            gameScript.StartGame();
        }
        if (instructions == true)
        {
            gameScript.Instructions();
        }
        if (credits == true)
        {
            gameScript.Credits();
        }
        if (storyPage1 == true)
        {
            gameScript.StoryPage1();
        }
        if (storyPage2 == true)
        {
            gameScript.StoryPage2();
        }
        if (storyPage3 == true)
        {
            gameScript.StoryPage3();
        }
        if (turnPageSound == true)
        {
            gameScript.PlayTurnPage();
            //Debug.Log("PlaySound");
        }
        //Instructions
        if (page1 == true)
        {
            gameScript.Page1();
        }
        if (page2 == true)
        {
            gameScript.Page2();
        }
        if (page3 == true)
        {
            gameScript.Page3();
        }
        if (harpPage == true)
        {
            gameScript.HarpPage();
        }
        if (trumpetPage == true)
        {
            gameScript.TrumpetPage();
        }
        if (flutePage == true)
        {
            gameScript.FlutePage();
        }
        if (shieldPage == true)
        {
            gameScript.ShieldPage();
        }
        if (specialPage == true)
        {
            gameScript.SpecialPage();
        }
        if (interruptingPage == true)
        {
            gameScript.FlinchingPage();
        }

        if (creditsPage1 == true)
        {
            gameScript.CreditsPage1();
        }
        if (creditsPage2 == true)
        {
            gameScript.CreditsPage2();
        }

        if (levelSelect == true)
        {
            gameScript.LevelSelect();
        }

        if (retry ==true)
        {
            gameScript.RestartLevel();
        }
        if (quit == true)
        {
            gameScript.LevelSelect();
        }
        if (statIncrease == true)
        {
            gameScript.StatIncrease();
        }
        if (nextLevel == true)
        {
            gameScript.Continue();
        }
        if (continueQuit == true)
        {
            gameScript.ContinueOrQuit();
        }

        if (normal == true)
        {
            difficulty.SetNormal();
        }
        if (hard == true)
        {
            difficulty.SetHard();
        }
        if (noEXP == true)
        {
            player.NoEXP();
            //Sound effect that tells you if there is or is no EXP
            //Or change button text
        }

        if (title == true)
        {
            SceneManager.LoadScene(0);
        }
        //Story


        if (levelSelectSound ==true)
        {
            player.LevelSelectSound();
        }
        if (level1==true)
        {
            SceneManager.LoadScene(2);
        }
        if (level2 == true)
        {
            SceneManager.LoadScene(3);
        }
        if (level3 == true)
        {
            SceneManager.LoadScene(4);
        }
        if (level4 == true)
        {
            SceneManager.LoadScene(5);
        }
        if (level5 == true)
        {
            SceneManager.LoadScene(6);
        }
        if (level6 == true)
        {
            SceneManager.LoadScene(7);
        }
        if (level7 == true)
        {
            SceneManager.LoadScene(8);
        }
        if (level8 == true)
        {
            SceneManager.LoadScene(9);
        }
        if (level9 == true)
        {
            SceneManager.LoadScene(10);
        }

        if (statUpSound == true)
        {
            player.StatUpSound();
            //Debug.Log("PlaySound");
        }
        if (increaseHP == true)
        {
            player.HPUp();
        }
        if (increaseViolin == true)
        {
            player.HarpUp();
        }
        if (increaseTrumpet == true)
        {
            player.TrumpetUp();
        }
        if (increaseFlute == true)
        {
            player.FluteUp();
        }
        if (increaseShield == true)
        {
            player.ShieldUp();
        }
        if (increasePotion == true)
        {
            player.PotionUp();
        }
    }
}
