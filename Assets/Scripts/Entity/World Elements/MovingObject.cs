using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovingObject : MonoBehaviour
{
	public bool DoRandom = true;
	public TextMeshProUGUI WarnText;
	public TextMeshProUGUI TypeText;
	public Vector3 speed = Vector3.zero; //1�t���[���œ�������(�}�C�i�X�͋t����)
	public Vector3 distance = Vector3.zero; //���̋����܂œ�
	public Vector3 SecondSpeed = Vector3.zero; //1�t���[���œ�������(�}�C�i�X�͋t����)
	public Vector3 SeoondDistance = Vector3.zero; //���̋����܂œ�
	public int NextStage = 10000;
	public float CooldownTime = 0f;
	//distance�܂œ�������ɔ��Ε����֐܂�Ԃ��ē������H
	//false����distance�܂œ������炻���Ŏ~��
	public bool turn = true;
	private int count = 0;
	private int random = 0;
	private int turncount = 0;
	private Vector3 moved = Vector3.zero; //�ړ�����������ێ�
	private float NextCooldownTime = 0f;
	private List<GameObject> ride = new List<GameObject>(); //���ɏ���Ă�I�u�W�F�N�g
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
					//���𓮂���
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
					//������������ۑ�
					moved.x += Mathf.Abs(speed.x);
					moved.y += Mathf.Abs(speed.y);
					moved.z += Mathf.Abs(speed.z);
					if (moved.x >= distance.x && moved.y >= distance.y && moved.z >= distance.z && turn)
					{
						//Debug.Log(turncount + " Ok:" + count);
						speed *= -1; //�t�����֓�����
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
						g.transform.position = new Vector3(v.x + x, v.y + y);   //y�̈ړ��͕s�v////////////
					}
				}
				if (NextStage <= count)
				{
					//���𓮂���
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
					//������������ۑ�
					moved.x += Mathf.Abs(SecondSpeed.x);
					moved.y += Mathf.Abs(SecondSpeed.y);
					moved.z += Mathf.Abs(SecondSpeed.z);
					if (moved.x >= SeoondDistance.x && moved.y >= SeoondDistance.y && moved.z >= SeoondDistance.z && turn)
					{
						SecondSpeed *= -1; //�t�����֓�����
						moved = Vector3.zero;
						CooldownTime = NextCooldownTime;
					}
					foreach (GameObject g in ride)
					{
						Vector2 v = g.transform.position;
						g.transform.position = new Vector3(v.x + x, v.y + y);   //y�̈ړ��͕s�v////////////
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
		//���̏�ɏ�����I�u�W�F�N�g��ۑ�
		ride.Add(other.gameObject);
		//Debug.LogError ("ride");
	}

	void OnTriggerExit2D(Collider2D other)
	{
		//�����痣�ꂽ�̂ō폜
		ride.Remove(other.gameObject);
	}
}
