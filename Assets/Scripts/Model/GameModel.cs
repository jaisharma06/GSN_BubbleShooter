namespace BubbleShooter{

	public enum GameState {Playing, Loose, Win};
	public class GameModel {
		
		public const int PointsPerExplosion = 20;
		public const int PointsPerFall = 25;
		public GameState state;
		
		private int _points;
		private int _bubblesDestroyed;
		
		public GameModel(){
			_points = 0;
			_bubblesDestroyed = 0;
		}

		public int score{
			get{
				return _points;
			}
		}
		
		public int bubblesDestroyed{
			get{
				return _bubblesDestroyed;
			}
		}
		
		public void destroyBubbles(int bubbleCount, bool exploded){
			_points +=  exploded ? bubbleCount * PointsPerExplosion : bubbleCount * PointsPerFall;
			_bubblesDestroyed += bubbleCount;	
		}
	}
	
}
