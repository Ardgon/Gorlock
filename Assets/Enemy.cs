using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;

    private WaveSpawner waveSpawner;

    private float countdown = 5f;
    // Start is called before the first frame update
    private void Start()
    {
        waveSpawner = GetComponentInParent<WaveSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);

        countdown -= Time.deltaTime;

        if (countdown <= 0)
        {
            Destroy(gameObject);

            waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;
        }
    }
}
