using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour{
    [HideInInspector]
    public PlayerDirection direction;
    [HideInInspector]
    public float step_Length = 0.2f;
    [HideInInspector]
    public float movement_Frequency = 0.1f;
    private float counter;
    private bool move;
    [SerializeField]
    private GameObject tailPrefab;
    private List<Vector3> delta_Position;
    private List<Rigidbody> nodes;
    private Rigidbody main_Body;
    private Rigidbody head_Body;
    private Transform tr;
    private bool create_Node_At_Tail;
    public static bool endGame;
    
    void Awake(){
        if(SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Level1"))){
            movement_Frequency = 0.07f;
        }else{
            movement_Frequency = 0.1f;
        }
        tr = transform;
        main_Body = GetComponent<Rigidbody>();
        InitSnakeNodes();
        InitPlayer();
        delta_Position = new List<Vector3>(){
            new Vector3(-step_Length, 0f),  // -x-> Izquierda
            new Vector3(0f, step_Length),   // y -> Arriba
            new Vector3(step_Length, 0f),   // x -> Derecha
            new Vector3(0f,-step_Length)    // -y -> Abajo
        };
    }
    
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        CheckMovementFrequency();
    }

    void FixedUpdate(){
        if(move){
            move = false;
            Move();
        }
    }

    void InitSnakeNodes(){
        nodes = new List<Rigidbody>();
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>());
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>());

        head_Body = nodes[0];
    }

    void SetDirectionRandom(){
        direction = (PlayerDirection)Random.Range(0, (int)PlayerDirection.COUNT);
        RotateHead();
    }

    void InitPlayer(){
        SetDirectionRandom();
        switch(direction){
            case PlayerDirection.RIGHT:
                nodes[1].position = nodes[0].position - new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position - new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.LEFT:
                nodes[1].position = nodes[0].position + new Vector3(Metrics.NODE, 0f, 0f);
                nodes[2].position = nodes[0].position + new Vector3(Metrics.NODE * 2f, 0f, 0f);
                break;
            case PlayerDirection.UP:
                nodes[1].position = nodes[0].position - new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position - new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
            case PlayerDirection.DOWN:
                nodes[1].position = nodes[0].position + new Vector3(0f, Metrics.NODE, 0f);
                nodes[2].position = nodes[0].position + new Vector3(0f, Metrics.NODE * 2f, 0f);
                break;
        }
    }



    void CheckMovementFrequency(){
        counter += Time.deltaTime;
        if(counter >= movement_Frequency){
            counter = 0;
            move = true;
        }
    }

    public void SetInputDirection(PlayerDirection dir){
        if((dir == PlayerDirection.UP && direction == PlayerDirection.DOWN) || (dir == PlayerDirection.DOWN && direction == PlayerDirection.UP) ||
            (dir == PlayerDirection.RIGHT && direction == PlayerDirection.LEFT) || (dir == PlayerDirection.LEFT && direction == PlayerDirection.RIGHT)){
                return;
        }

        direction = dir;
        RotateHead();
        ForceMove();
    }

    void ForceMove(){
        counter = 0;
        move = false;
        Move();
    }

    void RotateHead(){
        if(!endGame){
            if(direction == PlayerDirection.UP){
                head_Body.transform.localRotation = Quaternion.Euler(180f, 0f, 90f);
            }else if(direction == PlayerDirection.LEFT){
                head_Body.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            }else if(direction == PlayerDirection.RIGHT){
                head_Body.transform.localRotation = Quaternion.Euler(180f, 0f, 180f);
            }else if(direction == PlayerDirection.DOWN){
                head_Body.transform.localRotation = Quaternion.Euler(180f, 0f, 270f);
            }
        }
    }

    void Move(){
        if(!endGame){
            Vector3 dPosition = delta_Position[(int)direction];
            Vector3 parentPos = head_Body.position;
            Vector3 prevPosition;

            main_Body.position = main_Body.position + dPosition;
            head_Body.position = head_Body.position + dPosition;

            for(int i=1; i<nodes.Count ; i++){
                prevPosition = nodes[i].position;
                nodes[i].position = parentPos;
                parentPos = prevPosition;
            }

            if(create_Node_At_Tail){
                create_Node_At_Tail = false;
                GameObject newNode = Instantiate(tailPrefab, nodes[nodes.Count -1].position, Quaternion.identity);
                newNode.transform.SetParent(transform, true);
                nodes.Add(newNode.GetComponent<Rigidbody>());
            }
        }
    }

    void OnTriggerEnter(Collider target){

        if(target.tag == Tags.FRUIT){
            Destroy(target.gameObject);
            create_Node_At_Tail = true;
            GamePlayController.instance.IncreaseScore();
            AudioManager.instance.Play_PickUpSound();
        }

        if(target.tag == Tags.WALL || target.tag == Tags.TAIL){
            EndGame();
        }

        if(target.tag == Tags.BOMB){
            Rigidbody r = nodes[nodes.Count - 1];
            nodes.Remove(r);
            Destroy(r.gameObject);
            if(nodes.Count<3){
                EndGame();
            }else{
                GamePlayController.instance.DecreaseScore();
                Destroy(target.gameObject);
            }
        }
    }

    void EndGame(){
        AudioManager.instance.Play_DeadSound();
        endGame = true;
        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad() {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("LevelSelectionScene");
    }
     
}
