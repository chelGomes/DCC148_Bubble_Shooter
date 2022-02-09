using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour{
    public Transform armaFogo;
    public bool podeAtirar;
    public float velocidade = 6f;

    public Transform proximaPosicaoBolha;
    public GameObject bolhaAtual;
    public GameObject proximaBolha;

    private Vector2 direcao;
    private float angulo;
    public bool trocado = false;
    public float tempo = 0.02f;

    public void Update(){
        direcao = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        armaFogo.rotation = Quaternion.Euler(0f, 0f, angulo - 90f);

        if(trocado){
            if(Vector2.Distance(bolhaAtual.transform.position, proximaPosicaoBolha.position) <= 0.2f
                && Vector2.Distance(proximaBolha.transform.position, transform.position) <= 0.2f){
                proximaBolha.transform.position = transform.position;
                bolhaAtual.transform.position = proximaPosicaoBolha.position;

                bolhaAtual.GetComponent<Collider2D>().enabled = true;
                proximaBolha.GetComponent<Collider2D>().enabled = true;

                trocado = false;

                GameObject reference = bolhaAtual;
                bolhaAtual = proximaBolha;
                proximaBolha = reference;
            }

            proximaBolha.transform.position = Vector2.Lerp(proximaBolha.transform.position, transform.position, tempo);
            bolhaAtual.transform.position = Vector2.Lerp(bolhaAtual.transform.position, proximaPosicaoBolha.position, tempo);
        }
    }

    public void Atirar(){
        transform.rotation = Quaternion.Euler(0f, 0f, angulo - 90f);
        bolhaAtual.transform.rotation = transform.rotation;
        bolhaAtual.GetComponent<Rigidbody2D>().AddForce(bolhaAtual.transform.up * velocidade, ForceMode2D.Impulse);
        bolhaAtual = null;
    }

    [ContextMenu("SwapBubbles")]
    public void TrocarBolhas(){
        bolhaAtual.GetComponent<Collider2D>().enabled = false;
        proximaBolha.GetComponent<Collider2D>().enabled = false;
        trocado = true;
    }

    [ContextMenu("CreateNextBubble")]
    public void CriarProximasBolhas(){
        List<GameObject> bolhasCena = LevelManager.instancia.bolhasCena;
        List<string> cores = LevelManager.instancia.coresCena;

        if (proximaBolha == null){
            proximaBolha = InstanciarNovaBolha(bolhasCena);
        }
        else{
            if(!cores.Contains(proximaBolha.GetComponent<Bubble>().bubbleColor.ToString())){
                Destroy(proximaBolha);
                proximaBolha = InstanciarNovaBolha(bolhasCena);
            }
        }

        if(bolhaAtual == null){
            bolhaAtual = proximaBolha;
            bolhaAtual.transform.position = new Vector2(transform.position.x, transform.position.y);
            proximaBolha = InstanciarNovaBolha(bolhasCena);
        }
    }

    private GameObject InstanciarNovaBolha(List<GameObject> bolhasCena){
        GameObject novaBolha = Instantiate(bolhasCena[(int)(Random.Range(0, bolhasCena.Count * 1000000f) / 1000000f)]);
        novaBolha.transform.position = new Vector2(proximaPosicaoBolha.position.x, proximaPosicaoBolha.position.y);
        novaBolha.GetComponent<Bubble>().Fixo = false;
        Rigidbody2D rb2d = novaBolha.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        rb2d.gravityScale = 0f;

        return novaBolha;
    }
}
