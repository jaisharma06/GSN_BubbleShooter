using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter
{

    public class GameController : MonoBehaviour
    {

        protected Game _game;

        [SerializeField]
        private GameObject bubbleShooter;
        [SerializeField]
        private GameObject playButton;
        [SerializeField]
        private GameObject restartButton;

        private BubbleMatrixController _bubbleMatrixController;
        void Awake()
        {
            _game = new BubbleShooter.Game();
        }

        void Start()
        {

        }

        void OnEnable()
        {
            GameEvents.OnBubblesRemoved += onBubblesRemoved;
            GameEvents.OnGameFinished += onGameFinished;
        }

        void OnDisable()
        {
            GameEvents.OnBubblesRemoved -= onBubblesRemoved;
            GameEvents.OnGameFinished -= onGameFinished;
        }

        public void startGame()
        {
            bubbleShooter.transform.position = new Vector3(0, 0, 0);
            if (playButton)
                playButton.SetActive(false);
            _bubbleMatrixController = bubbleShooter.GetComponent<BubbleMatrixController>();

            _bubbleMatrixController.startGame();
        }
        // Game Controllers Specializations can override this function to provide
        // specific score behaviour
        protected virtual void onBubblesRemoved(int bubbleCount, bool exploded)
        {
            this._game.destroyBubbles(bubbleCount, exploded);
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
