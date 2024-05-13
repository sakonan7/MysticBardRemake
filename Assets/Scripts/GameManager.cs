using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject gameOverObject;
    private GameObject gameOverText;
    private GameObject gameOverFilter;
    public bool gameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        gameOverObject = GameObject.Find("Game Over Object");
        gameOverText = gameOverObject.transform.Find("Game Over Text").gameObject;
        gameOverFilter = gameOverObject.transform.Find("Red Filter").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
