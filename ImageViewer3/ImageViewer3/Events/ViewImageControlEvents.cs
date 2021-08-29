using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Events
{
    public class ViewImageControlEvents
    {
        readonly protected ErrorLog.IErrorLog _errorLog;
        readonly ViewImageObjects _viewImageObjects;

        //readonly ViewImageMouseEventHandler _mouseEventHandler;
        readonly protected ClickNextPreviewEvents _clickNextPreviewEventsForViewControl;
        readonly protected DragDropEvents _dragDropEventsForViewControl;
        protected MoveInnerControlEvents _moveControlEvents;
        public ViewImageControlEvents(ErrorLog.IErrorLog errorLog,ViewImageObjects viewImageObjects)
        {
            _errorLog = errorLog;
            _viewImageObjects = viewImageObjects;

            _clickNextPreviewEventsForViewControl = new ClickNextPreviewEvents(
                errorLog, _viewImageObjects.Controls.ViewImageControl.GetControl());

            _clickNextPreviewEventsForViewControl.MouseEventHandler.OnMouseWheel += 
                _viewImageObjects.Functions.ForControl.MouseWheelEvent;
            _clickNextPreviewEventsForViewControl.MouseEventHandler.OnRightClick +=
                _viewImageObjects.Functions.ForViewBasic.ViewNext;
            _clickNextPreviewEventsForViewControl.MouseEventHandler.OnLeftClick +=
                _viewImageObjects.Functions.ForViewBasic.ViewPrevious;

            _dragDropEventsForViewControl = new DragDropEvents(
                errorLog, _viewImageObjects.Controls.ViewImageControl.GetControl());

            _dragDropEventsForViewControl.MouseEventHandler.OnDragDrop +=
                _viewImageObjects.Functions.ForFileList.RegistFileListByDragDrop;

            _moveControlEvents = new MoveInnerControlEvents(
                errorLog, 
                _viewImageObjects.Controls.ViewInnerControl.GetControl(),
                _viewImageObjects.Controls.ViewImageControl.GetControl() );
        }
    }

    public class ViewImageControlEventsClasses
    {

    }
}
