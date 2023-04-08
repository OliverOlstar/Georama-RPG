using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootflowStateTransition : GameStateTransition
{
	public class Context { }
	private readonly Context DefaultContext = new Context();

	public BootflowStateTransition(GameState state) : base(state) { }


	protected override IEnumerator OnLoad(object stateChangeData)
	{
		if (!(stateChangeData is Context context))
		{
			context = DefaultContext;
		}

		// --------------- Hotfix ---------------
		// Probably best if these things don't exist when the ABMs go away
		// if (MenuManager.Exists)
		// {
		// 	UnityEngine.Object.Destroy(MenuManager.Instance.gameObject);
		// }
		// RegisterCanvases.ClearLoadedBatchTypes();
		// DBManager.Clear();

		// Log("Start initializing ABMs");
		// DataABM.Create();
		// ContentABM.Create();
		// ABMs.Reset();
		// DataABM.Instance.Initialize(DataABM.ABM_NAME, EndpointManager.Instance.CurrentEndpoint.DataBundleLocation, true);
		// ContentABM.Instance.Initialize(ContentABM.ABM_NAME, EndpointManager.Instance.CurrentEndpoint.ContentBundleLocation, true);

		// while (!ABMs.Initialized())
		// {
		// 	yield return null;
		// }
		// ABMs.SetDownloadingMode(ABMBase.DownloadMode.Restricted); // Restrict downloads after initializing

		// Log("Start loading localization");
		// BundleAssetHandle<I2.Loc.LanguageSourceAsset> loc =
		// 	DataABM.Instance.LoadAssetAsyncFromCatalog<I2.Loc.LanguageSourceAsset>(LocUtil.ASSET_NAME);

		// Allow DataABM a window to download data before setting restricted mode again
		// We might have clients that are built without certain variants existing so it's impossible they will have them in Streaming Assets
		// Therefore we can't guarentee loading data wont trigger a download of a new variant
		// Log("Start loading data assets");
		// DataABM.Instance.SetDownloadMode(ABMBase.DownloadMode.Allowed);
		// DataVariantDirector.Instance.StartLoadingDataBundle(out string variant, out BundleAssetHandle<DBMasterSO> handle);
		// while (!handle.IsDone())
		// {
		// 	yield return null;
		// }
		// DataABM.Instance.SetDownloadMode(ABMBase.DownloadMode.Restricted);
		// Log("Finished loading data assets");

		// DBManager.Initialize(variant, handle.GetAsset().m_DataSOs);
		// Log("Finished initializing data");


		// List<string> bundleNames = ListPool<string>.Request();
		// Director.GetOrCreate<BundleDownloadService>().GetSortedHotfixPriorityBundleNames(bundleNames);
		// // TODO: Potentially do not want to include this log in release.
		// Log("Queueing bundle downloads", $"Adding {bundleNames.Count} bundles to download queue:\n{AnalyticsUtil.ConvertListToString(bundleNames, "\n")}");
		// ContentABM.Instance.AddBundlesToDownloadQueue(bundleNames);
		// ListPool<string>.Return(bundleNames);

		// QualityManager.UpdateQualitySettingsData(); // Update quality once we've loaded data

		// if (!NotificationManager.Instance.IsInited)
		// {
		// 	OkoNotificationScheduler scheduler = new GameObject("OkoNotificationScheduler").AddComponent<OkoNotificationScheduler>();
		// 	GameObject.DontDestroyOnLoad(scheduler);
		// 	bool showNotificationLogs = !Core.DebugOption.IsSet(GeoDebugOptions.BackgroundAppInEditor);
		// 	NotificationManager.Instance.Init(Color.blue, GlobalTunableDB.NotificationRestrictedStart,
		// 		GlobalTunableDB.NotificationRestrictedEnd, GlobalTunableDB.NotificationGap,
		// 		GlobalTunableDB.NotificationChannelName, GlobalTunableDB.NotificationChannelDesc, scheduler, showNotificationLogs);
		// }

		// if (loc != null)
		// {
		// 	while (!loc.IsDone())
		// 	{
		// 		yield return null;
		// 	}
		// 	LocUtil.UpdateSources(loc.GetAsset());
		// 	loc.Unload();
		// }
		// Log("Finished loading localization");

		// Cleared menus above, reload them.
		// if (!RegisterCanvases.IsBatchTypeLoaded(RegisterCanvases.BatchType.Shared))
		// {
		// 	yield return m_StateTransition.AwaitAsyncSceneLoad(GamePaths.Menus.SceneName_Shared, true, false, true); // TODO make this async
		// }

		// --------------- Account Setup ---------------
		Log("Initialize account");
		PlayerAccount account = Director.GetOrCreate<PlayerAccount>();
		account.InitializeFromDisk();

		yield break;
	}

	protected override IEnumerator OnFinish(object stateChangeData)
	{
		Log("Bootflow complete, choosing game state");
#if UNITY_EDITOR
		if (QuickPlay.TryGetBootToScene(out string sceneName))
		{
			Log($"QuickPlay override, loading Scene {sceneName}.");
			GameStateManager.RequestState(GameState.Testing, new EmptyStateTransition.Context() { SceneName = sceneName });
			yield break;
		}
#endif
		if (GeoDebugOptions.BootToState.IsEnumSet(out GeoDebugOptions.BootTo state))
		{
			switch (state)
			{
				// case GeoDebugOptions.BootTo.Battle:
				// 	AccountBattle.StartDevBattle();
				// 	yield break;
				// case GeoDebugOptions.BootTo.Sandbox:
				// 	AccountBattle.StartSandbox();
				// 	yield break;
				case GeoDebugOptions.BootTo.CharacterTestScene:
					GameStateManager.RequestState(GameState.Testing, new EmptyStateTransition.Context() { SceneName = "CharacterTestScene" });
					yield break;
				case GeoDebugOptions.BootTo.EmptyScene:
					GameStateManager.RequestState(GameState.Testing, new EmptyStateTransition.Context() { SceneName = "Empty" });
					yield break;
				default:
					throw new System.NotImplementedException($"GeoDebugOptions.BootTo.{state} has not been implemented.");
			}
		}
		GameStateManager.RequestState(GameState.MainMenu);
		yield break;
	}
}