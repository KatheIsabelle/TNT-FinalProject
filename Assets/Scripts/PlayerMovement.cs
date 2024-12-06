using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Script para movimentação do personagem principal, jogo 3D.Todos
os movimentos estão neste script, pulo, dash e movimento nos eixos x e z.

*/

public class PlayerMovement : MonoBehaviour
{   
    [Header ("Componentes Player")]
    private CharacterController controller;
    private Transform myCamera;
    private Animator animator;

    [Header ("Var Move and Jump")]
    public float velocity = 2f;  
    private bool isGround;  //verifica se está no chão
    private float yForce;  //força do salto 

    [Header("var Dash")]
    public float dashForce = 5f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private float dashTime = 0.2f; // Duração do dash

    [Header ("Fields")]
    //SerializeField permite que variáveis privadas sejam visíveis no inspector
    [SerializeField] private Transform foot;  //pe do personagem
    [SerializeField] private LayerMask Platform;  //camada do chão 


    void Start()
    {   
        //iniciando componentes
        //isDead = false;
        controller = GetComponent<CharacterController>();
        myCamera = Camera.main.transform; //maincamera transforme xyz
        animator = GetComponent<Animator>();
    }


    void Update()
    {   

        Move();
        Jump();
        Dash();

    }


    public void Move()
    {       

        //pegando input do teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // relaciona movimento player com movimento camera
        Vector3 movimento = new Vector3(horizontal, 0, vertical); //define x e z, 0 em y
        movimento = myCamera.TransformDirection(movimento);

        //zera movimento no eixo y porque deve aparecer só com o pulo 
        movimento.y = 0; 

        //controlador com metodo mover (movimento eixo x e y e camera * deltaTime * velocidade)
        controller.Move(movimento * Time.deltaTime * velocity);

        //aplicando gravidade no eixo y = -9.8f
        controller.Move(new Vector3(0, -9.8f, 0) * Time.deltaTime);
        //Debug.Log(movimento);

        //suavizando rotação da camera, movimento player = movimento camera
        if (movimento != Vector3.zero)
        {

            transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(movimento), Time.deltaTime * 10);
        }
        
        //chama animação de movimento quando movimento é dif de zero em xyz
        animator.SetBool("Move", movimento != Vector3.zero);
       
    } 


    public void Jump()
    {

        //verifica se está no chão
        isGround = Physics.CheckSphere(foot.position, 0.3f, Platform);
        animator.SetBool("isGround", isGround);
        
        //input espaço e verifica se está no chao 
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {   
            //adiciona força no eixo y e ativa animação
            yForce = 15f;
            animator.SetTrigger("Jump");
        }

        if(yForce > -9.81f)
        {
            //adiciona força no eixo y
            yForce += -9.8f * Time.deltaTime; 
        }

        //move player no eixo y de acordo com a força do pulo
        controller.Move(new Vector3(0, yForce, 0) * Time.deltaTime);
       
    }


    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {   
            StartCoroutine(PerformDash());
            Debug.Log("Apertou Shift");
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;

        // Direção do dash baseada no movimento atual do personagem
        Vector3 dashDirection = transform.forward;

        // Duração do dash
        float dashElapsed = 0f;

        // Realiza o dash enquanto o tempo do dash não é excedido
        while (dashElapsed < dashTime)
        {
            controller.Move(dashDirection * dashForce * Time.deltaTime);
            dashElapsed += Time.deltaTime;
            yield return null; // Aguarda o próximo frame
        }

        // Aguarda o cooldown antes de permitir outro dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


}
