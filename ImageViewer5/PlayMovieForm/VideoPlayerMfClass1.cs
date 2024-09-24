using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AppLoggerModule;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace PlayMovieForm
{
    public class VideoPlayerMf : IDisposable
    {
        private Control _videoControl;
        private bool _isPlaying;
        private string _moviePath = "";

        private AppLogger _logger;

        private IMFMediaSession _mediaSession;
        private IMFMediaSource _mediaSource;
        private IMFTopology _topology;

        private IMFVideoDisplayControl _videoDisplayControl;
        private IMFVolumeControl _volumeControl;

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

            if (majorType == MFMediaType.Audio)
            {
                // オーディオレンダラーの作成
                IMFActivate activate;
                MFCreateAudioRendererActivate(out activate);

                outputNode.SetObject(activate);

                // ボリュームコントロールの取得
                object volumeObject;
                activate.ActivateObject(typeof(IMFSimpleAudioVolume).GUID, out volumeObject);
                _volumeControl = (IMFVolumeControl)volumeObject;

                Marshal.ReleaseComObject(activate);
            }
            else if (majorType == MFMediaType.Video)
            {
                // ビデオレンダラーの作成
                IMFActivate activate;
                MFCreateVideoRendererActivate(_videoControl.Handle, out activate);

                outputNode.SetObject(activate);

                // ビデオディスプレイコントロールの取得
                object videoDisplayObject;
                activate.ActivateObject(typeof(IMFVideoDisplayControl).GUID, out videoDisplayObject);
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
        public void GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Standard;
        }

        public void Invoke(IMFAsyncResult pAsyncResult)
        {
            IMFMediaEvent mediaEvent;
            _mediaSession.EndGetEvent(pAsyncResult, out mediaEvent);

            Guid guidType;
            mediaEvent.GetGUID(MFAttributesClsid.MF_EVENT_TYPE, out guidType);

            var eventType = (MediaEventType)guidType;

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

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7BE19E73-C9BF-468A-AC5A-A5E8653BEC87")]
        private interface IMFSourceResolver
        {
            void CreateObjectFromURL(
                [MarshalAs(UnmanagedType.LPWStr)] string pwszURL,
                MFResolution dwFlags,
                IMFPropertyStore pProps,
                out MF_OBJECT_TYPE pObjectType,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppObject);

            void CreateObjectFromByteStream(
                IMFByteStream pByteStream,
                [MarshalAs(UnmanagedType.LPWStr)] string pwszURL,
                MFResolution dwFlags,
                IMFPropertyStore pProps,
                out MF_OBJECT_TYPE pObjectType,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppObject);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56C03D9C-9DBB-45F5-AB4B-D80F47C05938")]
        private interface IMFMediaSource
        {
            void GetEvent(uint dwFlags, out IMFMediaEvent ppEvent);
            void BeginGetEvent(IMFAsyncCallback pCallback, [MarshalAs(UnmanagedType.IUnknown)] object o);
            void EndGetEvent(IMFAsyncResult pResult, out IMFMediaEvent ppEvent);
            void QueueEvent(MediaEventType met, [In] ref Guid guidExtendedType, int hrStatus, [In] PropVariant pvValue);
            void GetCharacteristics(out uint pdwCharacteristics);
            void CreatePresentationDescriptor(out IMFPresentationDescriptor ppPresentationDescriptor);
            void Start(IMFPresentationDescriptor pPresentationDescriptor, [In] ref Guid pguidTimeFormat, [In] PropVariant pvarStartPosition);
            void Stop();
            void Pause();
            void Shutdown();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("2CD0BD52-BCD5-4B89-B62C-EADC0C031E7D")]
        private interface IMFPresentationDescriptor
        {
            void GetStreamDescriptorCount(out int pdwDescriptorCount);
            void GetStreamDescriptorByIndex(int dwIndex, out bool pfSelected, out IMFStreamDescriptor ppDescriptor);
            void SelectStream(int dwIndex);
            void DeselectStream(int dwIndex);
            void Clone(out IMFPresentationDescriptor ppPresentationDescriptor);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56C03D9C-9DBB-45F5-AB4B-D80F47C05939")]
        private interface IMFStreamDescriptor
        {
            void GetStreamIdentifier(out uint pdwStreamIdentifier);
            void GetMediaTypeHandler(out IMFMediaTypeHandler ppMediaTypeHandler);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("E93DCF6C-4B07-4E1E-8123-AA16ED6EAB5F")]
        private interface IMFMediaTypeHandler
        {
            void IsMediaTypeSupported(IMFMediaType pMediaType, out IMFMediaType ppMediaType);
            void GetMediaTypeCount(out int pdwTypeCount);
            void GetMediaTypeByIndex(int dwIndex, out IMFMediaType ppType);
            void SetCurrentMediaType(IMFMediaType pMediaType);
            void GetCurrentMediaType(out IMFMediaType ppMediaType);
            void GetMajorType(out Guid pguidMajorType);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7651A3D4-0E5E-4DCA-A36E-B61D9A1C0C40")]
        private interface IMFMediaType
        {
            // IMFAttributes のメソッドを継承
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("83CF873A-F6DA-4BC8-823F-BACFD55DC433")]
        private interface IMFTopology
        {
            void GetTopologyID(out ulong pID);
            void AddNode(IMFTopologyNode pNode);
            void RemoveNode(IMFTopologyNode pNode);
            void GetNodeCount(out ushort pwNodes);
            void GetNode(ushort wIndex, out IMFTopologyNode ppNode);
            void Clear();
            void Clone(out IMFTopology ppTopology);
            void GetNodeByID(ulong qwTopoNodeID, out IMFTopologyNode ppNode);
            void GetSourceNodeCollection(out IMFCollection ppCollection);
            void GetOutputNodeCollection(out IMFCollection ppCollection);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("83CF873A-F6DA-4BC8-823F-BACFD55DC430")]
        private interface IMFTopologyNode
        {
            void SetObject([MarshalAs(UnmanagedType.IUnknown)] object pObject);
            void GetObject([MarshalAs(UnmanagedType.IUnknown)] out object ppObject);
            void GetNodeType(out MFTopologyType pType);
            void GetTopoNodeID(out ulong pID);
            void SetTopoNodeID(ulong ullTopoID);
            void GetInputCount(out uint pcInputs);
            void GetOutputCount(out uint pcOutputs);
            void ConnectOutput(uint dwOutputIndex, IMFTopologyNode pDownstreamNode, uint dwInputIndexOnDownstreamNode);
            void DisconnectOutput(uint dwOutputIndex);
            void GetInput(uint dwInputIndex, out IMFTopologyNode ppUpstreamNode, out uint pdwOutputIndexOnUpstreamNode);
            void GetOutput(uint dwOutputIndex, out IMFTopologyNode ppDownstreamNode, out uint pdwInputIndexOnDownstreamNode);
            void SetOutputPrefType(uint dwOutputIndex, IMFMediaType pType);
            void GetOutputPrefType(uint dwOutputIndex, out IMFMediaType ppType);
            void SetInputPrefType(uint dwInputIndex, IMFMediaType pType);
            void GetInputPrefType(uint dwInputIndex, out IMFMediaType ppType);
            void CloneFrom(IMFTopologyNode pNode);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("FA993888-4383-415A-A930-DD472A8CF6F7")]
        private interface IMFActivate
        {
            // IMFAttributes のメソッドを継承
            void ActivateObject([In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
            void ShutdownObject();
            void DetachObject();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3")]
        private interface IMFVideoDisplayControl
        {
            void SetVideoWindow(IntPtr hwndVideo);
            void GetVideoWindow(out IntPtr phwndVideo);
            void SetVideoPosition([In] MFVideoNormalizedRect pnrcSource, [In] Rectangle prcDest);
            void GetVideoPosition(out MFVideoNormalizedRect pnrcSource, out Rectangle prcDest);
            void SetAspectRatioMode(MFVideoAspectRatioMode dwAspectRatioMode);
            void GetAspectRatioMode(out MFVideoAspectRatioMode pdwAspectRatioMode);
            void SetVideoClipRect([In] Rectangle prcClip);
            void GetVideoClipRect(out Rectangle prcClip);
            void SetRenderingPrefs(MFVideoRenderPrefs dwRenderFlags);
            void GetRenderingPrefs(out MFVideoRenderPrefs pdwRenderFlags);
            void SetFullscreen(bool fFullscreen);
            void GetFullscreen(out bool pfFullscreen);
            void SetBorderColor(int Clr);
            void GetBorderColor(out int pClr);
            void SetRenderingMode(MFVideoRenderMode dwMode);
            void GetRenderingMode(out MFVideoRenderMode pdwMode);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3FDFFA9B-4F0C-47FF-BF36-4FAF1D2C5F02")]
        private interface IMFSimpleAudioVolume
        {
            void SetMasterVolume(float fLevel);
            void GetMasterVolume(out float pfLevel);
            void SetMute(bool bMute);
            void GetMute(out bool pbMute);
        }

        // その他の必要な定数や列挙型、構造体
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

        private enum MFMediaType
        {
            Audio,
            Video
        }

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

        [StructLayout(LayoutKind.Sequential)]
        private struct MFVideoNormalizedRect
        {
            public float left;
            public float top;
            public float right;
            public float bottom;
        }

        private enum MFVideoAspectRatioMode
        {
            None = 0,
            PreservePicture = 0x00000001,
            PreservePixel = 0x00000002
        }

        private enum MFVideoRenderPrefs
        {
            None = 0,
            DoNotRenderBorder = 0x00000001,
            DoNotClipToDevice = 0x00000002
        }

        private enum MFVideoRenderMode
        {
            RenderToWindow = 0,
            RenderToSurface = 1
        }

        private interface IMFAttributes
        {
            // 定義省略
        }

        private interface IMFAsyncCallback
        {
            void GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue);
            void Invoke(IMFAsyncResult pAsyncResult);
        }

        private interface IMFAsyncResult
        {
            // 定義省略
        }

        private interface IMFMediaEvent
        {
            void GetItem([In] ref Guid guidKey, [Out] PropVariant pValue);
            void GetItemType([In] ref Guid guidKey, out int pType);
            void CompareItem([In] ref Guid guidKey, [In] PropVariant Value, out bool pbResult);
            void Compare([MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs, int MatchType, out bool pbResult);
            void GetUINT32([In] ref Guid guidKey, out int punValue);
            void GetUINT64([In] ref Guid guidKey, out long punValue);
            void GetDouble([In] ref Guid guidKey, out double pfValue);
            void GetGUID([In] ref Guid guidKey, out Guid pguidValue);
            void GetStringLength([In] ref Guid guidKey, out int pcchLength);
            void GetString([In] ref Guid guidKey, [Out] string pwszValue, int cchBufSize, out int pcchLength);
            void GetAllocatedString([In] ref Guid guidKey, out string ppwszValue, out int pcchLength);
            void GetBlobSize([In] ref Guid guidKey, out int pcbBlobSize);
            void GetBlob([In] ref Guid guidKey, [Out] byte[] pBuf, int cbBufSize, out int pcbBlobSize);
            void GetAllocatedBlob([In] ref Guid guidKey, out IntPtr ip, out int pcbSize);
            void GetUnknown([In] ref Guid guidKey, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
            void SetItem([In] ref Guid guidKey, [In] PropVariant Value);
            void DeleteItem([In] ref Guid guidKey);
            void DeleteAllItems();
            void SetUINT32([In] ref Guid guidKey, int unValue);
            void SetUINT64([In] ref Guid guidKey, long unValue);
            void SetDouble([In] ref Guid guidKey, double fValue);
            void SetGUID([In] ref Guid guidKey, [In] ref Guid guidValue);
            void SetString([In] ref Guid guidKey, [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue);
            void SetBlob([In] ref Guid guidKey, [In] byte[] pBuf, int cbBufSize);
            void SetUnknown([In] ref Guid guidKey, [MarshalAs(UnmanagedType.IUnknown)] object pUnknown);
            void LockStore();
            void UnlockStore();
            void GetCount(out int pcItems);
            void GetItemByIndex(int unIndex, out Guid pguidKey, out PropVariant pValue);
            void CopyAllItems([MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest);

            void GetType(out MediaEventType pmet);
            void GetExtendedType(out Guid pguidExtendedType);
            void GetStatus(out int phrStatus);
            void GetValue(out PropVariant pvValue);
        }

        private interface IMFClock
        {
            // 定義省略
        }

        private interface IMFCollection
        {
            // 定義省略
        }

        #endregion
    }
}
