

namespace ExcelCellsManager.ExcelCellsManager.Event
{
    public class StatusStripEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected string _formName = "";
        //public MouseEventHandler MouseEventHandler;
        public StatusStripEvent(ErrorManager.ErrorManager error, string formName)
        {
            _error = error;
            _formName = formName;
            //this.MouseEventHandler = new MouseEventHandler(MouseMove);
        }


    }
}
