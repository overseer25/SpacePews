using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private PhotonView PV;
    private GameObject character;

    public Camera myCamera;
    public AudioListener myAL;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered);
        }
        Destroy(myCamera);
        Destroy(myAL);
    }

    [PunRPC]
    void RPC_AddCharacter()
    {
        character = Instantiate(Resources.Load("_Prefabs/Photon/PhotonNetworkPlayer") as GameObject, transform.position, transform.rotation, transform);
    }
}
