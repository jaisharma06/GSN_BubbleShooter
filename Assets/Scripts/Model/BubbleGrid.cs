using UnityEngine;
using System.Collections;

namespace BubbleShooter{
	
	
	public class BubbleGridModel {
		public bool isBaselineAlignedLeft;
		
		private int _rows;
		private int _columns;
		private BubbleModel[,] _bubbleGrid;

		public BubbleGridModel(int rows, int columns){
			isBaselineAlignedLeft = true;
			_rows = rows;
			_columns = columns;
			_bubbleGrid = new BubbleModel[rows, columns];
		}

		public void Insert(BubbleModel bubble, int x, int y){
            if (x < 0 || x > _rows - 1 || y < 0 || y > _columns - 1)
                return;
			
			_bubbleGrid[x,y] = bubble;
		}
	
		public void Remove(int x, int y){
            if (x < 0 || x > _rows - 1 || y < 0 || y > _columns - 1)
                return;	

			_bubbleGrid[x,y] = null;
		}

		public void Remove(BubbleModel bubble){
			Vector2 location = Location(bubble);
			if ((int)location.x > -1 && (int)location.y > -1)
				Remove((int)location.x, (int)location.y);
		}
		
		public ArrayList bubbles{
			get{
				
				ArrayList bubbles = new ArrayList();
				for (int i=0; i < _rows; i++){
					for (int j=0; j< _columns; j++){
						if (_bubbleGrid[i,j] != null)
							bubbles.Add(_bubbleGrid[i,j]);
					}
				}
				return bubbles;
			}
		}
		
		public bool HasBubble(int row, int column){
			return Bubble(row, column) != null;
		}
		
		public BubbleModel Bubble(int row, int column){
			return _bubbleGrid[row, column];
		}

		public ArrayList Neighbours(Vector2 location){
			int row = (int)location.x;
			int column = (int)location.y;
			
			ArrayList _neigbours = new ArrayList();
            if (row < 0 || row > _rows - 1 || column < 0 || column > _columns - 1)
                return null;
			
			if (_bubbleGrid[row, column] != null){			
				_neigbours.AddNotNull(_bubbleGrid[row,column]);

				if (column > 0) _neigbours.AddNotNull(_bubbleGrid[row,column -1]);
				if (column < _columns-1) _neigbours.AddNotNull(_bubbleGrid[row,column+1]);
				
				bool isRowEven = row % 2 == 0;
				if ((isBaselineAlignedLeft && isRowEven) || (!isBaselineAlignedLeft && !isRowEven) ){
					if (row > 0){
							if (column > 0) _neigbours.AddNotNull(_bubbleGrid[row-1, column-1]);
							_neigbours.AddNotNull(_bubbleGrid[row-1, column]);
						}
						if (row < _rows -1){
							if (column > 0) _neigbours.AddNotNull(_bubbleGrid[row+1, column-1]);
							_neigbours.AddNotNull(_bubbleGrid[row +1, column]);
						}
				}
				else{
					if (row > 0){
						_neigbours.AddNotNull(_bubbleGrid[row-1, column]);
						if (column < _columns - 1) _neigbours.AddNotNull(_bubbleGrid[row-1, column+1]);
					}
					if (row < _rows - 1){
						_neigbours.AddNotNull(_bubbleGrid[row+1, column]);
						if (column < _columns - 1) _neigbours.AddNotNull(_bubbleGrid[row+1, column+1]);
					}
				}
				return _neigbours;
			}
			return null;
		}
		

		public Vector2 Location(BubbleModel bubble){
			for (int i = 0 ; i< _rows; i++)
			{
				for (int j = 0; j< _columns; j++){
					BubbleModel someBubble = _bubbleGrid[i,j];
					if (bubble == someBubble){
						return new Vector2(i, j);
					}
				}				
			}
			return new Vector2(-1,-1);
		}
		

		public ArrayList Neighbours(BubbleModel bubble){
			return Neighbours(Location(bubble));
		}
		
		public ArrayList ColorCluster(BubbleModel bubble){
			return ColorClusterRecursive(bubble, new ArrayList()).Distinct();
		}

		public ArrayList LooseBubbles(){
            ArrayList anchoredBubbles = AnchoredBubbles();
			ArrayList connectedBubbles = new ArrayList();
			
			foreach (BubbleModel anchoredBubble in anchoredBubbles){
				ArrayList connected = ConnectedBubbles(anchoredBubble);
				connectedBubbles.AddRange(connected);
				connectedBubbles = connectedBubbles.Distinct();
			}
			ArrayList theBubbles = bubbles;
			theBubbles.Exclusive(connectedBubbles);
			return theBubbles;
		}
		
		private ArrayList AnchoredBubbles(){
			ArrayList anchoredBubbles = new ArrayList();
			for (int j = 0; j < _columns; j++){
				if (_bubbleGrid[0,j] != null){
					anchoredBubbles.Add(_bubbleGrid[0,j]);
				}
			}
			return anchoredBubbles;
		}
			
		
		private ArrayList ConnectedBubbles(BubbleModel bubble){
			return ConnectedBubblesRecursive(bubble, new ArrayList(), isBaselineAlignedLeft);	
		}
		
		private ArrayList ConnectedBubblesRecursive(BubbleModel bubble, ArrayList visited, bool isBasedAlignedLeft){
			ArrayList neighboursNotVisited = Neighbours(bubble);
			neighboursNotVisited.Exclusive(visited);
			visited.Add(bubble);
			ArrayList returnArray = new ArrayList();
			returnArray.Add(bubble);
			foreach (BubbleModel someBubble in neighboursNotVisited){
				if (bubble != someBubble)
					returnArray.AddRange(ConnectedBubblesRecursive(someBubble, visited, isBasedAlignedLeft));
			}
			return returnArray;
		}
		
		private bool AnchorsToBaseline(ArrayList bubbles){
			foreach (BubbleModel bubble in bubbles){
				if (Location(bubble).x ==0)
					return true;
			}
			return false;
		}
		
		private ArrayList ColorClusterRecursive(BubbleModel bubble, ArrayList visited){		
			ArrayList similarColorNeighbours = Utils.FilterByColor(Neighbours(bubble), bubble.color);
			similarColorNeighbours.Exclusive(visited);
			visited.Add(bubble);
			ArrayList returnArray = new ArrayList();
			returnArray.Add(bubble);
			foreach (BubbleModel aBubble in similarColorNeighbours){
				if (bubble != aBubble)
					returnArray.AddRange(ColorClusterRecursive(aBubble, visited));
			}
			return returnArray;
		}

		public bool shiftOneRow()
		{
			bool overflows = false;
			for (int i = _rows -1; i >= 0; i--)
			{
				for (int j = 0; j < _columns; j++)
				{
					if (_bubbleGrid[i,j] != null)
					{
						if (i >= _rows -1)
						{
							overflows = true;
						}else					
						{
							_bubbleGrid[i+1,j] = _bubbleGrid[i,j];
							_bubbleGrid[i,j] = null;
						}
					}else{

					}	
				}
			}
			isBaselineAlignedLeft = !isBaselineAlignedLeft;
			return overflows;
		}
	}

}