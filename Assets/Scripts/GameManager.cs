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
            ProgressLevel();
        }
    }
    public void GameOver()
    {
        gameOverText.SetActive(true);
        gameOverFilter.SetActive(true);
    }
    public void ProgressLevel()
    {
        //player.LevelUp();
        levelUpStatic = true;
        SceneManager.LoadScene(1);
    }
    public void LevelUpOff()
    {
        levelUpStatic = false;
    }
}
