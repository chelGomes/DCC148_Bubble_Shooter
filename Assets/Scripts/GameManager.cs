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
        
        atirandoRoteiro.canShoot = true;
        atirandoRoteiro.CreateNextBubble();
    }

    void Update() {
        if (atirandoRoteiro.canShoot
            && Input.GetMouseButtonUp(0)
            && (Camera.main.ScreenToWorldPoint(Input.mousePosition).y > atirandoRoteiro.transform.position.y)){
            atirandoRoteiro.canShoot = false;
            atirandoRoteiro.Shoot();
        }
    }

    public void ProcessTurn(Transform currentBubble){
        bolhaSequencia.Clear();
        CheckBubbleSequence(currentBubble);

        if(bolhaSequencia.Count >= tamanho){
            DestroyBubblesInSequence();
            DropDisconectedBubbles();
        }

        LevelManager.instancia.ListarAtualizacaoBolhas();

        atirandoRoteiro.CreateNextBubble();
        atirandoRoteiro.canShoot = true;
    }

    private void CheckBubbleSequence(Transform currentBubble){
        bolhaSequencia.Add(currentBubble);

        Bubble roteiroBolha = currentBubble.GetComponent<Bubble>();
        List<Transform> vizinhanca = roteiroBolha.GetNeighbors();

        foreach(Transform t in vizinhanca) {
            if (!bolhaSequencia.Contains(t)){

                Bubble bScript = t.GetComponent<Bubble>();

                if (bScript.bubbleColor == roteiroBolha.bubbleColor){
                    CheckBubbleSequence(t);
                }
            }
        }
    }

    private void DestroyBubblesInSequence(){
        foreach(Transform t in bolhaSequencia){
            Destroy(t.gameObject);
        }
    }

    private void DropDisconectedBubbles(){
        SetAllBubblesConnectionToFalse();
        SetConnectedBubblesToTrue();
        SetGravityToDisconectedBubbles();
    }

    #region Drop Disconected Bubbles
    private void SetAllBubblesConnectionToFalse(){
        foreach (Transform bolha in LevelManager.instancia.aresBolhas){
            bolha.GetComponent<Bubble>().Conectado = false;
        }
    }

    private void SetConnectedBubblesToTrue(){
        bolhaSequencia.Clear();

        RaycastHit2D[] batendo = Physics2D.RaycastAll(ponteiroUltimaLinha.position, ponteiroUltimaLinha.right, 15f);

        for (int i = 0; i < batendo.Length; i++){
            if (batendo[i].transform.gameObject.tag.Equals("Bubble"))
                SetNeighboursConnectionToTrue(batendo[i].transform);
        }
    }

    private void SetNeighboursConnectionToTrue(Transform bolha){
        Bubble roteiroBolha = bolha.GetComponent<Bubble>();
        roteiroBolha.Conectado = true;
        bolhaSequencia.Add(bolha);

        foreach(Transform t in roteiroBolha.GetNeighbors()){
            if(!bolhaSequencia.Contains(t)){
                SetNeighboursConnectionToTrue(t);
            }
        }
    }

    private void SetGravityToDisconectedBubbles(){
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