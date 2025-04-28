using System.Collections;
using Redcode.Pools;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PoolManager PoolManager;

    
    public GameObject GameStartScreen;
    public GameObject GoText;
    public GameObject GameOverScreen;
    private float _waitReadyTime = 3f;
    private float _showGoTime = 1f;
    private bool _isGameStarted;
    private bool _isGameOver;
    
    public void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        StartCoroutine(GameStartRoutine());
        GameOverScreen.SetActive(false);
    }

    private IEnumerator GameStartRoutine()
    {
        GameStartScreen.SetActive(true);
        GoText.SetActive(false);
        Time.timeScale = 0f;

        yield return StartCoroutine(WaitForRealSeconds(_waitReadyTime));

        GameStartScreen.SetActive(false);
        GoText.SetActive(true);
        Time.timeScale = 1f;
        _isGameStarted = true;
        yield return StartCoroutine(WaitForRealSeconds(_showGoTime));
        
        GoText.SetActive(false);
    }

    private IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + seconds)
        {
            yield return null;
        }
    }
    
    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        GameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
