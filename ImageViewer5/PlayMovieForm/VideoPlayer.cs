using System;
using System.Windows.Forms;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using SharpDX.Mathematics.Interop;
using AppLoggerModule;
using SharpDX.Direct3D;
using Timer = System.Windows.Forms.Timer;

namespace PlayMovieForm
{
    public class VideoPlayer : IDisposable
    {
        private Panel _panel;
        private AppLogger _logger;
        private MediaEngine _mediaEngine;
        private MediaEngineEx _mediaEngineEx;
        private Texture2D _texture;
        private SharpDX.Direct3D11.Device _device; // Direct3D11.Deviceを明示
        private SwapChain _swapChain;

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

            var swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(_panel.Width, _panel.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _panel.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            using (var dxgiFactory = _device.QueryInterface<SharpDX.DXGI.Device>().Adapter.GetParent<Factory>())
            {
                _swapChain = new SwapChain(dxgiFactory, _device, swapChainDescription);
            }

            var textureDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
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

            // usingを削除して、MediaEngineのインスタンスを保持するように修正
            _mediaEngine = new MediaEngine(factory, attributes, MediaEngineCreateFlags.None);
            _mediaEngineEx = _mediaEngine.QueryInterface<MediaEngineEx>();
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

                // タイマーを使ってフレーム描画を繰り返す
                Timer timer = new Timer();
                timer.Interval = 16; // 60 FPSに近いレートで呼び出し
                timer.Tick += (s, e) =>
                {
                    if (!_mediaEngineEx.IsPaused)
                    {
                        RenderFrame();
                    }
                    else
                    {
                        _logger.PrintInfo("Video is paused or not ready for rendering.");
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, "Error playing movie.");
            }
        }


        private void RenderFrame()
        {
            try
            {
                // フレームが利用可能かを確認
                bool isFrameAvailable = _mediaEngine.OnVideoStreamTick(out long presentationTime);

                if (!isFrameAvailable)
                {
                    _logger.PrintInfo("No video frame available at this time.");
                    return; // フレームが取得できなければ描画を中断
                }

                // スワップチェインのバックバッファを取得
                using (var backBuffer = _swapChain.GetBackBuffer<Texture2D>(0))
                {
                    var renderTargetView = new RenderTargetView(_device, backBuffer);

                    // 画面をクリア
                    _device.ImmediateContext.ClearRenderTargetView(renderTargetView, new RawColor4(0, 0, 0, 1));

                    // MediaEngineでビデオフレームを描画
                    _mediaEngine.TransferVideoFrame(_texture, null, new RawRectangle(0, 0, _panel.Width, _panel.Height), null);

                    // スワップチェインを表示
                    _swapChain.Present(1, PresentFlags.None);
                }
            }
            catch (SharpDX.SharpDXException ex)
            {
                if (ex.ResultCode.Code == unchecked((int)0xC00D4E26))
                {
                    _logger.PrintError(ex, "Frame is not ready yet, skipping this render.");
                }
                else
                {
                    _logger.PrintError(ex, "Error rendering video frame.");
                }
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
            _swapChain?.Dispose();
            MediaManager.Shutdown();
        }
        public void SetVolume(int volume)
        {
            try
            {
                _mediaEngineEx.Volume = volume / 100.0; // 音量は0.0～1.0の範囲で設定
                _logger.PrintInfo($"Volume set to {volume}");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, $"Failed to set volume to {volume}");
            }
        }
    }
}
