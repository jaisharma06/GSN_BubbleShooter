using System.Collections;
namespace BubbleShooter
{
    public static class ArrayListExtension
    {
        public static void AddObjects(this ArrayList arrayList, object theObject)
        {
            if (theObject != null)
            {
                arrayList.Add(theObject);
            }
        }

        public static void Exclusive(this ArrayList arrayList, ArrayList sourceArrayList)
        {
            ArrayList exclusives = new ArrayList();
            foreach (object anObject in arrayList)
            {
                if (!sourceArrayList.Contains(anObject))
                {
                    exclusives.Add(anObject);
                }
            }
            arrayList.RemoveRange(0, arrayList.Count);
            arrayList.AddRange(exclusives);
        }

        public static ArrayList Distinct(this ArrayList arrayList)
        {
            ArrayList returnArray = new ArrayList();
            foreach (object someObject in arrayList)
            {
                if (!returnArray.Contains(someObject))
                {
                    returnArray.Add(someObject);
                }
            }
            return returnArray;
        }
    }

}
