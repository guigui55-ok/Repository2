﻿using System.Drawing;

namespace ImageViewer.Values
{
    public class ViewControlState : IViewControlState
    {
        private bool _isInitialize = true;
        private bool _isFrameChanging = false;
        private bool _nowSizeUpdate;
        private bool _isPausePaint;
        private bool _isMoseHover;

        private PointF _differenceSizeFromContents;
        private PointF _differencePositionFromContents;
        private Point _differenceSizeInnerFromFrame;
        private Point _differencePositionInnerInFrame;
        //private PointF _ratioLocationInnerFromFrame;

        private PointF _ratioSizeInnerFromFrame;
        private double _ratioLocationInnerFromFrameX;
        private double _ratioLocationInnerFromFrameY;
        protected PointF _ratioSizeFrameFromContents;
        protected double _ratioLocationFrameFromContentsX;
        protected double _ratioLocationFrameFromContentsY;

        private bool _isActiveControl;
        public bool IsActiveControl { get { return _isActiveControl; } set { _isActiveControl = value; } }
        public bool IsMouseHover { get { return _isMoseHover; } set { _isMoseHover = value; } }
        public ViewControlState()
        {
            _isActiveControl = true;
        }
        public PointF DifferenceSizeFromContents
        {
            get { return _differenceSizeFromContents; }
            set
            {
                _differenceSizeFromContents.X = value.X;
                _differenceSizeFromContents.Y = value.Y;
            }
        }
        public PointF DifferencePositionFromContents
        {
            get { return _differencePositionFromContents; }
            set
            {
                _differencePositionFromContents.X = value.X;
                _differencePositionFromContents.Y = value.Y;
            }
        }
        
        bool IViewControlState.NowSizeUpdate { get { return _nowSizeUpdate; } set { _nowSizeUpdate = value; } }
        bool IViewControlState.IsPausePaint { get { return _isPausePaint; } set { _isPausePaint = value; } }
        public Point DifferenceSizeInnerFromFrame { get { return _differenceSizeInnerFromFrame; } set { _differenceSizeInnerFromFrame = value; } }
        public Point DifferencePositionInnerInFrame { get { return _differencePositionInnerInFrame; } set { _differencePositionInnerInFrame = value; } }
        public PointF RatioSizeInnerFromFrame { get { return _ratioSizeInnerFromFrame; } set { _ratioSizeInnerFromFrame = value; } }
        //public PointF RatioLocationInnerFromFrame { get {return _ratioLocationInnerFromFrame;} set { _ratioLocationInnerFromFrame = value; }}
        public double RatioLocationInnerFromFrameX { get { return _ratioLocationInnerFromFrameX; } set { _ratioLocationInnerFromFrameX = value; } }
        public double RatioLocationInnerFromFrameY { get { return _ratioLocationInnerFromFrameY; } set { _ratioLocationInnerFromFrameY = value; } }
        public bool IsFrameSizeChanging { get { return _isFrameChanging; } set { _isFrameChanging = value; } }
        public bool IsInitializeControl { get { return _isInitialize; } set { _isInitialize = value; } }
        public PointF RaitoSizeFrameFromContents { get { return _ratioSizeFrameFromContents; } set { _ratioSizeFrameFromContents = value; } }
        public double RaitoLocationFrameFromContentsX { get { return _ratioLocationFrameFromContentsX; } set { _ratioLocationFrameFromContentsX = value; } }
        public double RaitoLocationFrameFromContentsY { get { return _ratioLocationFrameFromContentsY; } set { _ratioLocationFrameFromContentsY = value; } }
    }
}
