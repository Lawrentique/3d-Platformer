using System.Collections;
using UnityEngine;

public class RefactorEnemy : MonoBehaviour
{
    public Stats enemyStats;

    [Tooltip("The transform that will lock onto the player once the enemy has spotted them.")]
    private Transform sight;

    [Tooltip("Blue explosion particles")]
    [SerializeField] public GameObject enemyExplosionParticles;
    
    private GameObject player;

    private PatrolFinished patrolBehavior;

    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]
        [Tooltip("How fast the enemy walks (only when idle is true).")]
        public float walkSpeed;

        [Tooltip("How fast the enemy runs after you (only when idle is false).")]
        public float chaseSpeed;
        
        [Tooltip("How close the enemy needs to be to explode")]
        public float explodeDist;
        
        public bool idle;
    }
    private void Start()
    {
        patrolBehavior = GetComponent<PatrolFinished>();
    }
    private void Update()
    {
        // changes the enemy's behavior: pacing in circles or chasing the player
        if (enemyStats.idle == true)
        {
            patrolBehavior.Move(enemyStats.walkSpeed);
        }
        else if (enemyStats.idle == false)
        {
            Chase();
            CheckExplode();
        }
    }

    private void CheckExplode()
    {
        //Explode if we get within the enemyStats.explodeDist
        if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.explodeDist)
        {
            StartCoroutine("Explode");
            enemyStats.idle = true;
        }
    }

    private void Chase()
    {
        //Chase the player
        sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(sight);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        //start chassing if the player gets close enough
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            enemyStats.idle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //stop chassing if the player gets far enough away
        if (other.gameObject.tag == "Player")
        {
            enemyStats.idle = true;

        }
    }

    private IEnumerator Explode()
    {
        GameObject particles = Instantiate(enemyExplosionParticles, transform.position, new Quaternion());
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
    }
}