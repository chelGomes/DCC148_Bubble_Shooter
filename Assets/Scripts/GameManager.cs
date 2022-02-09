using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
    #region Singleton
    public static GameManager instancia;

    private void Despertar(){
        if (instancia == null) {
            instancia = this;
        }
    }
    #endregion

    public Shooter atirandoRoteiro;
    public Transform ponteiroUltimaLinha;

    private int tamanho = 3;
    [SerializeField]
    private List<Transform> bolhaSequencia;

    void Start(){
        bolhaSequencia = new List<Transform>();

        LevelManager.instancia.GerarNivel();
        
        atirandoRoteiro.podeAtirar = true;
        atirandoRoteiro.CriarProximasBolhas();
    }

    void Update() {
        if (atirandoRoteiro.podeAtirar
            && Input.GetMouseButtonUp(0)
            && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > atirandoRoteiro.transform.position.y)){
            atirandoRoteiro.podeAtirar = false;
            atirandoRoteiro.Atirar();
        }
    }

    public void ProcessTurn(Transform bolhaAtual){
        bolhaSequencia.Clear();
        BolhaVerificacao(bolhaAtual);

        if(bolhaSequencia.Count >= tamanho){
            DestruirBolha();
            DerrubarBolhas();
        }

        LevelManager.instancia.ListarAtualizacaoBolhas();

        atirandoRoteiro.CriarProximasBolhas();
        atirandoRoteiro.podeAtirar = true;
    }

    private void BolhaVerificacao(Transform bolhaAtual){
        bolhaSequencia.Add(bolhaAtual);

        Bubble roteiroBolha = bolhaAtual.GetComponent<Bubble>();
        List<Transform> vizinhanca = roteiroBolha.GetNeighbors();

        foreach(Transform t in vizinhanca) {
            if (!bolhaSequencia.Contains(t)){

                Bubble bScript = t.GetComponent<Bubble>();

                if (bScript.bubbleColor == roteiroBolha.bubbleColor){
                    BolhaVerificacao(t);
                }
            }
        }
    }

    private void DestruirBolha(){
        foreach(Transform t in bolhaSequencia){
            Destroy(t.gameObject);
        }
    }

    private void DerrubarBolhas(){
        ConexaoBolhaFalsa();
        ConectadaBolhaVerdadeira();
        GravidadeBolhasDesconectadas();
    }

    #region Drop Disconected Bubbles
    private void ConexaoBolhaFalsa(){
        foreach (Transform bolha in LevelManager.instancia.aresBolhas){
            bolha.GetComponent<Bubble>().Conectado = false;
        }
    }

    private void ConectadaBolhaVerdadeira(){
        bolhaSequencia.Clear();

        RaycastHit2D[] batendo = Physics2D.RaycastAll(ponteiroUltimaLinha.position, ponteiroUltimaLinha.right, 15f);

        for (int i = 0; i < batendo.Length; i++){
            if (batendo[i].transform.gameObject.tag.Equals("Bubble"))
                DefinirVizinhosConectados(batendo[i].transform);
        }
    }

    private void DefinirVizinhosConectados(Transform bolha){
        Bubble roteiroBolha = bolha.GetComponent<Bubble>();
        roteiroBolha.Conectado = true;
        bolhaSequencia.Add(bolha);

        foreach(Transform t in roteiroBolha.GetNeighbors()){
            if(!bolhaSequencia.Contains(t)){
                DefinirVizinhosConectados(t);
            }
        }
    }

    private void GravidadeBolhasDesconectadas(){
        foreach (Transform bolha in LevelManager.instancia.aresBolhas){
            if (!bolha.GetComponent<Bubble>().Conectado){
                bolha.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                if(!bolha.GetComponent<Rigidbody2D>()){
                    Rigidbody2D rb2d = bolha.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
                }       
            }
        }
    }
    #endregion
}