using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuilometragemManeger : MonoBehaviour
{
    public Button PararQuilometragem;
    public Button IniciarQuilometragem;
    public Button PauseQuilometragem;
    private float distanciaPercorrida;
    public float numeroRotações;
    public static float ComprimentoDaPivela = 0.175f;
    private bool contandoQuilometragem = false;
    private Vector3 aceleracaoAnterior;
    private bool subiu = false;
    private float limiteDeteccao = 0.5f; // Ajustado para reduzir falsos positivos
    private float anguloAnterior;
    public TextMeshProUGUI textoQuilometragem;
    public TextMeshProUGUI Avisos;
    private int contadorSubida = 0;
    private float tempoUltimaPedalada = 0f;
     private float intervaloMinimo = 0.8f;
     private bool pausado = false;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }

        IniciarQuilometragem.onClick.AddListener(IniciarContagem);
        PararQuilometragem.onClick.AddListener(PararContagem);
        PauseQuilometragem.onClick.AddListener(AlternarPause);
        aceleracaoAnterior = Input.acceleration;
        anguloAnterior = Input.gyro.attitude.eulerAngles.x;
    }

    void Update()
    {
        if (!contandoQuilometragem) return;

        Vector3 aceleracao = Input.acceleration;
        float anguloAtual = Input.gyro.attitude.eulerAngles.x;
        
        print("Aceleração x: " + aceleracao.x + " | Ângulo: " + anguloAtual);
        
        if (aceleracao.x > aceleracaoAnterior.x + limiteDeteccao)
        {
            print("Perna subindo");
            contadorSubida++;
            if(contadorSubida>=1){
                subiu = true;
            }
        }
        else{
             contadorSubida=0;
        }

        if (subiu && aceleracao.x < aceleracaoAnterior.x - limiteDeteccao)
        {
            float diferencaAngulo = Mathf.Abs(anguloAtual - anguloAnterior);
            
            if (diferencaAngulo > 10f) 
            {
                if (Time.time - tempoUltimaPedalada > intervaloMinimo)
                {    
                    numeroRotações++;
                    tempoUltimaPedalada = Time.time;  
                    DistanciaTotal();
                    textoQuilometragem.text = "Distância: " + distanciaPercorrida.ToString("F2") + " metros";
                    print("Pedalada detectada! Total: " + numeroRotações);
                }
                contadorSubida = 0;
                subiu = false; 
                anguloAnterior = anguloAtual; 
            }
        }

        aceleracaoAnterior = aceleracao;
        if (!contandoQuilometragem || pausado) return;

    }

    void DistanciaTotal()
    {
        distanciaPercorrida = numeroRotações * CalculoCircunferenciaPedalada();
        print("Distância percorrida: " + distanciaPercorrida + " metros");
        string distancia = distanciaPercorrida.ToString("F2");
        textoQuilometragem.text = "Distância: " + distancia + " metros";
        
    }

    float CalculoCircunferenciaPedalada()
    {
        return 2.0f * Mathf.PI * ComprimentoDaPivela;
        
    }

    public void IniciarContagem()
    {
    contandoQuilometragem = true;
    numeroRotações = 0;
    distanciaPercorrida = 0;
    textoQuilometragem.text = "Distância: 0.00 metros";
    print("Botão INICIAR pressionado! Contagem deve começar.");
    Avisos.text = "Contagem iniciou";
    Invoke("EsconderAvisos", 5f);
    }

    public void PararContagem()
    {
        contandoQuilometragem = false;
        Avisos.text="Contagem parou";
        Avisos.gameObject.SetActive(true);
        Invoke("EsconderAvisos", 5f);
        print("Botão PARAR pressionado! Contagem deve parar.");

    }
     void EsconderAvisos(){
        Avisos.gameObject.SetActive(false);
     }
    public void AlternarPause()
    {
    pausado = !pausado;

    if (pausado)
    {
        Avisos.text = "Pausado";
    }
    else
    {
        Avisos.text = "Retomado";
    }

    Avisos.gameObject.SetActive(true);
    Invoke("EsconderAvisos", 3f);
}

}
