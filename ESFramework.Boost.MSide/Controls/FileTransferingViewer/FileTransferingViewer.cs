using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ESBasic;
using ESPlus.FileTransceiver;
using ESPlus.Application.FileTransfering.Passive;
using ESBasic.Threading.Engines;
using CCWin.Win32.Const;

namespace ESFramework.Boost.Controls
{
    /// <summary>
    /// 文件传送查看器。用于查看与某个好友的所有文件传送的进度状态信息。
    /// 注意，FileTransferingViewer的所有事件都是在UI线程中触发的。
    /// </summary>
    public partial class FileTransferingViewer : UserControl, IFileTransferingViewer ,IEngineActor
    {
        private ESBasic.Func<TransferingProject, bool> projectFilter;
        private IFileOutter fileOutter;   
        private AgileCycleEngine agileCycleEngine;

        #region event FileTransferingViewer的所有事件都是在UI线程中触发的。

        /// <summary>
        /// 当某个文件开始续传时，触发该事件。参数为FileName - isSending
        /// </summary>
        public event CbGeneric<string, bool> FileResumedTransStarted; 

        /// <summary>
        /// 当某个文件传送完毕时，触发该事件。参数为FileName - isSending - comment - isFolder
        /// </summary>
        public event CbGeneric<string, bool, string ,bool> FileTransCompleted;

        /// <summary>
        /// 当某个文件传送完毕时，触发该事件。
        /// </summary>
        public event CbGeneric<TransferingProject> FileTransCompleted2;

        /// <summary>
        /// 当某个文件传送中断时，触发该事件。参数为FileName - isSending - FileTransDisrupttedType - cause
        /// </summary>
        public event CbGeneric<string, bool, FileTransDisrupttedType ,string> FileTransDisruptted;

        /// <summary>
        /// 当某个文件传送开始时，触发该事件。参数为FileName - isSending
        /// </summary>
        public event CbGeneric<string, bool> FileTransStarted;

        /// <summary>
        /// 当所有文件都传送完成时，触发该事件。
        /// </summary>
        public event CbSimple AllTaskFinished;

        /// <summary>
        /// 当某个文件需要离线发送时，触发该事件。参数为TransferingProject
        /// </summary>
        public event CbGeneric<TransferingProject> FileNeedOfflineSend; 
        #endregion       

        #region Ctor
        public FileTransferingViewer()
        {
            InitializeComponent();

            this.agileCycleEngine = new AgileCycleEngine(this);
            this.agileCycleEngine.DetectSpanInSecs = 1;
            this.agileCycleEngine.Start();

            this.FileResumedTransStarted += delegate { };
            this.FileTransCompleted += delegate { };
            this.FileTransDisruptted += delegate { };
            this.FileTransStarted += delegate { };
            this.AllTaskFinished += delegate { };
            this.FileTransCompleted2 += delegate { };
        } 
        #endregion

        #region Initialize
        public void Initialize(IFileOutter _fileOutter)
        {
            this.Initialize(_fileOutter, (ESBasic.Func<TransferingProject, bool>)null);
        }

        public void Initialize(IFileOutter _fileOutter, string friendUserID)
        {
            if (friendUserID == null)
            {
                friendUserID = ESFramework.NetServer.SystemUserID;
            }
            this.Initialize(_fileOutter, delegate(TransferingProject pro) { return pro.DestUserID == friendUserID; });
        }


        /// <summary>
        /// 初始化控件。
        /// </summary>         
        /// <param name="filter">当前的Viewer要显示哪些传送项目的状态信息</param>        
        public void Initialize(IFileOutter _fileOutter, ESBasic.Func<TransferingProject, bool> filter)
        {            
            this.fileOutter = _fileOutter;
            this.projectFilter = filter;            

            //this.fileOutter.FileRequestReceived += new ESPlus.Application.FileTransfering.CbFileRequestReceived(fileOutter_FileRequestReceived);
            this.fileOutter.FileSendingEvents.FileTransStarted += new CbGeneric<ITransferingProject>(fileTransStarted);
            this.fileOutter.FileSendingEvents.FileTransCompleted += new CbGeneric<ITransferingProject>(fileTransCompleted);
            this.fileOutter.FileSendingEvents.FileTransDisruptted += new CbGeneric<ITransferingProject, FileTransDisrupttedType ,string>(fileTransDisruptted);
            this.fileOutter.FileSendingEvents.FileTransProgress += new CbFileSendedProgress(fileTransProgress);
            this.fileOutter.FileSendingEvents.FileResumedTransStarted += new CbGeneric<ITransferingProject>(fileSenderManager_FileResumedTransStarted);

            this.fileOutter.FileReceivingEvents.FileTransStarted += new CbGeneric<ITransferingProject>(fileTransStarted);
            this.fileOutter.FileReceivingEvents.FileResumedTransStarted += new CbGeneric<ITransferingProject>(fileReceiverManager_FileResumedTransStarted);
            this.fileOutter.FileReceivingEvents.FileTransCompleted += new CbGeneric<ITransferingProject>(fileTransCompleted);
            this.fileOutter.FileReceivingEvents.FileTransDisruptted += new CbGeneric<ITransferingProject, FileTransDisrupttedType, string>(fileTransDisruptted);
            this.fileOutter.FileReceivingEvents.FileTransProgress += new CbFileSendedProgress(fileTransProgress);            
        }

        public List<string> GetTransferingProjectIDsInCurrentViewer()
        {
            List<string> list = new List<string>();
            foreach (FileTransferItem item in this.flowLayoutPanel1.Controls)
            {
               list.Add(item.TransmittingProject.ProjectID) ;                
            }

            return list; 
        }

        /// <summary>
        /// 当收到发送文件的请求时,接收方调用此方法显示fileTransferItem
        /// </summary>        
        public void NewFileTransferItem(string projectID, bool offlineFile, bool doneAgreed)
        {
            TransferingProject pro = this.fileOutter.GetTransferingProject(projectID);
            if (pro == null)
            {
                return;
            }

            if (this.projectFilter != null)
            {
                if (!this.projectFilter(pro))
                {
                    return;
                }
            }

            this.AddFileTransItem(pro, offlineFile, doneAgreed);
        }

        /// <summary>
        ///  一定要在控件销毁的时候，取消预订的事件。否则，已被释放的控件的处理函数仍然会被调用，而引发"对象已经被释放"的异常。
        /// </summary>        
        public void BeforeDispose()
        {
            this.agileCycleEngine.StopAsyn();

            this.fileOutter.FileSendingEvents.FileTransStarted -= new CbGeneric<ITransferingProject>(fileTransStarted);
            this.fileOutter.FileSendingEvents.FileTransCompleted -= new CbGeneric<ITransferingProject>(fileTransCompleted);
            this.fileOutter.FileSendingEvents.FileTransDisruptted -= new CbGeneric<ITransferingProject, FileTransDisrupttedType ,string>(fileTransDisruptted);
            this.fileOutter.FileSendingEvents.FileTransProgress -= new CbFileSendedProgress(fileTransProgress);
            this.fileOutter.FileSendingEvents.FileResumedTransStarted -= new CbGeneric<ITransferingProject>(fileSenderManager_FileResumedTransStarted);
            
            this.fileOutter.FileReceivingEvents.FileTransStarted -= new CbGeneric<ITransferingProject>(fileTransStarted);
            this.fileOutter.FileReceivingEvents.FileResumedTransStarted -= new CbGeneric<ITransferingProject>(fileReceiverManager_FileResumedTransStarted);
            this.fileOutter.FileReceivingEvents.FileTransCompleted -= new CbGeneric<ITransferingProject>(fileTransCompleted);
            this.fileOutter.FileReceivingEvents.FileTransDisruptted -= new CbGeneric<ITransferingProject, FileTransDisrupttedType, string>(fileTransDisruptted);
            this.fileOutter.FileReceivingEvents.FileTransProgress -= new CbFileSendedProgress(fileTransProgress);
        }   

        void fileReceiverManager_FileResumedTransStarted(ITransferingProject pro)
        {
            TransferingProject info = pro as TransferingProject;
            if (this.projectFilter != null)
            {
                if (!this.projectFilter(info))
                {
                    return;
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new CbGeneric<TransferingProject>(this.fileReceiverManager_FileResumedTransStarted), info);
            }
            else
            {             
                FileTransferItem item = this.GetExistedItem(info.ProjectID);
                if (item != null)
                {
                    this.FileResumedTransStarted(info.ProjectName, false);
                }
            }
        }

        void fileSenderManager_FileResumedTransStarted(ITransferingProject pro)
        {
            TransferingProject info = pro as TransferingProject;
            if (this.projectFilter != null)
            {
                if (!this.projectFilter(info))
                {
                    return;
                }
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new CbGeneric<TransferingProject>(this.fileSenderManager_FileResumedTransStarted), info);
            }
            else
            {
                this.AddFileTransItem(info ,false, true);
                FileTransferItem item = this.GetExistedItem(info.ProjectID);
                if (item != null)
                {
                    this.FileResumedTransStarted(info.ProjectName ,true);
                }
            }
        }

        /// <summary>
        /// 当前是否有文件正在传送中。
        /// </summary>   
        public bool IsFileTransfering
        {
            get
            {
                return (this.flowLayoutPanel1.Controls.Count > 0);
            }
        }
        #endregion    
               
        #region fileTransStarted
        void fileTransStarted(ITransferingProject pro)
        {
            //if (!this.IsHandleCreated)
            //{
            //    return;
            //}
            TransferingProject info = (TransferingProject)pro;
            if (this.projectFilter != null)
            {
                if (!this.projectFilter(info))
                {
                    return;
                }
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbGeneric<TransferingProject>(this.fileTransStarted), info);
            }
            else
            {
                this.AddFileTransItem(info ,false ,true);
                FileTransferItem item = this.GetExistedItem(info.ProjectID);
                if (item != null)
                {
                    item.IsTransfering = true;
                }
            }
        } 
        #endregion

        #region fileTransProgress
        void fileTransProgress(string projectID, ulong total, ulong sended)
        {
            //if (!this.IsHandleCreated)
            //{
            //    return;
            //}

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new CbGeneric<string, ulong, ulong>(this.fileTransProgress), projectID, total, sended);
            }
            else
            {
                FileTransferItem item = this.GetExistedItem(projectID);
                if (item != null)
                {
                    item.SetProgress(total, sended);
                }
            }
        } 
        #endregion

        #region fileTransDisruptted
        void fileTransDisruptted(ITransferingProject pro, FileTransDisrupttedType disrupttedType ,string cause)
        {
            //if (!this.IsHandleCreated)
            //{
            //    return;
            //}
            TransferingProject info = pro as TransferingProject;
            if (this.InvokeRequired)
            {
                this.Invoke(new CbGeneric<TransferingProject, FileTransDisrupttedType, string>(this.fileTransDisruptted), info, disrupttedType, cause);
            }
            else
            {               
                FileTransferItem item = this.GetExistedItem(info.ProjectID);
                if (item != null)
                {
                    this.flowLayoutPanel1.Controls.Remove(item);
                    this.FileTransDisruptted(info.ProjectName, info.IsSender, disrupttedType ,cause);
                }
            }
        } 
        #endregion

        #region fileTransCompleted
        void fileTransCompleted(ITransferingProject pro)
        {
            //if (!this.IsHandleCreated)
            //{
            //    return;
            //}
            TransferingProject info = pro as TransferingProject;
            if (this.InvokeRequired)
            {
                this.Invoke(new CbGeneric<TransferingProject>(this.fileTransCompleted), info);
            }
            else
            {                
                FileTransferItem item = this.GetExistedItem(info.ProjectID);
                if (item != null)
                {
                    this.FileTransCompleted2(info);
                    this.flowLayoutPanel1.Controls.Remove(item);
                    this.FileTransCompleted(info.ProjectName, info.IsSender, info.Comment ,info.IsFolder);
                }
            }
        } 
        #endregion          

       
        #region AddFileTransItem ,GetFileTransItem        
        private void AddFileTransItem(TransferingProject project, bool offlineFile, bool doneAgreed)
        {
            if (this.projectFilter != null)
            {
                if (!this.projectFilter(project))
                {
                    return;
                }
            }

            FileTransferItem fileTransItem = this.GetExistedItem(project.ProjectID);
            if (fileTransItem != null)
            {
                return;
            }
            fileTransItem = new FileTransferItem();
            fileTransItem.FileCanceled += new CbFileCanceled(fileTransItem_FileCanceled);
            fileTransItem.FileReceived += new CbFileReceived(fileTransItem_FileReceived);
            fileTransItem.FileRejected += new CbFileRejected(fileTransItem_FileRejected);
            fileTransItem.ConvertToOfflineFile += new CbGeneric<TransferingProject, FileTransferItem>(fileTransItem_ConvertToOfflineFile);
            fileTransItem.Initialize(project, offlineFile ,doneAgreed);
            this.flowLayoutPanel1.Controls.Add(fileTransItem);
            this.FileTransStarted(project.ProjectName, project.IsSender);
        }

        void fileTransItem_ConvertToOfflineFile(TransferingProject project, FileTransferItem item)
        {           
            if (this.FileNeedOfflineSend != null)
            {
                this.FileNeedOfflineSend(project);
            }

        }

        private FileTransferItem GetExistedItem(string projectID)
        {
            foreach (FileTransferItem item in this.flowLayoutPanel1.Controls)
            {
                if (item.TransmittingProject.ProjectID == projectID)
                {
                    return item;
                }
            }      

            return null; 
        }        

        void fileTransItem_FileRejected(string projectID)
        {
            this.fileOutter.RejectFile(projectID);            
        }

        void fileTransItem_FileReceived(FileTransferItem item, string projectID, bool isSend, string savePath)
        {
            this.fileOutter.BeginReceiveFile(projectID, savePath);
        }        

        void fileTransItem_FileCanceled(FileTransferItem item, string projectID, bool isSend)
        {
            this.fileOutter.CancelTransfering(projectID);             
        }
        #endregion      

        #region flowLayoutPanel1_ControlRemoved
        private void flowLayoutPanel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (this.flowLayoutPanel1.Controls.Count == 0)
            {
                this.AllTaskFinished();
            }
        } 
        #endregion

        #region EngineAction
        public bool EngineAction()
        {
            try
            {
                this.CheckZeroSpeed();
            }
            catch { }
            return true;
        }

        private void CheckZeroSpeed()
        {
            //if (!this.IsHandleCreated)
            //{
            //    return;
            //}

            if (this.InvokeRequired)
            {
                this.Invoke(new CbGeneric(this.CheckZeroSpeed));
            }
            else
            {
                foreach (object obj in this.flowLayoutPanel1.Controls)
                {
                    FileTransferItem item = obj as FileTransferItem;
                    if (item != null && item.IsTransfering)
                    {
                        item.CheckZeroSpeed();
                    }
                }
            }
        }
        #endregion
    }

    internal delegate FileTransferItem CbGetFileTransItem(string projectID, bool isSended);
    internal delegate FileTransferItem CbGetFileTransItem2(string projectID);
    public delegate void CbFileReceiveCanceled(string friend ,string projectID) ;
    public delegate void CbCancelFile(string projectID,bool isSsend);
}
