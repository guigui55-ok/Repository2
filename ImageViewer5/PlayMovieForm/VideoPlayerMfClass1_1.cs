using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AppLoggerModule;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace PlayMovieForm
{
    public class VideoPlayerMf : IDisposable, IMFAsyncCallback
    {
        private Control _videoControl;
        private bool _isPlaying;
        private string _moviePath = "";

        private AppLogger _logger;

        private IMFMediaSession _mediaSession;
        private IMFMediaSource _mediaSource;
        private IMFTopology _topology;

        private IMFVideoDisplayControl _videoDisplayControl;
        private IMFSimpleAudioVolume _volumeControl;

        private const int MF_VIDEO_RENDERER_DRAW_TO_HWND = 0x00000001;
        private const int MF_VERSION = 0x00020070;

        public VideoPlayerMf(Control videoControl, AppLogger logger)
        {
            _videoControl = videoControl;
            _logger = logger;

            // Media Foundation の初期化
            MFStartup(MF_VERSION, 0);
            _logger.PrintInfo("Media Foundation initialized.");
        }

        public void SetMovie(string moviePath)
        {
            _moviePath = moviePath;
            _logger.PrintInfo($"Movie path set to: {moviePath}");
        }

        public void Play()
        {
            if (_isPlaying)
                return;

            if (string.IsNullOrEmpty(_moviePath))
            {
                _logger.PrintError("No movie path specified.");
                return;
            }

            try
            {
                InitializeMediaSession();
                CreateMediaSource();
                CreateTopology();
                _mediaSession.SetTopology(0, _topology);

                StartPlayback();

                _isPlaying = true;
                _logger.PrintInfo("Playback started.");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, $"Error in Play(): {ex.Message}");
                Stop();
            }
        }

        private void InitializeMediaSession()
        {
            DisposeMediaSession();

            MFCreateMediaSession(null, out _mediaSession);
            BeginGetEvent();
        }

        private void CreateMediaSource()
        {
            DisposeMediaSource();

            var url = _moviePath;

            IMFSourceResolver sourceResolver;
            MFCreateSourceResolver(out sourceResolver);

            MF_OBJECT_TYPE ObjectType = MF_OBJECT_TYPE.Invalid;

            object source;
            sourceResolver.CreateObjectFromURL(
                url,
                MFResolution.MediaSource,
                null,
                out ObjectType,
                out source);

            _mediaSource = (IMFMediaSource)source;
            Marshal.ReleaseComObject(sourceResolver);
        }

        private void CreateTopology()
        {
            MFCreateTopology(out _topology);

            IMFPresentationDescriptor presentationDescriptor;
            _mediaSource.CreatePresentationDescriptor(out presentationDescriptor);

            int streamCount;
            presentationDescriptor.GetStreamDescriptorCount(out streamCount);

            for (int i = 0; i < streamCount; i++)
            {
                bool selected;
                IMFStreamDescriptor streamDescriptor;
                presentationDescriptor.GetStreamDescriptorByIndex(i, out selected, out streamDescriptor);

                if (selected)
                {
                    CreateTopologyBranch(_topology, _mediaSource, presentationDescriptor, streamDescriptor);
                }

                Marshal.ReleaseComObject(streamDescriptor);
            }

            Marshal.ReleaseComObject(presentationDescriptor);
        }

        private void CreateTopologyBranch(IMFTopology topology, IMFMediaSource mediaSource, IMFPresentationDescriptor presentationDescriptor, IMFStreamDescriptor streamDescriptor)
        {
            // ソースノードの作成
            IMFTopologyNode sourceNode;
            MFCreateTopologyNode(MFTopologyType.SourcestreamNode, out sourceNode);
            sourceNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_SOURCE, mediaSource);
            sourceNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_PRESENTATION_DESCRIPTOR, presentationDescriptor);
            sourceNode.SetUnknown(MFAttributesClsid.MF_TOPONODE_STREAM_DESCRIPTOR, streamDescriptor);

            // 出力ノードの作成
            IMFTopologyNode outputNode;
            MFCreateTopologyNode(MFTopologyType.OutputNode, out outputNode);

            IMFMediaTypeHandler mediaTypeHandler;
            streamDescriptor.GetMediaTypeHandler(out mediaTypeHandler);
            Guid majorType;
            mediaTypeHandler.GetMajorType(out majorType);

            if (majorType == MFMediaType_Audio)
            {
                // オーディオレンダラーの作成
                IMFActivate activate;
                MFCreateAudioRendererActivate(out activate);

                outputNode.SetObject(activate);

                // ボリュームコントロールの取得
                // IMFVolumeControlからIMFSimpleAudioVolumeに変更
                activate.ActivateObject(typeof(IMFSimpleAudioVolume).GUID, out object volumeObject);
                _volumeControl = (IMFSimpleAudioVolume)volumeObject;

                Marshal.ReleaseComObject(activate);
            }
            else if (majorType == MFMediaType_Video)
            {
                // ビデオレンダラーの作成
                IMFActivate activate;
                MFCreateVideoRendererActivate(_videoControl.Handle, out activate);

                outputNode.SetObject(activate);

                // ビデオディスプレイコントロールの取得
                activate.ActivateObject(typeof(IMFVideoDisplayControl).GUID, out object videoDisplayObject);
                _videoDisplayControl = (IMFVideoDisplayControl)videoDisplayObject;

                Marshal.ReleaseComObject(activate);
            }
            else
            {
                _logger.PrintInfo("Unsupported media type.");
                return;
            }

            // ノードの接続
            sourceNode.ConnectOutput(0, outputNode, 0);

            topology.AddNode(sourceNode);
            topology.AddNode(outputNode);

            Marshal.ReleaseComObject(mediaTypeHandler);
            Marshal.ReleaseComObject(sourceNode);
            Marshal.ReleaseComObject(outputNode);
        }

        private void StartPlayback()
        {
            var varStart = new PropVariant();
            _mediaSession.Start(Guid.Empty, varStart);
        }

        private void BeginGetEvent()
        {
            _mediaSession.BeginGetEvent(this, null);
        }

        public void SetVolume(float volumeLevel)
        {
            if (_volumeControl != null)
            {
                // 0.0 ～ 1.0 の範囲にクランプ
                volumeLevel = Math.Max(0.0f, Math.Min(1.0f, volumeLevel));
                _volumeControl.SetMasterVolume(volumeLevel);

                _logger.PrintInfo($"Volume set to: {volumeLevel * 100}%");
            }
            else
            {
                _logger.PrintError("Volume control is not available.");
            }
        }

        public void Stop()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;

            if (_mediaSession != null)
            {
                _mediaSession.Stop();
                _mediaSession.Close();
            }

            DisposeMediaSource();

            _logger.PrintInfo("Playback stopped.");
        }

        private void DisposeMediaSource()
        {
            if (_mediaSource != null)
            {
                _mediaSource.Shutdown();
                Marshal.ReleaseComObject(_mediaSource);
                _mediaSource = null;
            }

            if (_topology != null)
            {
                Marshal.ReleaseComObject(_topology);
                _topology = null;
            }

            if (_volumeControl != null)
            {
                Marshal.ReleaseComObject(_volumeControl);
                _volumeControl = null;
            }

            if (_videoDisplayControl != null)
            {
                Marshal.ReleaseComObject(_videoDisplayControl);
                _videoDisplayControl = null;
            }
        }

        private void DisposeMediaSession()
        {
            if (_mediaSession != null)
            {
                _mediaSession.Shutdown();
                Marshal.ReleaseComObject(_mediaSession);
                _mediaSession = null;
            }
        }

        public void Dispose()
        {
            Stop();
            DisposeMediaSession();

            // Media Foundation の終了処理
            MFShutdown();
            _logger.PrintInfo("Media Foundation shutdown.");
        }

        // IMFAsyncCallback の実装
        private void GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Standard;
        }

        private void Invoke(IMFAsyncResult pAsyncResult)
        {
            _mediaSession.EndGetEvent(pAsyncResult, out IMFMediaEvent mediaEvent);

            mediaEvent.GetType(out MediaEventType eventType);

            _logger.PrintInfo($"Media Session Event: {eventType}");

            if (eventType == MediaEventType.MESessionEnded)
            {
                _logger.PrintInfo("Session ended.");
                Stop();
            }

            // 次のイベントを取得
            BeginGetEvent();

            Marshal.ReleaseComObject(mediaEvent);
        }

        #region PInvokeとCOMインターフェースの定義

        [DllImport("mfplat.dll", PreserveSig = false)]
        private static extern void MFStartup(int version, int dwFlags);

        [DllImport("mfplat.dll", PreserveSig = false)]
        private static extern void MFShutdown();

        [DllImport("mf.dll", PreserveSig = false)]
        private static extern void MFCreateMediaSession([In] IMFAttributes pConfiguration, out IMFMediaSession ppMediaSession);

        [DllImport("mf.dll", PreserveSig = false)]
        private static extern void MFCreateSourceResolver(out IMFSourceResolver ppSourceResolver);

        [DllImport("mfplat.dll", PreserveSig = false)]
        private static extern void MFCreateTopology(out IMFTopology ppTopo);

        [DllImport("mf.dll", PreserveSig = false)]
        private static extern void MFCreateTopologyNode(MFTopologyType NodeType, out IMFTopologyNode ppNode);

        [DllImport("mf.dll", PreserveSig = false)]
        private static extern void MFCreateAudioRendererActivate(out IMFActivate ppActivate);

        [DllImport("evr.dll", PreserveSig = false)]
        private static extern void MFCreateVideoRendererActivate(IntPtr hwndVideo, out IMFActivate ppActivate);

        // COMインターフェースの定義
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("D19F8E98-B126-4446-890C-5DCB7AD71453")]
        private interface IMFMediaSession
        {
            void GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
            void BeginGetEvent(IMFAsyncCallback pCallback, [MarshalAs(UnmanagedType.IUnknown)] object o);
            void EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
            void QueueEvent(MediaEventType met, [In] ref Guid guidExtendedType, int hrStatus, [In] PropVariant pvValue);
            void Start([In] ref Guid pguidTimeFormat, [In] PropVariant pvarStartPosition);
            void Stop();
            void Pause();
            void Close();
            void Shutdown();
            void GetClock(out IMFClock ppClock);
            void GetSessionCapabilities(out uint pdwCaps);
            void GetFullTopology(uint dwGetFullTopologyFlags, ulong TopoId, out IMFTopology ppFullTopology);
        }

        // 他のインターフェースの定義も同様に記載します
        // （省略のため、ここでは詳細を割愛します）

        // 必要な定数とGUIDの定義
        private static readonly Guid MFMediaType_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");
        private static readonly Guid MFMediaType_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");

        private enum MF_OBJECT_TYPE
        {
            Invalid,
            MediaSource,
            ByteStream,
            Stream
        }

        private enum MFResolution
        {
            MediaSource = 0x00000001,
            ByteStream = 0x00000002,
            ContentDoesNotHaveToMatchExtensionOrMimeType = 0x00000010,
            KeepByteStreamAliveOnFail = 0x00000020,
            DisableLocalPlugins = 0x00000040,
            DisableCachedPlugins = 0x00000080
        }

        private enum MediaEventType
        {
            MESessionEnded = 0x000D
            // 他のイベントタイプも必要に応じて追加
        }

        private enum MFTopologyType
        {
            SourcestreamNode = 0,
            OutputNode = 3
        }

        private enum MFAttributesClsid
        {
            MF_TOPONODE_SOURCE = 0xC,
            MF_TOPONODE_PRESENTATION_DESCRIPTOR = 0xD,
            MF_TOPONODE_STREAM_DESCRIPTOR = 0xE,
            MF_EVENT_TYPE = 0x2
        }

        // IMFAsyncCallback インターフェースの実装
        private enum MFASync
        {
            None = 0x00000000
        }

        private enum MFAsyncCallbackQueue
        {
            Standard = 0x00000000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropVariant
        {
            public short vt;
            public short wReserved1;
            public short wReserved2;
            public short wReserved3;
            public IntPtr p;
            public int p2;
        }

        #endregion
    }
}
