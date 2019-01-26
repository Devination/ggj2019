using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private List<int> m_mushrooms;

    private void Start()
    {
        m_mushrooms = new List<int>();
    }

    private void OnDestroy()
    {
        EnemySpawner.RemoveEnemy();
    }
}
