using System.Collections;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EGameState
{
    Ready,
    Run,
    Pause,
    Over
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PoolManager PoolManager;

    
    public GameObject GameStartScreen;
    public GameObject GoText;
    public GameObject GameOverScreen;
    private float _waitReadyTime = 3f;
    private float _showGoTime = 1f;
    private bool _isGameStarted = false;
    private bool _isGameOver;

    private EGameState _gameState = EGameState.Run;
    public EGameState GameState => _gameState;
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
        Time.timeScale = 0;
        
        yield return StartCoroutine(WaitForRealSeconds(_waitReadyTime));

        GameStartScreen.SetActive(false);
        GoText.SetActive(true);
        Time.timeScale = 1;
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

    public void Pause()
    {
        _gameState = EGameState.Pause;
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        
        PopupManager.Instance.Open(EPopupType.UI_OptionPopup, closeCallback: Continue);
    }
    
    public void Continue()
    {
        _gameState = EGameState.Run;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Restart()
    {
        _gameState = EGameState.Run;
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        GameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
