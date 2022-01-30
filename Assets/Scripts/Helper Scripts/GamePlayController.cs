using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GamePlayController : MonoBehaviour{


    public static GamePlayController instance;
    public GameObject fruit_PickUp, bomb_PickUp;
    private float min_X = -4.25f, max_X = 4.25f, min_Y = -2.26f, max_Y = 2.26f;


    private float z_Pos = -0.16f;

    private Text score_Text;
    private int scoreCount;

    void Awake(){
        MakeInstance();
    }

    // Start is called before the first frame update
    void Start(){
        score_Text = GameObject.Find("Score").GetComponent<Text>();
        Invoke("StartSpawning",0.5f);
    }

    void MakeInstance(){
        if(instance == null){
            instance = this;
        }
    }

    // Update is called once per frame
    void Update(){
        
    }

    void StartSpawning(){
        StartCoroutine(SpawnPickUps());
    }

    public void CancelSpawning(){
        CancelInvoke("StartSpawning");
    }

    IEnumerator SpawnPickUps(){
        if(!PlayerController.endGame){
            yield return new WaitForSeconds(Random.Range(1f,1.5f));
            //Incrementar dificultad
            if(Random.Range(0f, 10f)>=2f){
                Instantiate(fruit_PickUp, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
            }else{
                Instantiate(bomb_PickUp, new Vector3(Random.Range(min_X, max_X), Random.Range(min_Y, max_Y), z_Pos), Quaternion.identity);
                AudioManager.instance.Play_BombSound();
            }
            Invoke("StartSpawning", 0f);
        }else{
            CancelSpawning();
        }
    }

    public void IncreaseScore(){
        scoreCount++;
        score_Text.text = "Puntuación: " +scoreCount;
    }

    public void DecreaseScore(){
        scoreCount--;
        score_Text.text = "Puntuación: " +scoreCount;        
    }

    public int getScoreCount(){
        return scoreCount;
    }
}
