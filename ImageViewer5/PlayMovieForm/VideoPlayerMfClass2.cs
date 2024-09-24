using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AppLoggerModule;
using System.Drawing;

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

            MFCreateSourceResolver(out IMFSourceResolver sourceResolver);

            MF_OBJECT_TYPE ObjectType;

            sourceResolver.CreateObjectFromURL(
                url,
                MFResolution.MediaSource,
                null,
                out ObjectType,
                out object source);

            _mediaSource = (IMFMediaSource)source;
            Marshal.ReleaseComObject(sourceResolver);
        }

        private void CreateTopology()
        {
            MFCreateTopology(out _topology);

            _mediaSource.CreatePresentationDescriptor(out IMFPresentationDescriptor presentationDescriptor);

            presentationDescriptor.GetStreamDescriptorCount(out int streamCount);

            for (int i = 0; i < streamCount; i++)
            {
                presentationDescriptor.GetStreamDescriptorByIndex(i, out bool selected, out IMFStreamDescriptor streamDescriptor);

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
            MFCreateTopologyNode(MFTopologyType.SourcestreamNode, out IMFTopologyNode sourceNode);
            sourceNode.SetUnknown(MF_TOPONODE_ATTRIBUTE.MF_TOPONODE_SOURCE, mediaSource);
            sourceNode.SetUnknown(MF_TOPONODE_ATTRIBUTE.MF_TOPONODE_PRESENTATION_DESCRIPTOR, presentationDescriptor);
            sourceNode.SetUnknown(MF_TOPONODE_ATTRIBUTE.MF_TOPONODE_STREAM_DESCRIPTOR, streamDescriptor);

            // 出力ノードの作成
            MFCreateTopologyNode(MFTopologyType.OutputNode, out IMFTopologyNode outputNode);

            streamDescriptor.GetMediaTypeHandler(out IMFMediaTypeHandler mediaTypeHandler);
            mediaTypeHandler.GetMajorType(out Guid majorType);

            if (majorType == MFMediaType_Audio)
            {
                // オーディオレンダラーの作成
                MFCreateAudioRendererActivate(out IMFActivate activate);

                outputNode.SetObject(activate);

                // ボリュームコントロールの取得
                activate.ActivateObject(typeof(IMFSimpleAudioVolume).GUID, out object volumeObject);
                _volumeControl = (IMFSimpleAudioVolume)volumeObject;

                Marshal.ReleaseComObject(activate);
            }
            else if (majorType == MFMediaType_Video)
            {
                // ビデオレンダラーの作成
                MFCreateVideoRendererActivate(_videoControl.Handle, out IMFActivate activate);

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
        public HRESULT GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Standard;
            return HRESULT.S_OK;
        }

        public HRESULT Invoke(IMFAsyncResult pAsyncResult)
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

            return HRESULT.S_OK;
        }

        #region PInvokeとCOMインターフェースの定義

        // HRESULT 定義
        private enum HRESULT : uint
        {
            S_OK = 0x00000000,
            MF_E_NO_MORE_TYPES = 0xC00D36B9
        }

        // PropVariant 構造体の定義
        [StructLayout(LayoutKind.Sequential)]
        private struct PropVariant
        {
            public ushort vt;
            public ushort wReserved1;
            public ushort wReserved2;
            public ushort wReserved3;
            public IntPtr p;
            public int p2;
        }

        // PInvoke の定義
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

        // 必要な定数とGUIDの定義
        private static readonly Guid MFMediaType_Audio = new Guid("73647561-0000-0010-8000-00AA00389B71");
        private static readonly Guid MFMediaType_Video = new Guid("73646976-0000-0010-8000-00AA00389B71");

        // ENUM 定義
        private enum MF_OBJECT_TYPE
        {
            Invalid,
            MediaSource,
            ByteStream,
            Stream
        }

        [Flags]
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
            MESessionStarted = 0x000D,
            MESessionEnded = 0x0011
            // 他のイベントタイプも必要に応じて追加
        }

        private enum MFTopologyType
        {
            SourcestreamNode = 0,
            OutputNode = 3
        }

        private enum MF_TOPONODE_ATTRIBUTE
        {
            MF_TOPONODE_SOURCE = 0xC,
            MF_TOPONODE_PRESENTATION_DESCRIPTOR = 0xD,
            MF_TOPONODE_STREAM_DESCRIPTOR = 0xE,
            MF_EVENT_TYPE = 0x2
        }

        private enum MFASync
        {
            None = 0x00000000
        }

        private enum MFAsyncCallbackQueue
        {
            Standard = 0x00000000
        }

        // COMインターフェースの定義
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("D19F8E98-B126-4446-890C-5DCB7AD71453")]
        private interface IMFMediaSession
        {
            [PreserveSig]
            HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
            [PreserveSig]
            HRESULT BeginGetEvent(IMFAsyncCallback pCallback, [MarshalAs(UnmanagedType.IUnknown)] object o);
            [PreserveSig]
            HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
            [PreserveSig]
            HRESULT QueueEvent(MediaEventType met, [In] ref Guid guidExtendedType, int hrStatus, [In] PropVariant pvValue);
            [PreserveSig]
            HRESULT Start([In] ref Guid pguidTimeFormat, [In] PropVariant pvarStartPosition);
            [PreserveSig]
            HRESULT Stop();
            [PreserveSig]
            HRESULT Pause();
            [PreserveSig]
            HRESULT Close();
            [PreserveSig]
            HRESULT Shutdown();
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7BE19E73-C9BF-468A-AC5A-A5E8653BEC87")]
        private interface IMFSourceResolver
        {
            [PreserveSig]
            HRESULT CreateObjectFromURL(
                [MarshalAs(UnmanagedType.LPWStr)] string pwszURL,
                MFResolution dwFlags,
                [In] IMFAttributes pProps,
                out MF_OBJECT_TYPE pObjectType,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56C03D9C-9DBB-45F5-AB4B-D80F47C05938")]
        private interface IMFMediaSource
        {
            [PreserveSig]
            HRESULT GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
            [PreserveSig]
            HRESULT BeginGetEvent(IMFAsyncCallback pCallback, [MarshalAs(UnmanagedType.IUnknown)] object o);
            [PreserveSig]
            HRESULT EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
            [PreserveSig]
            HRESULT QueueEvent(MediaEventType met, [In] ref Guid guidExtendedType, int hrStatus, [In] PropVariant pvValue);
            [PreserveSig]
            HRESULT GetCharacteristics(out uint pdwCharacteristics);
            [PreserveSig]
            HRESULT CreatePresentationDescriptor(out IMFPresentationDescriptor ppPresentationDescriptor);
            [PreserveSig]
            HRESULT Start(IMFPresentationDescriptor pPresentationDescriptor, [In] ref Guid pguidTimeFormat, [In] PropVariant pvarStartPosition);
            [PreserveSig]
            HRESULT Stop();
            [PreserveSig]
            HRESULT Pause();
            [PreserveSig]
            HRESULT Shutdown();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("03CB2711-24D7-4DB6-A17F-F3A7A479A536")]
        private interface IMFPresentationDescriptor
        {
            [PreserveSig]
            HRESULT GetStreamDescriptorCount(out int pdwDescriptorCount);
            [PreserveSig]
            HRESULT GetStreamDescriptorByIndex(int dwIndex, out bool pfSelected, out IMFStreamDescriptor ppDescriptor);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56C03D9C-9DBB-45F5-AB4B-D80F47C05939")]
        private interface IMFStreamDescriptor
        {
            [PreserveSig]
            HRESULT GetStreamIdentifier(out uint pdwStreamIdentifier);
            [PreserveSig]
            HRESULT GetMediaTypeHandler(out IMFMediaTypeHandler ppMediaTypeHandler);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("E93DCF6C-4B07-4E1E-8123-AA16ED6EAB5F")]
        private interface IMFMediaTypeHandler
        {
            [PreserveSig]
            HRESULT GetMajorType(out Guid pguidMajorType);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("83CF873A-F6DA-4BC8-823F-BACFD55DC433")]
        private interface IMFTopology
        {
            [PreserveSig]
            HRESULT AddNode(IMFTopologyNode pNode);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("83CF873A-F6DA-4BC8-823F-BACFD55DC430")]
        private interface IMFTopologyNode
        {
            [PreserveSig]
            HRESULT SetUnknown(MF_TOPONODE_ATTRIBUTE guidKey, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown);
            [PreserveSig]
            HRESULT SetObject([MarshalAs(UnmanagedType.IUnknown)] object pObject);
            [PreserveSig]
            HRESULT ConnectOutput(uint dwOutputIndex, IMFTopologyNode pDownstreamNode, uint dwInputIndexOnDownstreamNode);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FA993888-4383-415A-A930-DD472A8CF6F7")]
        private interface IMFActivate
        {
            [PreserveSig]
            HRESULT ActivateObject([In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
        private interface IMFVideoDisplayControl
        {
            // メソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("89EF044C-F9C7-4E7B-8833-3435C2C34C90")]
        private interface IMFSimpleAudioVolume
        {
            [PreserveSig]
            HRESULT SetMasterVolume(float fLevel);
            // 他のメソッドは省略
        }

        // IMFAsyncCallback インターフェースの定義
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("A27003CF-2354-4F2A-8D6A-AB7CFF15437E")]
        private interface IMFAsyncCallback
        {
            [PreserveSig]
            HRESULT GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue);
            [PreserveSig]
            HRESULT Invoke(IMFAsyncResult pAsyncResult);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("AC6B7889-0740-4D51-8619-905994A55CC6")]
        private interface IMFAsyncResult
        {
            // メソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
        private interface IMFMediaEvent
        {
            [PreserveSig]
            HRESULT GetType(out MediaEventType pmet);
            // 他のメソッドは省略
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("2CD0BD52-BCD5-4B89-B62C-EADC0C031E7D")]
        private interface IMFAttributes
        {
            // メソッドは省略
        }

        #endregion
    }
}
