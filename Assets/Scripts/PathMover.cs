using UnityEngine;

/// <summary>
/// 배열의 정해진 앵커포인트 순서대로 이동시키는 컴포넌트 
/// </summary>
public class PathMover : MonoBehaviour
{
  #region Variables
  public Transform[] points;
  public Transform target;
  public float speed = 5f;

  private int index = 0;
  #endregion

  #region Unity Method  
  private void OnEnable()
  {
    //활성화될 때마다 재사용하기 위해 인덱스 초기화
    index = 0;
  }

  private void Update()
  {
    //목적지를 향해 초당 스피드 기준으로 이동할 위치 계산해 적용.
    Vector3 pos = Vector3.MoveTowards(target.position, points[index].position, speed * Time.deltaTime);
    target.position = pos;

    //도착했다고 판단되면 인덱스에 1을 더해 다음 목적지를 설정하는데
    //모든 목적지을 경유했다면 다시 처음 목적지를 가리키도록 설정
    if (Vector3.Distance(points[index].position, pos) <= float.Epsilon)
    {
       if(++index >= points.Length)
        index = 0;
    }
  }
  #endregion
}
