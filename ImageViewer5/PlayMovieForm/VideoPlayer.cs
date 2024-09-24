using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.MediaFoundation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Imaging;
using Timer = System.Windows.Forms.Timer;
using AppLoggerModule;

/*
 * 
 * 
 * 注意点:

このコードは、動画のフレームを読み込み、PictureBox に表示しますが、音声の再生は行っていません。
音声を再生するには、追加の実装が必要であり、Media Foundationを直接使用して音声を再生するのは複雑です。
RtlMoveMemory を使用してネイティブメモリからマネージドメモリにデータをコピーしています。
フォームには PictureBox と再生・停止ボタンが必要です。
参考サイトとの関連:

参考サイトのコードをベースに、動画フレームを取得し、それを連続して表示することで動画再生を実現しています。
MediaManager.Startup() と MediaManager.Shutdown() の適切な使用。
SourceReader を使用して動画からフレームを取得。
追加のアドバイス:

音声も含めて動画を再生したい場合は、Windows Media Player コントロールや他のマルチメディアライブラリの使用を検討してください。
SharpDX は高度なグラフィックス処理に適していますが、マルチメディア再生全般を簡単に行うためのものではありません。
 * 
 * 
 */
namespace PlayMovieForm
{
    public class VideoPlayerMediaFoundation : IDisposable
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", CallingConvention = CallingConvention.StdCall)]
        private static extern void RtlMoveMemory(IntPtr Destination, IntPtr Source, int Length);

        private SourceReader _sourceReader;
        private int _frameWidth, _frameHeight, _stride;
        private double _frameRate;
        private Timer _timer;
        private PictureBox _pictureBox;
        private bool _isPlaying;

        private AppLogger _logger;
        public VideoPlayerMediaFoundation(PictureBox pictureBox, AppLogger logger)
        {
            _pictureBox = pictureBox;
            _logger = logger;
            MediaManager.Startup();
        }
        public void SetMovie(string moviePath)
        {
            DisposeSourceReader();

            var attributes = new MediaAttributes(1);
            attributes.Set(SourceReaderAttributeKeys.EnableVideoProcessing.Guid, true);

            _sourceReader = new SourceReader(moviePath, attributes);

            // 入力動画ストリームのネイティブメディアタイプを取得
            var nativeMediaType = _sourceReader.GetNativeMediaType(SourceReaderIndex.FirstVideoStream, 0);

            // 入力動画ストリームのサブタイプ（フォーマット）を取得
            Guid inputVideoFormat = nativeMediaType.Get(MediaTypeAttributeKeys.Subtype);

            // フォーマット名を取得
            string formatName = GetVideoFormatName(inputVideoFormat);

            Debug.WriteLine($"Input video format GUID: {inputVideoFormat}");
            Debug.WriteLine($"Input video format: {formatName}");

            // 希望するメディアタイプを設定
            var mediaType = new MediaType();
            mediaType.Set(MediaTypeAttributeKeys.MajorType, MediaTypeGuids.Video);
            mediaType.Set(MediaTypeAttributeKeys.Subtype, VideoFormatGuids.Rgb32);

            try
            {
                _sourceReader.SetCurrentMediaType(SourceReaderIndex.FirstVideoStream, mediaType);

                var actualMediaType = _sourceReader.GetCurrentMediaType(SourceReaderIndex.FirstVideoStream);

                long frameSize = actualMediaType.Get(MediaTypeAttributeKeys.FrameSize);
                _frameWidth = (int)(frameSize >> 32);
                _frameHeight = (int)(frameSize & 0xFFFFFFFF);

                _stride = actualMediaType.Get(MediaTypeAttributeKeys.DefaultStride);

                // フレームレートの取得（既存のコードを使用）
                // ...
            }
            catch (SharpDX.SharpDXException ex)
            {
                // エラー情報と入力動画のフォーマットをログに出力
                _logger.PrintInfo($"Error setting media type: {ex.Message}");
                _logger.PrintInfo($"Input video format GUID: {inputVideoFormat}");
                _logger.PrintInfo($"Input video format: {formatName}");

                // ユーザーにメッセージを表示
                //MessageBox.Show($"動画のデコードに失敗しました。\nフォーマット: {formatName}\nGUID: {inputVideoFormat}\n必要なコーデックをインストールしてください。", "デコーダーが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Error); MessageBox.Show($"動画のデコードに失敗しました。\nフォーマット: {formatName}\nGUID: {inputVideoFormat}\n必要なコーデックをインストールしてください。", "デコーダーが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.PrintError(ex, $"動画のデコードに失敗しました。\nフォーマット: {formatName}\nGUID: {inputVideoFormat}\n必要なコーデックをインストールしてください。");

                // 必要に応じて例外を再スロー
                // throw;
            }
        }

        // フォーマットGUIDを人間が読める形式のフォーマット名にマッピングするヘルパーメソッド
        private string GetVideoFormatName(Guid formatGuid)
        {
            if (formatGuid == VideoFormatGuids.H264)
                return "H.264";
            if (formatGuid == VideoFormatGuids.Mjpg)
                return "Motion JPEG";
            if (formatGuid == VideoFormatGuids.Mpeg2)
                return "MPEG-2";
            if (formatGuid == VideoFormatGuids.Wmv3)
                return "WMV3";
            if (formatGuid == VideoFormatGuids.Hevc)
                return "HEVC (H.265)";
            // 必要に応じて他のフォーマットを追加

            // 不明なフォーマットの場合はGUIDをそのまま返す
            return formatGuid.ToString();
        }

        public void Play()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            int interval = (int)(1000 / _frameRate);

            _timer = new Timer();
            _timer.Interval = interval > 0 ? interval : 33; // intervalが0以下の場合は33msに設定
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isPlaying)
                return;

            int actualStreamIndex;
            SourceReaderFlags flags;
            long timeStamp;

            try
            {
                var sample = _sourceReader.ReadSample(SourceReaderIndex.FirstVideoStream, SourceReaderControlFlags.None, out actualStreamIndex, out flags, out timeStamp);

                if (sample != null)
                {
                    using (sample)
                    {
                        using (var buffer = sample.ConvertToContiguousBuffer())
                        {
                            IntPtr bufferPtr;
                            int maxLength, currentLength;

                            bufferPtr = buffer.Lock(out maxLength, out currentLength);

                            // コピーサイズをバッファの実際の長さと比較して決定
                            int copySize = Math.Min(currentLength, _stride * _frameHeight);

                            Bitmap bitmap = new Bitmap(_frameWidth, _frameHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                            var bmpData = bitmap.LockBits(new Rectangle(0, 0, _frameWidth, _frameHeight), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                            // メモリコピーを安全に行う
                            RtlMoveMemory(bmpData.Scan0, bufferPtr, copySize);

                            bitmap.UnlockBits(bmpData);

                            buffer.Unlock();

                            // PictureBoxを更新
                            _pictureBox.BeginInvoke((Action)(() =>
                            {
                                if (_pictureBox.Image != null)
                                {
                                    _pictureBox.Image.Dispose();
                                }
                                _pictureBox.Image = bitmap;
                            }));
                        }
                    }
                }
                else
                {
                    // 再生終了
                    Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Timer_Tick: {ex.Message}");
                Stop();
            }
        }

        public void Stop()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        private void DisposeSourceReader()
        {
            if (_sourceReader != null)
            {
                _sourceReader.Dispose();
                _sourceReader = null;
            }
        }

        public void Dispose()
        {
            Stop();
            DisposeSourceReader();
            MediaManager.Shutdown();
        }
    }
}