using UnityEngine;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour
{
    public GameObject friends;
    public Text msgText;
    public bool isInvite = false;

    public void ShowFriendListData()
    {
        clearFriendList();
        Debug.Log("ShowFriendListData Called");
        if(Server.Instance.friendList.friendlist.Count==0)
        {

        }
        for (int i = 0; i < Server.Instance.friendList.friendlist.Count; i++)
        {
            GameObject Friends = Instantiate(friends, gameObject.transform);

            if (isInvite)
            {
                InviteFrinds frends = Friends.GetComponent<InviteFrinds>();
                frends.indexing.text = (i + 1).ToString();
                frends.playerNameText.text = Server.Instance.friendList.friendlist[i].name;
                frends.id = Server.Instance.friendList.friendlist[i].id;
            }
            else
            {
                Friends frends = Friends.GetComponent<Friends>();
                frends.indexing.text = (i + 1).ToString();
                frends.playerNameText.text = Server.Instance.friendList.friendlist[i].name;
                frends.id = Server.Instance.friendList.friendlist[i].id;
                Debug.Log("Server.Instance.friendList.friendlist[i].name = " + Server.Instance.friendList.friendlist[i].name);
                if (Server.Instance.friendList.friendlist[i].join_code != null && Server.Instance.friendList.friendlist[i].join_code.Trim() != "")
                {
                    frends.join.gameObject.SetActive(true);
                    frends.code = Server.Instance.friendList.friendlist[i].join_code;
                }
                else
                    frends.join.gameObject.SetActive(false);
            }
        }
    }

    public void clearFriendList()
    {
        foreach (Transform a in gameObject.transform)
        {
            Debug.Log("/////////////////////////////////////////////////////////");
            Destroy(a.gameObject);
        }
    }
}