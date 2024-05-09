using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.PictureColoring
{
	public class ShareManager : SingletonComponent<ShareManager>
	{
		#region Inspector Variables

		[SerializeField] private string    androidGallaryImageName        = "";
		[SerializeField] private string    androidGallaryImageDescription = "";
		[SerializeField] private Texture2D waterMark;
		

		#endregion

		#region Member Variables

		// The permission description that will appear on iOS if the user selects the share other button then clicks the Save Image option.
		private const string LibraryUsageDescription = "Save completed images to the device.";

		private Texture2D			saveToPhotosTexture;
		private System.Action<bool>	saveToPhotosCallback;

		#endregion

		#region Public Variables

		public bool ShareToTwitter(Texture2D imageTexture)
		{
			string imagePath = SaveImageForSharing(imageTexture);

			return NativePlugin.TryShareToTwitter(imagePath);
		}

		public bool ShareToInstagram(Texture2D imageTexture)
		{
			string imagePath = SaveImageForSharing(imageTexture);

			return NativePlugin.TryShareToInstagram(imagePath);
		}

		public void ShareToOther(Texture2D imageTexture)
		{
			string imagePath = SaveImageForSharing(imageTexture);

			// NativePlugin.ShareToOther(imagePath);
			new NativeShare().AddFile( imagePath )
				.SetSubject( "Adult Sexy Coloring" ).SetText( "Congratulation!" )
				.SetCallback( ( result, shareTarget ) => Debug.Log( "Share result: " + result + ", selected app: " + shareTarget ) )
				.Share();
		}

		public void SaveImageToPhotos(Texture2D imageTexture, System.Action<bool> callback)
		{
			
			// Create a new texture for the final image
			Texture2D finalTexture = new Texture2D(imageTexture.width, imageTexture.height);

			// Apply the watermark by iterating through pixels
			for (int x = 0; x < imageTexture.width; x++)
			{
				for (int y = 0; y < imageTexture.height; y++)
				{
					Color pixelColor = imageTexture.GetPixel(x, y);
					// Color watermarkPixel = waterMark.GetPixel(x % waterMark.width, y % waterMark.height);
					// Apply the watermark on the pixel
					// pixelColor = Color.Lerp(pixelColor, watermarkPixel, 0.1f); // Adjust the transparency of the watermark if needed
					finalTexture.SetPixel(x, y, pixelColor);
				}
			}
			
			NativeGallery.SaveImageToGallery(finalTexture, "Color-Piece", "Screenshot.png");
		}

		#endregion

		#region Private Variables

		/// <summary>
		/// Saves the image for sharing
		/// </summary>
		private string SaveImageForSharing(Texture2D imageTexture)
		{
			string imagesDirectory	= string.Format("{0}/images", Application.persistentDataPath);
			string imagePath		= string.Format("{0}/share_image.png", imagesDirectory);

			if (!System.IO.Directory.Exists(imagesDirectory))
			{
				System.IO.Directory.CreateDirectory(imagesDirectory);
			}

			// Save the texture to the device so another application can read it
			System.IO.File.WriteAllBytes(imagePath, imageTexture.EncodeToPNG());

			return imagePath;
		}

		/// <summary>
		/// Invoked when an iOS device grants permission to use the photos library
		/// </summary>
		private void OnPhotosPermissionGranted(string message)
		{
			if (message == "true")
			{
				// Call the method again knowning we have now permission
				SaveImageToPhotos(saveToPhotosTexture, saveToPhotosCallback);
			}
			else
			{
				// Notify callback that permission was denied
				saveToPhotosCallback(false);
			}

			saveToPhotosTexture		= null;
			saveToPhotosCallback	= null;
		}

		#if UNITY_EDITOR && UNITY_IOS
		/// <summary>
		/// Adds some fields to the Info.plist file for iOS builds so that sharing works properly.
		/// </summary>
		[UnityEditor.Callbacks.PostProcessBuild]
		public static void ChangeXcodePlist(UnityEditor.BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == UnityEditor.BuildTarget.iOS)
			{
				string								plistPath	= pathToBuiltProject + "/Info.plist";
				UnityEditor.iOS.Xcode.PlistDocument	plist		= new UnityEditor.iOS.Xcode.PlistDocument();

				plist.ReadFromString(System.IO.File.ReadAllText(plistPath));

				UnityEditor.iOS.Xcode.PlistElementDict rootDict = plist.root;

				// Add the library description, this is so the app can use the "Save Image" feature on the share other view
				rootDict.SetString("NSPhotoLibraryUsageDescription", LibraryUsageDescription);
				rootDict.SetString("NSPhotoLibraryAddUsageDescription", LibraryUsageDescription);

				// Add the instagram app to the queries array
				UnityEditor.iOS.Xcode.PlistElementArray queriesArray = rootDict.CreateArray("LSApplicationQueriesSchemes");

				queriesArray.AddString("twitter");
				queriesArray.AddString("instagram");

				// Write to file
				System.IO.File.WriteAllText(plistPath, plist.WriteToString());
			}
		}
		#endif

		#endregion
	}
}
