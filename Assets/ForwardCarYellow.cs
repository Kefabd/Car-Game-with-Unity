using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ForwardCarYellow : MonoBehaviour
{
    private float moveSpeed = 15f;
    public GameObject road;
    public GameObject yellowSpherePrefab; // R�f�rence au prefab de la sph�re jaune
    public GameObject blackCubePrefab; // R�f�rence au prefab du cube noir
    private float normalMoveSpeed = 15f; // Vitesse normale
    private float reducedMoveSpeed = 14f; // Vitesse r�duite apr�s collision
    private float speedRecoveryDelay = 2f; // D�lai pour r�cup�rer la vitesse normale apr�s collision
    private bool isSpeedReduced = false; // Indique si la vitesse est r�duite apr�s une collision


    private float moveCamera = 15f;
    private int score = 0;

    // Fonction pour ajouter un score
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            Camera.main.transform.Translate(Vector3.forward * moveCamera * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
           
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision"+collision.collider.gameObject.tag);
        if (collision.gameObject.tag == "nextPlane")
        {
            // D�truire l'ancienne route
            DestroyObjectWithTag("CurrentRoad");

            // Changer les tags des objets de la collision
            collision.transform.parent.gameObject.tag = "CurrentRoad";
            collision.collider.tag = "currentPlane";

            // Cr�er une nouvelle route
            Vector3 newRoadPosition = CalculateNewRoadPosition(collision.collider.gameObject.transform.position);
            GameObject newRoad = Instantiate(road, newRoadPosition, Quaternion.identity);
            newRoad.tag = "NextRoad";

            // Attribuer des tags aux enfants de la nouvelle route
            TagChildrenWithNextRoadTag(newRoad);
            Debug.Log("Instanciation de sphere");
            // Instancier des objets al�atoires (sph�re jaune et cube noir)
            SpawnRandomObjectsOnRoad(newRoad);
        }
        else if (collision.gameObject.tag == "YellowSphere")
        {
            // D�truire la sph�re jaune
            Destroy(collision.gameObject);
            AddScore(10);

        }
        else if (collision.gameObject.tag == "blackCubePrefab"  || collision.gameObject.tag == "blocker" )
        {
            // R�duire la vitesse
            ReduceSpeed();
            // D�marrer la coroutine pour r�cup�rer la vitesse apr�s un d�lai
            StartCoroutine(RecoverSpeedAfterDelay());
        }
    }
    void ReduceSpeed()
    {
        if (!isSpeedReduced)
        {
            moveSpeed = reducedMoveSpeed;
            moveCamera = reducedMoveSpeed;
            isSpeedReduced = true;
        }
    }

    IEnumerator RecoverSpeedAfterDelay()
    {
        yield return new WaitForSeconds(speedRecoveryDelay);
        // Augmenter la vitesse apr�s le d�lai
        moveSpeed = normalMoveSpeed;
        isSpeedReduced = false;
    }

    Vector3 CalculateNewRoadPosition(Vector3 currentPosition)
    {
        // Calculer la nouvelle position de la route (d�cal�e vers l'avant)
        float newPositionZ = currentPosition.z + 39.93f;
        return new Vector3(currentPosition.x, currentPosition.y, newPositionZ);
    }

    void TagChildrenWithNextRoadTag(GameObject parentObject)
    {
        Transform child = parentObject.transform.Find("Plane");
        if (child != null)
        {
            child.gameObject.tag = "nextPlane";
        }
        else
        {
            foreach (Transform childTransform in parentObject.transform)
            {
                childTransform.gameObject.tag = "blocker";
            }
            Debug.LogWarning("Child with name 'Plane' not found in parent object. Tagging all children as 'Blocker'.");
        }
    }

    private bool isYellowLeft = true; // Variable pour alterner la position de la sph�re jaune

    void SpawnRandomObjectsOnRoad(GameObject road)
    {
        float randomXYellow;
        float randomXBlack;

        if (isYellowLeft)
        {
            randomXYellow = Random.Range(-4f, -1f); // Position al�atoire pour la sph�re jaune � gauche
            randomXBlack = Random.Range(1f, 4f);    // Position al�atoire pour le cube noir � droite
        }
        else
        {
            randomXYellow = Random.Range(1f, 4f);    // Position al�atoire pour la sph�re jaune � droite
            randomXBlack = Random.Range(-4f, -1f);   // Position al�atoire pour le cube noir � gauche
        }

        Vector3 yellowPosition = new Vector3(road.transform.position.x + randomXYellow, road.transform.position.y + 1, road.transform.position.z);
        Instantiate(yellowSpherePrefab, yellowPosition, Quaternion.identity);

        Vector3 blackPosition = new Vector3(road.transform.position.x + randomXBlack, road.transform.position.y + 1, road.transform.position.z);
        Instantiate(blackCubePrefab, blackPosition, Quaternion.identity);

        // Inverser la position de la sph�re jaune pour le prochain appel
        isYellowLeft = !isYellowLeft;
    }





    public void DestroyObjectWithTag(string tag)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objectsWithTag)
        {
            Destroy(obj);
        }
    }
}
