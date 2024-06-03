using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    KnockBackHandler knockbackHandler;
    [SerializeField] PlayerPotrait playerPotraitPrefab;
    PlayerPotrait myPlayerPotrait;
    Transform potraitParentGroup;

    void Start()
    {
        SetUp();
    }

    private void OnDestroy()
    {
        if (knockbackHandler == null) {return; }

        knockbackHandler.OnKnockbackRecieved -= UpdateMyUI;
    }

    private void SetUp()
    {
        if (playerPotraitPrefab == null) { return; }
        PlayerStatDisplay potraitParentObject = FindAnyObjectByType<PlayerStatDisplay>();
        if (potraitParentObject == null) { return; }
        potraitParentGroup = potraitParentObject.transform;

        if (TryGetComponent<KnockBackHandler>(out knockbackHandler)) {
            knockbackHandler.OnKnockbackRecieved += UpdateMyUI;
        }
        else {
            return;
        }

        myPlayerPotrait = Instantiate(playerPotraitPrefab, potraitParentGroup);

        if(TryGetComponent<PlayerStats>(out PlayerStats myPlayerStats))
        {
            if(myPlayerStats.playerSprites.Count != 0)
            {
                myPlayerPotrait.playerPotrait.sprite = myPlayerStats.playerSprites[myPlayerStats.playerIndex];
            }

            if(myPlayerStats.backGrpundSprites != null)
            {
                myPlayerPotrait.background.sprite = myPlayerStats.backGrpundSprites;
            }

            if(myPlayerStats.colors.Count != 0)
            {
                //myPlayerPotrait.playerPotrait.color = myPlayerStats.colors[myPlayerStats.playerIndex];
                myPlayerPotrait.background.color = myPlayerStats.colors[myPlayerStats.playerIndex];
                myPlayerPotrait.damagePercentage.color = myPlayerStats.colors[myPlayerStats.playerIndex];
            }
        }

        UpdateMyUI();
        
    }

    private void UpdateMyUI()
    {
        if (myPlayerPotrait == null) { return; }
        if (knockbackHandler == null) { return; }

        myPlayerPotrait.damagePercentage.text = $"{knockbackHandler.recievedKnockbackDisplay}%";
    }
    
    public void UpdateEndGameUI()
    {
        if (myPlayerPotrait == null) { return; }
        if (!TryGetComponent<PlayerStats>(out PlayerStats myPlayerStats)) { return; }

        if(myPlayerStats.endGamePlayerSprites.Count != 0)
        {
            myPlayerPotrait.playerPotrait.sprite = myPlayerStats.endGamePlayerSprites[myPlayerStats.playerIndex];
        }

        myPlayerPotrait.damagePercentage.text = $"{myPlayerStats.finalKnockbackDisplay}%";
    }

    public void HidePlayerPotraits()
    {
        if (myPlayerPotrait != null)
        {
            myPlayerPotrait.gameObject.SetActive(false);
        }
    }
}
