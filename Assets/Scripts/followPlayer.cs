using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class followPlayer : MonoBehaviour
{
    public NetworkClient player = null;
    public Vector3 playerPos;
    float cameraHeight = 10f;
    public ulong localClientId;

    void Start()
    {
        
    }

    // https://www.youtube.com/watch?v=iQDGLaSI3Cg 
    // Update is called once per frame
    void Update()
    {

        if(player != null)
        {
            if (player.PlayerObject != null)
            {
                playerPos = player.PlayerObject.transform.position;
                transform.position = new Vector3(playerPos.x - 5, playerPos.y + cameraHeight, playerPos.z - 5);

            }
        }
    }

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



        
        player = networkClient;
    }
}
