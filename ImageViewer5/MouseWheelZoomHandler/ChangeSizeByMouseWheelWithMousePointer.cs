using System;
using System.Drawing;
using System.Windows.Forms;
using AppLoggerModule;

namespace CommonControlUtilityModuleB
{
    public class ChangeSizeByMouseWheelWithMousePointer
    {
        protected AppLogger _logger;
        protected Control _innerControl;
        protected Control _parentControl;
        public EventHandler MouseWheelBegin;
        public EventHandler MouseWheelEnd;
        protected PointF bufPointF;
        protected PointF RatioSizeInnerFromFrame;
        protected Double RatioLocationInnerFromFrameX;
        protected Double RatioLocationInnerFromFrameY;
        public double ExpantionRaito = 1.05;
        public double ShrinkRaito = 0.952;
        public ChangeSizeByMouseWheelWithMousePointer(AppLogger logger, Control control, Control parentControl)
        {
            _logger = logger;
            _innerControl = control;
            _parentControl = parentControl;
            _parentControl.MouseWheel += Control_MouseWheel;
        }
        public ChangeSizeByMouseWheelWithMousePointer(
            AppLogger logger, 
            Control control,
            Control parentControl,
            EventHandler wheelBegin,
            EventHandler wheelEnd)
        {
            _logger = logger;
            _innerControl = control;
            _parentControl = parentControl;
            MouseWheelBegin = wheelBegin;
            MouseWheelEnd = wheelEnd;
            _parentControl.MouseWheel += Control_MouseWheel;
        }


        private void Control_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (MouseWheelBegin != null) { MouseWheelBegin.Invoke(sender, e); }
                // 描画を止める
                _innerControl.SuspendLayout();
                _parentControl.SuspendLayout();
                if (e.Delta > 0)
                {
                    // 上回転したとき
                    // 拡大 1.05倍
                    ExpansionSizeViewControl(ExpantionRaito);
                }
                else if (e.Delta < 0)
                {
                    // 下回転したとき
                    // 縮小 0.952倍
                    ExpansionSizeViewControl(ShrinkRaito);
                }
                // 描画を再開
                _innerControl.ResumeLayout();
                _parentControl.ResumeLayout();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "MouseWheelEvent");
            }
            finally
            {
                if (MouseWheelEnd != null) { MouseWheelEnd.Invoke(sender, e); }
            }
        }
        public void ControlPausePaint(bool flag)
        {
            try
            {
                if (flag)
                {
                    _innerControl.SuspendLayout();
                    _parentControl.SuspendLayout();
                } else
                {
                    _innerControl.ResumeLayout();
                    _parentControl.ResumeLayout();
                }
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "ControlSuspendLayout Failed");
            }
        }

        /// <summary>
        /// 縦横比をキープしたままサイズを縮小させる、
        /// WidthをfixedValueX この値にして、縦のY座標をこれに合わせる
        /// </summary>
        /// <param name="size"></param>
        /// <param name="fixedValueX"></param>
        /// <returns></returns>
        private Size GetSizeShrinkKeepingAspect(Size size, int fixedValueX)
        {
            double ratio = fixedValueX / size.Width;
            return new Size(fixedValueX, (int)(size.Height * ratio));
        }

        /// <summary>
        /// コントロールの拡大・縮小
        /// <param name="raito">現在のサイズからの倍率</param>
        /// </summary>
        public void ExpansionSizeViewControl(double raito)
        {
            try
            {
                _logger.AddLog(this, "ExpansionSizeViewControl");
                Size newSize = new Size((int)(_innerControl.Width * raito), (int)(_innerControl.Height * raito));

                // Size ゼロかつ raito が1以上の時はサイズを調整する
                // Size 20以下に変更（おそらく倍率積でintにすると値が変化しなくなる）
                //((newSize.Width == 0)&&(raito > 1.0))
                if ((newSize.Width < 20)&&(raito > 1.0)) {
                    _logger.AddLog("  ((newSize.Width == 0)||(raito > 1.0))==true => Resize");
                    newSize = GetSizeShrinkKeepingAspect(newSize, 32); 
                }

                // 拡大縮小時にLocationを変更のための計算
                Point newLocation = GetLocationByCalcExpansionWhenChangeSize(_innerControl.Size, newSize);
                // Control の描画を停止
                ControlPausePaint(true);
                // サイズ変更
                _innerControl.Size = newSize;
                _logger.AddLog(" Size:"+_innerControl.Size.Width + "," + _innerControl.Size.Height + "=>" 
                    + newSize.Width + "," + newSize.Height);
                // ポジション変更
                _innerControl.Location = newLocation;
                _logger.AddLog(" Location:" + _innerControl.Location.X + "," + _innerControl.Location.Y + "=>"
                    + newLocation.X  + "," + newLocation.Y);
                // Control の描画を再開
                ControlPausePaint(false);
                // 位置とサイズを記憶する
                SaveRaitoSizeAndPositionFromFrameControl();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ChangeSizeViewControl Failed");
            }
        }

        // 画像表示時、マウスドラッグでコントロール移動時、拡大縮小時に記録する
        public void SaveRaitoSizeAndPositionFromFrameControl()
        {
            try
            {

                // 意図的に InnerSize を変えたときには保存する
                // 外枠 FrameControl での InnerControl.Location と InnerControl.Size を保存
                // InnerControlが変化した際に保存するが、FrameControlがともに変化しているときは保存しない
                //if (State.IsFrameSizeChanging) { return; }
                Size frameSize = _parentControl.Size;
                // Size Ratio
                bufPointF.X = (float)_innerControl.Size.Width / (float)_parentControl.Size.Width;
                bufPointF.Y = (float)_innerControl.Size.Height / (float)_parentControl.Size.Height;
                RatioSizeInnerFromFrame = bufPointF;
                // Location Raito
                RatioLocationInnerFromFrameX = (double)_innerControl.Location.X / (double)_parentControl.Size.Width;
                RatioLocationInnerFromFrameY = (double)_innerControl.Location.Y / (double)_parentControl.Size.Height;

                //Debug.WriteLine("save size ratio = " + State.RatioSizeInnerFromFrame.X  + " , " + State.RatioSizeInnerFromFrame.Y);
                //Debug.WriteLine("save pos  ratio = " + State.RatioLocationInnerFromFrameX + " , " + State.RatioLocationInnerFromFrameY);

                // Size Difference
                //bufPoint.X = frameSize.Width - _innerControl.Width;
                //bufPoint.Y = frameSize.Height - _innerControl.Height;
                //State.DifferenceSizeFromContents = bufPoint;

                //// Position
                //State.DifferencePositionFromContents = _innerControl.Location;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "saveDifferenceSizeAndPositionFromFramecControl Failed");
                return;
            }
        }

        enum DIRECTION
        {
            UP,
            DOWN,
            RIGHT,
            LEFT
        }


        /// <summary>
        /// 座標Bを座標Aに向かって指定した割合で移動させる関数。
        /// </summary>
        /// <param name="pointA">固定する座標A</param>
        /// <param name="pointB">移動させる座標B</param>
        /// <param name="reductionRatio">距離を短くする割合（0.0〜1.0）</param>
        /// <returns>新しい座標B</returns>
        public Point GetModifiedPoint(Point pointA, Point pointB, double reductionRatio)
        {
            // 割合が0.0以下または1.0以上の場合はそのまま返す
            if (reductionRatio <= 0.0) return pointA;
            if (reductionRatio >= 1.0) return pointB;

            // 2点間の距離の差分を計算
            double dx = pointB.X - pointA.X;
            double dy = pointB.Y - pointA.Y;

            // 新しい座標を計算
            int newX = pointA.X + (int)(dx * reductionRatio);
            int newY = pointA.Y + (int)(dy * reductionRatio);

            return new Point(newX, newY);
        }

        /// <summary>
        /// 座標Aから座標Bが特定の距離内に含まれるかどうかを判定します。
        /// </summary>
        /// <param name="pointA">基準となる座標A</param>
        /// <param name="pointB">判定対象の座標B</param>
        /// <param name="distance">判定する距離</param>
        /// <returns>座標Bが距離内に含まれていればtrue、それ以外はfalse</returns>
        public bool IsPointWithinDistance(Point pointA, Point pointB, double distance)
        {
            // 2点間の距離を計算する
            double dx = pointB.X - pointA.X;
            double dy = pointB.Y - pointA.Y;
            double distanceBetweenPoints = Math.Sqrt(dx * dx + dy * dy);

            _logger.PrintInfo(String.Format("distanceBetweenPoints = {0}", distanceBetweenPoints));

            // 2点間の距離が指定された距離以内かどうかを判定する
            return distanceBetweenPoints <= distance;
        }


        /// <summary>
        /// 座標Bを座標Aに向かって、指定した距離だけ移動させる関数。
        /// </summary>
        /// <param name="pointA">固定する座標A</param>
        /// <param name="pointB">移動させる座標B</param>
        /// <param name="moveDistance">移動させる距離</param>
        /// <returns>新しい座標B</returns>
        public Point MovePointTowards(Point pointA, Point pointB, double moveDistance)
        {
            // 2点間の距離を計算
            double dx = pointB.X - pointA.X;
            double dy = pointB.Y - pointA.Y;
            double distanceBetweenPoints = Math.Sqrt(dx * dx + dy * dy);

            // 座標Bがすでに座標Aに重なっている場合、移動しない
            if (distanceBetweenPoints == 0.0)
            {
                return pointB;
            }

            // 指定した距離が現在の距離より大きい場合、座標Bを座標Aに重ねる
            if (moveDistance >= distanceBetweenPoints)
            {
                return pointA;
            }

            // 指定した距離分だけ移動する割合を計算
            double ratio = moveDistance / distanceBetweenPoints;

            // 新しい座標を計算
            int bufX;
            int bufY;
            //int bufX = (int)(dx * ratio);
            //int bufY = (int)(dy * ratio);
            bufX = (int)(double)Math.Round(dx * ratio);
            bufY = (int)(double)Math.Round(dy * ratio);
            //double dbufX = dx * ratio;
            //double dbufX;
            int newX = pointA.X + bufX;
            int newY = pointA.Y + bufY;

            return new Point(newX, newY);
        }


        public Point MovePointTowardsB(Point pointA, Point pointB, double moveDistanceX, double moveDistanceY)
        {
            // 2点間の距離を計算
            //double dx = pointB.X - pointA.X;
            //double dy = pointB.Y - pointA.Y;
            //double distanceBetweenPoints = Math.Sqrt(dx * dx + dy * dy);
            int moveX;
            int moveY;

            int diffX = pointB.X - pointA.X;
            if (diffX == 0)
            {
                moveX = 0;
            }
            else
            {
                if (Math.Abs(diffX) < (int)moveDistanceX)
                {
                    moveX = diffX;
                }
                else
                {
                    moveX = (int)moveDistanceX;
                }                
            }

            int diffY = pointB.Y - pointA.Y;
            if (pointA.Y == pointB.Y)
            {
                moveY = 0;
            }
            else
            {
                if (Math.Abs(diffY) < (int)moveDistanceY)
                {
                    moveY = diffY;
                }
                else
                {
                    moveY = (int)moveDistanceY;
                }
            }
            // 新しい座標を計算
            return new Point(moveX, moveY);
        }

        /// <summary>
        /// controlのSize.W,HがOtherControl.Size.W,Hと比べて両方小さいか判定する
        /// 同じはFalse
        /// </summary>
        /// <param name="control"></param>
        /// <param name="otherControl"></param>
        /// <returns></returns>
        private bool ControlSizeSmallerThanOtherControl_XYBoth(Control control, Control otherControl)
        {
            if ((control.Width < otherControl.Width) &&
                (control.Height < otherControl.Height)
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// controlのSize.W,HがOtherControl.Size.W,Hと比べて両方小さいか判定する
        /// 同じはFalse
        /// </summary>
        /// <param name="control"></param>
        /// <param name="otherControl"></param>
        /// <returns></returns>
        private bool ObjectSizeSmallerThanOtherObject_XYBoth(object objectValue, object otherObjectValue)
        {
            Point pointA;
            Point pointOther;
            //#
            if ( objectValue.GetType() == typeof(Control))
            {
                Control con = (Control)objectValue;
                pointA = new Point(con.Width, con.Height);
            } else if( objectValue.GetType() == typeof(Size))
            {
                Size sz = (Size)objectValue;
                pointA = new Point(sz.Width, sz.Height);
            } else if (objectValue.GetType() == typeof(Point))
            {
                pointA = (Point)objectValue;
            } else
            {
                Control con = (Control)objectValue;
                pointA = new Point(con.Width, con.Height);
            }
            //#
            if (otherObjectValue.GetType() == typeof(Control))
            {
                Control con = (Control)otherObjectValue;
                pointOther = new Point(con.Width, con.Height);
            }
            else if (otherObjectValue.GetType() == typeof(Size))
            {
                Size sz = (Size)otherObjectValue;
                pointOther = new Point(sz.Width, sz.Height);
            }
            else if (otherObjectValue.GetType() == typeof(Point))
            {
                pointOther = (Point)otherObjectValue;
            }
            else
            {
                Control con = (Control)otherObjectValue;
                pointOther = new Point(con.Width, con.Height);
            }
            //#
            if ((pointA.X < pointOther.X) &&
                (pointA.Y < pointOther.Y)
                )
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// control の座標が otherControl にすべて含まれるか判定する
        /// （同じは含まれる）
        /// </summary>
        /// <param name="control"></param>
        /// <param name="otherControl"></param>
        /// <returns></returns>
        private bool ControlRectIncludeOtherControl(Control control, Control otherControl)
        {
            Point controlLocationMax = new Point(
                control.Location.X + control.Size.Width, control.Location.Y + control.Size.Height);
            Point otherControlLocationMax = new Point(
                otherControl.Location.X + otherControl.Size.Width, otherControl.Location.Y + otherControl.Size.Height);
            if ((otherControl.Location.X <= control.Location.X) 
                && (otherControl.Location.Y <= control.Location.Y)
                && (controlLocationMax.X <= otherControlLocationMax.X)
                && (controlLocationMax.Y <= otherControlLocationMax.Y)
                )
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// コントロールの拡大・縮小時の位置変更のための計算
        /// <param name="beforeSize">以前サイズ</param>
        /// <param name="afterSize">変化後のサイズ</param>
        /// </summary>
        public Point GetLocationByCalcExpansionWhenChangeSize(Size beforeSize, Size afterSize)
        {
            try
            {
                bool simpleShrink = false;
                bool isExpand = false;
                Point newLocation;
                int x = (int)(beforeSize.Width - afterSize.Width) / 2;
                int y = (int)(beforeSize.Height - afterSize.Height) / 2;


                // 現在のマウスの位置とInnerControlの中心との差分を計算
                Point diffPoint = GetMouseCenterDiff(_parentControl);
                Point mousePoint = GetMousePointInControl(_parentControl);
                Point innerCenter = GetControlCenterLocation(_innerControl);
                Point parentCenter = GetControlCenterLocation(_parentControl);

                if (beforeSize.Width < afterSize.Width)
                {
                    //// 拡大時
                    isExpand = true;
                    x += _innerControl.Location.X;
                    y += _innerControl.Location.Y;
                    if (! ControlRectIncludeOtherControl(_innerControl, _parentControl))
                    {
                        //縦と横を足して2で割った物をベースに
                        double ratio = 0.15;
                        int baseNum = (int)(((_parentControl.Width + _parentControl.Height) / 2) * ratio);
                        // マウスポインタが、ParentControl真ん中に近いか
                        if (!IsPointWithinDistance(parentCenter, mousePoint, baseNum))
                        {
                            //int moveX = -(int)(_parentControl.Size.Width * 0.05);
                            //int moveY = -(int)(_parentControl.Size.Height * 0.05);
                            int moveX = (int)(_parentControl.Size.Width * 0.05);
                            int moveY = (int)(_parentControl.Size.Height * 0.05);
                            //左右
                            if (0 < (mousePoint.X - parentCenter.X))
                            {
                                x -= moveX;
                            }
                            else
                            {
                                x += moveX;
                            }
                            //上下
                            if (0 < (mousePoint.Y - parentCenter.Y))
                            {
                                y -= moveY;
                            }
                            else
                            {
                                y += moveY;
                            }
                        }
                    }
                }
                else
                {
                    //// 縮小
                    isExpand = false;
                    x += _innerControl.Location.X;
                    y += _innerControl.Location.Y;
                    _logger.PrintInfo(String.Format("oldPt = {0}", new Point(x, y)));
                    //innerサイズがparentサイズより大きい時
                    if (!ControlSizeSmallerThanOtherControl_XYBoth(_innerControl, _parentControl))
                    {
                        bool isDistance = IsPointWithinDistance(parentCenter, innerCenter, 150);
                        Point newPt = new Point();
                        // マウスポインタが中心以外なら、Locationを少し調整する
                        if (!isDistance)
                        {
                            //newPt = GetModifiedPoint(diffPoint, innerCenter, 0.33);
                            //newPt = GetModifiedPoint(parentCenter, innerCenter, 0.033);
                            //newPt = MovePointTowards(parentCenter, innerCenter, 30);
                            int moveX = (int)(_parentControl.Size.Width * 0.05);
                            int moveY = (int)(_parentControl.Size.Height * 0.05);
                            newPt = MovePointTowardsB(parentCenter, innerCenter, moveX, moveY);
                            x += newPt.X;
                            y += newPt.Y;
                        }
                        else
                        {
                            if (!innerCenter.Equals(parentCenter))
                            {
                                //innerのセンターが、parentのセンターになるように、innerのlocationを設定する
                                newPt = parentCenter;
                                x = parentCenter.X - _innerControl.Size.Width / 2;
                                y = parentCenter.Y - _innerControl.Size.Height / 2;
                            }
                        }
                    }
                    else
                    {
                        //位置調整せずに縮小のみtrue
                        simpleShrink = true;
                        //縮小
                        //innerサイズがparentサイズより小さいとき
                        //縦と横を足して2で割った物をベースに
                        double ratio = 0.1;
                        int baseNum = (int)(((_parentControl.Width + _parentControl.Height) / 2) * ratio);
                        if (!IsPointWithinDistance(parentCenter, mousePoint, baseNum))
                        {
                            //int moveX = -(int)(_parentControl.Size.Width * 0.05);
                            //int moveY = -(int)(_parentControl.Size.Height * 0.05);
                            ////左右
                            //if (0 < (mousePoint.X - parentCenter.X))
                            //{
                            //    x -= moveX;
                            //}
                            //else
                            //{
                            //    x += moveX;
                            //}
                            ////上下
                            //if (0 < (mousePoint.Y - parentCenter.Y))
                            //{
                            //    y -= moveY;
                            //}
                            //else
                            //{
                            //    y += moveY;
                            //}
                        }
                    }

                }
                //拡大縮小両方

                //拡大時画像はしによりすぎて余白が大きくなりすぎないよう
                //画像端が真ん中に来ないようにする
                //画像が枠より多きときで、画像端が、枠より内側に来た時は枠と同じにする
                //横
                //if (_parentControl.Size.Width < afterSize.Width)
                // ただ縮小したときは実行しない
                // 拡大時、外枠より画像が小さいときは実行しない
                bool expandAndSmallImage;
                expandAndSmallImage = isExpand && ObjectSizeSmallerThanOtherObject_XYBoth(afterSize, (Control)_parentControl);
                if ((!simpleShrink)&&(!expandAndSmallImage))
                {
                    //Location小さいとき
                    if (_parentControl.Location.X < x)
                    {
                        x = _parentControl.Location.X;
                    }
                    //Location大きいとき
                    if ((x + afterSize.Width) < _parentControl.Width)
                    {
                        x = _parentControl.Width - afterSize.Width;
                    }
                }
                //縦
                if ((!simpleShrink) && (!expandAndSmallImage)) // (_parentControl.Size.Height < afterSize.Height)
                {
                    //Location小さいとき
                    if (_parentControl.Location.Y < y)
                    {
                        y = _parentControl.Location.Y;
                    }
                    //Location大きいとき
                    if ((y + afterSize.Height) < _parentControl.Height)
                    {
                        y = _parentControl.Height - afterSize.Height;
                    }
                }

                newLocation = new Point(x, y);
                _logger.PrintInfo(String.Format("newLocation = {0}", newLocation));
                //////////////////////////////////
                //// 拡大時
                //if (beforeSize.Width < afterSize.Width)
                //{
                //    //// 現在のマウスの位置とInnerControlの中心との差分を計算
                //    //Point bufPoint = GetMouseCenterDiff(_parentControl);
                //    // 現在の_InnerControlの中心と_parentoCOntrolの中心との差分を計算
                //    Point bufPoint = GetCenterDiffInnerToParent(_innerControl, _parentControl);
                //    _logger.PrintInfo(string.Format("bufPoint = {0}", bufPoint));
                //    //拡大時は、FrameControlの中心からの距離からマウスポインタまでの距離が、
                //    //大きいなら、多めに移動加算、小さいなら、少なめに移動加算、中心近くなら移動加算しない
                //    // 距離度合から実際の距離を計算
                //    Point movePoint = GetMovePointByDistanceLevel(3, bufPoint, 1.0);
                //    _logger.PrintInfo(string.Format("movePoint = {0}", movePoint));
                //    //// InnerControl.Location からその差分を引く
                //    x = afterSize.Width - movePoint.X;
                //    y = afterSize.Height - movePoint.Y;
                //    newPoint = new Point(x, y);
                //}
                //else
                //{
                //    //縮小時
                //    //縮小時は、寄っていないほうのマイナスを大きくする
                //    //
                //    // 現在の_InnerControlの中心と_parentoCOntrolの中心との差分を計算
                //    Point bufPoint = GetCenterDiffInnerToParent(_innerControl,_parentControl);
                //    _logger.PrintInfo(string.Format("bufPoint = {0}", bufPoint));
                //    if (bufPoint.X < 0)
                //    {

                //    }


                //    Point movePoint = GetMovePointByDistanceLevel(3, bufPoint, 1.0);
                //    _logger.PrintInfo(string.Format("movePoint = {0}", movePoint));
                //    if (movePoint.Equals(new Point()))
                //    {
                //        _logger.PrintInfo("movePoint Zero 0");
                //        x += afterSize.Width;
                //        y += afterSize.Height;
                //    }
                //    else
                //    {
                //        x = afterSize.Width - movePoint.X;
                //        y = afterSize.Height - movePoint.Y;
                //    }

                //    newPoint = new Point(x, y);
                //}

                ///////////////
                // Location.X,Yがマイナスならゼロに、
                // Location.X,Y + InnerControl.Width, Htight が、_parentControl.Width, Heightより越えたら
                // Location.X,Y = _parentControl.Width, Height - _InnerControl.Width, Height にする
                //値が大きすぎ
                if ( _parentControl.Width < newLocation.X)
                {
                    newLocation.X = (int)(_parentControl.Width * 0.95);
                    _logger.PrintInfo("X Large");
                }
                if (_parentControl.Height < newLocation.Y)
                {
                    newLocation.Y = (int)(_parentControl.Height * 0.95);
                    _logger.PrintInfo("Y Large");
                }
                //値が小さすぎ
                if (newLocation.X + afterSize.Width < _parentControl.Location.X)
                {
                    newLocation.X = (int)(_parentControl.Width * 0.05);
                    _logger.PrintInfo("X Small");
                }
                if (newLocation.Y + afterSize.Height < _parentControl.Location.Y)
                {
                    newLocation.Y = (int)(_parentControl.Height * 0.05);
                    _logger.PrintInfo("Y Small");
                }
                return newLocation;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ChangeLocationWhenChangeSize Failed");
                return new Point();
            }
        }

        private Point GetControlCenterLocation(Control control)
        {
            return new Point(control.Location.X + (int)(control.Width / 2), control.Location.Y + (int)(control.Height/2));
        }

        //private Point PointYoseToCenter(Point diffPoint, Control control, Control parentControl, double ratio=0.75)
        //{
        //    int moveX;
        //    int moveY;
        //    moveY = (int)(parentControl.Height * ratio);
        //    int newCenterX;
        //    int newCenterY;
        //    if (diffPoint.X < 0)
        //    {
        //        moveX = (int)(parentControl.Width * ratio);
        //        newCenterX = (int)(control.Size.Width / 2) + moveX;
        //        if (0 < (newCenterX - (int)(parentControl.Width / 2)))
        //        {
        //            newCenterX = (parentControl.Width / 2);
        //        }
        //    }
        //    return new Point();
        //}

        private Point GetMovePointByDistanceLevel(int divNum, Point bufPoint, double moveRatio = 1.0)
        {
            //中心からの距離なのでさらに÷２する
            Point distanceLevelBase = new Point(_parentControl.Width / 2 / divNum, _parentControl.Height / 2 / divNum);
            // 
            //Point distanceLevelBase = new Point(_parentControl.Width /  divNum, _parentControl.Height /  divNum);
            // 距離の度合い
            Point distanceLevel = new Point(0, 0);
            for (int i = divNum; i > 0; i--)
            {
                if (distanceLevel.X <= 0)
                {
                    if (Math.Abs(bufPoint.X) > (distanceLevelBase.X * i))
                    {
                        distanceLevel.X = i-1;
                    }
                }
                if (distanceLevel.Y <= 0)
                {
                    if (Math.Abs(bufPoint.Y) > (distanceLevelBase.Y * i))
                    {
                        distanceLevel.Y = i-1;
                    }
                }
            }
            _logger.PrintInfo(string.Format("distanceLevel = {0}", distanceLevel));
            int x = (int)(distanceLevelBase.X * moveRatio * distanceLevel.X);
            int y = (int)(distanceLevelBase.Y * moveRatio * distanceLevel.Y);
            return new Point(x, y);
        }

        /// <summary>
        /// controlの中心座標とparentControlの中心座標との差を取得する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="parentControl"></param>
        /// <returns></returns>
        private Point GetCenterDiffInnerToParent(Control control, Control parentControl)
        {
            int x = control.Location.X + (int)(control.Size.Width / 2);
            int y = control.Location.Y + (int)(control.Size.Height / 2);
            Point diffPoint = new Point(
                x - (int)(parentControl.Size.Width / 2),
                y - (int)(parentControl.Size.Height / 2));
            return diffPoint;
        }

        /// <summary>
        /// 現在のマウスの位置とControlの中心との差分を計算する
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private Point GetMouseCenterDiff(Control control)
        {
            Point nowMousePoint = GetMousePointInControl(control);
            nowMousePoint.X = nowMousePoint.X - (control.Width / 2);
            nowMousePoint.Y = nowMousePoint.Y - (control.Height / 2);
            _logger.PrintInfo(string.Format("nowMousePoint = {0}", nowMousePoint));
            return nowMousePoint;
        }

        /// <summary>
        /// フォーム上の座標でマウスポインタの位置を取得する
        /// </summary>
        /// <param name="contorl"></param>
        /// <returns></returns>
        private Point GetMousePointInControl(Control contorl)
        {
            //フォーム上の座標でマウスポインタの位置を取得する
            //画面座標でマウスポインタの位置を取得する
            System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
            //画面座標をクライアント座標に変換する
            System.Drawing.Point cp = contorl.PointToClient(sp);
            ////X座標を取得する
            //int x = cp.X;
            ////Y座標を取得する
            //int y = cp.Y;
            return cp;
        }

    }
}
