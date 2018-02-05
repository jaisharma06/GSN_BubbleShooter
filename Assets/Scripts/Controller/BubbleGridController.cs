using UnityEngine;
using System.Collections;
namespace BubbleShooter
{
    public class BubbleGridController : MonoBehaviour
    {
        #region View
        public float leftBorder = 0.0f;
        public float rightBorder = 10.5f;
        public float topBorder = 10.0f;

        public int rows = 10;
        public int columns = 10;
        public float bubbleRadius = 0.5f;
        public float addRowPeriod = 10.0f;

        public GameObject bubblePrefab;
        public GameObject _bubblesContainer;
        public GameObject _bubbleShooter;

        public BubbleGridGeometry geometry;
        #endregion

        private const float _bubbleLinearSpeed = 12.0f;
        private const int _defaultRowsCount = 5;

        private BubbleGridModel _grid;
        private BubbleController _currentBubble;
        private ArrayList _bubbleControllers;
        private bool _pendingToAddRow;
        private bool _isPlaying;

        void Awake()
        {
            _isPlaying = false;
            _pendingToAddRow = false;
            _grid = new BubbleGridModel(rows, columns);
            _bubbleControllers = new ArrayList();
        }

        void Start()
        {
        }

        public void startGame()
        {
            geometry = new BubbleGridGeometry(leftBorder, rightBorder, topBorder, 0.0f, rows, columns, bubbleRadius);
            _currentBubble = this.createBubble();
            _isPlaying = true;
            StartCoroutine(addRowScheduler());

            for (int i = 0; i < _defaultRowsCount; i++)
            {
                addRow();
            }

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && _isPlaying)
            {
                if (_currentBubble != null)
                {
                    _currentBubble.isMoving = true;
                    _currentBubble.angle = this.shootingRotation();
                    _currentBubble = null;
                }
            }
        }

        private BubbleController createBubble()
        {
            GameObject bubble = Instantiate(bubblePrefab);
            bubble.transform.parent = _bubblesContainer.transform;
            bubble.transform.position = new Vector3((rightBorder - leftBorder) / 2.0f - geometry.bubbleRadius / 2.0f, -0.65f, 0);
            BubbleController bubbleController = bubble.GetComponent<BubbleController>();
            bubbleController.leftBorder = geometry.leftBorder;
            bubbleController.rightBorder = geometry.rightBorder;
            bubbleController.topBorder = geometry.topBorder;
            bubbleController.radius = geometry.bubbleRadius;
            bubbleController.speed = _bubbleLinearSpeed;
            bubbleController.angle = 90.0f;
            bubbleController.isMoving = false;
            bubbleController.CollisionDelegate = onBubbleCollision;
            bubbleController.MotionDelegate = canMoveToPosition;
            _bubbleControllers.Add(bubbleController);
            return bubbleController;
        }

        private float shootingRotation()
        {
            float shooterRotation = _bubbleShooter.transform.eulerAngles.z;
            float ballRotation = 90;
            if (shooterRotation <= 360 && shooterRotation >= 270.0)
            {
                ballRotation = shooterRotation - 270;
            }
            if (shooterRotation <= 90 && shooterRotation >= 0)
            {
                ballRotation = 90 + shooterRotation;
            }
            return ballRotation;
        }

        private void destroyBubble(BubbleController bubbleController, bool explodes)
        {
            _grid.Remove(bubbleController.bubble);
            _bubbleControllers.Remove(bubbleController);
            bubbleController.CollisionDelegate = null;
            bubbleController.kill(explodes);
        }

        private BubbleController controllerForBubble(BubbleModel bubble)
        {
            foreach (BubbleController bubbleController in _bubbleControllers)
            {
                if (bubbleController.bubble == bubble)
                    return bubbleController;
            }
            return null;
        }

        IEnumerator addRowScheduler()
        {

            yield return new WaitForSeconds(addRowPeriod);
            _pendingToAddRow = true;

        }

        private void destroyCluster(ArrayList cluster, bool explodes)
        {
            foreach (BubbleModel bubble in cluster)
            {
                destroyBubble(controllerForBubble(bubble), explodes);
            }
        }


        private void addRow()
        {
            _pendingToAddRow = false;
            bool overflows = _grid.shiftOneRow();


            for (int i = 0; i < geometry.columns; i++)
            {
                BubbleController bubbleController = createBubble();
                bubbleController.isMoving = false;
                this._grid.Insert(bubbleController.bubble, 0, i);
            }

            foreach (BubbleController bubbleController in _bubbleControllers)
            {
                if (bubbleController != _currentBubble)
                {
                    Vector3 position = BubbleGridControllerHelper.PositionForCell(_grid.Location(bubbleController.bubble), geometry, _grid.isBaselineAlignedLeft);			
                    bubbleController.transform.position = position;
                }

            }

            if (overflows)
            {
                FinishGame(GameState.Loose);
                return;
            }
        }

        private void FinishGame(GameState state)
        {
            BubbleShooterController shooterController = _bubbleShooter.GetComponent<BubbleShooterController>();
            shooterController.isAiming = false;
            EventsManager.GameFinished(state);
            _isPlaying = false;
        }

        void onBubbleCollision(GameObject bubble)
        {
            Vector2 bubblePos = BubbleGridControllerHelper.CellForPosition(bubble.transform.position, this.geometry, this._grid.isBaselineAlignedLeft);
            if ((int)bubblePos.x >= geometry.rows)
            {
                FinishGame(GameState.Loose);
                return;
            }
            
            BubbleController bubbleController = bubble.GetComponent<BubbleController>();
            Vector2 gridPosition = BubbleGridControllerHelper.CellForPosition(bubble.transform.position, this.geometry, this._grid.isBaselineAlignedLeft);
            
            this._grid.Insert(bubbleController.bubble, (int)gridPosition.x, (int)gridPosition.y);

            if (!_pendingToAddRow)
            {
                bubbleController.moveTo(BubbleGridControllerHelper.PositionForCell(gridPosition, geometry, this._grid.isBaselineAlignedLeft), 0.1f);
            }
            else
            {
                bubbleController.transform.position = BubbleGridControllerHelper.PositionForCell(gridPosition, geometry, this._grid.isBaselineAlignedLeft);
            }

            ArrayList cluster = _grid.ColorCluster(bubbleController.bubble);

            if (cluster.Count > 2)
            {
                bubbleController.transform.position = BubbleGridControllerHelper.PositionForCell(gridPosition, geometry, this._grid.isBaselineAlignedLeft);
                this.destroyCluster(cluster, true);
                EventsManager.BubblesRemoved(cluster.Count, true);
            }

            cluster = _grid.LooseBubbles();
            this.destroyCluster(cluster, false);
            if (cluster.Count > 0)
                EventsManager.BubblesRemoved(cluster.Count, false);
            
            if (_pendingToAddRow)
            {
                this.addRow();
                StartCoroutine(addRowScheduler());
            }
            
            if (_grid.bubbles.Count == 0)
            {
                FinishGame(GameState.Win);
                return;
            }
            
            _currentBubble = createBubble();
        }

        bool canMoveToPosition(Vector3 position)
        {
            Vector2 location = BubbleGridControllerHelper.CellForPosition(position, geometry, _grid.isBaselineAlignedLeft);
            if ((int)location.x <= geometry.rows - 1)
            {
                return !_grid.HasBubble((int)location.x, (int)location.y);
            }
            return true;
        }
    }

}
