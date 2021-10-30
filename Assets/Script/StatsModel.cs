using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
	KING,
	QUEEN
}

public class StatsModel : MonoBehaviour
{
	

	[Header("Attributes")]
	[SerializeField] private float hp;
	[SerializeField] private float hpMax;
	[SerializeField] private int attackPower;
	[SerializeField] private int attackSpeed;
	[SerializeField] private int attackRange;
	[SerializeField] private Side side;

	public float Hp { get => hp; set => hp = value; }
	public float HpMax { get => hpMax; set => hpMax = value; }
	public int AttackPower { get => attackPower; set => attackPower = value; }
	public int AttackRange { get => attackRange; set => attackRange = value; }
	public Side Side1 { get => side; set => side = value; }
	public int AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

	void Start()
    {
        
    }

    void Update()
    {
        
    }
}
