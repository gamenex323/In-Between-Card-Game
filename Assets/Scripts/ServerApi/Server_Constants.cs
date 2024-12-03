using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server_Constants
{

    //public const string serverApi_BaseUrl = "https://3dedittool.24livehost.com/tea_project/v1";
    //public const string serverApi_BaseUrl = "https://trinksapp.org/v1";
    //public const string serverApi_BaseUrl = "https://3dedittool.24livehost.com/chess_game/v1";
    
    //public const string serverApi_BaseUrl = "https://inbetweengame.24livehost.com/public/v1";
    public const string serverApi_BaseUrl = "https://royalvgaming.com/v1";

    //we use this identifier to register the client as a guest when he first opens the application.
    protected static string uniqueIdentifier;
    public static string GetUniqueIdentifier
    {
        get
        {
            if (uniqueIdentifier == null)
                uniqueIdentifier = PlayerPrefs.HasKey("uniqueIdentifier") ? PlayerPrefs.GetString("uniqueIdentifier") : GetUniqueID();

            return uniqueIdentifier;
        }
    }
    private static string GetUniqueID()
    {
        string key = "ID";

        var random = new System.Random();
        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

        string uniqueID = Application.systemLanguage                            //Language
                + "-" + Application.platform                                            //Device    
                + "-" + String.Format("{0:X}", Convert.ToInt32(timestamp))                //Time
                + "-" + String.Format("{0:X}", Convert.ToInt32(Time.time * 1000000))        //Time in game
                + "-" + String.Format("{0:X}", random.Next(1000000000));                //random number

        if (PlayerPrefs.HasKey(key))
        {
            uniqueID = PlayerPrefs.GetString(key);
        }
        else
        {
            PlayerPrefs.SetString(key, uniqueID);
            PlayerPrefs.Save();
        }

        return uniqueID;
    }
}