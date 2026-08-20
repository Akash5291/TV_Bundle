using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("User interface")]
    [SerializeField] TMP_Text T_score;
    [SerializeField] TMP_Text T_scoreGameoverPanel;
    [SerializeField] TMP_Text T_highscore;
    [SerializeField] TMP_Text T_coin;

    [Header ("values")]
    [SerializeField] int scoreCount;
    [SerializeField] int coinCount;
    [SerializeField] float scoreLerpValue;
    [Space]
    [SerializeField] int highscoreCount;
    [SerializeField] float accelerationValue;

    [Header("Tags")]
    [SerializeField] string highscore;

    private void Awake()
    {
        highscoreCount = PlayerPrefs.GetInt(highscore, 0);
        T_highscore.text = ($"HIGHSCORE \n {highscoreCount}");
    }

    private void OnEnable()
    {
        PlayerHitDetection.CoinCollected += AddScore;
    }

    private void OnDisable()
    {
        PlayerHitDetection.CoinCollected -= AddScore;
    }

    private void Update()
    {
        scoreLerpValue = Mathf.MoveTowards(scoreLerpValue, scoreCount, accelerationValue * Time.deltaTime);
        T_score.text = ($"SCORE {scoreLerpValue.ToString("0")}");

    }

    void AddScore()
    {
        scoreCount += 5;
        coinCount += 1;
        T_coin.text = ($"X{coinCount}");
        T_scoreGameoverPanel.text = ($"SCORE {scoreCount}");
        PlayerPrefs.SetInt("score", scoreCount);
        if (scoreCount > highscoreCount)
        {
            PlayerPrefs.SetInt(highscore, scoreCount);

            highscoreCount = PlayerPrefs.GetInt(highscore, 0);
            T_highscore.text = ($"HIGHSCORE \n {highscoreCount}");
        }
    }
}
