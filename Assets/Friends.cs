using UnityEngine;
using UnityEngine.UI;

public class Friends : MonoBehaviour
{
    public Text indexing, playerNameText;
    public Button removeFriendBtn, join;
    public int id;
    public string code;

    private void OnEnable()
    {
        //if(DataManager.Instance.FriendListdata.friendlist.Contains(id))
    }

    private void Start()
    {
        if (removeFriendBtn != null)
        {
            removeFriendBtn.onClick.AddListener(() =>
            {
                MessageBox.Instance.Show("Are you sure, you want to remove " + playerNameText.text, MsgType.OKCANCELBOTH);
                MessageBox.Instance.setOkText("Yes");
                MessageBox.Instance.setCancelText("No");
                MessageBox.Instance.OnOkClicked += RemoveFriend;
            });
        }
        if (join != null)
        {
            join.onClick.AddListener(() =>
            {
                if (code == "")
                {
                    MessageBox.Instance.Show("Please enter private code ", MsgType.OKONLY);
                    return;
                }

                PhotonRoomManager.instance.JoinPrivateRoom(code);
            });
        }
    }

    private void RemoveFriend()
    {
        MessageBox.Instance.OnOkClicked -= RemoveFriend;
        FriendsManager.Instance.Unfriend(id);
        Destroy(gameObject);
    }
}