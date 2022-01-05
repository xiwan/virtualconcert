using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PolyPerfect
{
    [ExecuteInEditMode]
    public class RandomCharacterPlacer : MonoBehaviour
    {
        [SerializeField] float spawnSize;
        [SerializeField] int spawnAmmount;

        [SerializeField] GameObject[] characters;

        [ContextMenu("Spawn Characters")]
        void SpawnAnimals()
        {
            var parent = new GameObject("SpawnedCharacters");

            //for (int i = 0; i < spawnAmmount; i++)
            //{
            //    var value = Random.Range(0, characters.Length);

            //    Instantiate(characters[value], RandomNavmeshLocation(spawnSize), Quaternion.identity, parent.transform);
            //}

            SpawnAnimals(parent, spawnAmmount, spawnSize);
        }

        public GameObject[] SpawnAnimals(GameObject parent, int spawnAmmount = 10, float spawnSize = 5)
        {
            //var characters = NetworkManager.singleton.spawnPrefabs.ToArray();
            //for (int i = 0; i < spawnAmmount; i++)
            //{
            //    var value = Random.Range(0, characters.Length);
            //    var instance = Instantiate(characters[value], RandomNavmeshLocation(spawnSize), Quaternion.identity);
            //    NetworkServer.Spawn(instance);
            //}
            return SpawnAnimals(characters, parent, spawnAmmount, spawnSize);

        }

        public GameObject[] SpawnAnimals(GameObject[] characters, GameObject parent, int spawnAmmount = 10, float spawnSize = 5)
        {
            GameObject[] instances = new GameObject[spawnAmmount];
            for (int i = 0; i < spawnAmmount; i++)
            {
                var value = Random.Range(0, characters.Length);
                var instance = Instantiate(characters[value], RandomNavmeshLocation(spawnSize), Quaternion.identity, parent.transform);
                instance.name = characters[value].name + instance.GetInstanceID();
                instances[i] = instance;
            }
            return instances;
        }

        public GameObject[] SpawnAnimals(GameObject mainRig, Avatar[] characters, GameObject parent, int spawnAmmount = 10, float spawnSize = 5)
        {
            GameObject[] instances = new GameObject[spawnAmmount];
            for (int i = 0; i < spawnAmmount; i++)
            {
                var value = Random.Range(0, characters.Length);
                var avatar = characters[value];
                var instance = Instantiate(mainRig, RandomNavmeshLocation(spawnSize), Quaternion.identity, parent.transform);

                var spawnedInstance = AvatarManager.SpawnFromAvatar(instance, avatar);
                if (spawnedInstance != null)
                {
                    instances[i] = instance;
                }
                
            }
            return instances;
        }


        public Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, spawnSize);
        }
    }
}
