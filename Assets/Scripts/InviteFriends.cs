using UnityEngine;
using UnityEngine.UI;

public class InviteFriends : MonoBehaviour
{
    public GameObject FriendsScreen;
    public Text msgText;

    private void OnEnable()
    {
        FriendsManager.Instance.CallUserFriendList();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}