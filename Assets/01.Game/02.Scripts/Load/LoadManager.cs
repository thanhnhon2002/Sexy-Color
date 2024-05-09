using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BBG.PictureColoring
{
	public class LoadManager : SingletonComponent<LoadManager>
	{
		#region Classes

		private class LevelLoadHandler
		{
			public enum State
			{
				Loading,
				Loaded,
				Released
			}

			public string				levelId;
			public string				assetPath;
			public int					refCount;
			public State				state;
			public LevelFileData		levelFileData;
			public Sprite[]				atlasSprites;
			public List<LoadComplete>	loadCompleteCallbacks;
		}

		#endregion

		#region Member Variables

		private Dictionary<string, LevelLoadHandler> levelLoadHandlers;

		#endregion

		#region Delegates

		public delegate void LoadComplete(string levelId, bool success);

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			levelLoadHandlers = new Dictionary<string, LevelLoadHandler>();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Loads the level file for the level.
		/// </summary>
		public bool LoadLevel(LevelData levelData, LoadComplete loadCompleteCallback)
		{
			if (levelLoadHandlers.TryGetValue(levelData.Id, out var levelLoadHandler))
			{
				levelLoadHandler.refCount++;
				
				if (levelLoadHandler.state == LevelLoadHandler.State.Loaded)
				{
					// Level already successfully loaded
					return false;
				}

				levelLoadHandler.loadCompleteCallbacks.Add(loadCompleteCallback);
			}
			else
			{
				// Start loading the level
				Load(CreateLoadHandler(levelData, loadCompleteCallback));
			}

			return true;
		}

		public void ReleaseLevel(string levelId)
		{
			if (levelLoadHandlers.TryGetValue(levelId, out var levelLoadHandler))
			{
				levelLoadHandler.refCount--;

				if (levelLoadHandler.refCount == 0)
				{
					Release(levelLoadHandler);
					levelLoadHandlers.Remove(levelId);
				}
			}
		}

		public LevelFileData GetLevelFileData(string levelId)
		{
			if (!levelLoadHandlers.TryGetValue(levelId, out var levelLoadHandler))
			{
				Debug.LogErrorFormat("[LoadManager] GetLevelFileData: Level {0} is not loaded", levelId);
				return null;
			}

			if (levelLoadHandler.state != LevelLoadHandler.State.Loaded)
			{
				Debug.LogErrorFormat("[LoadManager] GetLevelFileData: Level {0} has not finished loading - state: {1}", levelId, levelLoadHandler.state);
				return null;
			}

			return levelLoadHandler.levelFileData;
		}

		public Sprite GetRegionSprite(string levelId, int atlasIndex)
		{
			if (!levelLoadHandlers.TryGetValue(levelId, out var levelLoadHandler))
			{
				Debug.LogErrorFormat("[LoadManager] GetRegionSprite: Level {0} is not loading", levelId);
				return null;
			}

			if (levelLoadHandler.state != LevelLoadHandler.State.Loaded)
			{
				Debug.LogErrorFormat("[LoadManager] GetRegionSprite: Level {0} has not finished loading - state: {1}", levelId, levelLoadHandler.state);
				return null;
			}

			if (atlasIndex < 0 || atlasIndex >= levelLoadHandler.atlasSprites.Length)
			{
				Debug.LogErrorFormat("[LoadManager] GetRegionSprite: Invalid region index {0} for level {1} which has {2} region sprites", atlasIndex, levelId, levelLoadHandler.atlasSprites.Length);
			}

			return levelLoadHandler.atlasSprites[atlasIndex];
		}

		#endregion

		#region Private Methods

		private LevelLoadHandler CreateLoadHandler(LevelData levelData, LoadComplete loadCompleteCallback)
		{
			LevelLoadHandler levelLoadHandler = new LevelLoadHandler()
			{
				levelId = levelData.Id,
				assetPath = levelData.AssetPath,
				state = LevelLoadHandler.State.Loading,
				refCount = 1,
				loadCompleteCallbacks = new List<LoadComplete>() { loadCompleteCallback }
			};

			levelLoadHandlers.Add(levelLoadHandler.levelId, levelLoadHandler);

			return levelLoadHandler;
		}

		private async void Load(LevelLoadHandler levelLoadHandler)
		{
			//Debug.Log("[LoadManager] Loading level " + levelLoadHandler.levelId + " AssetPath: " + levelLoadHandler.assetPath);

			TextAsset bytesFile = await Addressables.LoadAssetAsync<TextAsset>(levelLoadHandler.assetPath + "/bytes.bytes").Task;

			if (bytesFile == null)
			{
				LoadFinished(levelLoadHandler, "Failed to load bytes.bytes file");
				return;
			}

			var worker = new LoadLevelFileDataWorker(bytesFile.bytes);

			worker.StartWorker();

			while (!worker.Stopped)
			{
				await Task.Delay(100);
			}

			LevelFileData lfd = worker.outLevelFileData;

			levelLoadHandler.levelFileData = lfd;

			Addressables.Release(bytesFile);

			if (levelLoadHandler.refCount == 0)
			{
				// If refCount is 0 now then the callers no longer need this levels assets, just return now
				return;
			}

			levelLoadHandler.atlasSprites = new Sprite[lfd.atlases];

			for (int i = 0; i < lfd.atlases; i++)
			{
				string spriteAssetPath = levelLoadHandler.assetPath + string.Format("/atlas_" + i + ".png", i);

				//Debug.Log("[LoadManager] Loading sprite " + spriteAssetPath);

				Sprite sprite = await Addressables.LoadAssetAsync<Sprite>(spriteAssetPath).Task;

				if (sprite == null)
				{
					LoadFinished(levelLoadHandler, "Failed to sprite at " + spriteAssetPath);
					return;
				}

				levelLoadHandler.atlasSprites[i] = sprite;

				if (levelLoadHandler.refCount == 0)
				{
					// If refCount is 0 now then the callers no longer need this levels assets, release any loaded sprites and return
					Release(levelLoadHandler);
					return;
				}
			}

			LoadFinished(levelLoadHandler);
		}

		private void LoadFinished(LevelLoadHandler levelLoadHandler, string errorMessage = null)
		{
			bool success = string.IsNullOrEmpty(errorMessage);

			if (success)
			{
				levelLoadHandler.state = LevelLoadHandler.State.Loaded;
			}
			else
			{
				Debug.LogErrorFormat("[LoadManager] Error loading level: Id {0}, AssetPath {1}, Error: {2}", levelLoadHandler.levelId, levelLoadHandler.assetPath, errorMessage);
				levelLoadHandlers.Remove(levelLoadHandler.levelId);
				Release(levelLoadHandler);
			}

			for (int i = 0; i < levelLoadHandler.loadCompleteCallbacks.Count; i++)
			{
				levelLoadHandler.loadCompleteCallbacks[i].Invoke(levelLoadHandler.levelId, success);
			}
		}

		private void Release(LevelLoadHandler levelLoadHandler)
		{
			if (levelLoadHandler.atlasSprites != null)
			{
				for (int i = 0; i < levelLoadHandler.atlasSprites.Length; i++)
				{
					Sprite sprite = levelLoadHandler.atlasSprites[i];

					if (sprite != null)
					{
						Addressables.Release(sprite);

						levelLoadHandler.atlasSprites[i] = null;
					}
				}
			}

			levelLoadHandler.state = LevelLoadHandler.State.Released;
		}

		#endregion
	}
}
