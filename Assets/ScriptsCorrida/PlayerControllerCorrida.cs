using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCorrida : MonoBehaviour
{
    private Vector3 direcao;
    private Animator animator;
    public float velocidade = 5;
    private Rigidbody rb;
    public InterfaceControllerCorrida scriptInterfaceControllerCorrida;
    public float distanciaFrente = 5f;
    private bool caiu = false;
    private int latasPerdidas = 3;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Mover();

        if (transform.position.y < -4 && !caiu)
        {
            caiu = true;
            Reposicionar();
        }
    }

    private void Reposicionar()
    {
        PerdeLatas(latasPerdidas);
        Transform cameraTransform = Camera.main.transform;

        Vector3 posicaoNaFrente = cameraTransform.position + cameraTransform.forward * distanciaFrente;
        posicaoNaFrente.y = 0;

        // Reposicionar o personagem em vez de instanciar
        transform.position = posicaoNaFrente;
        transform.rotation = Quaternion.identity;

        caiu = false; // Permite que ele caia novamente no futuro
    }

    private void Mover()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direcao = new Vector3(horizontal, 0, vertical);

        rb.MovePosition(rb.position + direcao.normalized * velocidade * Time.deltaTime);

        if (direcao != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direcao), Time.deltaTime * 10);
        }

        animator.SetBool("Move", direcao != Vector3.zero);
    }

    public void ColetarLatas()
    {
        scriptInterfaceControllerCorrida.AumentarQuantidadeDeLatasColetadas();
    }

    public void PerdeLatas(float latasPerdidas)
    {
        
        scriptInterfaceControllerCorrida.DiminuirQuantidadeLatasColetadas(latasPerdidas);
        
    }
}
