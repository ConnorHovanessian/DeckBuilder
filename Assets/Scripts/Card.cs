using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardName;

    public enum cardTarget
    {
        Enemy,
        Self
    }

    public cardTarget target;

    public int damage;
    public int block;

}
