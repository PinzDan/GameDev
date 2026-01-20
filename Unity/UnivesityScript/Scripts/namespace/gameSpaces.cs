using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameSpaces.enemies
{

    public enum EnemyType
    {

        Slime,
        Golem,
        Mushroom,
        Turtle,
        Cactus,
        Dragon
    }

    public static class EnemyDatabase
    {
        public static readonly Dictionary<EnemyType, string> DataPaths = new()
        {
            { EnemyType.Slime, "Data/Enemies/EnemyData.Slime" },
            { EnemyType.Golem, "Data/Enemies/EnemyData.Golem" },
            { EnemyType.Mushroom, "Data/Enemies/EnemyData.Mushroom" },
            { EnemyType.Turtle, "Data/Enemies/EnemyData.Turtle" },
            { EnemyType.Cactus, "Data/Enemies/EnemyData.Cactus" },
            { EnemyType.Dragon, "Data/Enemies/EnemyData.Dragon" }


        };
    }
}

// namespace gameSpaces.data
// {

// }



