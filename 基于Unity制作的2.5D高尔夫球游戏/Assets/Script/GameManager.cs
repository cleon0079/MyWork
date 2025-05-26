using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] spawn;
    [SerializeField] GameObject[] ballSpawn;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject UI;
    private Player player;
    private UIManager uIManager;
    private int level = 1;
    private int par = 0;
    private int finalPar = 0;

    // Start is called before the first frame update
    void Start()
    {
        par = 0;
        player = FindObjectOfType<Player>();
        uIManager = FindObjectOfType<UIManager>();
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartButton()
    {
        Time.timeScale = 1;
        player.transform.position = spawn[level - 1].transform.position;
        ball.transform.position = ballSpawn[level - 1].transform.position;
        player.Reset();
        finalPar += par;
        par = 0;
    }

    public int GetLevel()
    {
        return level;
    }

    public void Goal()
    {
        player.Goal();
        uIManager.NextLevel();
        level++;
        UI.SetActive(true);
    }

    public int ParGet()
    {
        return par;
    }
    public void ParAdd()
    {
        par++;
    }
}