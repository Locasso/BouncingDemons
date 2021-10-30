using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBehavior : StatsModel
{
	public delegate void DoDamage(int damagePower, Side side);
	public static event DoDamage OnDoDamage;

	public delegate void GameOver(Side side);
	public static event GameOver OnGameOver;

	[Header("UI components")]
	[SerializeField] private Image hpBar;
	[SerializeField] private Text hpText;

	[Header("References")]
	[SerializeField] private GameObject damagePrefab;

	[Header("Control Variables")]
	[SerializeField] private bool damageTurn;
	[SerializeField] private bool canWalk;
	[SerializeField] private GameObject currentTarget;

	void Start()
	{
		damageTurn = true;
	}

	void Update()
	{

	}

	public void HealthControl(int attackPower, Side side)
	{
		if (side != Side1)
		{
			//if (!collision.IsNull())
			//	if (collision.bounds.Intersects(gameObject.GetComponent<Collider2D>().bounds) &&
			//		!collision.GetComponent<DemonBehavior>().IsNull())
			//{
			//Hp -= attackPower;
			hpText.text = Hp.ToString() + " / " + HpMax.ToString();
			//Debug.Log(Hp / HpMax);
			hpBar.fillAmount = (Hp / HpMax);

			if (Hp <= 0)
			{
				Hp = 0;
				hpText.text = Hp.ToString() + " / " + HpMax.ToString();
				hpBar.fillAmount = 0;
				OnGameOver?.Invoke(Side1);
				//Debug.Log("Game Over");
			}
			//}
		}
	}


	IEnumerator Damage()
	{
		//Debug.Log("LANÇOU O DANO");
		yield return new WaitForSeconds(0.1f);
		canWalk = false;
		yield return new WaitForSeconds(AttackSpeed);

		if (!currentTarget.IsNull())
		{
			currentTarget.GetComponent<StatsModel>().Hp -= AttackPower;

			GameObject damage = Instantiate(damagePrefab, new Vector2(currentTarget.transform.position.x,
				currentTarget.transform.position.y), currentTarget.transform.localRotation);
			damage.transform.GetChild(0).GetComponent<Text>().text = "-" + AttackPower;

			if (this.Side1 == Side.KING)
				damage.transform.GetChild(0).GetComponent<Text>().color = Color.red;
			else
				damage.transform.GetChild(0).GetComponent<Text>().color = Color.black;
		}

		OnDoDamage?.Invoke(AttackPower, Side1);
		damageTurn = true;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		//Debug.Log(collision.gameObject.name);
		if (!collision.gameObject.GetComponent<DemonBehavior>().IsNull() &&
			collision.gameObject.GetComponent<DemonBehavior>().Side1 != this.Side1 ||
			!collision.gameObject.GetComponent<TowerBehavior>().IsNull() &&
			collision.gameObject.GetComponent<TowerBehavior>().Side1 != this.Side1)
		{
			//Debug.Log("ENTROU NO DAMAGE");
			if (damageTurn)
			{
				if (currentTarget.gameObject.IsNull() ||
					!currentTarget.GetComponent<BoxCollider2D>().bounds.Intersects(gameObject.GetComponent<Collider2D>().bounds))
					currentTarget = collision.gameObject;
				StartCoroutine(Damage());
				damageTurn = false;
			}
		}
	}

	#region Inscrição e trancamento nos eventos
	void OnEnable()
	{
		DemonBehavior.OnDoDamage += HealthControl;
	}

	void OnDisable()
	{
		DemonBehavior.OnDoDamage -= HealthControl;
	}
	#endregion
}
