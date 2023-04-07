
namespace OliverLoescher
{
	/// <summary>
	/// Interface to give additional functions for classes inhieriting from either Singleton or MonoBehaviourSingleton
	/// </summary>
	public interface ISingleton
	{
		public void OnAccessed();
	}
}