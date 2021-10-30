using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	protected int valor;

	[Header("Control Variables")]
	[SerializeField] private Side playerSide; //Variável que guarda o lado que o jogador vai iniciar
	[SerializeField] private Side enemySideWest; //Variável que guarda o lado que o inimigo vai iniciar

	[Header("Game Currency and Balance")]
	[SerializeField] private int playerMoney, enemyMoney; //Guardam o valor de dinheiros que o jogador e inimigo tem.
	[SerializeField] private Text playerMoneyUI, enemyMoneyUI; //Textos que exibem o valor de dinheirinhos do jogador e inimigo

	[Header("Spawn Demons control")]
	[SerializeField] private GameObject[] demonsPrefabs;
	[SerializeField] private GameObject[] spawnHandlers;
	[SerializeField] private Button[] btnsReference;
	[SerializeField] private GameObject[] heroeReference;
	[SerializeField] private float cooldownDemon;
	[SerializeField] private int random;

	[Header("Sprite References")]
	[SerializeField] private Sprite[] kingTroop;
	[SerializeField] private Sprite[] queenTroop;

	[Header("Menu")]
	[SerializeField] private GameObject menuCanvas;
	[SerializeField] private AudioSource gameMusic;

	[Header("GameOver Objects")]
	[SerializeField] private GameObject gameOverScreen;
	[SerializeField] private Image playerChar;
	[SerializeField] private Sprite[] possibleChars;
	[SerializeField] private Text finalText;

	public Sprite[] KingTroop { get => kingTroop; set => kingTroop = value; }
	public Sprite[] QueenTroop { get => queenTroop; set => queenTroop = value; }

	private void Awake()
	{
		
	}

	void Start()
	{
		menuCanvas.SetActive(true);
	}

	public void ChooseCharacter(string side)
	{
		if (side == "King")
			playerSide = Side.KING;
		else if (side == "Queen")
			playerSide = Side.QUEEN;
		menuCanvas.SetActive(false);

		SetupGame();
		StartCoroutine(IA());
	}

	void SetupGame()
	{
		if (playerSide == Side.KING)
		{
			playerMoneyUI = GameObject.Find("kingCoins").transform.GetChild(0).GetComponent<Text>();
			enemyMoneyUI = GameObject.Find("queenCoins").transform.GetChild(0).GetComponent<Text>();
			enemySide = Side.QUEEN;
		}
		else
		{
			playerMoneyUI = GameObject.Find("queenCoins").transform.GetChild(0).GetComponent<Text>();
			enemyMoneyUI = GameObject.Find("kingCoins").transform.GetChild(0).GetComponent<Text>();
			enemySide = Side.KING;
		}

		playerMoney = 1000;
		enemyMoney = 1000;
		playerMoneyUI.text = "x " + playerMoney;
		enemyMoneyUI.text = enemyMoney + " x";
	}

	public void SetupGameOver(Side side)
	{
		Time.timeScale = 0.3f;
		gameOverScreen.SetActive(true);
		playerChar.sprite = possibleChars[(int)playerSide];

		if (side == playerSide)
			if (side == Side.KING)
				finalText.text = "You lose! Your King is more evil than you!";
			else if (side == Side.QUEEN)
				finalText.text = "You lose! Your Queen is more evil than you!";
		if (side != playerSide)
			if (side == Side.KING)
				finalText.text = "You win! You are the most evil Queen!";
			else if (side == Side.QUEEN)
				finalText.text = "You win! You are the most evil King!";
	}

	public void PlayAgain(string scene)
	{
		SceneManager.LoadScene(scene);
		if (Time.timeScale != 1)
		{
			Time.timeScale = 1;
		}
	}

	public void InvokeCooldown()
	{
		StartCoroutine(CoolDownButtons());
	}

	IEnumerator CoolDownButtons()
	{
		foreach (Button btn in btnsReference)
		{
			btn.GetComponent<Image>().fillAmount = 0;
			btn.GetComponent<Button>().interactable = false;
		}

		bool clicked = true;
		float timer = 0;

		while (clicked)
		{
			timer += Time.deltaTime;
			foreach (Button btn in btnsReference)
			{
				btn.GetComponent<Image>().fillAmount = timer / cooldownDemon;
				yield return null;
			}

			if (timer >= cooldownDemon)
			{
				foreach (Button btn in btnsReference)
				{
					btn.GetComponent<Button>().interactable = true;
				}
				clicked = false;
			}
			yield return null;
		}
	}

	void SetupMoneyUI(int value, bool sum, bool playerSide)
	{
		if (playerSide)
		{
			if (sum)
			{
				playerMoney += value;
			}
			else
			{
				playerMoney -= value;
			}
		}
		else
		{
			if (sum)
			{
				enemyMoney += value;
			}
			else
			{
				enemyMoney -= value;
			}
		}

		playerMoneyUI.text = "x " + playerMoney;
		enemyMoneyUI.text = enemyMoney + " x";
	}

	public void ReceiveMoney(Side side, DemonType demonType)
	{
		bool playerMoney = false;

		if (playerSide != side)
		{
			playerMoney = true;
			if (demonType == DemonType.KNIGHT)
				SetupMoneyUI(10, true, playerMoney);
			else if (demonType == DemonType.TANK)
				SetupMoneyUI(50, true, playerMoney);
			else if (demonType == DemonType.CARD)
				SetupMoneyUI(20, true, playerMoney);
		}
		else
		{
			if (demonType == DemonType.KNIGHT)
				SetupMoneyUI(10, true, playerMoney);
			else if (demonType == DemonType.TANK)
				SetupMoneyUI(50, true, playerMoney);
			else if (demonType == DemonType.CARD)
				SetupMoneyUI(20, true, playerMoney);
		}
	}

	public void SummonDemon()
	{
		if (EventSystem.current.currentSelectedGameObject.gameObject.name == "invokeKnight")
		{
			if (playerMoney >= 20)
			{
				random = Random.Range(0, 3);
				GameObject newDemon = Instantiate(demonsPrefabs[0], spawnHandlers[(int)playerSide].transform.GetChild(random).transform);
				newDemon.GetComponent<DemonBehavior>().Side1 = playerSide;
				SetupMoneyUI(20, false, true);
			}
		}
		else if (EventSystem.current.currentSelectedGameObject.gameObject.name == "invokeTank")
		{
			if (playerMoney >= 300)
			{
				random = Random.Range(0, 3);
				GameObject newDemon = Instantiate(demonsPrefabs[1], spawnHandlers[(int)playerSide].transform.GetChild(random).transform);
				newDemon.GetComponent<DemonBehavior>().Side1 = playerSide;
				newDemon.GetComponent<DemonBehavior>().DemonType1 = DemonType.TANK;
				SetupMoneyUI(300, false, true);
			}
		}
		else if (EventSystem.current.currentSelectedGameObject.gameObject.name == "invokeCard")
		{
			if (playerMoney >= 100)
			{
				random = Random.Range(20000, 30000);
				GameObject newDemon = Instantiate(demonsPrefabs[2], heroeReference[(int)playerSide].transform.position, heroeReference[(int)playerSide].transform.rotation);
				newDemon.GetComponent<DemonBehavior>().Side1 = playerSide;
				newDemon.GetComponent<DemonBehavior>().DemonType1 = DemonType.CARD;
				if (playerSide == Side.KING)
					newDemon.GetComponent<Rigidbody2D>().AddForce(Vector2.right * random);
				else if (playerSide == Side.QUEEN)
					newDemon.GetComponent<Rigidbody2D>().AddForce(Vector2.left * random);
				SetupMoneyUI(100, false, true);
			}
		}
	}

	public IEnumerator IA()
	{
		while (true)
		{
			int randomInvoke = Random.Range(0, 12);

			if (randomInvoke >= 0 && randomInvoke < 8)
			{
				if (enemyMoney >= 20)
				{
					float randomTimer = Random.Range(6f, 12f);
					yield return new WaitForSeconds(randomTimer);
					random = Random.Range(0, 3);
					GameObject newDemon = Instantiate(demonsPrefabs[0], spawnHandlers[(int)enemySide].transform.GetChild(random).transform);
					newDemon.GetComponent<DemonBehavior>().Side1 = enemySide;
					SetupMoneyUI(20, false, false);
				}
				yield return null;
			}
			else if (randomInvoke >= 8 && randomInvoke < 10)
			{
				if (enemyMoney >= 300)
				{
					float randomTimer = Random.Range(5f, 10f);
					yield return new WaitForSeconds(randomTimer);
					random = Random.Range(0, 3);
					GameObject newDemon = Instantiate(demonsPrefabs[1], spawnHandlers[(int)enemySide].transform.GetChild(random).transform);
					newDemon.GetComponent<DemonBehavior>().Side1 = enemySide;
					newDemon.GetComponent<DemonBehavior>().DemonType1 = DemonType.TANK;
					SetupMoneyUI(300, false, false);
				}
				yield return null;
			}
			else if (randomInvoke >= 10)
				if (enemyMoney >= 100)
				{
					float randomTimer = Random.Range(5f, 10f);
					yield return new WaitForSeconds(randomTimer);

					random = Random.Range(20000, 30000);
					GameObject newDemon = Instantiate(demonsPrefabs[2], heroeReference[(int)enemySide].transform.position, heroeReference[(int)enemySide].transform.rotation);
					newDemon.GetComponent<DemonBehavior>().Side1 = enemySide;
					newDemon.GetComponent<DemonBehavior>().DemonType1 = DemonType.CARD;
					if (enemySide == Side.KING)
						newDemon.GetComponent<Rigidbody2D>().AddForce(Vector2.right * random);
					else if (enemySide == Side.QUEEN)
						newDemon.GetComponent<Rigidbody2D>().AddForce(Vector2.left * random);
					SetupMoneyUI(100, false, false);
				}
			yield return null;
		}
	}

	#region Inscrição e trancamento nos eventos
	void OnEnable()
	{
		DemonBehavior.OnSendMoney += ReceiveMoney;
		TowerBehavior.OnGameOver += SetupGameOver;
	}

	void OnDisable()
	{
		DemonBehavior.OnSendMoney -= ReceiveMoney;
		TowerBehavior.OnGameOver -= SetupGameOver;
	}
	#endregion
}