using System.Collections;
using System.Collections.Generic;
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
        potraitParentGroup = FindAnyObjectByType<PlayerStatDisplay>().transform;
        if (potraitParentGroup == null) { return; }

        if (TryGetComponent<KnockBackHandler>(out knockbackHandler)) {
            knockbackHandler.OnKnockbackRecieved += UpdateMyUI;
        }
        else {
            return;
        }

        myPlayerPotrait = Instantiate(playerPotraitPrefab, potraitParentGroup);

        if(TryGetComponent<PlayerStats>(out PlayerStats myPlayerStats))
        {
            if(myPlayerStats.colors.Count != 0)
            {
                myPlayerPotrait.playerPotrait.color = myPlayerStats.colors[myPlayerStats.playerIndex];
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
}
