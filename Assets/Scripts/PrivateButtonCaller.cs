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
    public bool startGame = false;
    public bool levelSelect = false;
    public bool retry = false;
    public bool quit = false;
    public bool statIncrease = false;
    public bool nextLevel = false;
    public bool continueQuit = false;

    [Header("Increase")]
    public bool increaseHP = false;
    public bool increaseViolin = false;
    public bool increaseTrumpet = false;
    public bool increaseFlute = false;
    public bool increaseShield = false;
    public bool increasePotion = false;

    [Header("Level Select")]
    public bool level1 = false;
    public bool level2 = false;
    public bool level3 = false;

    // Start is called before the first frame update
    void Start()
    {
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(EvokeButton);

        if(GameObject.Find("Player") !=null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
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
            //gameScript.ReturnToTitle();
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

        if(level1==true)
        {
            gameScript.StartGame();
        }
        if (level2 == true)
        {
            SceneManager.LoadScene(3);
        }
        if (level3 == true)
        {
            SceneManager.LoadScene(4);
        }

        if (increaseHP == true)
        {
            player.HPUp();
        }
        if (increaseViolin == true)
        {
            player.ViolinUp();
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
