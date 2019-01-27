using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private List<int> m_mushrooms;
    private MushroomSpawner mushSpawn;
    private GameObject player;
    private NavMeshAgent agent;

    private enum ENEMYSTATE : int
    {
        HUNGRY,
        EATING,
        ANGRY
    };

    private int currentState;
    private bool inPursuit;

    // Starting with 10% chance of being angry
    private float angryChance = 0.1f; 

    private void Start()
    {
        m_mushrooms = new List<int>();

        currentState = (Random.value <= angryChance) ?
            (int)ENEMYSTATE.ANGRY :
            (int)ENEMYSTATE.HUNGRY;
        inPursuit = false;
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDestroy()
    {
        EnemySpawner.RemoveEnemy();
    }

    private void Update()
    {
        if (mushSpawn == null)
        {
            mushSpawn = GameObject.Find("MushroomSpawner").GetComponent<MushroomSpawner>();
        }

        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (!inPursuit)
        {
            switch (currentState)
            {
                case (int)ENEMYSTATE.HUNGRY:
                    GameObject closestMush = MushroomSpawner.FindClosestIdleMushroom(transform.position);
                    if (closestMush == null) return;
                    closestMush.GetComponent<Mushroom>().isEnemyTracking = true;
                    agent.destination = closestMush.transform.position;
                    inPursuit = true;
                    break;

                case (int)ENEMYSTATE.ANGRY:

                    break;
            }
        }
    }
}


