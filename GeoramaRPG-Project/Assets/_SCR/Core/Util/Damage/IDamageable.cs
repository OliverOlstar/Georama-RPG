using UnityEngine;

namespace OliverLoescher
{
	public interface IDamageable
	{
		void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection, Color pColor);
		void Damage(float pValue, GameObject pSender, Vector3 pPoint, Vector3 pDirection);
		// void SetStatus(StatusEffects.status pStatus, float pSeconds);
		GameObject GetGameObject();
		IDamageable GetParentDamageable();
		SOTeam GetTeam();
	}
}