using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Common.Helpers.Localization.Components
{
    [RequireComponent(typeof(Image))]
    public class AutoLocalizationImageGetter : MonoBehaviour
    {
        [Header("Short Name. Without lang prefix")]
        [SerializeField] private string imageId = null;

        private Image _imageComponent;
        private string _image;

        AsyncOperationHandle handle;

        public void Awake()
        {
            _imageComponent = GetComponent<Image>();
            if (LocalizationManager.IsParsed) SetLocalizedImage();
            LocalizationManager.OnParsed += SetLocalizedImage;
        }

        private void OnDestroy()
        {
            LocalizationManager.OnParsed -= SetLocalizedImage;
            Addressables.Release(handle);
        }

        private async void SetLocalizedImage()
        {
            _image = LocalizationManager.GetImageKey(imageId);
            Sprite sprite = await GetAsset<Sprite>(_image);

            if (sprite != null)
            {
                _imageComponent.sprite = await GetAsset<Sprite>(_image);
            }
        }

        public async UniTask<T> GetAsset<T>(string key)
        {
            try
            {
                var _handle = Addressables.LoadAssetAsync<T>(key);

                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                handle = _handle;
                var asset = await _handle;
                return asset;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}
