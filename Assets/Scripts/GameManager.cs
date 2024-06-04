using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerController player;
    private GameObject gameOverObject;
    private GameObject gameOverText;
    private GameObject gameOverFilter;
    private GameObject gameOverButtons;
    public bool gameOver = false;

    private int numEnemies = 0;

    public static bool levelUpStatic = false;
    public bool levelUp = false;

    public static int currentLevel = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        if (levelUpStatic == true)
        {
            levelUp = true;
        }
    }
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameOverObject = GameObject.Find("Game Over Object");
        gameOverText = gameOverObject.transform.Find("Game Over Text").gameObject;
        gameOverFilter = gameOverObject.transform.Find("Red Filter").gameObject;
        gameOverButtons = gameOverObject.transform.Find("Buttons").gameObject;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReduceNumEnemies()
    {
        numEnemies--;
        if (numEnemies <= 0)
        {
            EXP();
            player.WeaponReset();
        }
    }
    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOverFilter.SetActive(true);
        gameOverButtons.SetActive(true);
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
    }
    //I'm going to ask players if they want to progress or to go back to the menu
    public void ContinueOrQuit()
    {
        GameObject.Find("Level Done Object").transform.Find("Increase Stat").gameObject.SetActive(false);
        GameObject.Find("Level Done Object").transform.Find("Continue Or Quit").gameObject.SetActive(true);
    }
    public void Continue()
    {
        ProgressLevel();
    }
    public void ProgressLevel()
    {
        //player.LevelUp();
        levelUpStatic = true;
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
}
