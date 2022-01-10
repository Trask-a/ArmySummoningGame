﻿using System.Collections;
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
    private NetworkVariable<bool> armySummoned = new NetworkVariable<bool>(false);
    public GameObject allyButton;
    public GameObject allyInformation;
    GameObject newAllyInfo;

    private void Start()
    {
        shadowArmy = GameObject.FindGameObjectWithTag("ShadowArmy");
        allyInformation = GameObject.FindGameObjectWithTag("AllyInformation");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerSpawnAllyServerRpc(char type, float att, float attSpe, float h, int l, ulong localClientId)
    {
        Debug.Log("Client sends serverRpc spawn ally");
        AddToArmy(type, att, attSpe, h, l, localClientId);
        
    }

    [ServerRpc]
    public void SpawnAllyButtonServerRpc()
    {
        GameObject newAllyInfo = Instantiate(allyButton, allyInformation.transform.GetChild(0).GetChild(1).GetChild(0));
        newAllyInfo.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    public void ClientSpawnAllyClientRpc()
    {
        
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
        if(armySummoned.Value == false)
        {
            armySummoned.Value = true;
        }
        else
        {
            armySummoned.Value = false;
        }
        
        //ClientSummonArmyClientRpc();
    }

    private void OnEnable()
    {
        armySummoned.OnValueChanged += OnSummonArmy;
    }

    private void OnDisable()
    {
        armySummoned.OnValueChanged -= OnSummonArmy;
    }

    private void OnSummonArmy(bool oldValue, bool newValue)
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

        if (!IsClient) { return; }
        for (int i = 3; i < gameObject.transform.childCount; i++)
        {


            if (armySummoned.Value == false)
            {
                gameObject.transform.GetChild(i).transform.position = gameObject.transform.position;
                gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);
            }
            else
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);
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
        if(!IsLocalPlayer) { return; }
        for (int i = 3; i < gameObject.transform.childCount; i++)
        {
            

            

                gameObject.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);

                //SummonArmy();
                //ClientSummonArmyClientRpc();
            

            
        }
        //SummonArmy();
    }

    // Send a ServerRPC to spawn the ally from the server side and also send the client id and then add the spawned alley as a child to
    // that client
    public void AddToArmy(char type, float att, float attSpe, float h, int l, ulong localClientId)
    {

        if (!IsHost)
        {
            return;
        }

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

        newAlly.GetComponent<NetworkObject>().Spawn();

        if (newAlly != null)
        {
            
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
            {
                return;
            }

            localPlayer = networkClient.PlayerObject.gameObject;

            
            newAlly.SetActive(false);
            newAlly.GetComponent<AllyMeleeController>().player = localPlayer;

            newAlly.transform.parent = gameObject.transform;
            newAlly.transform.position = new Vector3(0, 2, 0);
            newAlly.GetComponent<AllyController>().attack.Value = att;
            newAlly.GetComponent<AllyController>().attackSpeed.Value = attSpe;
            newAlly.GetComponent<AllyController>().setMaxHealth(h);
            newAlly.GetComponent<AllyController>().setHealth(h);
            for(int i = 0; i < l-1; i++)
            {
                newAlly.GetComponent<AllyController>().levelUp();
            }
        }
        newAllyInfo = Instantiate(allyButton, allyInformation.transform.GetChild(0).GetChild(1).GetChild(0));
        newAllyInfo.GetComponent<NetworkObject>().Spawn();
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
                ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
                if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localPlayerId, out NetworkClient networkClient))
                {
                    return;
                }


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
                        if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
                        {
                            shadowArmy.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);


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
                        if (shadowArmy.transform.GetChild(i).gameObject.GetComponent<AllyMeleeController>().player == networkClient.PlayerObject.gameObject)
                        {
                            // reset army positions
                            shadowArmy.transform.GetChild(i).transform.position = shadowArmy.transform.position;
                            // de-activate ally from army
                            shadowArmy.transform.GetChild(i).gameObject.SetActive(armySummoned.Value);
                        }
                    }
                }
            }
        }
    }
}
