using UnityEngine;


/// <summary>
/// 데미지 받을 수 있는 
/// </summary>
public interface IDamagable
{
  /// <summary>
  /// 데미지를 줌
  /// </summary>
  /// <param name="damage">데미지량</param>
  /// <param name="hitPoint">맞은 지점</param>
  /// <param name="hitNormal">맞은 면의 노멀값</param>
  /// <returns>데미지로 인해 죽었는지</returns>
  bool TakeDamage(int damage, Vector3 hitPoint, Quaternion hitRotation);
  
  /// <summary>
  /// 조준될 중심 위치를 가져옴
  /// </summary>
  /// <returns>위치값</returns>
  Vector3 GetCenterPosition();    
}
