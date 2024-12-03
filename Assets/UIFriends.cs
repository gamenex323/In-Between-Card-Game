using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIFriends : MonoBehaviour
{

    public Toggle ToggleFriendOption, ToggleAddFriendOption;
    public GameObject FriendsScreen,AddFriendsScreen;
    private void OnEnable()
    {
        if (ToggleFriendOption.isOn)
        {
            FriendsManager.Instance.CallUserFriendList();
            FriendsScreen.SetActive(true);
            AddFriendsScreen.SetActive(false);

        }
        else
        {
            FriendsManager.Instance.PendingFriendReq();
            FriendsScreen.SetActive(false);
            AddFriendsScreen.SetActive(true);
        }
    }
    private void Start()
    {
        if (ToggleFriendOption.isOn)
        {
            FriendsManager.Instance.CallUserFriendList();
            FriendsScreen.SetActive(true);
            AddFriendsScreen.SetActive(false);
            
        }
        else
        {
            FriendsManager.Instance.PendingFriendReq();
            FriendsScreen.SetActive(false);
            AddFriendsScreen.SetActive(true);
        }



        ToggleFriendOption.onValueChanged.AddListener(delegate {
            Debug.Log("sdf");
            if (ToggleFriendOption.isOn)
            {
                Debug.Log("sdfinn");
                FriendsManager.Instance.CallUserFriendList();
                FriendsScreen.SetActive(true);
                AddFriendsScreen.SetActive(false);
               
            }
        }); 
        ToggleAddFriendOption.onValueChanged.AddListener(delegate {
            if (ToggleAddFriendOption.isOn)
            {
                FriendsManager.Instance.PendingFriendReq();
                FriendsScreen.SetActive(false);
                AddFriendsScreen.SetActive(true);
            }

        });
    }

   
}
