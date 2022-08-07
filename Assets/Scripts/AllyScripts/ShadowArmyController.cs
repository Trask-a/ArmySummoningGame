using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ShadowArmyController : NetworkBehaviour
{
    public GameObject tank;
    public GameObject melee;
    public GameObject ranged;
    public GameObject healer;
    List<GameObject> shadowArmyList;
    public GameObject shadowArmy = null;
    public GameObject localPlayer;
    public GameObject newAlly;
    //private NetworkVariable<bool> armySummoned = new NetworkVariable<bool>(false);
    public bool armySummoned = false;
    public GameObject allyButton;
    public GameObject allyInformation;
    GameObject newAllyInfo;

    private void Start()
    {
        shadowArmy = GameObject.FindGameObjectWithTag("ShadowArmy");
        allyInformation = GameObject.FindGameObjectWithTag("AllyInformation");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerSpawnAllyServerRpc(char type, float att, float attSpe, float h, int l, ulong localClientId = 0)
    {
        Debug.Log("Client sends serverRpc spawn ally");
        AddToArmy(type, att, attSpe, h, l, localClientId);
        
    }

    [ServerRpc]
    public void SpawnAllyButtonServerRpc()
    {
        GameObject newAllyInfo = Instantiate(allyButton, allyInformation.transform.GetChild(0).GetChild(1).GetChild(0));
        // undo comment for multiplayer
        //newAllyInfo.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    public void ClientSpawnAllyClientRpc()
    {
        // undo comment for multiplayer
        // if (IsHost) { return; }
        GameObject[] ally = GameObject.FindGameObjectsWithTag("Ally");

       foreach(GameObject a in ally)
        {
            //a.SetActive(false);
            if(a.transform.parent == null)
            {
                //a.transform.parent = gameObject.transform;
                a.transform.parent = gameObject.transform;
            }
        }
        
    }

    [ServerRpc]
    public void ServerSummonArmyServerRpc()
    {
        // uncomment for multiplayer
        /*if(armySummoned.Value == false)
        {
            armySummoned.Value = true;
        }
        else
        {
            armySummoned.Value = false;
        }
        */
        //ClientSummonArmyClientRpc();
    }

    private void OnEnable()
    {
        //uncomment for multiplayer
        //armySummoned.OnValueChanged += OnSummonArmy;
    }

    private void OnDisable()
    {
        // uncomment for multiplayer
       // armySummoned.OnValueChanged -= OnSummonArmy;
    }

    public void OnSummonArmy(bool oldValue, bool newValue)
    {
        /*if (!IsClient) { return; }
       

        ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localPlayerId, out NetworkClient networkClient))
        {
            return;
        }
        if (shadowArmy == null) { return; }

        for (int i = 0; i < shadowArmy.transform.childCount; i++)
        {
            if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
            {
                shadowArmy.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);
                shadowArmy.transform.GetChild(i).position = new Vector3(shadowArmy.transform.position.x + i, 2, shadowArmy.transform.position.z);
            }
            //SummonArmy();
            //ClientSummonArmyClientRpc();
        }*/

        // TODO: Rename your fucking variables bitch - mike
        int j = 1;
        int b = 1;
        int a = 1;
        int k = 1;
        int c = 0;

        armySummoned = !armySummoned;

        // undo comment for multiplayer
        //if (!IsClient) { return; }
        for (int i = 3; i < gameObject.transform.childCount; i++)
        {


            if (armySummoned == false)
            {
                // uncomment for multiplayer
                gameObject.transform.GetChild(i).transform.position = gameObject.transform.position;
                gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned);
            }
            else
            {
                // uncomment for multiplayer
               // gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);
                gameObject.transform.GetChild(i).gameObject.SetActive(!gameObject.transform.GetChild(i).gameObject.activeSelf);
                // set positions around player

                // increase circle spawn position outwards by 1 once circle completed
                if (c % 8 == 0)
                {
                    k++;
                }

                // starting spawn spot
                if (i == 3)
                {
                    gameObject.transform.GetChild(i).position = new Vector3(gameObject.transform.position.x + k, 2, gameObject.transform.position.z + k);
                }

                // for not diagonal spawns
                else if (i % 2 == 0)
                {
                    // for z spawning
                    if (i % 4 == 0)
                    {
                        k *= j;
                        gameObject.transform.GetChild(i).position = new Vector3(gameObject.transform.position.x, 2, gameObject.transform.position.z + k);
                        k *= j;
                        a *= -1;
                    }
                    // for x spawning
                    else
                    {
                        j *= -1;
                        k *= j;
                        gameObject.transform.GetChild(i).position = new Vector3(gameObject.transform.position.x + k, 2, gameObject.transform.position.z);
                        k *= j;
                        b *= -1;
                    }
                }
                // for diagonal spawns
                else
                {
                    gameObject.transform.GetChild(i).position = new Vector3(gameObject.transform.position.x + (k * a), 2, gameObject.transform.position.z + (k * b));
                }
                c++;
            }
            
            
            
        }
            //SummonArmy();
            //ClientSummonArmyClientRpc();
        
        

    }

    [ClientRpc]
    public void ClientSummonArmyClientRpc()
    {
        // undo comment for multiplayer
        //if(!IsLocalPlayer) { return; }
        for (int i = 3; i < gameObject.transform.childCount; i++)
        {
            

            
                // uncomment for multiplayer
               // gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);

                //SummonArmy();
                //ClientSummonArmyClientRpc();
            

            
        }
        //SummonArmy();
    }

    // Send a ServerRPC to spawn the ally from the server side and also send the client id and then add the spawned alley as a child to
    // that client
    public void AddToArmy(char type, float att, float attSpe, float h, int l, ulong localClientId)
    {
        // Uncomment for multiplayer
      //  if (!IsHost)
        //{
          //  return;
        //}

        newAlly = null;
        if (type == 'm')
        {
            newAlly = Instantiate(melee, transform.position, transform.rotation);
            
        }
        else if (type == 'r')
        {
            newAlly = Instantiate(ranged, transform.position, transform.rotation);
        }
        else if (type == 'h')
        {
            newAlly = Instantiate(healer, transform.position, transform.rotation);
        }
        else if (type == 't')
        {
            newAlly = Instantiate(tank, transform.position, transform.rotation);
        }

        // uncomment for multiplayer
       // newAlly.GetComponent<NetworkObject>().Spawn();

        if (newAlly != null)
        {
            //uncommnet for multiplayer
            // if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
            //{
            //  return;
            //}

            localPlayer = GameObject.FindGameObjectWithTag("Player"); // networkClient.PlayerObject.gameObject; undo comment for multiplayer

            
            
            newAlly.GetComponent<AllyMeleeController>().player = localPlayer;

            newAlly.transform.parent = shadowArmy.transform;
            newAlly.transform.position = new Vector3(0, 2, 0);
            newAlly.GetComponent<AllyController>().attack.Value = att;
            newAlly.GetComponent<AllyController>().attackSpeed.Value = attSpe;
            newAlly.GetComponent<AllyController>().setMaxHealth(h);
            newAlly.GetComponent<AllyController>().setHealth(h);
            newAlly.GetComponent<AllyController>().allyName = "BEAK Gamer " + Random.Range(1, 1000).ToString();
            newAlly.GetComponent<AllyController>().allyAggro = GetComponent<PlayerMovementControler>().armyAggro;
            for (int i = 0; i < l-1; i++)
            {
                newAlly.GetComponent<AllyController>().levelUp();
            }
            newAlly.SetActive(false);
        }
        newAllyInfo = Instantiate(allyButton, allyInformation.transform.GetChild(0).GetChild(1).GetChild(0));
        // undo comment for multiplayer
        //newAllyInfo.GetComponent<NetworkObject>().Spawn();
        AllyButtonClientRpc();
        //newAllyInfo.GetComponent<AllyButtonScript>().FindStatsPanel();
        newAllyInfo.GetComponent<AllyButtonScript>().ally = newAlly;
        newAllyInfo.GetComponent<AllyButtonScript>().allyType.Value = type;
        newAlly.GetComponent<AllyController>().allyButton = newAllyInfo;
        ClientSpawnAllyClientRpc();
    }

    [ClientRpc]
    public void AllyButtonClientRpc()
    {
        GameObject[] allyButtons = GameObject.FindGameObjectsWithTag("AllyButton");

        foreach (GameObject a in allyButtons)
        {
            //a.SetActive(false);
            if (a.transform.parent == null)
            {
                //a.transform.parent = gameObject.transform;
                a.transform.parent = allyInformation.transform.GetChild(0).GetChild(1).GetChild(0);
                a.GetComponent<AllyButtonScript>().FindStatsPanel();
                //a.GetComponent<AllyButtonScript>().ally = newAlly;
            }
        }
        
    }

    public void SummonArmy()
    {
        if (shadowArmy != null)
        {
            if (shadowArmy.transform.childCount > 0)
            {
                // undo comment for multiplayer
                //ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
                //if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localPlayerId, out NetworkClient networkClient))
                //{
                  //  return;
                //}


                //&& shadowArmy.transform.GetChild(0).gameObject.GetComponent<AllyMeleeController>().player == transform.gameObject
                if (shadowArmy.transform.GetChild(0).gameObject.activeSelf == false)
                {
                    shadowArmy.transform.position = transform.position;
                    int j = 1;
                    int b = 1;
                    int a = 1;
                    int k = 1;
                    for (int i = 0; i < shadowArmy.transform.childCount; i++)
                    {
                        // activate ally from army
                        // undo comment for multiplayer
                        //if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
                        if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == this.gameObject)
                        {
                            shadowArmy.transform.GetChild(i).gameObject.SetActive(true);


                            // set positions around player

                            // increase circle spawn position outwards by 1 once circle completed
                            if (i % 8 == 0)
                            {
                                k++;
                            }

                            // starting spawn spot
                            if (i == 0)
                            {
                                shadowArmy.transform.GetChild(i).position = new Vector3(shadowArmy.transform.position.x + k, 2, shadowArmy.transform.position.z);
                            }

                            // for not diagonal spawns
                            if (i % 2 == 0)
                            {
                                // for z spawning
                                if (i % 4 == 0)
                                {
                                    k *= j;
                                    shadowArmy.transform.GetChild(i).position = new Vector3(shadowArmy.transform.position.x + k, 2, shadowArmy.transform.position.z);
                                    k *= j;
                                    a *= -1;
                                }
                                // for x spawning
                                else
                                {
                                    k *= j;
                                    shadowArmy.transform.GetChild(i).position = new Vector3(shadowArmy.transform.position.x, 2, shadowArmy.transform.position.z + k);
                                    k *= j;
                                    j *= -1;
                                    b *= -1;
                                }
                            }
                            // for diagonal spawns
                            else
                            {
                                shadowArmy.transform.GetChild(i).position = new Vector3(shadowArmy.transform.position.x + (k * a), 2, shadowArmy.transform.position.z + (k * b));
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < shadowArmy.transform.childCount; i++)
                    {
                        // undo comment for multiplayer
                        //if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
                        if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == this.gameObject)
                        {
                            // reset army positions
                            shadowArmy.transform.GetChild(i).transform.position = shadowArmy.transform.position;
                            // de-activate ally from army
                            shadowArmy.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void SetAggroStatus(AggroEnum _allyAggro)
    {
        for (int i = 0; i < shadowArmy.transform.childCount; i++)
        {
            // undo comment for multiplayer
            //if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
            //if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == this.gameObject)
            //{
                
            // change ally aggro
            shadowArmy.transform.GetChild(i).GetComponent<AllyController>().allyAggro = _allyAggro;
            //}
        }
    }
}
