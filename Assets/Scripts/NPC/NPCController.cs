using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.AI;

public class NPCController : NetworkTransform
{
    NavMeshAgent navMeshAgent;

    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    public override void Spawned()
    {
        navMeshAgent.SetDestination(new Vector3(100, 0, 100));
    }

    public void Kill()
    {
        RPC_Kill();
    }
    [Rpc]
    public void RPC_Kill()
    {
        Destroy(this.gameObject);
    }
}
