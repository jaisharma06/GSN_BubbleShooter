using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter
{

    public class GameController : MonoBehaviour
    {
        [HideInInspector]
        public GameModel _game;

        [SerializeField]
        private GameObject bubbleShooter;
        [SerializeField]
        private GameObject playButton;
        [SerializeField]
        private GameObject restartButton;
        [SerializeField]
        private GameObject winText;

        private BubbleGridController _bubbleGridController;
        void Awake()
        {
            _game = new GameModel();
        }

        void OnEnable()
        {
            EventsManager.OnBubblesRemoved += OnBubblesRemoved;
            EventsManager.OnGameFinished += OnGameFinished;
        }

        void OnDisable()
        {
            EventsManager.OnBubblesRemoved -= OnBubblesRemoved;
            EventsManager.OnGameFinished -= OnGameFinished;
        }

        public void startGame()
        {
            bubbleShooter.transform.position = new Vector3(0, 0, 0);
            if (playButton)
                playButton.SetActive(false);
            _bubbleGridController = bubbleShooter.GetComponent<BubbleGridController>();

            _bubbleGridController.StartGame();
        }

        protected virtual void OnBubblesRemoved(int bubbleCount, bool exploded)
        {
            _game.destroyBubbles(bubbleCount, exploded);
        }

        protected virtual void OnGameFinished(GameState state)
        {
            if (state == GameState.Win)
            {
                winText.SetActive(true);
            }
            restartButton.SetActive(true);
        }

        public void OnGameStartSelected()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
