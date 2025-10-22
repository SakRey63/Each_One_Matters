using System;
using TMPro;
using UnityEngine;
using YG;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private int _pointsPerZombie;

    private int _currentScore;

    public int CurrentScore => _currentScore;

    public void SetInitialScore(int score)
    {
        _currentScore = score;
        _score.text = Convert.ToString(_currentScore);
        _level.text = Convert.ToString(YG2.saves.Level);
    }

    public void AddPointsForZombie()
    {
        _currentScore += _pointsPerZombie;
        _score.text = Convert.ToString(_currentScore);
    }

    public void DeductPointsForHelp(int price)
    {
        _currentScore -= price;
        _score.text = _currentScore.ToString();
    }
}
