using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerController player;
    private GameObject gameOverObject;
    private GameObject gameOverText;
    private GameObject gameOverFilter;
    public bool gameOver = false;

    private int numEnemies = 0;

    public static bool levelUpStatic = false;
    public bool levelUp = false;

    public static int currentLevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameOverObject = GameObject.Find("Game Over Object");
        gameOverText = gameOverObject.transform.Find("Game Over Text").gameObject;
        gameOverFilter = gameOverObject.transform.Find("Red Filter").gameObject;
        numEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (levelUpStatic ==true)
        {
            levelUp = true;
        }
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
            StartCoroutine(AfterBattle());
        }
    }
    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOverFilter.SetActive(true);
    }
    IEnumerator AfterBattle()
    {
        GameObject.Find("Level Done Object").transform.Find("Level Done").gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
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
}
