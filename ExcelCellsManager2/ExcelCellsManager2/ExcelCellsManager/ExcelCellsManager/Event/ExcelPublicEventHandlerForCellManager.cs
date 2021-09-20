using Microsoft.Office.Interop.Excel;
using System;

namespace ExcelCellsManager.ExcelCellsManager.Event
{
    public class ExcelPublicEventHandlerBredgeForCellManager : ExcelIO.IExcelAppsEventBridgeInterface
    {
        protected ErrorManager.ErrorManager _error;
        private event WorkbookEvents_WindowActivateEventHandler MWorkbook_WindowActivateEvent;
        private event AppEvents_WindowActivateEventHandler MApplication_WindowActivateEvent;
        private event WorkbookEvents_DeactivateEventHandler MWorkbook_DeactivateEvent;
        private event AppEvents_WindowDeactivateEventHandler MApplication_DeactivateEvent;
        private event WorkbookEvents_SheetActivateEventHandler MWorkSheet_ActivateEvent;
        private event AppEvents_SheetSelectionChangeEventHandler MApplication_SheetSelectionChangeEvent;
        private event AppEvents_SheetActivateEventHandler MApplication_SheetActivteEventt;
        private event AppEvents_WorkbookOpenEventHandler MApplication_WorkbookOpenEvent;
        private event AppEvents_WorkbookBeforeCloseEventHandler MApplication_WorkbookBeforeCloseEvent;

        private event EventHandler MApplication_SheetSelectionChangeAfterEvent;
        private event EventHandler MApplication_WindowActivateAfterEvent;
        private event EventHandler MWorkSheet_ActivateAfterEvent;
        private event EventHandler MApplication_WorkbookDeactivateEvent;
        private event EventHandler MApplication_SheetActivateEventAfterHandler;
        private event EventHandler MApplication_WorkbookOpenAfterEvent;
        private event EventHandler MApplication_WorkbookBeforeCloseAddEvent;
        protected bool IsWorkbookBeforeClose = false;


        public int Number { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public WorkbookEvents_WindowActivateEventHandler Workbook_WindowActivateEvent 
        { get => MWorkbook_WindowActivateEvent; set => MWorkbook_WindowActivateEvent = value; }
        public AppEvents_WindowActivateEventHandler Application_WindowActivateEvent
        { get => MApplication_WindowActivateEvent; set => MApplication_WindowActivateEvent = value; }
        public WorkbookEvents_DeactivateEventHandler Workbook_DeactivateEvent
        { get => MWorkbook_DeactivateEvent; set => MWorkbook_DeactivateEvent = value; }
        public AppEvents_WindowDeactivateEventHandler Application_DeactivateEvent
        { get => MApplication_DeactivateEvent; set => MApplication_DeactivateEvent = value; }
        public WorkbookEvents_SheetActivateEventHandler WorkSheet_ActivateEvent
        { get => MWorkSheet_ActivateEvent; set => MWorkSheet_ActivateEvent = value; }
        public AppEvents_SheetSelectionChangeEventHandler Application_SheetSelectionChangeEvent
        { get => MApplication_SheetSelectionChangeEvent; set => MApplication_SheetSelectionChangeEvent = value; }
        public AppEvents_SheetActivateEventHandler Application_SheetActivateEvent {
            get => MApplication_SheetActivteEventt; set => MApplication_SheetActivteEventt = value; }
        // EventHandler
        public EventHandler Application_SheetSelectionChangeAfterEvent {
            get => MApplication_SheetSelectionChangeAfterEvent; set => MApplication_SheetSelectionChangeAfterEvent = value; }
        public EventHandler Application_WindowActivateAfterEvent {
            get => MApplication_WindowActivateAfterEvent; set => MApplication_WindowActivateAfterEvent = value;
        }
        public EventHandler Application_DeactivateAddEvent
        {get => MApplication_WorkbookDeactivateEvent; set => MApplication_WorkbookDeactivateEvent = value;}
        public EventHandler WorkSheet_ActivateAfterEvent {
            get => MWorkSheet_ActivateAfterEvent; set => MWorkSheet_ActivateAfterEvent = value; }
        public EventHandler Application_SheetActivateAfterEvent {
            get => MApplication_SheetActivateEventAfterHandler; set => MApplication_SheetActivateEventAfterHandler = value; }
        public AppEvents_WorkbookOpenEventHandler Application_WorkbookOpenEvent { 
            get => MApplication_WorkbookOpenEvent; set => MApplication_WorkbookOpenEvent = value; }
        public EventHandler Application_WorkbookOpenAfterEvent { 
            get => MApplication_WorkbookOpenAfterEvent; set => MApplication_WorkbookOpenAfterEvent = value; }
        public AppEvents_WorkbookBeforeCloseEventHandler Application_WorkbookBeforeCloseEvent { 
            get => MApplication_WorkbookBeforeCloseEvent; set => MApplication_WorkbookBeforeCloseEvent = value; }
        public EventHandler Application_WorkbokBeforeCloseAddEvent { 
            get => MApplication_WorkbookBeforeCloseAddEvent; set => MApplication_WorkbookBeforeCloseAddEvent = value; }

        public ExcelPublicEventHandlerBredgeForCellManager(ErrorManager.ErrorManager error)
        {
            _error = error;
            Workbook_WindowActivateEvent = new WorkbookEvents_WindowActivateEventHandler(Workbook_WindowActivate);
            Application_WindowActivateEvent = new AppEvents_WindowActivateEventHandler(Appliction_WindowActivate);
            Workbook_DeactivateEvent = new WorkbookEvents_DeactivateEventHandler(Workbook_WindowDeactivate);
            Application_DeactivateEvent = new AppEvents_WindowDeactivateEventHandler(Application_WindowDeactivate);
            Application_SheetSelectionChangeEvent = new AppEvents_SheetSelectionChangeEventHandler(Application_SheetSelectionChange);
            WorkSheet_ActivateEvent = new WorkbookEvents_SheetActivateEventHandler(Worksheet_Activate);
            Application_SheetActivateEvent = new AppEvents_SheetActivateEventHandler(Application_SheetActivate);
            MApplication_WorkbookOpenEvent = new AppEvents_WorkbookOpenEventHandler(Application_WorkbookOpen);
            MApplication_WorkbookBeforeCloseEvent = new AppEvents_WorkbookBeforeCloseEventHandler(Application_WorkbookBeforeClose);
        }

        private void Application_WorkbookBeforeClose(Workbook Wb,ref bool Cancel)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Application_WorkbookBeforeClose : CellManager : ");
                IsWorkbookBeforeClose = true;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_WorkbookBeforeClose");
                _error.ClearError();
            } finally
            {
                MApplication_WorkbookBeforeCloseAddEvent?.Invoke(Wb.Name, EventArgs.Empty);
            }
        }

        private void Application_WorkbookOpen(Workbook Wb)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Application_WorkbookOpen : CellManager : ");
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_WorkbookOpen");
            } finally
            {
                MApplication_WorkbookOpenAfterEvent?.Invoke(Wb.Name, EventArgs.Empty);
            }
        }

        private void Application_SheetActivate(Object Sh)
        {
            try
            {
                _error.AddLog(this.ToString()+".Application_SheetActivate : CellManager : ");

                string sheetname = ((Worksheet)Sh).Name;
                string bookname = ((Workbook)((Worksheet)Sh).Parent).Name;
                string activecellAddress = ((Application)(((Workbook)((Worksheet)Sh).Parent).Parent)).ActiveCell.Address;
                activecellAddress = activecellAddress.Replace("$", "");
                _error.AddLog(" " + bookname + " > " + sheetname + " > " + activecellAddress);
                string buf = activecellAddress + " < " + sheetname + " < " + bookname;
                string[] ary = { buf, bookname, sheetname, activecellAddress };
                MApplication_SheetActivateEventAfterHandler?.Invoke(ary, EventArgs.Empty);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_SheetSelectionChange");
            }
        }

        private void Worksheet_Activate(Object Sh)
        {
            try
            {
                _error.AddLog(this.ToString()+".Worksheet_Activate : CellManager : ");

                string sheetname = ((Worksheet)Sh).Name;
                string bookname = ((Workbook)((Worksheet)Sh).Parent).Name;
                string activecellAddress = ((Application)(((Workbook)((Worksheet)Sh).Parent).Parent)).ActiveCell.Address;
                activecellAddress = activecellAddress.Replace("$", "");
                _error.AddLog(" " + bookname + " > " + sheetname + " > " + activecellAddress);
                string buf = activecellAddress + " < " + sheetname + " < " + bookname;
                string[] ary = { buf, bookname, sheetname, activecellAddress };
                MWorkSheet_ActivateAfterEvent?.Invoke(ary, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_SheetSelectionChange");
            }
        }

        private void Application_SheetSelectionChange(Object Sh, Range Target)
        {
            try
            {
                _error.AddLog(this.ToString()+".Application_SheetSelectionChange : CellManager : ");
                string sheetname = ((Worksheet)Sh).Name;
                string bookname = ((Workbook)((Worksheet)Sh).Parent).Name;
                string activecellAddress = Target.Address;
                activecellAddress = activecellAddress.Replace("$", "");
                _error.AddLog(" " + bookname  + " > " + sheetname + " > " + activecellAddress);
                string buf = activecellAddress + " < " + sheetname + " < " + bookname;
                string[] ary = { buf, bookname, sheetname, activecellAddress };
                MApplication_SheetSelectionChangeAfterEvent?.Invoke(ary, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_SheetSelectionChange");
            }
        }
        private void Application_WindowDeactivate(Workbook Wb, Window Wn)
        {
            string[] ary = new string[] { "book > sheet > address","book","sheet","address"};
            try
            {

                string sheetname = ((Worksheet)Wn.Application.ActiveSheet).Name;
                string bookname = Wn.Application.ActiveWorkbook.Name;
                string activecellAddress = Wn.Application.ActiveCell.Address;
                activecellAddress = activecellAddress.Replace("$", "");
                _error.AddLog(" " + bookname + " > " + sheetname + " > " + activecellAddress);
                string buf = activecellAddress + " < " + sheetname + " < " + bookname;
                ary = new string[]{ buf, bookname, sheetname, activecellAddress };

                _error.AddLog(this.ToString()+".Application_WindowDeactivate : CellManager : "+buf);

            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Application_WindowDeactivate");
            }
            finally
            {
                this.Application_DeactivateAddEvent?.Invoke(ary, EventArgs.Empty);
            }
        }

        private void Workbook_WindowActivate(Window Wn)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Workbook_WindowActivate : CellManager : ");

                //string sheetname = ((Worksheet)Wn.ActiveSheet).Name;
                //string bookname = Wn.Application.ActiveWorkbook.Name;
                //string activecellAddress = Wn.ActiveCell.Address;
                //string buf = activecellAddress + " < " + sheetname + " < " + bookname;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Workbook_WindowActivate");
            }
        }

        private void Appliction_WindowActivate(Workbook Wb, Window Wn)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Appliction_WindowActivate : CellManager : ");
                if (!IsWorkbookBeforeClose)
                {
                    string sheetname = ((Worksheet)Wn.ActiveSheet).Name;
                    string bookname = Wn.Application.ActiveWorkbook.Name;
                    string activecellAddress = Wn.ActiveCell.Address;
                    activecellAddress = activecellAddress.Replace("$", "");
                    _error.AddLog(" " + bookname + " > " + sheetname + " > " + activecellAddress);
                    string buf = activecellAddress + " < " + sheetname + " < " + bookname;
                    string[] ary = { buf, bookname, sheetname, activecellAddress };

                    MApplication_WindowActivateAfterEvent?.Invoke(ary, EventArgs.Empty);
                }
                else
                {
                    _error.AddLog("  IsWorkbookBeforeClose=true");
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Appliction_WindowActivate");
            }
        }

        private void Workbook_WindowDeactivate()
        {
            try
            {
                _error.AddLog(this.ToString()+".Workbook_WindowDeactivate : CellManager");
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Workbook_WindowDeactivate");
            }
        }



    }
}
