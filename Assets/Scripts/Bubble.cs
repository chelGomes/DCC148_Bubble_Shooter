using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour{ //nao pode em verde
    public float raycastRange = 0.7f; //alterar
    public float raycastOffset = 0.51f;

    public bool Fixo;//alterar
    public bool Conectado;

    public BubbleColor bubbleColor; //alterar

    private void OnCollisionEnter2D(Collision2D colisao) { //azul pode 
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
        LevelManager.instance.SetAsBubbleAreaChild(transform);
        GameManager.instance.ProcessTurn(transform);
    }

    public List<Transform> GetNeighbors(){
        List<RaycastHit2D> bater = new List<RaycastHit2D>();
        List<Transform> neighbors = new List<Transform>();

        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y), Vector3.left, raycastRange));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y), Vector3.right, raycastRange));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y + raycastOffset), new Vector2(-1f, 1f), raycastRange));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y - raycastOffset), new Vector2(-1f, -1f), raycastRange));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y + raycastOffset), new Vector2(1f, 1f), raycastRange));
        bater.Add(Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y - raycastOffset), new Vector2(1f, -1f), raycastRange));

        foreach(RaycastHit2D hit in bater) {
            if(hit.collider != null && hit.transform.tag.Equals("Bubble")){
                neighbors.Add(hit.transform);
            }
        }

        return neighbors;
    }

    void OnBecameInvisible(){
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
    }

    public enum BubbleColor {
        BLUE, YELLOW, RED, GREEN
    }
}
