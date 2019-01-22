using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    private GameObject myAvatar;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        var spawn = Random.Range(0, GameSetup.GS.spawnPoints.Length);
        var spawnPoint = GameSetup.GS.spawnPoints[spawn];
        if(PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("_Prefabs/Photon", "NetworkPlayer"), spawnPoint.position, spawnPoint.rotation, 0);
        }
    }
}
