using System;
using System.Windows.Forms;
using SharpDX.MediaFoundation;
using SharpDX.DXGI;          // DXGIを使用するための名前空間
using SharpDX.Direct3D11;    // Direct3D11を使用するための名前空間
using AppLoggerModule;
using SharpDX.Direct3D;

namespace PlayMovieForm
{
    public class VideoPlayer : IDisposable
    {
        private Panel _panel;
        private AppLogger _logger;
        private MediaEngine _mediaEngine;
        private MediaEngineEx _mediaEngineEx;
        private Texture2D _texture;
        private SharpDX.Direct3D11.Device _device; // 明示的にDirect3D11.Deviceを使用

        public VideoPlayer(Panel panel, AppLogger logger)
        {
            _panel = panel;
            _logger = logger;

            try
            {
                // Media Foundationの初期化
                MediaManager.Startup();

                // Direct3Dデバイスの初期化
                InitializeDirect3D();

                // Media Engineのセットアップ
                InitializeMediaEngine();
                _logger.PrintInfo("Video player initialized.");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, "Error initializing video player.");
            }
        }

        private void InitializeDirect3D()
        {
            var factory = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
            _device = factory;

            var textureDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm, // DXGIのFormatを明示的に使用
                Height = _panel.Height,
                Width = _panel.Width,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None
            };

            _texture = new Texture2D(factory, textureDesc);
        }

        private void InitializeMediaEngine()
        {
            var factory = new MediaEngineClassFactory();
            var attributes = new MediaEngineAttributes
            {
                DxgiManager = new DXGIDeviceManager()
            };

            using (var mediaEngine = new MediaEngine(factory, attributes, MediaEngineCreateFlags.AudioOnly))
            {
                _mediaEngine = mediaEngine;
                _mediaEngineEx = mediaEngine.QueryInterface<MediaEngineEx>();
            }
        }

        public void SetMovie(string moviePath)
        {
            try
            {
                _mediaEngineEx.Source = moviePath;
                _logger.PrintInfo($"Movie set to: {moviePath}");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, $"Failed to set movie: {moviePath}");
            }
        }

        public void Play()
        {
            try
            {
                _mediaEngineEx.Play();
                _logger.PrintInfo("Movie playing.");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, "Error playing movie.");
            }
        }

        public void Stop()
        {
            try
            {
                _mediaEngineEx.Pause();
                _logger.PrintInfo("Movie stopped.");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, "Error stopping movie.");
            }
        }

        public void Dispose()
        {
            _mediaEngine?.Dispose();
            _texture?.Dispose();
            _device?.Dispose();
            MediaManager.Shutdown();
        }
    }
}
