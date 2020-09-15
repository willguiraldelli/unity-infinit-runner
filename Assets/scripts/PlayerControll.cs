using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{

  public Rigidbody player;
  public GameObject scenario;
  public float speedScenario;
  private int currentLane;

  public float laneDistance;
  private Vector3 target;
  private Vector2 initalPosition;

  public GameObject floor;
  public GameObject obstacle;
  public GameObject coin;

  private AudioSource coinSound;
  private AudioSource explosionSound;
  private bool isGameOver = false;

  public Text txtPoints;
  private int points = 0;

  public Text gameoverText;

  private int currentStage = 1;
    // Start is called before the first frame update
    void Start()
    {

        coinSound = GetComponents<AudioSource>()[0];
        explosionSound = GetComponents<AudioSource>()[1];
        currentLane = 1;   
        target = player.transform.position;
        buildScenario();
        txtPoints.text = ""+points;
    }

    // Update is called once per frame
    void Update() {

        if(isGameOver){
            return;
        }

      int newLane = -1;
      bool jump = false;

      // keyboard
      if(Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2) {
        newLane = currentLane + 1;
      } else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0) {
        newLane = currentLane - 1;
      }
      if (Input.GetKeyDown(KeyCode.Space)){
            jump = true;
        };
      
      // mouse
      if(Input.GetMouseButtonDown(0)) {
        initalPosition = Input.mousePosition;
      } else if (Input.GetMouseButtonUp(0)) {
        if (Input.mousePosition.x > initalPosition.x && currentLane < 2) {
          newLane = currentLane + 1;
        } else if (Input.mousePosition.x < initalPosition.x && currentLane > 0) {
          newLane = currentLane - 1;
        }
        if (Input.mousePosition.y > initalPosition.y){
                jump = true;
            };
      }

      // Touch
      if (Input.touchCount >= 1) {
        if (Input.GetTouch(0).phase == TouchPhase.Began) {
          initalPosition = Input.GetTouch(0).position;
        } else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled) {
          if (Input.GetTouch(0).position.x > initalPosition.x && currentLane < 2) {
            newLane = currentLane + 1;    
          } else if (Input.GetTouch(0).position.x < initalPosition.x && currentLane > 0) {
            newLane = currentLane - 1;
          }
          if (Input.GetTouch(0).position.y > initalPosition.y){
            jump = true;
           }
        }
      }

      if (newLane>= 0){
        currentLane = newLane;
        target = new Vector3((currentLane - 1) * laneDistance, player.transform.position.y, player.transform.position.z);
      }
      if(player.transform.position.x != target.x){
        player.transform.position = Vector3.Lerp(player.transform.position, target, 5*Time.deltaTime);
      }


      if(jump){
            if (player.transform.position.x < 2.5) {
                target.y = 3.0F;
                player.transform.position = Vector3.Lerp(player.transform.position, target, 5 * Time.deltaTime);
            } else {
                jump = false;
            };
        } else if (jump == false && player.transform.position.y > 0.5) {
            target.y = 0.5F;
            player.transform.position = Vector3.Lerp(player.transform.position, target, 5 * Time.deltaTime);
        } else if (player.transform.position.x != target.x) {
            player.transform.position = Vector3.Lerp(player.transform.position, target, 5 * Time.deltaTime);
        }

        speedScenario += (Time.deltaTime * 0.1F);
      scenario.transform.Translate(0,0, speedScenario * Time.deltaTime * -1);

        float scenarioZ = scenario.transform.position.z;
        int stage = (int)(Mathf.Floor((scenarioZ - 80.0F) / -100.0F) + 1);
        if(stage > currentStage)        {
            GameObject floor2 = Instantiate(floor);
            float posz = ((100 * currentStage) + 15) + scenarioZ;
            float posx = floor.transform.position.x;
            float posy = floor.transform.position.y;

            floor2.transform.SetParent(scenario.transform);
            floor2.transform.position = new Vector3(posx, posy, posz);
            currentStage++;

            buildScenario();
        }
    }

    void buildScenario(){
        int start = 0;

        if (currentStage == 1){
            start = 2;
        }

        for (int i=start; i < 7; i++){
            int[] element = new int[3];

            for (int j=0; j < 3; j++){
                element[j] = Random.Range(0, 3);

                if (element[0] == 1 && element[1] == 1 && element[2] == 1){
                    element[2] = 0;
                }

                if (element[j] == 1){
                    instantiateObstacle(i, j);
                }
                else if (element[j] == 2){
                    instantiateCoin(i, j);
                }
            }
        }
    }

    void instantiateObstacle(int indexZ, int indexX){
        GameObject obstacle2 = Instantiate(obstacle);
        float posZ = ((14.28F * indexZ)+ ((currentStage - 1) * 100) + scenario.transform.position.z) - 20;
        float posX = (indexX - 1) * laneDistance;
        obstacle2.transform.SetParent(scenario.transform);
        obstacle2.transform.position = new Vector3(posX, 0.5F, posZ);
    }

    void instantiateCoin(int indexZ, int indexX)
    {
        GameObject coin2 = Instantiate(coin);
        float posZ = ((14.28F * indexZ) + ((currentStage - 1) * 100) + scenario.transform.position.z) - 20;
        float posX = (indexX - 1) * laneDistance;
        coin2.transform.SetParent(scenario.transform);
        coin2.transform.position = new Vector3(posX, 0.5F, posZ);
    }
    /*
    void OnCollisionEnter(Collision col) {
        Debug.Log(col.gameObject.tag);
      if (col.gameObject.CompareTag("coin")) {
        Destroy(col.gameObject);
      } 
      if (col.gameObject.CompareTag("obstacle")) {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
      }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("coin"))
        {
            coinSound.Play();
            Destroy(other.gameObject);
            points++;
            txtPoints.text = "" + points;
        }
        if (other.gameObject.CompareTag("obstacle"))
        {
            gameoverText.text = "GAME OVER";
            isGameOver = true;
            explosionSound.Play();
            Invoke("GameOver", 2);
        }
    }

    void GameOver()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);

    }
}

