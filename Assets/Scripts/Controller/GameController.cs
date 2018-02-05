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

        private BubbleGridController _bubbleGridController;
        void Awake()
        {
            _game = new GameModel();
        }

        void OnEnable()
        {
            EventsManager.OnBubblesRemoved += onBubblesRemoved;
            EventsManager.OnGameFinished += onGameFinished;
        }

        void OnDisable()
        {
            EventsManager.OnBubblesRemoved -= onBubblesRemoved;
            EventsManager.OnGameFinished -= onGameFinished;
        }

        public void startGame()
        {
            bubbleShooter.transform.position = new Vector3(0, 0, 0);
            if (playButton)
                playButton.SetActive(false);
            _bubbleGridController = bubbleShooter.GetComponent<BubbleGridController>();

            _bubbleGridController.startGame();
        }

        protected virtual void onBubblesRemoved(int bubbleCount, bool exploded)
        {
            _game.destroyBubbles(bubbleCount, exploded);
        }

        protected virtual void onGameFinished(GameState state)
        {
            restartButton.SetActive(true);
        }

        public void onGameStartSelected()
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
