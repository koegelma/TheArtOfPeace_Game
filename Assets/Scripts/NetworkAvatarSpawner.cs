using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkAvatarSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedAvatarPrefab;
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedAvatarPrefab = PhotonNetwork.Instantiate("NetworkAvatar", transform.position, transform.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Destroy(spawnedAvatarPrefab);
    }
}
