
using UnityEngine;

public class Knife : MonoBehaviour {

	
#region Public_Variables
	public float speed=1f;
	public bool isFire=false;
	public bool isHitted=false;
#endregion

	public Rigidbody2D rb;
	public AudioClip knifeHitsfx,ThrowKnifeSfx;

	[SerializeField] Vector2 throwForce;
	[SerializeField] Vector2 blastForce;
	float rndVelocity = 0f;

	void Start () {
		rb = GetComponentInChildren<Rigidbody2D> ();    
		rb.isKinematic = true;

		rndVelocity = Random.Range(20f, 50f) * 25f;
		blastForce.x = Random.Range(-5f, 5f);
	}
	
	public void ThrowKnife()
	{	
		if (!isFire && !GameManagerNinjaKnife.isGameOver)
		{
			Debug.Log("ThrowKnife");
			isFire = true;
			SoundManager.instance.PlaySingle(ThrowKnifeSfx);
			BoxCollider2D[] b = GetComponents<BoxCollider2D>();
			b[0].enabled = true;
			b[1].enabled = true;
			rb.isKinematic = false;
			rb.AddForce(throwForce, ForceMode2D.Impulse); //(new Vector2 (0f, speed), ForceMode2D.Impulse);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {

		Debug.Log("Coollide with: " + coll.gameObject.tag);
		if (coll.gameObject.tag == "Knife" && !isHitted && coll.gameObject.GetComponent<Knife> ().isFire && isFire && !GameManagerNinjaKnife.isGameOver)
		{
			SoundManager.instance.PlaySingle(knifeHitsfx);
			isHitted = true;
			GameManagerNinjaKnife.isGameOver = true;
			BoxCollider2D[] b = GetComponents<BoxCollider2D>();
			b[0].enabled = false;
			b[1].enabled = false;
			SoundManager.instance.playVibrate ();
			rb.freezeRotation = false;
			rb.linearVelocity = Vector2.zero;
			rb.angularVelocity = rndVelocity;//Random.Range (20f, 50f) * 25f;
			rb.AddForce(blastForce, ForceMode2D.Impulse);// new Vector2 (Random.Range (-5f, 5f), -30f), ForceMode2D.Impulse);
			DestroyMe ();
			Invoke ("gameOver", 0.5f);
			print ("Game  Over from Knife");
			//Application.LoadLevel ("Main");
		}
		else if (coll.gameObject.tag == "Wood" && !isHitted && !GameManagerNinjaKnife.isGameOver)
		{
			GamePlayManagerNinjaKnife.instance.StartCoroutine(GamePlayManagerNinjaKnife.instance.GenerateKnife());
			coll.gameObject.GetComponent<Circle>().OnKnifeHit(this);
		}
	}

	void gameOver()
	{
		GamePlayManagerNinjaKnife.instance.GameOver ();

	}
	public void DestroyMe()
	{
		LeanTween.alpha (gameObject, 0f, 2f).setOnComplete(()=>{
			Destroy(gameObject);
		});
	}

}
