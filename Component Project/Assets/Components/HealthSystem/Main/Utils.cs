using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public static class Utils
{
    public static string SplitCapitals(this string s) => Regex.Replace(s, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").Trim();
    public static string[] SplitCapitalsArray(this string[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = s[i].SplitCapitals();
        }

        return s;
    }
}
