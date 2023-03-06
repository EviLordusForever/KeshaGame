using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathFinder : MonoBehaviour
{
    public WayPoint[] wayPoints;
    public GameObject player;

    public void Start()
    {

    }

/*    public Vector2 FindPath()
    {
        for (int i = 0; i < wayPoints.Length; i++)
        {

        }

    }*/

    public bool SeePlayer()
    {
        /*        var rayDirection = player.position - transform.position;

                if (Physics.Raycast(transform.position, rayDirection, $$anonymous$$t))
                    return $$anonymous$$t.transform == player;
        */
        return true;
    }
}
