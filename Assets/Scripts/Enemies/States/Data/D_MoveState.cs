using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMoveStateData", menuName = "Data/State Data/Move State")]

public class D_MoveState : ScriptableObject // data container that you can use to save large amounts of data independent of class instances
{
    public float movementSpeed = 3f; 
}
