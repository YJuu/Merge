﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchController : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 3.0f; // 오브젝트 움직임 속도
    Vector3 movement; // 오브젝트 움직임 좌표
    GameObject traceTarget; // 추적할 대상 오브젝트
    bool isTracing;
    public Vector3 PlayerScale;
    public Vector3 playerPos;
    float skill_Active = 3.0f;  // 스킬 쿨타임
    float skill_timer = 0.0f;   // 시간 측정용 변수
    public GameObject Skill1;
    public GameObject Monster;
    float monster_Active = 15.0f;
    float monster_timer = 0.0f;
    private int maxHealth = 500;
    private int Health;
    public bool isDead;
    public float width = -2.0f;
    public float height = 2.0f;
    bool isHit = false;
    private float Tick_Timer = 2.0f;    // 체력 회복 간격
    private float Hit_Timer = 3.0f;     // 피격 판정 시간
    private float Tick_Time = 0.0f;
    private float Hit_Time = 0.0f;
    private float Death_Timer = 2.0f;   // 2페이즈 돌입 시 대기 시간
    private float Death_Time = 0.0f;
    public GameObject Witch_2;
    public GameObject DeathAnime;
    Vector3 M_Zone;
    GameObject My_SpwanZone;
    Vector3 moveVelocity = Vector3.zero;
    bool isout = false;
    private int shotpos;
    private bool onskill = false;
    private int skillcount = 0;
    GameObject director;
    private bool firstChat = true;
    private bool secondChat = true;
    private bool thirdChat = true;
    private bool fourthChat = true;
    private void Witch1_Dead(){
        GameObject Witch2 = Instantiate(Witch_2) as GameObject;
        Witch2.transform.position = new Vector3(-12,-6,0);
    }
    public void Set_Zone(Vector3 Zone){
        this.M_Zone = Zone;
    }
    public void Set_SpwanZone(GameObject Zone){
        this.My_SpwanZone = Zone;
        Set_Zone(My_SpwanZone.transform.position);
    }
    public void Zone_Out(){
        if(isTracing){
            moveVelocity = Vector3.zero;
        }
        else{
            //moveVelocity = (M_Zone-this.transform.position).normalized;
            float xx = Random.Range(-3.0f, 12.0f);
            float yy = Random.Range(-3.5f, 0f);
            Vector3 randomzone = new Vector3(xx,yy,1);
            moveVelocity = (randomzone - this.transform.position).normalized;
        }
    }
    public void damage(int dmg){    // 피격 시 실행할 데미지 함수
        this.Health -= dmg;
    }
    
    void Move()     // Witch의 움직임 함수
    {
        if (isTracing && !isout)
        {
            moveVelocity = Vector3.zero;
            playerPos = traceTarget.transform.position;

            if (playerPos.x < transform.position.x)
                moveVelocity += Vector3.left;
            else if (playerPos.x > transform.position.x)
                moveVelocity += Vector3.right;
            if (playerPos.y < transform.position.y)
                moveVelocity += Vector3.down;
            else if (playerPos.y > transform.position.y)
                moveVelocity += Vector3.up;
        }
        else if(isTracing && isout){
            moveVelocity = Vector3.zero;
        }
        else if(isout){
            float xx = Random.Range(-3.0f, 12.0f);
            float yy = Random.Range(-3.5f, 0f);
            Vector3 randomzone = new Vector3(xx,yy,1);
            moveVelocity = (randomzone - this.transform.position).normalized;
        }
        //if(moveVelocity.x > this.transform.position.x)
        if(moveVelocity.x > 0)
            transform.localScale = new Vector3(width,height,1);
        else
            transform.localScale = new Vector3(-width,height,1);
        transform.position += moveVelocity * speed * Time.deltaTime;
    }
    void OnCollisionEnter2D(Collision2D col){
        if(col.transform.name =="player"){ // 공격 스킬 피격판정시
        // col.transform.tag로 사용 가능, 단, tag 사용시 모든 스킬의 tag 변경할 것
            Health -= 10; // 피격물체에 따른 체력 감소
            isHit = true;
        }
        if(col.transform.tag == "HSkill01"){
            this.Health -= 60;
            isHit = true;
        }
        if(col.transform.tag == "HSkill02"){
            this.Health -= 100;
            isHit = true;
        }
        if(col.transform.tag == "GSkill01"){
            this.Health -= 35;
            Debug.Log(Health);
            isHit = true;
        }
    }
    void OnTriggerEnter2D(Collider2D col){  // Player 태그를 가진 콜라이더와 충돌시 추적 타겟을 설정
        if(col.gameObject.tag == "Player"){
            traceTarget = col.gameObject;
        }
        if(col.gameObject.tag == "Witch_Zone"){
            isout = false;
        }
    }
    void OnTriggerStay2D(Collider2D col){   // 추적중 함수
        if(col.gameObject.tag == "Player"){
            isTracing = true;
        }
    }
    void OnTriggerExit2D (Collider2D col){  // 추적 종료 함수, 난수이동 Coroutine 시작
        if(col.gameObject.tag == "Player"){
            isTracing = false;
        }
        if(col.gameObject.tag == "Witch_Zone"){
            isout = true;
            Debug.Log("ZO!");
            Zone_Out();
        }
    }
    void Die(){
        isDead = true;
        StopCoroutine(SkillShot());
        GameObject DeathAnim = Instantiate(DeathAnime) as GameObject;
        DeathAnim.transform.localScale = new Vector3(10,10,0);
        DeathAnim.transform.position = this.transform.position;
        StartCoroutine(Diee());
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        director = GameObject.FindWithTag("GameDirector");
        Health = maxHealth;
        isDead = false;
        float xx = Random.Range(-3.0f, 12.0f);
        float yy = Random.Range(-3.5f, 0f);
        Vector3 randomzone = new Vector3(xx,yy,1);
        moveVelocity = (randomzone-this.transform.position).normalized;
    }

    //코루틴 부분
    IEnumerator Diee(){
        yield return new WaitForSecondsRealtime(3.0f);
        Witch1_Dead();
        My_SpwanZone.GetComponent<Witch_Spwanzone>().witch_dead();
        Destroy(this.gameObject); // 몬스터 오브젝트 삭제
    }
    IEnumerator SkillShot(){
        GameObject skill = Instantiate(Skill1) as GameObject;   // 스킬 오브젝트 프리팹으로 생성
        skill.transform.position = new Vector3(shotpos,0,0)+this.transform.position;    // 스킬의 위치는 Witch 오브젝트의 위치로 생성
        skill.transform.localScale = new Vector3(-0.3f,0.3f,0);
        skillcount += 1;
        Debug.Log(skillcount);
        if(skillcount > 5){
            onskill = false;
            yield break;
        }
        yield return new WaitForSecondsRealtime(1.5f);
        StartCoroutine(SkillShot());
    }
    // void Skill(){
    //     StartCoroutine(SkillShot());
    // }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(Health < 400&&firstChat){
            firstChat = false;
            director.GetComponent<DialogueDirector>().setCondition();                
            director.GetComponent<DialogueDirector>().chatFunc(4);
        }
        if (Health < 250&&secondChat){
            secondChat = false;
            director.GetComponent<DialogueDirector>().setCondition();                
            director.GetComponent<DialogueDirector>().chatFunc(5);
        }
        if (Health < 100&&thirdChat){
            thirdChat = false;
            director.GetComponent<DialogueDirector>().setCondition();                
            director.GetComponent<DialogueDirector>().chatFunc(6);
        }
        if(skillcount > 5){
            StopCoroutine(SkillShot());
            skillcount = 0;
        }
        if(Health > maxHealth){ 
            Health = maxHealth;
        }
        if(Health <= 0){
            if(!isDead){
                //GetComponent<ParticleSystem>().Play();
                if (fourthChat) {
                    fourthChat = false;
                    director.GetComponent<DialogueDirector>().setCondition();
                    director.GetComponent<DialogueDirector>().chatFunc(7);
                }
                    Die();
            }
                return;
        }
        if(!isHit){
            Tick_Time += Time.deltaTime;
            if(Tick_Time > Tick_Timer){
                Health += 2;
                Tick_Time = 0.0f;
            }
        }
        else{
            Hit_Time += Time.deltaTime;
            Tick_Time = 0.0f;
            if(Hit_Time > Hit_Timer){
                isHit = false;
                Hit_Time = 0.0f;
            }
        }
        if(!onskill){
            Move();
            if(isTracing){
                skill_timer += Time.deltaTime;  // 스킬 발동을 위한 시간 측정
                if(skill_timer > skill_Active){ // 스킬 발동을 위한 시간이 쿨타임만큼 지나면
                   skill_timer = 0.0f;         // 시간 변수 초기화
                   GetComponent<AudioSource>().Play();
                   int rlrl = Random.Range(0,2);
                   if(rlrl > 0){
                       this.transform.position = new Vector3(traceTarget.transform.position.x-5,-6.4f,0);
                        transform.localScale = new Vector3(width,height,1);
                        shotpos = 2;
                        StartCoroutine(SkillShot());
                  }
                   else{
                        this.transform.position = new Vector3(traceTarget.transform.position.x+5,-6.4f,0);
                        transform.localScale = new Vector3(-width,height,1);
                        shotpos = -2;
                        StartCoroutine(SkillShot());
                    }
                    onskill = true;
                }
            }
            else{
                monster_timer += Time.deltaTime;
                if(isTracing)
                    monster_timer = 0.0f;
                if(monster_timer > monster_Active){
                    monster_timer = 0.0f;
                    GameObject Monsters = Instantiate(Monster) as GameObject;
                    int rnadomPosX = Random.Range(-3,3);
                    int rnadomPosY = Random.Range(-3,0);
                    Monsters.transform.position = new Vector3(this.transform.position.x+rnadomPosX,this.transform.position.y+rnadomPosY,this.transform.position.z);
                }
            }
        }
    }
}