﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public Slider slider;
    public Canvas mybag;
    public int maxJumpTime = 1;
    public float hp=100;
    public float normalspeed = 5;
    //public float rushspeed = 15;
    public float jumpSpeed = 30;
    public bool ifground;
    public bool[] chip=new bool[10];
    public float hitBoxCd;

    private PolygonCollider2D polygonCollider;
    AudioSource audio;
    BoxCollider2D collider;
    Animator animator;
    Rigidbody2D rigidbody;
    int isflip = 0;
    int nowjump=0;
    bool iscrouch;
    public bool isboxnearBy;
    bool canleft;
    bool canright;
    bool canjump;
    bool ifbag=false;
    bool canAttract;
    bool cancrouch;
    Vector2 tempcolliderSize;
    //float rushCoolTime;//冲刺时间
    //float coolrush = 1;//冲刺冷却
    bool rushflag = false;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        tempcolliderSize = collider.size;
        iscrouch = false;
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Horizontal");
        hpManager();
        chipUpdate();
        openBag();
        CheckOnTheGround();
        //if (canrush==true) Rush();
        Run(v);
        if (Input.GetButtonDown("Jump") && canjump ==true) Jump();
        if (cancrouch) Crouch(false);
        if (canAttract) Attract();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            isboxnearBy = true;
        }
        else isboxnearBy = false;
    }
    public void takeDamage(float damage)
    {
        hp -= damage;
        polygonCollider.enabled = false;
        StartCoroutine(ShowPlayerHitBox());
    }

    IEnumerator ShowPlayerHitBox()
    {
        yield return new WaitForSeconds(hitBoxCd);
        polygonCollider.enabled = true;
    }
    public void act(int property)
    {
        if (property == 0) Run(-1);
        else if (property == 1) Run(1);
        else if (property == 2) Jump();
        else if (property == 3) Crouch(true);
    }
    void openBag()
    {
        if (Input.GetKeyDown(KeyCode.E))
            mybag.gameObject.SetActive(ifbag);
        ifbag = !ifbag;
    }
    void hpManager()
    {
        slider.value = hp / 100.0f;
    }
    void chipUpdate()
    {
        if (chip[0] == true) canleft = true; else canleft = false;
        if (chip[1] == true) canright = true; else canright = false;
        if (chip[2] == true) canjump = true; else canjump = false;
        if (chip[3] == true) cancrouch = true;else cancrouch = false;
        if (chip[4] == true) canAttract = true;else canAttract = false;
    }
    void Attract()
    {

    }
    void Crouch(bool flag)
    {
        if (Input.GetKeyDown(KeyCode.S) || flag == true) iscrouch = !iscrouch;
        if (iscrouch==true)
        {
            animator.SetBool("Crouch", true);
            GetComponent<BoxCollider2D>().offset = new Vector2(0, -tempcolliderSize.y/4);
            GetComponent<BoxCollider2D>().size = new Vector2(tempcolliderSize.x, tempcolliderSize.y / 2);
            //改变碰撞体积
        }
        else
        {
            animator.SetBool("Crouch", false);
            GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
            GetComponent<BoxCollider2D>().size = new Vector2(tempcolliderSize.x, tempcolliderSize.y);
        }
    }
    /*void Rush()
    {
        float rushdir=0;
        rushCoolTime += Time.deltaTime;
        coolrush += Time.deltaTime;
        rushCoolTime = rushCoolTime > 10 ? 10 : rushCoolTime;
        coolrush = coolrush > 10 ? 10 : coolrush;
        float v = Input.GetAxis("Horizontal");
        if (v>0) rushdir = 1;
        else if (v<0) rushdir = -1;
        if (coolrush>0.7f&&rushflag==false&&rushdir!=0&&((ifground==false&&flyrush==false)||(ifground==true)))
        {
            if (ifground == false) flyrush = true;
            rigidbody.gravityScale=0;
            rushflag = true;
            Vector2 playervel = new Vector2(rushspeed*rushdir, 0);
            rigidbody.velocity = playervel;
            Flip(rushdir);
            animator.SetBool("Run", true);
            rushCoolTime = 0;
            coolrush = 0;
        }
        if (rushCoolTime > 0.3f && rushflag == true)
        {
            rigidbody.gravityScale = gravity;
            rushflag =false;animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
        }
    }*/
    bool CheckOnTheGround()
    {
        ifground = collider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (ifground == true) nowjump = maxJumpTime;
        //if (ifground == true) flyrush = false;
        return ifground;
    }
    void Jump()
    {
        if (nowjump>0)
        {
            nowjump--;
            Vector2 jumpvel = new Vector2(rigidbody.velocity.x, jumpSpeed);
            rigidbody.velocity = Vector2.up * jumpvel;
            animator.SetBool("Jump", true);
        }
        animator.SetBool("Jump", false);
    }
    void Run(float v)
    {
        if (v != 0)
        {
            audio.Play();
            if (v < 0 && canleft == false) return;
            if (v > 0 && canright == false) return;
            Vector2 playervel = new Vector2(v * normalspeed, rigidbody.velocity.y);
            rigidbody.velocity = playervel;
            Flip(v);
            if (iscrouch) animator.SetBool("crouchwalk", true);
            else
            if (isboxnearBy) animator.SetBool("pushBox", true);
            else animator.SetBool("Run", true);
            if (!isboxnearBy) animator.SetBool("pushBox", false);
        }
        else
        {
            audio.Stop();
            animator.SetBool("pushBox", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Run", false);
            animator.SetBool("crouchwalk", false);
        }
    }
    void Flip(float v)
    {
        Vector3 temp = transform.localScale;
        if ((v > 0.1f && isflip == 1) || (v < -0.1f && isflip == 0))
        {
            temp.x *= -1;
            isflip ^= 1;
        }
        transform.localScale = temp;
    }
}