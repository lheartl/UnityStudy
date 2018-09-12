using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePerfab;
    public Transform obstaclePerfab;
    public Vector2 mapSize;
    
    [Range(0, 1)]
    public float obstaclePercent;
    [Range(0,1)]
    public float outlinePercent;
    public int seed = 10;
    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    Coord mapCentre;

    private void Start(){
        GenaratorMap();
    }
    public void GenaratorMap() {

        allTileCoords = new List<Coord>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        //맵의 중앙 설정
        mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderNmae = "Generated Map";

        if (transform.Find(holderNmae)) {
            //일반적인 호출이 아니라 에디터에서 호출하기 때문에 DestroyImmediate 사용
            DestroyImmediate(transform.Find(holderNmae).gameObject);
        }
        Transform mapHolder = new GameObject(holderNmae).transform;
        //현재 오브젝트에 자식으로 넣어서 한번에 삭제
        mapHolder.parent = transform;
        for (int x = 0; x < mapSize.x; x ++) {
            for (int y = 0; y < mapSize.y; y++)
            {
                //mapSize.x/2 를 하면 x좌표 0을 중심으로 맵의 가로 길이의 절반만큼 왼쪽으로 이동한 점에서부터 타일생성
                Vector3 tilePosition = CoordToPosition(x,y);
                //Euler = > 이전 좌표축에서 이후 좌표축이 얼마나 회전했는지 기준으로 회전을 표편하는 좌표계
                Transform newTile = Instantiate(tilePerfab,tilePosition,Quaternion.Euler(Vector3.right*90));
                //컴포넌트에 테두리 주기
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
        //obstacleMap bool 배열에 맵 size 만큼 크기 할당
        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        //맵에서 장애물이 차지하는 비율이 얼마나 되게 할지 계산  맵타일 SIZE * 장애물퍼센트
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);

        int currentObstacleCount = 0;

        for (int i = 0; i <obstacleCount; i++) {
            Coord randomCoord = GetRandomCoord();
            //해당 타일이 장애물인지 아닌지 셋팅 
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePerfab, obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y); ;
    }

    //맵 생성시 맵의 전체 영역에서 접근 가능한지 확인
    //Flood Fill 알고리즘으로 체크
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        //맵 타일은 체크했는지 안했는지 확인하기 위한 bool
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        //mapCenter 에는 장애물을 생성하지 않도록 했기 때문에 맵중앙값을 넣고 거기서부터 길찾기를 시도한다.
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

        //접근가능 타일수
        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    //대각선 타일은 체크하지 않기 위해 0은 제외
                    if (x == 0 || y == 0)
                    {
                        //체크하는 이웃타일이 존재하는지 확인
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        //맵 전체사이즈 - 생성하려는 장애물 숫자가  MapIsFullyAccessible에서 계산한 접근가능한 타일 숫자와 다르다면 새로 생성한곳의 타일이 중앙맵에서 접근할수가 없는 것이므로 return false 한다.
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public Coord GetRandomCoord() {
        //큐의 첫번째 항목 가져옴
        Coord randomCoord = shuffledTileCoords.Dequeue();
        //방금 선택한 항목을 큐의 맨 뒤로
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }

    }
}
