namespace BubbleShooter{
	public enum BubbleColor {Red, Blue, Yellow, Green, Black, White, Pink};
	public class BubbleModel {
		
		private BubbleColor _color;
		
		public BubbleModel(BubbleColor color){
			this._color = color;
			
		}
		
		public BubbleColor color{
			get{
				return _color;
			}
			set {
				_color = value;
			}
		}
		
	}
}
