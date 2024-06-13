using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
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

    public static int level1 = 2;
    public static int currentLevel = 2;

    public bool victory = false;

    //06/05/24
    //This should be okay
    //Also, because I am not using bools, but using scene names
    public static bool title = false;
    public bool titleNonStatic = false;
    public static bool levelSelect = false;
    public bool levelSelectNonStatic = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Cover")
        {
            title = true;
            titleNonStatic = true;
        }
        else
        {
            title = false;
            titleNonStatic = false;
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
    }
    void Start()
    {
        
        if (title ==false) {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            gameOverObject = GameObject.Find("Game Over Object");
            gameOverText = gameOverObject.transform.Find("Game Over Text").gameObject;
            gameOverFilter = gameOverObject.transform.Find("Red Filter").gameObject;
            gameOverButtons = gameOverObject.transform.Find("Buttons").gameObject;
            numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(level1);
    }
    public void LevelSelect()
    {
        SceneManager.LoadScene("Level Select");
    }
    public void ReduceNumEnemies()
    {
        numEnemies--;
        if (numEnemies <= 0)
        {
            EXP();
            player.WeaponReset();
            victory = true;
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
    IEnumerator StartGenerateDebris()
    {
        yield return new WaitForSeconds(5);
    }
    public void GenerateDebris ()
    {
        //Randomize them
    }
}
