using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceControllerCorrida : MonoBehaviour
{

    private float quantidadeDeLatasColetadas;
    public Text textoQuantidadeDeLatasColetadas;
    public Text textoPerdeuLatas;

    public void AumentarQuantidadeDeLatasColetadas()
    {
        quantidadeDeLatasColetadas++;
        textoQuantidadeDeLatasColetadas.text = $"x {quantidadeDeLatasColetadas}";
    }

    public void DiminuirQuantidadeLatasColetadas(float latasPerdidas)
    {
        if (quantidadeDeLatasColetadas < 3)
        {
            quantidadeDeLatasColetadas -= quantidadeDeLatasColetadas;
        } else
        {
            quantidadeDeLatasColetadas -= latasPerdidas;
        }
        textoQuantidadeDeLatasColetadas.text = $"x {quantidadeDeLatasColetadas}";
        StartCoroutine(DesaparecerTexto());
    }

    IEnumerator DesaparecerTexto()
    {
        textoPerdeuLatas.gameObject.SetActive(true);
        Color corTexto = textoPerdeuLatas.color;
        corTexto.a = 1;
        textoPerdeuLatas.color = corTexto;
        yield return new WaitForSeconds(0.5f);
        float contador = 0;
        while (textoPerdeuLatas.color.a > 0)
        {
            contador += Time.deltaTime / 0.5f;
            corTexto.a = Mathf.Lerp(1, 0, contador);
            textoPerdeuLatas.color = corTexto;
            if (textoPerdeuLatas.color.a <= 0)
            {
                textoPerdeuLatas.gameObject.SetActive(false);
            }
            yield return null;
        }
    }
}
