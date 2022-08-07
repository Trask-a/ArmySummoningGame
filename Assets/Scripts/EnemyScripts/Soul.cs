using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Soul : NetworkBehaviour
{
    public GameObject soulPanel;
    public GameObject entity;
    int chances = 3;
    float chanceToClaim = 0.50f;
    public GameObject player;
    public NetworkVariable<float> attack = new NetworkVariable<float>();
    public NetworkVariable<float> health = new NetworkVariable<float>();
    public NetworkVariable<float> attackSpeed = new NetworkVariable<float>();
    public NetworkVariable<char> type = new NetworkVariable<char>();
    public NetworkVariable<int> level = new NetworkVariable<int>(0);
    public NetworkVariable<bool> claimOpen = new NetworkVariable<bool>(false);

    private void Start()
    {
        //findPlayer();
        
    }

    private void OnMouseDown()
    {
        findPlayer();
        // If player level is greater than or equal to enemy level then capture chance is 100%
        // else the chance is 1 divided by the difference in levels + 1
        if (player.GetComponent<PlayerMovementControler>().level >= level.Value)
        {
            chanceToClaim = 1;
        }
        else
        {
            chanceToClaim = (1f / (float)(Mathf.Abs(level.Value - player.GetComponent<PlayerMovementControler>().level) + 1f)) + 0.25f;
        }
        
        soulPanel.SetActive(true);
        soulPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = chances.ToString();
        soulPanel.transform.GetChild(2).transform.GetChild(1).GetComponent<Text>().text = (chanceToClaim * 100).ToString() + "%";
        soulPanel.transform.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = type.Value.ToString();
        soulPanel.transform.GetChild(2).transform.GetChild(3).GetComponent<Text>().text = level.Value.ToString();
        soulPanel.transform.GetChild(2).transform.GetChild(4).GetComponent<Text>().text = attack.Value.ToString();
        soulPanel.transform.GetChild(2).transform.GetChild(5).GetComponent<Text>().text = health.Value.ToString();
        soulPanel.transform.GetChild(2).transform.GetChild(6).GetComponent<Text>().text = attackSpeed.Value.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClaimServerRpc()
    {
        claimOpen.Value = true;
    }


    public void Claim()
    {
        if(claimOpen.Value == true) { return; }
        ClaimServerRpc();
        float rand = Random.Range(0.00f, 1.00f);
        Debug.Log("Rand: " + rand);
        if (rand <= chanceToClaim)
        {
            // successful claim
            // undo comment for multiplayer
            //player.GetComponent<ShadowArmyController>().ServerSpawnAllyServerRpc(type.Value, attack.Value, attackSpeed.Value, health.Value, level.Value, NetworkManager.Singleton.LocalClientId);
            player.GetComponent<ShadowArmyController>().AddToArmy(type.Value, attack.Value, attackSpeed.Value, health.Value, level.Value, 0);
            //player.GetComponent<ShadowArmyController>().SpawnAllyButtonServerRpc();
            soulPanel.SetActive(false);
            Destroy(GetComponent<SphereCollider>());
           
            // Play death animation if there is one
            //Destroy(gameObject, 1);
            StartCoroutine(Death());
            //DeathServerRpc();
        }
        else
        {
            chances--;
            if (chances == 0)
            {
                DontClaim();
            }
            soulPanel.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = chances.ToString();
        }

    }

    public void DontClaim()
    {
        soulPanel.SetActive(false);
        Destroy(GetComponent<SphereCollider>());
        //Destroy(gameObject);
        StartCoroutine(Death());
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeathServerRpc()
    {
        DeathClientRpc();
    }

    [ClientRpc]
    public void DeathClientRpc()
    {
        Destroy(gameObject);
    }

    public IEnumerator Death()
    {
        float disAmount = 0;
        while(disAmount <= 1)
        {
            GetComponent<MeshRenderer>().material.SetFloat("DissolveAmount", disAmount);
            yield return new WaitForSeconds(0.1f);
            disAmount += 0.1f;
        }
        //undo comment for multiplayer and remove line below it
        //DeathServerRpc();
        Destroy(gameObject);
    }

    public void findPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 
        // Uncomment for mupltiplayer and comment out above line
        /*
        GameObject [] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players)
        {
            if(p.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                player = p;
            }
        }
        */

        /*
        ulong localClientId = 0;
        localClientId = NetworkManager.Singleton.LocalClientId;
            
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
            {
                return;
            }
            
            if (networkClient != null)
            {
                if (networkClient.PlayerObject != null)
                {
                    if (player == null)
                    {
                        player = networkClient.PlayerObject.gameObject;
                    }
                

                }
            }
        */
        
    }
}
