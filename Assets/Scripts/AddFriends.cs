using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFriends : MonoBehaviour
{
    public InputField friendEmail;
    public Button searchBtn;
    public GameObject receivedFriendReqPrfab, friendsParent;
    public Text msgText;
    // Start is called before the first frame update
    void Start()
    {
        searchBtn.onClick.AddListener(() => {

            FriendsManager.Instance.CallFindFriend(friendEmail.text);
        });

        //FriendsManager.Instance.PendingFriendReq();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSearchedFriend()
    {
        MessageBox.Instance.Show("Found your friend!\n\n" + DataManager.Instance.FindFriendsdata.search.username, MsgType.OKCANCELBOTH);
        MessageBox.Instance.setOkText("Add Friend");
        MessageBox.Instance.OnOkClicked += OnAddFriend;
        // GameObject friendReq = Instantiate(friendReqPrfab, friendsParent.transform);
        //friendReq.GetComponent<FriendReq>().SearchFriendName.text = DataManager.Instance.FindFriendsdata.search.username;
        friendEmail.text = "";
    }

    void OnAddFriend()
    {
        MessageBox.Instance.OnOkClicked -= OnAddFriend;
        FriendsManager.Instance.CallUserFriendReq(DataManager.Instance.FindFriendsdata.search.id);
    }

    public void PendingFriendReqwUpdate(PendingfriendrequestData pendingFriendReqData)
    {
        ClearPendingReq();
        Debug.Log("PendingFriendReqwUpdate Calledd");
        for (int i = 0; i < pendingFriendReqData.pendingfriendrequest.Count; i++) 
        {
            GameObject PendingFriendReqObj = Instantiate(receivedFriendReqPrfab, friendsParent.transform);
            PendingFriendReqObj.GetComponent<ReceivedFriendReq>().s_No.text = (i + 1).ToString();
            PendingFriendReqObj.GetComponent<ReceivedFriendReq>().friendName.text = pendingFriendReqData.pendingfriendrequest[i].name;
            PendingFriendReqObj.GetComponent<ReceivedFriendReq>().id = pendingFriendReqData.pendingfriendrequest[i].id;
        }
    }

    private void ClearPendingReq()
    {
        
        foreach(Transform a in friendsParent.transform)
        {
            Debug.Log("****************************************************************");
            Destroy(a.gameObject);
        }
    }
}
