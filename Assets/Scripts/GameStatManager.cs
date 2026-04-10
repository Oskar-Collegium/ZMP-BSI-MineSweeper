using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStatManager : MonoBehaviour
{
    [Header("Timer Variables")]
    [SerializeField] private int bonusTimeInSeconds;

    [Header("Score Variables")]
    [SerializeField] private int baseTileScore;
    [SerializeField] private int baseBonusScore;

    [Header("Reference Variables")]
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;

    private float currentTime = 0f;
    private int currentScore = 0;
    private bool isTimerRunning = false;

    // Singleton Variable
    public static GameStatManager instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    public void StartTimer()
    {
        if (isTimerRunning) return;
        isTimerRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    public void StopTimer()
    {
        if (!isTimerRunning) return;
        isTimerRunning = false;
        StopCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
            yield return null;
        }
    }

    public void ScoreTileReveal()
    {
        // add combos and calculate score combo here
        AddScore(baseTileScore);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreDisplay();
    }

    public void AddBonusScore()
    {
        int gameTimeInSeconds = Mathf.FloorToInt(currentTime);
        int bonusTimeLeft = bonusTimeInSeconds - gameTimeInSeconds;
        if (bonusTimeLeft <= 0) bonusTimeLeft = 0;
        int bonusScoreToAdd = baseBonusScore * bonusTimeLeft;
        currentScore += bonusScoreToAdd;
        UpdateScoreDisplay();
        Debug.Log("Bonus score +" + bonusScoreToAdd + " points. Total score: " + currentScore);
    }

    public void ResetStats()
    {
        currentScore = 0;
        currentTime = 0f;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateTimerDisplay();
        UpdateScoreDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        int milliseconds = Mathf.FloorToInt((currentTime * 100f) % 100f);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = currentScore.ToString();
    }

    public int GetCurrentScore() { return currentScore; }
}