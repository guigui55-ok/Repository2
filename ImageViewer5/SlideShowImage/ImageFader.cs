using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading.Tasks;
using AppLoggerModule;

public class ImageFader
{
    private AppLogger _logger;
    private Control _control;
    private int _fadeDuration;

    // コンストラクタでAppLoggerとControlを指定する
    public ImageFader(AppLogger logger, Control control, int fadeDuration = 400)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _control = control ?? throw new ArgumentNullException(nameof(control));
        _fadeDuration = fadeDuration;

        _logger.PrintInfo($"ImageFader initialized with fade duration: {_fadeDuration}ms.");
    }

    // フェードアウト・フェードインを行うメソッド
    public async Task FadeOutAndInAsync(Image newImage)
    {
        try
        {
            _logger.PrintInfo("Starting fade out and in process.");
            
            if (_control is PictureBox pictureBox)
            {
                // フェードアウト
                await FadeOutAsync(pictureBox);
                _logger.PrintInfo("Fade out completed.");

                // 新しい画像を設定
                SetImage(pictureBox, newImage);

                // フェードイン
                await FadeInAsync(pictureBox);
                _logger.PrintInfo("Fade in completed.");
            }
            else
            {
                _logger.PrintError(new InvalidOperationException("Control is not a PictureBox."), "Control type mismatch.");
            }
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, "Error occurred during fade out and in process.");
        }
    }

    // フェードアウト処理
    private async Task FadeOutAsync(PictureBox pictureBox)
    {
        for (float opacity = 1f; opacity >= 0; opacity -= 0.05f)
        {
            pictureBox.Image = ChangeImageOpacity(pictureBox.Image, opacity);
            await Task.Delay(_fadeDuration / 20);
            pictureBox.Refresh();  // 画像を再描画
        }
    }

    // フェードイン処理
    private async Task FadeInAsync(PictureBox pictureBox)
    {
        for (float opacity = 0f; opacity <= 1f; opacity += 0.05f)
        {
            pictureBox.Image = ChangeImageOpacity(pictureBox.Image, opacity);
            await Task.Delay(_fadeDuration / 20);
            pictureBox.Refresh();  // 画像を再描画
        }
    }

    // 画像の透明度を変更するメソッド
    private Image ChangeImageOpacity(Image image, float opacity)
    {
        if (image == null)
            return null;

        try
        {
            // アニメーション画像（例：GIF）の場合は処理をスキップ
            if (ImageAnimator.CanAnimate(image))
            {
                _logger.PrintError(new ArgumentException("Animated images are not supported"), "Invalid image format.");
                return image;
            }

            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;  // 透明度の変更
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, "Failed to change image opacity.");
            return image;
        }
    }

    // 新しい画像をPictureBoxに設定するメソッド
    private void SetImage(PictureBox pictureBox, Image newImage)
    {
        try
        {
            pictureBox.Image?.Dispose();
            pictureBox.Image = newImage;
        }
        catch (Exception ex)
        {
            _logger.PrintError(ex, "Failed to set new image.");
        }
    }
}
