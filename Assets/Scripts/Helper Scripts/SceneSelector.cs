using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour{
    // Start is called before the first frame update
    private Button next;
    private Button prev;
    private Button play;
    private RawImage scene0;
    private RawImage scene1;
    void Start(){
        next = GameObject.Find("next").GetComponent<Button>();
        prev = GameObject.Find("prev").GetComponent<Button>();
        play = GameObject.Find("Playbtn").GetComponent<Button>();
        scene0 = GameObject.Find("scene0").GetComponent<RawImage>();
        scene1 = GameObject.Find("scene1").GetComponent<RawImage>();
        scene1.gameObject.SetActive(false);
        next.onClick.AddListener(()=> changeScene());
        prev.onClick.AddListener(()=> changeScene());
        play.onClick.AddListener(()=> playGame());
    }

    // Update is called once per frame
    void Update(){
        
    }

    void changeScene(){
        if(scene1.IsActive()){
            scene1.gameObject.SetActive(false);
            scene0.gameObject.SetActive(true);
        }else if(scene0.IsActive()){
            scene0.gameObject.SetActive(false);
            scene1.gameObject.SetActive(true);
        }
    }

    void playGame(){
        if(scene1.IsActive()){
            resetScore();
            PlayerController.endGame = false;
            StartCoroutine(WaitForSceneLoad("Level1"));
        }else{
            resetScore();
            PlayerController.endGame = false;
            StartCoroutine(WaitForSceneLoad("SampleScene"));
        }
    }

    void resetScore(){
        if(GamePlayController.instance!=null){
            if(GamePlayController.instance.getScoreCount()!=0){
                while(GamePlayController.instance.getScoreCount()!=0){
                    GamePlayController.instance.DecreaseScore();
                }
            }
        }
    }

    private IEnumerator WaitForSceneLoad(string scene) {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(scene);
    }
}
