using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovingObject : MonoBehaviour
{
	public bool DoRandom = true;
	public TextMeshProUGUI WarnText;
	public TextMeshProUGUI TypeText;
	public Vector3 speed = Vector3.zero; //1フレームで動く距離(マイナスは逆方向)
	public Vector3 distance = Vector3.zero; //この距離まで動
	public Vector3 SecondSpeed = Vector3.zero; //1フレームで動く距離(マイナスは逆方向)
	public Vector3 SeoondDistance = Vector3.zero; //この距離まで動
	public int NextStage = 10000;
	public float CooldownTime = 0f;
	//distanceまで動いた後に反対方向へ折り返して動くか？
	//falseだとdistanceまで動いたらそこで止る
	public bool turn = true;
	private int count = 0;
	private int random = 0;
	private int turncount = 0;
	private Vector3 moved = Vector3.zero; //移動した距離を保持
	private float NextCooldownTime = 0f;
	private List<GameObject> ride = new List<GameObject>(); //床に乗ってるオブジェクト
	private Vector3 StartPositon;
	private bool upper =true;
	private bool random_wave = false;
	private bool random_none = false;
	private bool random_normal = false;
	private float random_cooldown = 0f;
	public void Awake()
    {
		if(DoRandom)
        {
			random = Random.Range(0, 3);
			if(random == 0)
            {
				random_wave = true;
				random_cooldown = CooldownTime;
				TypeText.text = "Type:Random";
            }
			else if(random == 1)
            {
				random_none = true;
				TypeText.text = "Type:None";
			}
			else if(random == 2)
            {
				random_normal = true;
				TypeText.text = "Type:Normal";
			}
		}
		WarnText.text = "Lava:Stop";
		count = 0;
		turncount = 0;
		StartPositon = transform.position;
		NextCooldownTime = CooldownTime;
    }
	public void FixedUpdate()
	{
        if (!random_none)
        {
			if (CooldownTime <= 0f)
			{
				if (!upper)
				{
					WarnText.text = "Lava:Down...";
				}
				else
				{
					WarnText.text = "Lava:UP!!";
				}
				if (NextStage > count)
				{
					//床を動かす
					float x = speed.x;
					float y = speed.y;
					float z = speed.z;
					if (moved.x >= distance.x) x = 0;
					else if (moved.x + speed.x > distance.x) x = distance.x - moved.x;
					if (moved.y >= distance.y) y = 0;
					else if (moved.y + speed.y > distance.y) y = distance.y - moved.y;
					if (moved.z >= distance.z) z = 0;
					else if (moved.z + speed.z > distance.z) z = distance.z - moved.z;
					transform.Translate(x, y, z);
					//動いた距離を保存
					moved.x += Mathf.Abs(speed.x);
					moved.y += Mathf.Abs(speed.y);
					moved.z += Mathf.Abs(speed.z);
					if (moved.x >= distance.x && moved.y >= distance.y && moved.z >= distance.z && turn)
					{
						//Debug.Log(turncount + " Ok:" + count);
						speed *= -1; //逆方向へ動かす
						moved = Vector3.zero;
						CooldownTime = NextCooldownTime;
						WarnText.text = "Lava:Stop";
						if (turncount >= 1)
						{
							count++;
							turncount = 0;

						}
						else
						{
							WarnText.text = "Lava:Stop";
							turncount++;
							if (turncount >= 1)
							{
								count++;
								turncount = 0;

							}
						}
						if (upper)
						{
							upper = false;
						}
						else
						{
							upper = true;
						}
					}
					foreach (GameObject g in ride)
					{
						Vector2 v = g.transform.position;
						g.transform.position = new Vector3(v.x + x, v.y + y);   //yの移動は不要////////////
					}
				}
				if (NextStage <= count)
				{
					//床を動かす
					float x = SecondSpeed.x;
					float y = SecondSpeed.y;
					float z = SecondSpeed.z;
					if (moved.x >= SeoondDistance.x) x = 0;
					else if (moved.x + SecondSpeed.x > SeoondDistance.x) x = SeoondDistance.x - moved.x;
					if (moved.y >= SeoondDistance.y) y = 0;
					else if (moved.y + SecondSpeed.y > SeoondDistance.y) y = SeoondDistance.y - moved.y;
					if (moved.z >= SeoondDistance.z) z = 0;
					else if (moved.z + SecondSpeed.z > SeoondDistance.z) z = SeoondDistance.z - moved.z;
					transform.Translate(x, y, z);
					//動いた距離を保存
					moved.x += Mathf.Abs(SecondSpeed.x);
					moved.y += Mathf.Abs(SecondSpeed.y);
					moved.z += Mathf.Abs(SecondSpeed.z);
					if (moved.x >= SeoondDistance.x && moved.y >= SeoondDistance.y && moved.z >= SeoondDistance.z && turn)
					{
						SecondSpeed *= -1; //逆方向へ動かす
						moved = Vector3.zero;
						CooldownTime = NextCooldownTime;
					}
					foreach (GameObject g in ride)
					{
						Vector2 v = g.transform.position;
						g.transform.position = new Vector3(v.x + x, v.y + y);   //yの移動は不要////////////
					}
				}
			}
			if (CooldownTime >= 0f)
			{
				if(random_normal)
                {
					CooldownTime -= Time.deltaTime;
				}
				if(random_wave)
                {
					if(random_cooldown <= 0f)
                    {
						if (Random.Range(0, 10) == 9)
						{
							CooldownTime = -1;
						}
						else
						{
							random_cooldown = NextCooldownTime;
						}
					}
					if(random_cooldown >= 0f)
                    {
						random_cooldown -= Time.deltaTime;
                    }
                }
				if(!DoRandom)
                {
					CooldownTime -= Time.deltaTime;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//床の上に乗ったオブジェクトを保存
		ride.Add(other.gameObject);
		//Debug.LogError ("ride");
	}

	void OnTriggerExit2D(Collider2D other)
	{
		//床から離れたので削除
		ride.Remove(other.gameObject);
	}
}
