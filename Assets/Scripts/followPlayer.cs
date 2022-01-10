using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Singletons;

public class followPlayer : Singleton<followPlayer>
{
    public Transform player = null;
    public Vector3 playerPos;
    float cameraHeight = 10f;
    public ulong localClientId;

    public void FollowPlayer(Transform transform)
    {
        player = transform;
    }

    // https://www.youtube.com/watch?v=iQDGLaSI3Cg 
    // Update is called once per frame
    void Update()
    {

        if (player != null)
        {
            //if (player.PlayerObject != null)
            //{
            playerPos = player.position;
            transform.position = new Vector3(playerPos.x - 5, playerPos.y + cameraHeight, playerPos.z - 5);

            // }
        }
    }

    /*
    public void findPlayer()
    {

        if (localClientId != NetworkManager.Singleton.LocalClientId)
        {
            localClientId = NetworkManager.Singleton.LocalClientId;
        }

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }

        //player = networkClient;
    }
    */

}