using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineEnemy : MonoBehaviour
{
    
    public float speed;
    public float JumpForce;
    private Transform target;
    public float stoppingDistance;
    public float farDistance;
    public Rigidbody2D rb;
    public Transform groundCheckPoint;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isTouchingGround;

    public bool d�nme;

    public Animator anim;

    Collider2D myColLastHit;

    private static bool z�pla;
    public Collider2D jumpCol;
    public Collider2D bodyCol;

    int rnd = 0;

    public int can;
    public bool DieControl = true;

    //4 farkl� animasyonda �lebilir
    int rndAnim; //random die Animation
    int rndDmgS; //random hasar sesi

    bool IdleSoundControl = false;
    int rndIdleSound;

    public AudioSource SesKayna��; //ses ��kar�c�

    public PhotonView photonView;
    public GameObject COIN;
    bool amIdead;


    ////big optimizatin update //en yak�n oyuncuyu bulma (Baz� k�s�mlar mobil cihazlarda kasma yapt���ndan k�rpmalar yapt�m)
    bool closePlayerSelecter = false;
    bool despawnolamm�control = false;

    float distanceToClosestEnemy = Mathf.Infinity;
    float distanceToEnemy;
    Player closestEnemy = null;
    Player[] allEnemies;
    float distanceFory = 0f;
    float farDistanceForAnimator = 13f;

    //30 6
    void Start()
    {
        gameObject.GetComponent<Animator>().enabled = false; //en ba�ta animasy�nlar kapal�

        amIdead = false;
        d�nme = true;
        int rnd = Random.Range(0, 10);
        if (rnd == 1)
        {
            //  Physics2D.IgnoreLayerCollision(11, 11, true); //zombilerin �arp��mamas� i�in //zombilerin i�inden ge�me
        }

        Physics2D.IgnoreLayerCollision(11, 16, true); //bariyerlerin i�inden ge�me
        Physics2D.IgnoreLayerCollision(11, 17, true); //coinlerin i�inden ge�me
        Physics2D.IgnoreLayerCollision(11, 15, true); // i�inden ge�me

        //Diffuculty e g�re de�i�iklikler
        if (StartZoneScript.DiffucultyGAME == 0)
        {
            can = 3;
            speed = 2f;
        }
        if (StartZoneScript.DiffucultyGAME == 1)
        {
            can = 3;
            speed = 2.5f;
        }
        if (StartZoneScript.DiffucultyGAME == 2)
        {
            can = 4;
            speed = 3f;
        }
        if (StartZoneScript.DiffucultyGAME == 3)
        {
            can = 5;
            speed = 4f;
        }



        ////big optimizatin update
        ////
        StartCoroutine(IdleSound());

        distanceFory = 10f;

        //if (SceneManager.GetActiveScene().name == "S.School" || SceneManager.GetActiveScene().name == "S.School1")
        //{
        //    distanceFory = 10f;
        //}
        //else
        //{
        //    distanceFory = 25f;
        //}

        ////
    }


    void Update()
    {

        isTouchingGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer); //GroundCheck

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {

            ////big optimizatin update
            ////Closes Player Control
            //float distanceToClosestEnemy = Mathf.Infinity;
            //Player closestEnemy = null;
            //Player[] allEnemies = GameObject.FindObjectsOfType<Player>();

            //foreach (Player currentEnemy in allEnemies)
            //{
            //    float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            //    if (distanceToEnemy < distanceToClosestEnemy)
            //    {
            //        distanceToClosestEnemy = distanceToEnemy;
            //        closestEnemy = currentEnemy;
            //    }
            //}
            //target = closestEnemy.transform;
            ////
            ///
            ////big optimizatin update
            allEnemies = GameObject.FindObjectsOfType<Player>();
            if (closePlayerSelecter == false)
            {
                StartCoroutine(ClosestPlayerOP());
            }


            //if (despawnolamm�control == false && SceneManager.GetActiveScene().name != "ResulationOPTIMIZATION")
            //{
            //    StartCoroutine(DespawnOlamm�());
            //}
            StartCoroutine(DespawnOlamm�());
            ////


            if (Vector2.Distance(transform.position, target.position) > stoppingDistance && Vector2.Distance(transform.position, target.position) < farDistance) //y�r�me
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }



            ////big optimizatin update
            //if(SceneManager.GetActiveScene().name != "ResulationOPTIMIZATION")
            //{
            //    //if ((transform.position.x - target.position.x) > farDistance) //Uzaktaki zombileri despawn etmek i�in
            //    //{
            //    //    StartCoroutine(DeadAGA());
            //    //}
            //    //if ((transform.position.x - target.position.x) < -farDistance) //Uzaktaki zombileri despawn etmek i�in
            //    //{
            //    //    StartCoroutine(DeadAGA());
            //    //}

            //    //if ((transform.position.y - target.position.y) > 5f && Vector2.Distance(transform.position, target.position) < 10)
            //    //{
            //    //    StartCoroutine(DeadAGA());
            //    //}
            //    //if ((transform.position.y - target.position.y) < -5f && Vector2.Distance(transform.position, target.position) < 10)
            //    //{
            //    //    StartCoroutine(DeadAGA());
            //    //}
            //}
            ////


            if (can > 0) //�ld�kten sonra d�nmesin
            {
                if (target.position.x < rb.position.x)
                {
                    if (d�nme == false)
                    {
                        transform.Rotate(0, 180, 0);
                        d�nme = true;

                    }
                }
                if (target.position.x > rb.position.x)
                {
                    if (d�nme == true)
                    {
                        transform.Rotate(0, 180, 0);
                        d�nme = false;

                    }
                }
            }

            //y�r�me
            if (Vector2.Distance(transform.position, target.position) > stoppingDistance && rb.velocity.y < 0.3f && rb.velocity.y > -2f && can > 0)
            {
                if (rnd == 0)
                {
                    rnd = Random.Range(1, 5);
                }

                if (rnd == 1)
                {
                    anim.SetBool("isRunning", true);
                }
                else if (rnd == 2)
                {
                    anim.SetBool("isRunning1", true);
                }
                else if (rnd == 3)
                {
                    anim.SetBool("isRunning2", true);
                }
                else if (rnd == 4)
                {
                    anim.SetBool("isRunning3", true);
                }
            }
            else
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isRunning1", false);
                anim.SetBool("isRunning2", false);
                anim.SetBool("isRunning3", false);

                rnd = 0;

            }

            //sald�r�
            if (Vector2.Distance(transform.position, target.position) < 2f)
            {
                anim.SetBool("isAttact", true);
            }
            else
            {
                anim.SetBool("isAttact", false);
            }
        }





        //z�plama -  d��me
        if (rb.velocity.y < 0.3f && rb.velocity.y > -2f)
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isfall", false);
        }
        if (rb.velocity.y > 0.3f && can > 0)
        {
            anim.SetBool("isJump", true);
            anim.SetBool("isfall", false);
        }
        if (rb.velocity.y < -2f && can > 0)
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isfall", true);
        }


        if (rb.velocity.x < 0.1f && (isTouchingGround == true))
        {

            //rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);

        }


        if (z�pla == true && isTouchingGround == true)
        {
            //   rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
            z�pla = false;
        }

        if (can <= 0 && amIdead == false)  //�ld�kten sonras�
        {
            amIdead = true;


            anim.SetBool("isJump", false);
            anim.SetBool("isfall", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isRunning1", false);
            anim.SetBool("isRunning2", false);
            anim.SetBool("isRunning3", false);
            rndAnim = Random.Range(0, 5);
            if (rndAnim == 0)
            {
                anim.SetBool("isDead0", true);
                if (DieControl == true) { SoundManager.PlaySound("ZombieDie0", SesKayna��); DieControl = false; }


            }

            if (rndAnim == 1)
            {
                anim.SetBool("isDead1", true);
                if (DieControl == true) { SoundManager.PlaySound("ZombieDie1", SesKayna��); DieControl = false; }

            }

            if (rndAnim == 2)
            {
                anim.SetBool("isDead2", true);
                if (DieControl == true) { SoundManager.PlaySound("ZombieDie2", SesKayna��); DieControl = false; }

            }
            if (rndAnim == 3 || rndAnim == 4)
            {
                anim.SetBool("isDead3", true);
                if (DieControl == true) { SoundManager.PlaySound("ZombieDie3", SesKayna��); DieControl = false; }

            }
            StartCoroutine(DeadAGA());
            if (photonView.isMine) //online
            {
                if (SceneManager.GetActiveScene().name != "Tutorial")
                {
                    PhotonNetwork.Instantiate(COIN.name, gameObject.transform.position, Quaternion.identity, 0);
                }
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //VURU� H�SS�
        if (collision.gameObject.tag == "bulletr")
        {
            if (photonView.isMine)
            {
                Destroy(collision);
                d�nme = true;
                rb.AddForce(new Vector2(2f, 0), ForceMode2D.Impulse);
                Dead();
            }
        }
        if (collision.gameObject.tag == "bulletl")
        {
            if (photonView.isMine)
            {
                Destroy(collision);
                d�nme = false;
                rb.AddForce(new Vector2(-2f, 0), ForceMode2D.Impulse);
                Dead();
            }
        }
        if (collision.gameObject.tag == "Zdespawn")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "bosluk")
        {
            StartCoroutine(DeadAGA());
        }
    }
    public void OnTriggerStay2D(Collider2D collision)  //kar��s�na bi�ey gelirse z�pala
    {
        if (jumpCol == true && collision.gameObject.tag == "ground" && isTouchingGround == true)
        {
            rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }

    }
    public void Dead()
    {
        can--;
        photonView.RPC("Health", PhotonTargets.AllBuffered, can);
        if (can > 0)
        {
            rndDmgS = Random.Range(0, 2);
            if (rndDmgS == 0) { SoundManager.PlaySound("ZombieDamage0", SesKayna��); }
            else if (rndDmgS == 1) { SoundManager.PlaySound("ZombieDamage1", SesKayna��); }

        }

    }
    private IEnumerator DeadAGA()
    {
        //colliderlar� sildim
        Destroy(gameObject.GetComponent<CapsuleCollider2D>()); //�ld�kten sonra hasar verememe
        Destroy(gameObject.GetComponent<CircleCollider2D>()); //�ld�kten sonra hasar verememe

        //gameObject.GetComponent<Animator>().enabled = true; //bu sayede activve ve deactive edilecek

        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(0, 0, 0);

    }



    IEnumerator IdleSound() //ara s�ra ses yap
    {
    Ba�:
        IdleSoundControl = true;
        yield return new WaitForSeconds(5f);
        rndIdleSound = Random.Range(0, 5);
        if (rndIdleSound == 0)
        {
            SoundManager.PlaySound("ZombieIdle0", SesKayna��);
        }
        if (rndIdleSound == 1)
        {
            SoundManager.PlaySound("ZombieIdle1", SesKayna��);
        }
        if (rndIdleSound == 2)
        {
            SoundManager.PlaySound("ZombieIdle2", SesKayna��);
        }
        //
        goto Ba�;
        //
        IdleSoundControl = false;

    }

    ////big optimizatin update
    IEnumerator ClosestPlayerOP()
    {
        closePlayerSelecter = true;

        distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;

        foreach (Player currentEnemy in allEnemies)
        {
            distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;
            }
        }
        target = closestEnemy.transform;

        yield return new WaitForSeconds(0.5f);
        closePlayerSelecter = false;

    }

    IEnumerator DespawnOlamm�()
    {
        despawnolamm�control = true;
        if ((transform.position.x - target.position.x) > 25f) //Uzaktaki zombileri despawn etmek i�in
        {
            StartCoroutine(DeadAGA());
        }
        if ((transform.position.x - target.position.x) < -25f) //Uzaktaki zombileri despawn etmek i�in
        {
            StartCoroutine(DeadAGA());
        }

        if (Vector2.Distance(transform.position, target.position) < farDistanceForAnimator) //animasyon a�ma kapama  (uzaktayken animasyon yok)
        {
            gameObject.GetComponent<Animator>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Animator>().enabled = false;
        }

        if (Mathf.Abs(transform.position.y - target.position.y) > 5f && Vector2.Distance(transform.position, target.position) > distanceFory)
        {
            StartCoroutine(DeadAGA());
        }

        yield return new WaitForSeconds(1f);
        despawnolamm�control = false;
    }
 



    //canEnemy datas�n� server a yollar-al�r
    //photonView.RPC("Health", PhotonTargets.AllBuffered, can);
    [PunRPC] //online a veri yollama
    public void Health(int HealAmount)
    {
        if (photonView.isMine)
        {
            can = HealAmount;
        }
        else
        {
            can = HealAmount;
        }
    }

}
