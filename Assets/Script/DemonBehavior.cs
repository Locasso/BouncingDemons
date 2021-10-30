using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DemonType
{
	KNIGHT,
	TANK,
	CARD
}

public class DemonBehavior : StatsModel
{
	public delegate void DoDamage(int damagePower, Side side);
	public static event DoDamage OnDoDamage;

	public delegate void SendMoney(Side side, DemonType demonType);
	public static event SendMoney OnSendMoney;

	[Header("Demon Attributes")]
	[SerializeField] private int speed;
	[SerializeField] private DemonType demonType;

	[Header("Demon UI")]
	[SerializeField] private Image hpBar;
	[SerializeField] private Text nameText;
	[SerializeField] private Text hpText;
	//[SerializeField] private Color32[] teamColor;

	[Header("References")]
	[SerializeField] private GameManager gameManager;
	[SerializeField] private GameObject king, queen;
	[SerializeField] private GameObject damagePrefab;
	[SerializeField] private GameObject dieFeedback;

	[Header("Control Variables")]
	[SerializeField] private bool damageTurn;
	[SerializeField] private bool canWalk;
	[SerializeField] private GameObject currentTarget;

	public DemonType DemonType1 { get => demonType; set => demonType = value; }

	void Start()
	{
		gameManager = FindObjectOfType<GameManager>();
		king = GameObject.Find("king_side");
		queen = GameObject.Find("queen_side");
		DefineSprite();
		Setup();
	}

	void Setup()
	{
		damageTurn = true;

		if (Side1 == Side.KING)
		{
			this.gameObject.layer = LayerMask.NameToLayer("King");
			hpBar.color = Color.black;

			if (demonType == DemonType.KNIGHT)
				nameText.text = "King Knight";
			else if(demonType == DemonType.TANK)
				nameText.text = "King Tank";
			else if (demonType == DemonType.CARD)
				nameText.text = "King Card";
		}
		else if (Side1 == Side.QUEEN)
		{
			this.gameObject.layer = LayerMask.NameToLayer("Queen");
			hpBar.color = Color.red;

			if (demonType == DemonType.KNIGHT)
				nameText.text = "Queen Knight";
			else if (demonType == DemonType.TANK)
				nameText.text = "Queen Tank";
			else if (demonType == DemonType.CARD)
				nameText.text = "Queen Card";
		}
	}

	void FixedUpdate()
	{
		if (canWalk)
			WalkDefine();
	}

	public void WalkDefine()
	{
		if (this.Side1 == Side.KING)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed * Time.deltaTime);
			//Debug.Log("KING TROOP WALKING");
		}
		else if (this.Side1 == Side.QUEEN)
		{
			gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.left * speed * Time.deltaTime);
			//Debug.Log("QUEEN TROOP WALKING");
		}

	}

	public void DefineSprite()
	{
		if (this.Side1 == Side.KING)
		{
			if (demonType == DemonType.KNIGHT || demonType == DemonType.TANK)
			{
				gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gameManager.KingTroop[0];
			}
			else if(demonType == DemonType.CARD)
			{
				gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gameManager.KingTroop[1];
			}
		}
		if (this.Side1 == Side.QUEEN)
		{
			if (demonType == DemonType.KNIGHT || demonType == DemonType.TANK)
			{
				gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gameManager.QueenTroop[0];
			}
			else if (demonType == DemonType.CARD)
			{
				gameObject.transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gameManager.QueenTroop[1];
			}
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

	void Die()
	{
		if (Hp <= 0)
		{
			Instantiate(dieFeedback, this.gameObject.transform.position, this.gameObject.transform.rotation);
			Destroy(this.gameObject);
			OnSendMoney?.Invoke(Side1, DemonType1);
		}
		else
		{
			HealthControl();
		}
	}

	public void HealthControl(/*int attackPower, Side side*/)
	{
		hpText.text = Hp.ToString() + " / " + HpMax.ToString();

		//Debug.Log(Hp / HpMax);
		hpBar.fillAmount = (Hp / HpMax);
	}
	//void ReceiveDamage(int attackPower, Side side, Collider2D collision)
	//{
	//	Debug.Log("RECEBEU DANO");
	//	Debug.Log("Colisor: " + collision.gameObject.name + "Esse GameObject" + this.gameObject.name);
	//	Debug.Log("Colisor Side" + collision.gameObject.GetComponent<DemonBehavior>().Side1 + "this side: " + Side1);
	//	if(collision.gameObject.GetComponent<DemonBehavior>().Side1 != Side1)
	//	{
	//		Debug.Log("Colisor: " + collision.gameObject.name + "Esse GameObject" + this.gameObject.name);
	//		Hp -= attackPower;
	//		damageTurn = true;
	//	}
	//}

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
		else
			canWalk = true;

		Die();
	}

	#region Inscrição e trancamento nos eventos
	void OnEnable()
	{
		//DemonBehavior.OnDoDamage += ReceiveDamage;
	}

	void OnDisable()
	{
		//DemonBehavior.OnDoDamage -= ReceiveDamage;
	}
	#endregion
}
