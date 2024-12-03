using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : UIScreens
{
    [SerializeField] private Text emailTxt, nameTxt;
    [SerializeField] private Image mask;
    public Image imgProfile;
    public Button Btn_Settings, btnBack, btnProfile, btn_Wallet;

    private void Start()
    {
        Listeners();
    }

    private void OnEnable()
    {
        mask.gameObject.SetActive(false);
        UserDetailsDataRoot ud = DataManager.Instance.UserDetailsData;
        nameTxt.text = ud.userdata.name.ToUpper();
        emailTxt.text = ud.userdata.email;
        if (DataManager.Instance.GetProfileImage() != null)
            ShowLogo(DataManager.Instance.GetProfileImage());
    }

    private void Listeners()
    {
        Btn_Settings.onClick.AddListener(() =>
        {
            PopupsManager.Instance.Show("Settings");
        });
        btnBack.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
        });
        btnProfile.onClick.AddListener(() =>
        {
            OnLogoSelectClicked();
        });
        btn_Wallet.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Wallet);
        });
    }

    private void OnWalletSelectClicked()
    {
    }

    //pick image

    private void OnLogoSelectClicked()
    {
        GallaryImagePicker.Instance.LoadImageFromGallary();
        GallaryImagePicker.Instance.OnImageSelected += OnImageSelected;
    }

    private byte[] bytes;
    private Sprite sprite;

    private void OnImageSelected(Texture2D texture)
    {
        GallaryImagePicker.Instance.OnImageSelected -= OnImageSelected;
        bytes = texture.EncodeToPNG();
        Debug.Log(bytes.Length);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //ShowLogo(sprite);
        GameManager.Instance.Wait(1f, () => { UpdateProfile(); });
    }

    private void ShowLogo(Sprite sprite)
    {
        mask.gameObject.SetActive(true);
        mask.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        // placeHolder.SetActive(false);
        // container.enabled = true;
        // imgProfile.sprite = sprite;
        //imgProfile.SetNativeSize();
    }

    //profile update API
    public void UpdateProfile()
    {
        WWWForm formData = new WWWForm();
        formData.AddBinaryData("avatar", bytes, "screenshot.png", "image/png");

        Debug.Log("image byte sent---------" + formData);
        Server.Instance.ApiRequest(typeof(ProfileImage), OnProfileUpdated, formData, true);
    }

    private void OnProfileUpdated(JSONObject data)
    {
        Debug.Log("OnprofileupdateReq");
        ProfileRoot profileimage = JsonUtility.FromJson<ProfileRoot>(data.Print());
        DataManager.Instance.profileImageData = profileimage;

       
        if (profileimage.success)
        {           
            //_LoadingStart();
            //Debug.Log(profileimage);
            
            Wait(1f, () =>
            {
                Server._LoadingEnd();
                MessageBox.Instance.Show(profileimage.message);
                Server.Instance.OnUserDetailUpdateCall();
                ShowLogo(sprite);
            });
            
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(profileimage.message);
        }
    }

    


    
}