using UnityEngine;

public static class StringManager
{
    public static string RemoveCloneString(string input)
    {
        string newString = input.Replace("(Clone)", "").Trim();
        return newString;
    }
}