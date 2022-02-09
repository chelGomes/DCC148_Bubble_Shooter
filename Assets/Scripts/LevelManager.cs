using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour{

    #region Singleton
    public static LevelManager instancia;

    private void Despertar(){
        if (instancia == null){
            instancia = this;
        }
    }
    #endregion

    public Grid rede;
    public Transform aresBolhas;
    public List<GameObject> bolhasFabricadas;
    public List<GameObject> bolhasCena;
    public List<string> coresCena;

    public float deslocamento = 1f;
    public GameObject linhaEsquerda;
    public GameObject linhaDireita;
    private bool ultimaLinhaEsquerda = true;


    private void Start(){
        rede = GetComponent<Grid>();
    }

    public void GerarNivel(){
        EncherBolhas(GameObject.FindGameObjectWithTag("InitialLevelScene"), bolhasFabricadas);
        ParaRede(aresBolhas);
        ListarAtualizacaoBolhas();
    }

    #region Snap to Grid
    private void ParaRede(Transform parente){
        foreach (Transform t in parente){
            PosicaoProxima(t);
        }
    }

    public void PosicaoProxima(Transform t){
        Vector3Int posicaoCelula = rede.WorldToCell(t.position);
        t.position = rede.GetCellCenterWorld(posicaoCelula);
    }
    #endregion

    #region Add new line
    [ContextMenu("AddLine")]
    public void AdicionarNovaLinha(){
        DeslocamentoRede();
        DeslocamentoCenaBolha();
        GameObject novaLinha = ultimaLinhaEsquerda == true ? Instantiate(linhaDireita) : Instantiate(linhaEsquerda);
        EncherBolhas(novaLinha, bolhasCena);
        ParaRede(aresBolhas);
        ultimaLinhaEsquerda = !ultimaLinhaEsquerda;
    }

    private void DeslocamentoRede(){
        transform.position = new Vector2(transform.position.x, transform.position.y - deslocamento);
    }

    private void DeslocamentoCenaBolha(){
        foreach (Transform t in aresBolhas){
            t.transform.position = new Vector2(t.position.x, t.position.y - deslocamento);
        }
    }
    #endregion

    private void EncherBolhas(GameObject go, List<GameObject> bolhas){
        foreach (Transform t in go.transform){
            var bolha = Instantiate(bolhas[(int)(Random.Range(0, bolhas.Count * 1000000f) / 1000000f)], aresBolhas);
            bolha.transform.position = t.position;
        }

        Destroy(go);
    }

    public void ListarAtualizacaoBolhas(){
        List<string> colors = new List<string>();
        List<GameObject> newListOfBubbles = new List<GameObject>();

        foreach (Transform t in aresBolhas){
            Bubble roteiroBolha = t.GetComponent<Bubble>();
            if (colors.Count < bolhasFabricadas.Count && !colors.Contains(roteiroBolha.bubbleColor.ToString())){
                string color = roteiroBolha.bubbleColor.ToString();
                colors.Add(color);

                foreach (GameObject prefab in bolhasFabricadas){
                    if (color.Equals(prefab.GetComponent<Bubble>().bubbleColor.ToString())){
                        newListOfBubbles.Add(prefab);
                    }
                }
            }
        }

        coresCena = colors;
        bolhasCena = newListOfBubbles;
    }

    public void DefinirBolha(Transform bolha){
        PosicaoProxima(bolha);
        bolha.SetParent(aresBolhas);
    }
}
