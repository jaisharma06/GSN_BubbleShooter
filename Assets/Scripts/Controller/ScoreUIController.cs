using BubbleShooter;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUIController : MonoBehaviour {
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameController gameController;

    private void Update()
    {
        scoreText.text = "Score: " + gameController._game.score;
    }
}
