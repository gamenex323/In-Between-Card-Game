using System.Collections.Generic;
using UnityEngine;

public class DataManager : ExtendedBehavior
{
    private UserType currentUserType;

    [SerializeField]
    private LoginDataRoot loginData;

    [SerializeField]
    private WalletRoot walletData;

    [SerializeField]
    private PaymentCheckRoot paymentcheckData;

    [SerializeField]
    private CreateTableRoot createTableData;

    [SerializeField]
    private PlayBetRoot playBetData;

    [SerializeField]
    private DealResultRoot dealResultData;

    [SerializeField]
    private TableSettingRoot tableSetting;

    [SerializeField]
    private UserDetailsDataRoot userDetailData;

    [SerializeField]
    private FindFriends findFriendsdata;

    [SerializeField]
    private List<GameObject> lockObj;

    private ProfileRoot profileRoot;
    [SerializeField] private Sprite userProfileImage;
    private static DataManager _Instance;
    public static DataManager Instance
    { get { return _Instance; } }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
    }

    private void Start()
    {
        // UserDetailsData = new UserDetailsDataRoot();
    }

    //***********************************************************************************************************
    public UserType UserType
    { get { return currentUserType; } set { currentUserType = value; } }

    public LoginDataRoot LoginData
    { get { return loginData; } set { loginData = new LoginDataRoot(); loginData = value; } }
    public ProfileRoot profileImageData
    { get { return profileRoot; } set { profileRoot = new ProfileRoot(); profileRoot = value; } }

    public UserDetailsDataRoot UserDetailsData
    {
        get { return userDetailData; }
        set
        {
            userDetailData = new UserDetailsDataRoot(); userDetailData = value;
            if (userDetailData.userdata.profile_avatar.Trim() != "")
            {
                LoadUserThumbnail(userDetailData.userdata.profile_avatar);
                Debug.Log(userDetailData.userdata.profile_avatar);
            }
            else
            {
                userProfileImage = null;
            }
        }
    }

    public FindFriends FindFriendsdata
    { get { return findFriendsdata; } set { findFriendsdata = new FindFriends(); findFriendsdata = value; } }
    public FriendListData FriendListdata
    { get { return FriendListdata; } set { FriendListdata = new FriendListData(); FriendListdata = value; } }

    private string _emailId;
    public string EmailId
    { get { return _emailId; } set { _emailId = value; } }

    private string _authkey;
    internal DealResultRoot DealResult
    { get { return dealResultData; } set { dealResultData = new DealResultRoot(); dealResultData = value; } }

    public TableSettingRoot TableSettingData
    { get { return tableSetting; } set { tableSetting = new TableSettingRoot(); tableSetting = value; } }

    //public CreateTableRoot CreateTableData { get { return createTableData; } set { createTableData = new CreateTableRoot(); createTableData = value; } }

    public PaymentCheckRoot paymentCheck
    { get { return paymentcheckData; } set { paymentcheckData = new PaymentCheckRoot(); paymentcheckData = value; } }

    public string AuthKey
    { get { return _authkey; } set { _authkey = value; } }

    public WalletRoot WalletData
    { get { return walletData; } set { walletData = new WalletRoot(); walletData = value; } }

    public PlayBetRoot PlayBetData
    { get { return playBetData; } set { playBetData = new PlayBetRoot(); playBetData = value; } }

    //SETTER*************************************************************************************************************
    public void SetAuthKey(string _key)
    {
        PlayerPrefs.SetString("AuthKey", _key);
    }

    public void SetProfileImage(Sprite image)
    {
        userProfileImage = null;
    }

    //GETTER***********************************************************************************************************
    public string GetUserName()
    {
        return userDetailData.userdata.name;
    }
    public int GetUserBalance()
    {
        return userDetailData.userdata.wallet;
    }
    public string GetUserId()
    {
        return userDetailData.userdata.id.ToString();
    }

    public int GetUserIdInt()
    {
        return userDetailData.userdata.id;

    }

    public string GetUserEmailId()
    {
        return userDetailData.userdata.email;
    }

    /*public byte[] GetUserAvatar()
    {
        return userDetailData.userdata.avatar;
    }*/

    public string GetAuthKey()
    {
        return PlayerPrefs.GetString("AuthKey");
    }

    public Sprite GetProfileImage()
    {
        return userProfileImage;
    }

    //*********************************************************************************************************************
    private void LoadUserThumbnail(string _url)
    {
        print(":::" + _url + "::::checking string");
        if (!string.IsNullOrEmpty(_url) && _url != "")
        {
            Debug.Log("url" + _url);

            LoadImage(_url, OnImageLoaded);
        }
    }

    private void OnImageLoaded(Sprite sp)
    {
        userProfileImage = sp;
    }
}