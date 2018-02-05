using UnityEngine;
using System.Collections;

namespace BubbleShooter
{

    public class Utils
    {

        public static T GetRandomEnum<T>()
        {
            System.Array A = System.Enum.GetValues(typeof(T));
            T V = (T)A.GetValue(Random.Range(0, A.Length));
            return V;
        }

        public static Color BubbleColorToColor(BubbleColor color)
        {
            switch (color)
            {
                case BubbleColor.Black:
                    return Color.black;
                case BubbleColor.Blue:
                    return Color.blue;
                case BubbleColor.Green:
                    return Color.green;
                case BubbleColor.Red:
                    return Color.red;
                case BubbleColor.White:
                    return Color.white;
                case BubbleColor.Yellow:
                    return Color.yellow;
                case BubbleColor.Pink:
                    return Color.magenta;
                default:
                    return Color.clear;
            }
        }

        public static ArrayList FilterByColor(ArrayList bubbles, BubbleColor color)
        {
            ArrayList filtered = new ArrayList();
            foreach (BubbleModel bubble in bubbles)
            {
                if (bubble.color == color)
                    filtered.Add(bubble);
            }
            return filtered;
        }

        public static int TruncateToInterval(int number, int min, int max)
        {
            int outcome;
            outcome = number;
            if (number < min) outcome = min;
            if (number > max) outcome = max;
            return outcome;
        }
    }
}
