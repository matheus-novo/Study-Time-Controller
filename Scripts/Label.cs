using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public Text categoria, data, hora, tempo;
    public int thisID;
    public GameObject botaoRemover;
    public bool ativarBotao = false;

    //public List<Session> listaPrivada;

    public void Update()
    {
        if (ativarBotao == false)
        {
            botaoRemover.SetActive(false);
        }
        else
        {
            botaoRemover.SetActive(true);
        }
    }
    public void Destruir()
    {
        foreach (Session secao in Timer.secoesEstudo)
        {
            if (secao.Id == thisID)
            {
                Timer.secoesEstudo.Remove(secao);
                break;
            }
        }
        Destroy(this.gameObject);
    }

    public void Editar()
    {
        if (ativarBotao == false)
        {
            ativarBotao = true;
        }
        if (ativarBotao == true)
        {
            ativarBotao = false;
        }
    }
}
