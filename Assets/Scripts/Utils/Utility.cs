using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

public class Utility
{
    private static readonly SortedDictionary<float, string> abbrevations = new SortedDictionary<float, string>
     {
         {1000,"K"},
         {1000000, "M" },
         {1000000000, "B" }
     };
    public static string AbbreviateNumber(float number)
    {
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<float, string> pair = abbrevations.ElementAt(i);
            if (Mathf.Abs(number) >= pair.Key)
            {
                float roundedNumber = (number / pair.Key);
                return roundedNumber.ToString(".00") + (pair.Value);
            }
        }
        return number.ToString();
    }

    public static bool IsValidEmailAddress(string s)
    {
        var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        return regex.IsMatch(s);
    }

    public static bool IsValidURL(string URL)
    {
        string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return Rgx.IsMatch(URL);
    }

    public static string RandomString(int size)
    {
        var desiredCodeLength = size;
        var code = "";
        //var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        var characters = ("0123456789").ToCharArray();
        while (code.Length < desiredCodeLength)
        {
            code += characters[UnityEngine.Random.Range(0, characters.Length)];
        }
        return code;
        //Debug.Log("Random code: " + code);
    }

    public static List<int> Shuffle<T>(List<int> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            int temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

    public static string IntListToString(List<int> _list)
    {
        string str = string.Join(", ", _list.Select(i => i.ToString()).ToArray());

        return str;
    }

    public static List<int> StringToIntList(string _str)
    {

        int[] nums = System.Array.ConvertAll(_str.Split(','), int.Parse);
        List<int> cd = nums.ToList<int>();
        return cd;

    }

    public static List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
    {
        List<int> possibleNumbers = new List<int>();
        List<int> chosenNumbers = new List<int>();

        for (int index = minValue; index < maxValue; index++)
            possibleNumbers.Add(index);

        while (chosenNumbers.Count < count)
        {
            int position = Random.Range(0, possibleNumbers.Count);
            chosenNumbers.Add(possibleNumbers[position]);
            possibleNumbers.RemoveAt(position);
        }
        return chosenNumbers;

    }


    public static string GetUniqueID()
    {
        string[] split = System.DateTime.Now.TimeOfDay.ToString().Split(new System.Char[] { ':', '.' });
        string id = "";
        for (int i = 0; i < split.Length; i++)
        {
            id += split[i];
        }
        return id;
    }



}
public enum NavType
{
    NEXT,
    BACK
}
public enum UserType
{
    COMPANY,
    RETAIL
}
public enum ApiRequestType
{
    NONE,
    REGISTER,
    LOGIN,
    LOGOUT,
    FORGOT,
    MATCH_CODE,
    RESET_PASSWORD,
    GET_USER_DETAILS,
    PAYMENT_REQUEST,
    UPDATE_RECENT_TRANSACION,

};

public enum MsgType
{
    NONE,
    OKONLY,
    OKCANCELBOTH,
    YESNOBOTH,
    ALERT
}

public enum ResultType
{
    WIN, LOSE
}

public enum RoomState
{
    WAITING,
    PLAYING,
    GAME_OVER
}



