using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackDetails       // instead of being a public class, change class to struct we can declare everything we need (instead of storing x position as a float can store the thing doin the dmg as a vector2)
{
    public Vector2 position;
    public float damageAmount;
    public float stunDamageAmount;

}
[System.Serializable]
public struct WeaponAttackDetails
{
    public string attackName;
    public float movementSpeed;
    public float damageAmount;
}
