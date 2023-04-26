using System.Collections;

public static class ListExtension
{
    public static void ShiftList(this IList list)
    {
        var currentObject = list[0];
        list.RemoveAt(0);
        list.Add(currentObject);
    }
}
