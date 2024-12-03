using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemplateManager : MonoBehaviour
{
    [SerializeField] Transform potPos, betPos;
    [SerializeField] List<Transform> betObjList;

    [SerializeField] List<Sprite> backgroundImages;
    [SerializeField] List<Sprite> boardImages;
    [SerializeField] List<GameObject> playerTemplatePrefabs;



    [SerializeField] Image backgroundImageContainner;
    [SerializeField] Image boardImageContainner;   
    [SerializeField] List<Transform> playerUIPos;

    List<PlayerObj> tempGameObj = new List<PlayerObj>();
    int currentTemplateId;
    public static TemplateManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
        
    }
    private void Start()
    {
        
    }

    public void SetTemplate(int tId=0)
    {
        //currentTemplateId = tId;
        currentTemplateId = 0;// Random.Range(0, backgroundImages.Count - 1);
        backgroundImageContainner.sprite = backgroundImages[currentTemplateId];
        boardImageContainner.sprite = boardImages[currentTemplateId];
        GeneratePlayers();
    }

    void GeneratePlayers()
    {
        //clear objects 
        for (int i = 0; i < tempGameObj.Count; i++)
        {
            Destroy(tempGameObj[i].gameObject);
        }
        tempGameObj.Clear();
        
        //create object
        for (int i = 0; i < playerUIPos.Count; i++)
        {
            GameObject p = Instantiate(playerTemplatePrefabs[currentTemplateId], boardImageContainner.transform);
            p.transform.position = playerUIPos[i].position;
            PlayerObj pObj = p.transform.GetComponent<PlayerObj>();
            pObj.potPos = potPos;
            pObj.betPos = betPos;
            pObj.betObj = betObjList[i];
            tempGameObj.Add(pObj);

        }
        PhotonPlayerManager.instance.SetSittingPlayerList(tempGameObj);
    }
}
