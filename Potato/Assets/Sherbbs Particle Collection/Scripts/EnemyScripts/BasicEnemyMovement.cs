using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public enum ACTION { MOVEUP, MOVEDOWN, IDLE, FIRE }

    public float rotateAroundSpeed = 0f;
    public float yAxisRange = 20f;
    private float moveSpeed = 0f;
    private float rotationSpeed = 0f;

    public GameObject model;
    public GameObject bullet;
    private GameObject pivotObject;

    private ACTION currAction = ACTION.IDLE;
    private float lastChangeDirection;
    public float baseActionChangeDelay = 3f;
    public float shotDelay = 4f;
    private float lastTimeShot;
    private float adjustedActionDelay;
    private int randomDirection;

    // Start is called before the first frame update
    void Start()
    {
        rotateAroundSpeed = PlayerPrefs.GetFloat("RotateAroundSpeed");
        moveSpeed = PlayerPrefs.GetFloat("MoveSpeed");
        rotationSpeed = PlayerPrefs.GetFloat("RotationSpeed");
        pivotObject = GameObject.Find("Player");
        adjustedActionDelay = baseActionChangeDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (model != null)
        {
            model.transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);

            if (lastChangeDirection + adjustedActionDelay <= Time.time)
            {
                randomDirection = Random.Range(0, 2) == 0 ? -1 : 1;
                //make sure new action is not same as old
                ACTION newAction;
                do
                {
                    newAction = (ACTION)Random.Range(0, System.Enum.GetNames(typeof(ACTION)).Length);
                } while (newAction == currAction);
                adjustedActionDelay = baseActionChangeDelay + Random.Range(-1, 1);
                currAction = newAction;
                lastChangeDirection = Time.time;
            }

            if (transform.position.y >= yAxisRange)
            {
                currAction = ACTION.MOVEDOWN;
                lastChangeDirection = Time.time;
            }
            else if (transform.position.y <= 0)
            {
                currAction = ACTION.MOVEUP;
                lastChangeDirection = Time.time;
            }

            HandleAction();
        }
        else
        {
            PlayerPrefs.SetInt("CurrentEnemies", PlayerPrefs.GetInt("CurrentEnemies") - 1);
            Destroy(this.gameObject);
        }
    }

    private void HandleAction()
    {
        switch (currAction)
        {
            case ACTION.MOVEDOWN:
                transform.Translate(0, -moveSpeed * Time.deltaTime, 0);
                transform.RotateAround(pivotObject.transform.position, new Vector3(0, randomDirection, 0), rotateAroundSpeed * Time.deltaTime);
                break;
            case ACTION.MOVEUP:
                transform.Translate(0,moveSpeed * Time.deltaTime,0);
                transform.RotateAround(pivotObject.transform.position, new Vector3(0, randomDirection, 0), rotateAroundSpeed * Time.deltaTime);
                break;
            case ACTION.FIRE:
                ShootAtPlayer();
                break;
            default:
                break;
        }
    }

    private void ShootAtPlayer()
    {
        if (lastTimeShot + shotDelay <= Time.time)
        {
            GameObject newBullet = (GameObject)Instantiate(bullet);
            newBullet.transform.position = transform.position;
            newBullet.transform.Translate(0,-1,0);
            newBullet.transform.LookAt(pivotObject.transform);

            //reset timer
            lastTimeShot = Time.time;
        }
    }
}
