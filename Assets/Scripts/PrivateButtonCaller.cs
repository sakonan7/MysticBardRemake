using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrivateButtonCaller : MonoBehaviour
{
    private GameManager gameScript;
    private Button button;
    public bool retry = false;
    public bool quit = false;
    public bool statIncrease = false;
    public bool nextLevel = false;

    // Start is called before the first frame update
    void Start()
    {
        gameScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(EvokeButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EvokeButton()
    {
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
    }
}
