using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivedFriendReq : MonoBehaviour
{
    public Button acceptBtn, rejectBtn;
    public Text friendName, s_No;
    public string request_type;
    public int id;
    // Start is called before the first frame update
    void Start()
    {
        acceptBtn.onClick.AddListener(() => {

            FriendsManager.Instance.AcceptOrRejectFriendReq(id,1);
            Destroy(gameObject);

        }); 
        
        rejectBtn.onClick.AddListener(() => {

            FriendsManager.Instance.AcceptOrRejectFriendReq(id,2);
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
