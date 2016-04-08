using UnityEngine;
using System.Collections;
using System;

public class DungeonSquadron : Squadron {
    protected override void Initialize()
    {
        path = "DungeonMonsters";
        enemyCount = 4;
    }

    protected override void CheckIfDead()
    {
        if(transform.GetComponentsInChildren<Enemy>().Length == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
