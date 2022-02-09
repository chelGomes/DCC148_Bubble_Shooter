using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour{ 
    public float raioAlcance = 0.7f; //alterar
    public float raioDeslocamento = 0.51f;

    public bool Fixo;
    public bool Conectado;

    public BubbleColor bubbleColor;

    private void EnterColisao2D(Collision2D colisao) { //azul pode 
        if (colisao.gameObject.tag == "Bubble" && colisao.gameObject.GetComponent<Bubble>().Fixo){ 
            if (!Fixo){
                Colidiu();
            }
        }

        if (colisao.gameObject.tag == "Limit"){
            if (!Fixo){
                Colidiu();
            }
        }
    }

    private void Colidiu(){
        var rb = GetComponent<Rigidbody2D>();
        Destroy(rb);
        Fixo = true;
        LevelManager.instancia.DefinirBolha(transform);
        GameManager.instancia.ProcessTurn(transform);
    }

    public List<Transform> GetNeighbors(){
        List<RaycastHit2D> bater = new List<RaycastHit2D>();
        List<Transform> vizinhanca = new List<Transform>();

        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raioDeslocamento, transform.position.y), Vector3.left, raioAlcance));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raioDeslocamento, transform.position.y), Vector3.right, raioAlcance));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raioDeslocamento, transform.position.y + raioDeslocamento), new Vector2(-1f, 1f), raioAlcance));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raioDeslocamento, transform.position.y - raioDeslocamento), new Vector2(-1f, -1f), raioAlcance));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raioDeslocamento, transform.position.y + raioDeslocamento), new Vector2(1f, 1f), raioAlcance));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raioDeslocamento, transform.position.y - raioDeslocamento), new Vector2(1f, -1f), raioAlcance));

        foreach(RaycastHit2D hit in bater) {
            if(hit.collider != null && hit.transform.tag.Equals("Bubble")){
                vizinhanca.Add(hit.transform);
            }
        }

        return vizinhanca;
    }

    void SobreInvisivel(){
        Destroy(gameObject);
    }

    private void DesenharAparelhos(){
        Gizmos.color = Color.red;
    }

    public enum BubbleColor {
        BLUE, YELLOW, RED, GREEN
    }
}
