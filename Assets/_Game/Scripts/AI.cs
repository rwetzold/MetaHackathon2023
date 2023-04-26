using UnityEngine;

namespace Hackathon
{
    public class AI : Singleton<AI>
    {
        public GameObject aiPrefab;
        public Transform aiSpawnPoint;
        public bool createObstacles;
        public int obstacles = 5;
        public Vector2 obstacleWidth;
        public Vector2 obstacleLength;
        public Vector2 obstacleHeight;

        private BoxCollider _volume;

        private void Start()
        {
            _volume = GetComponent<BoxCollider>();

            if (createObstacles) CreateObstacles();

            GameObject go = Instantiate(aiPrefab, Vector3.zero, Quaternion.identity);
            go.name = "AI Player";
            go.transform.SetPositionAndRotation(aiSpawnPoint.position, aiSpawnPoint.rotation);

            GameManager.Instance.remotePlayer = go.transform;
            NetworkManager.Instance.SendSessionStart();
        }

        private void CreateObstacles()
        {
            for (int i = 0; i < obstacles; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "Obstacle";
                go.transform.position = GetRandomPointInsideCollider(_volume);
                go.transform.localScale = new Vector3(Random.Range(obstacleWidth.x, obstacleWidth.y),
                    Random.Range(obstacleHeight.x, obstacleHeight.y), Random.Range(obstacleLength.x, obstacleLength.y));
            }
        }

        private Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
        {
            Vector3 extents = boxCollider.size / 2f;
            Vector3 point = new Vector3(
                Random.Range(-extents.x, extents.x),
                0,
                Random.Range(-extents.z, extents.z)
            ) + boxCollider.center;
            return boxCollider.transform.TransformPoint(point);
        }
    }
}