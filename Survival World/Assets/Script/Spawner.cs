using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Wave[] waves;
    public Enemy enemy;
    //현재 스폰중인 Wave
    Wave currentWaves;
    //남아잇는 스폰할 적
    int enemyesRemainingToSpawn;
    //다음 스폰 시간
    float nextSpawnTime;
    //현재 웨이브 횟수
    int currentWaveNumber;
    //살아있는 적의 수 
    int enemiesRemainingAlive;

    private void Start(){
        NextWave();
    }


    private void Update(){
        if (enemyesRemainingToSpawn>0 && Time.time > nextSpawnTime) {
            enemyesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWaves.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero,Quaternion.identity) as Enemy;
            //OnDeath 이벤트에 함수 할당
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath() {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive==0) {
            NextWave();
        }

    }

    void NextWave() {
        
        currentWaveNumber++;
        //wave배열의 길이보다 현재 웨이브 횟수 -1 (처음에 ++ 시켜주기때문에 -1 시킴)가 작을경우 실행
        if (currentWaveNumber-1 < waves.Length) {
            currentWaves = waves[currentWaveNumber-1];

            enemyesRemainingToSpawn = currentWaves.enemyCount;
            enemiesRemainingAlive = enemyesRemainingToSpawn;
        }
    }

    [System.Serializable]
    public class Wave {
        public int enemyCount;
        public float timeBetweenSpawns;
    }
    
}
