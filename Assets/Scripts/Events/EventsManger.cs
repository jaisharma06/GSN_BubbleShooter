using System;

namespace BubbleShooter{
	
public class EventsManager{
	
		public static Action<int, bool> OnBubblesRemoved;
        public static Action<GameState> OnGameFinished;

        public static void BubblesRemoved(int bubbleCount, bool exploded){
			if (OnBubblesRemoved != null){
				OnBubblesRemoved(bubbleCount, exploded);
			}
		}
		
		public static void GameFinished(GameState state){
			if (OnGameFinished != null){
				OnGameFinished(state);
			}
		}
		
	}
	
}
