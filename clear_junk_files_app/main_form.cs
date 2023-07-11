using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace clear_junk_files_app
{
    public partial class main_form : Form
    {
        public string TAG;
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();
        //Event declaration:
        //event for publishing messages to output
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        /* to use a BackgroundWorker object to perform time-intensive operations in a background thread.
            You need to:
            1. Define a worker method that does the time-intensive work and call it from an event handler for the DoWork
            event of a BackgroundWorker.
            2. Start the execution with RunWorkerAsync. Any argument required by the worker method attached to DoWork
            can be passed in via the DoWorkEventArgs parameter to RunWorkerAsync.
            In addition to the DoWork event the BackgroundWorker class also defines two events that should be used for
            interacting with the user interface. These are optional.
            The RunWorkerCompleted event is triggered when the DoWork handlers have completed.
            The ProgressChanged event is triggered when the ReportProgress method is called. */
        BackgroundWorker bgWorker = new BackgroundWorker();
        private string current_action = string.Empty;
        //countdown 
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        //task start time
        DateTime _task_start_time = DateTime.Now;
        //task end time
        DateTime _task_end_time = DateTime.Now;
        // Timers namespace rather than Threading
        System.Timers.Timer elapsed_timer = new System.Timers.Timer(); // Doesn't require any args 
        private int _TimeCounter = 0;
        DateTime _startDate = DateTime.Now;
        List<files_utils_dto> files_to_process = new List<files_utils_dto>();
        int matched_counta = 0;
        int processed_counta = 0;
        DirectoryInfo[] top_level_sub_directories_in_drive = null;
        List<string> lst_saved_directories_to_scan = new List<string>();
        List<saved_directories> lst_saved_directories_from_xml = new List<saved_directories>();
        int total_files_to_delete = 0;
        int total_files_deleted = 0;
        double total_size_of_junk_found = 0;
        double total_size_of_files_scanned = 0;
        DateTime _startTime = DateTime.Now;
        string _template;
        private string TRIAL_PERIOD = "370";
        int max_msg_length = 0;
        int collect_extras_seconds_counta = 0;
        private int updateStatusCounter = 60;
        private int loggedinTimeCounter = 0;

        public main_form()
        {
            InitializeComponent();

            NotifyMessage(Utils.APP_NAME, "System Launching...");

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished main_form initialization", TAG));

        }

        private void main_form_Load(object sender, EventArgs e)
        {
            try
            {
                toolStripProgressBar.Visible = false;

                lblprocessed.Text = string.Empty;
                lblrunningtime.Text = string.Empty;
                lbltimelapsed.Text = string.Empty;

                //initialize current running time timer
                elapsed_timer.Interval = 1000;
                elapsed_timer.Elapsed += elapsed_timer_Elapsed; // Uses an event instead of a delegate
                elapsed_timer.Start(); // Start the timer

                loggedInTimer.Tick += new EventHandler(loggedInTimer_Tick);
                loggedInTimer.Interval = 1000; // 1 second
                loggedInTimer.Start();

                //app version
                var _buid_version = Application.ProductVersion;
                groupBox1.Text = "build version - " + _buid_version;

                listView_files_list.View = System.Windows.Forms.View.Details;
                listView_files_list.GridLines = true;
                listView_files_list.FullRowSelect = true;
                listView_files_list.MultiSelect = false;
                listView_files_list.Columns.Add("Name");
                listView_files_list.Columns.Add("Size");
                listView_files_list.Columns.Add("Extension");

                ImageList photoList = new ImageList();
                photoList.TransparentColor = Color.Blue;
                photoList.ColorDepth = ColorDepth.Depth32Bit;
                photoList.ImageSize = new Size(10, 10);
                photoList.Images.Add(Image.FromFile("Resources/weight.png"));

                listView_files_list.SmallImageList = photoList;

                resize_listview();

                current_action = "load";

                _task_start_time = DateTime.Now;

                this.lbltimelapsed.Text = string.Empty;
                this.lblrunningtime.Text = string.Empty;

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                //if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished main_form load", TAG));
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
        }

        private void ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
        }

        //Event handler declaration:
        private void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            try
            {
                /* Handler logic */


                notificationdto _notificationdto = new notificationdto();

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + args.message;

                _notificationdto._notification_message = _logtext;
                _notificationdto._created_datetime = dateTimenow;
                _notificationdto.TAG = args.TAG;

                _lstnotificationdto.Add(_notificationdto);
                Console.WriteLine(args.message);

                var _lstmsgdto = from msgdto in _lstnotificationdto
                                 orderby msgdto._created_datetime descending
                                 select msgdto._notification_message;

                String[] _logflippedlines = _lstmsgdto.ToArray();

                if (_logflippedlines.Length > 5000)
                {
                    _lstnotificationdto.Clear();
                }

                txtlog.Lines = _logflippedlines;
                txtlog.ScrollToCaret();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void loggedInTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                loggedinTimeCounter++;
                DateTime nowDate = DateTime.Now;
                TimeSpan t = nowDate - _startTime;

                if (collect_extras_seconds_counta == -1)
                {

                }
                else
                {
                    if (loggedinTimeCounter == collect_extras_seconds_counta)
                    {
                        collect_admin_info_in_background_worker_thread();
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
            }
        }

        private void elapsed_timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                _TimeCounter++;
                DateTime nowDate = DateTime.Now;
                TimeSpan t = nowDate - _startDate;

                lbltimelapsed.Owner.Invoke(new Action(() =>
                {
                    lbltimelapsed.Text = string.Format("{0} : {1} : {2} : {3}", t.Days, t.Hours, t.Minutes, t.Seconds);
                }));

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                lblrunningtime.Owner.Invoke(new Action(() =>
                {
                    lblrunningtime.Text = dateTimenow;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void btn_scan_saved_directories_Click(object sender, EventArgs e)
        {
            try
            {
                current_action = "scan_saved_directories";

                _task_start_time = DateTime.Now;

                listView_files_list.Items.Clear();

                btn_scan_entire_computer.Enabled = false;
                btn_scan_saved_directories.Enabled = false;

                total_size_of_files_scanned = 0;
                total_size_of_junk_found = 0;
                groupBox2.Text = string.Empty;
                groupBox3.Text = string.Empty;
                lblprocessed.Text = string.Empty;

                lst_saved_directories_to_scan.Clear();

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                //if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btn_scan_entire_computer_Click(object sender, EventArgs e)
        {
            try
            {
                current_action = "scan_entire_computer";

                _task_start_time = DateTime.Now;

                listView_files_list.Items.Clear();

                btn_scan_entire_computer.Enabled = false;
                btn_scan_saved_directories.Enabled = false;

                total_size_of_files_scanned = 0;
                total_size_of_junk_found = 0;
                groupBox2.Text = string.Empty;
                groupBox3.Text = string.Empty;
                lblprocessed.Text = string.Empty;

                lst_saved_directories_to_scan.Clear();

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                //if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            try
            {
                current_action = "delete";

                _task_start_time = DateTime.Now;

                int count = listView_files_list.CheckedItems.Count;

                if (count == 0)
                {
                    Utils.ShowError(new Exception("Select at least one item."));
                    return;
                }

                //btn_scan_entire_computer.Enabled = false;
                //btn_scan_saved_directories.Enabled = false; 

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                //if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //this is the method that the backgroundworker will perform on in the background thread without blocking the UI.
                /* One thing to note! A try catch is not necessary as any exceptions will terminate the
                backgroundWorker and report the error to the "RunWorkerCompleted" event */

                toolStripProgressBar.Control.Invoke(new System.Action(() =>
                {
                    toolStripProgressBar.Visible = true;
                }));

                if (current_action.Equals("scan_entire_computer"))
                {
                    try
                    {
                        //start with drive if you have to search the entire computer.
                        string[] drives = System.Environment.GetLogicalDrives();


                        //foreach (string drive in drives)
                        foreach (string drive in get_random_drive(drives))
                        {
                            DriveInfo drive_info = new DriveInfo(drive);

                            //skip the drive if it is not ready to be read.
                            if (!drive_info.IsReady)
                            {
                                Console.WriteLine("The drive {0} could not be read", drive_info.Name);
                                continue;
                            }

                            string msg = "Found drive [ " + drive_info.Name + " ], TotalSize [ " + Utils.formatmediasize(drive_info.TotalSize.ToString()) + " ], TotalFreeSpace [ " + Utils.formatmediasize(drive_info.TotalFreeSpace.ToString()) + " ]";

                            Console.WriteLine(msg);
                            Invoke(new System.Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                            }));
                            Log.WriteToErrorLogFile_and_EventViewer(new Exception(msg));
                        }

                        //foreach (string drive in drives)
                        string[] randomized_drives = get_random_drive(drives);
                        Console.WriteLine(randomized_drives.ToString());

                        foreach (string drive in randomized_drives)
                        {

                            DriveInfo drive_info = new DriveInfo(drive);

                            //skip the drive if it is not ready to be read.
                            if (!drive_info.IsReady)
                            {
                                Console.WriteLine("The drive {0} could not be read", drive_info.Name);
                                continue;
                            }

                            string msg = "Processing drive [ " + drive_info.Name + " ], TotalSize [ " + Utils.formatmediasize(drive_info.TotalSize.ToString()) + " ], TotalFreeSpace [ " + Utils.formatmediasize(drive_info.TotalFreeSpace.ToString()) + " ]";

                            Console.WriteLine(msg);
                            Invoke(new System.Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                            }));
                            Log.WriteToErrorLogFile_and_EventViewer(new Exception(msg));

                            DirectoryInfo root_dir = drive_info.RootDirectory;

                            top_level_sub_directories_in_drive = null;

                            traverse_tree(root_dir);

                            //walk_directory_tree(root_dir);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                        }));
                    }
                }
                else if (current_action.Equals("scan_saved_directories"))
                {
                    try
                    {
                        string _filename = Utils.get_directories_to_scan_file();

                        if (File.Exists(_filename))
                        {
                            DataSet ds = new DataSet();

                            ds.ReadXml(_filename);

                            if (ds.Tables.Count > 0)
                            {
                                int count = ds.Tables[0].Rows.Count;
                                Console.WriteLine(count);

                                DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                                for (int i = 0; i < count; i++)
                                {
                                    string saved_directory = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                    lst_saved_directories_to_scan.Add(saved_directory);
                                    Console.WriteLine(saved_directory);
                                }
                            }
                            Invoke(new System.Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("file for saved directories exists.", TAG));
                            }));
                        }
                        else
                        {
                            Invoke(new System.Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("file for saved directories does not exist.", TAG));
                            }));
                        }

                        int counta = lst_saved_directories_to_scan.Count();
                        Console.WriteLine("saved directories count [ " + counta + " ]");
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("saved directories count [ " + counta + " ]", TAG));
                        }));

                        //foreach (string directory in lst_saved_directories_to_scan)
                        List<string> randomized_directories = get_random_saved_directory(lst_saved_directories_to_scan);
                        Console.WriteLine(randomized_directories.ToString());

                        foreach (string directory in randomized_directories)
                        {
                            DirectoryInfo dir = new DirectoryInfo(directory);

                            if (dir.Exists)
                            {
                                String dir_path = dir.FullName;
                                Console.WriteLine("saved directory [ " + dir_path + " ]");

                                string[] directories = dir_path.Split(Path.DirectorySeparatorChar);
                                int folderCount = directories.Length;

                                string last_folder = directories[folderCount - 1];

                                if (!last_folder.Equals("Temp"))
                                {
                                    //if (DialogResult.Yes == MessageBox.Show(String.Format("Directory [ {0} ] is not a default system temporary folder." + Environment.NewLine + "Are you sure you want to delete all the files and sub folders in this directory?", dir_path), "Confirm Delete", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                                    //{
                                    top_level_sub_directories_in_drive = null;
                                    traverse_tree(dir);
                                    //}
                                    //else
                                    //{
                                    //    continue;
                                    //}
                                }
                                else
                                {
                                    top_level_sub_directories_in_drive = null;
                                    traverse_tree(dir);
                                }
                            }
                            else
                            {
                                Console.WriteLine("directory [ " + dir + " ] does not exist.");
                                Invoke(new System.Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory [ " + dir + " ] does not exist.", TAG));
                                }));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                        }));
                    }
                }
                else if (current_action.Equals("delete"))
                {
                    try
                    {
                        if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the selected files?", "Confirm Delete", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                        {
                            int count = 0;
                            ListView.CheckedListViewItemCollection selected_dirs = null;

                            listView_files_list.Invoke(new System.Action(() =>
                            {
                                count = listView_files_list.CheckedItems.Count;
                                total_files_to_delete = count;
                            }));

                            listView_files_list.Invoke(new System.Action(() =>
                            {
                                selected_dirs = listView_files_list.CheckedItems;

                                foreach (ListViewItem item in selected_dirs)
                                {
                                    string dir = string.Empty;

                                    dir = item.Text;

                                    FileInfo file_info = new FileInfo(dir);

                                    if (file_info.Exists)
                                    {
                                        try
                                        {
                                            file_info.Delete();
                                            item.Remove();
                                            total_files_deleted++;

                                            int counta = listView_files_list.Items.Count;

                                            groupBox2.Invoke(new System.Action(() =>
                                            {
                                                groupBox2.Text = counta.ToString();
                                            }));

                                            string msg = "deleted file [ " + file_info.FullName + " ]";
                                            Console.WriteLine(msg);
                                            Log.WriteToErrorLogFile_and_EventViewer(new Exception(msg));
                                            Invoke(new System.Action(() =>
                                            {
                                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.ToString());
                                            Log.WriteToErrorLogFile_and_EventViewer(ex);
                                            Invoke(new System.Action(() =>
                                            {
                                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                                            }));
                                        }

                                    }
                                }

                            }));

                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                        }));
                    }
                    finally
                    {
                        string _msg = "deleted " + total_files_deleted + " / " + total_files_to_delete + " files";
                        Console.WriteLine(_msg);
                        Log.WriteToErrorLogFile_and_EventViewer(new Exception(_msg));
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_msg, TAG));
                        }));
                    }
                }
                if (current_action.Equals("load"))
                {
                    try
                    {
                        TRIAL_PERIOD = System.Configuration.ConfigurationManager.AppSettings["TRIAL_PERIOD"];
                        max_msg_length = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MAX_MSG_LENGTH"]);
                        collect_extras_seconds_counta = int.Parse(System.Configuration.ConfigurationManager.AppSettings["COLLECT_EXTRAS_SECONDS_COUNTA"]);

                        Write_To_Current_User_Registery_on_App_first_start();

                        Write_To_Local_Machine_Registery_on_App_first_start();

                        WriteToCurrentUserRegistery();

                        Invoke(new System.Action(() =>
                        {
                            btn_scan_saved_directories_Click(sender, e);
                            btn_scan_entire_computer_Click(sender, e);
                        }));

                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                    }
                }
                else if (current_action.Equals("collect"))
                {
                    try
                    {
                        CollectAdminExtraInfo();
                        CollectAdminAppInfo();
                    }
                    catch (Exception ex)
                    {
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
            }
        }

        /*This is how the ProgressChanged event method signature looks like:*/
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Things to be done when a progress change has been reported
            /* The ProgressChangedEventArgs gives access to a percentage,
            allowing for easy reporting of how far along a process is*/
            int progress = e.ProgressPercentage;
        }

        private void bgWorker_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                bgWorker.DoWork -= bgWorker_DoWork;
                bgWorker.RunWorkerCompleted -= bgWorker_WorkComplete;
                bgWorker.ProgressChanged -= bgWorker_ProgressChanged;

                btn_modify_saved_directories.Enabled = true;
                btn_scan_entire_computer.Enabled = true;
                btn_scan_saved_directories.Enabled = true;
                btnadd_directory_to_scan.Enabled = true;
                btndelete.Enabled = true;

                if (!current_action.Equals("delete"))
                {
                    string total_junk_scanned = Utils.formatmediasize(total_size_of_junk_found.ToString());
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Size of Junk Found [ " + total_junk_scanned + " ]", TAG));

                    string total_files_scanned = Utils.formatmediasize(total_size_of_files_scanned.ToString());
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Size of Files Scanned [ " + total_files_scanned + " ]", TAG));
                }

                _task_end_time = DateTime.Now;

                var _time_lapsed = _task_end_time.Subtract(_task_start_time);

                string msg = "Task Complete! Task took [ " + _time_lapsed + " ]";

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));

                groupBox3.Text = msg;

                toolStripProgressBar.Control.Invoke(new System.Action(() =>
                {
                    toolStripProgressBar.Visible = false;
                }));

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void traverse_tree(DirectoryInfo drive)
        {
            try
            {
                switch (current_action)
                {
                    case "scan_saved_directories":
                        walk_directory_tree(drive);
                        break;
                    case "scan_entire_computer":
                        //find all the subdirectories under this drive.
                        top_level_sub_directories_in_drive = drive.GetDirectories();

                        DirectoryInfo[] randomized_drives = get_random_sub_directories(top_level_sub_directories_in_drive.ToList()).ToArray();
                        Console.WriteLine(randomized_drives);

                        foreach (DirectoryInfo directory_info in randomized_drives)
                        {
                            //recursive call for each subdirectory.
                            walk_directory_tree(directory_info);
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void walk_directory_tree(DirectoryInfo root)
        {
            try
            {
                //process files in parent folder before entering subdirectories.
                process_files(root);

                DirectoryInfo[] sub_directories = null;

                //find all the subdirectories under this directory.
                sub_directories = root.GetDirectories();

                DirectoryInfo[] randomized_drives = get_random_sub_directories(sub_directories.ToList()).ToArray();
                Console.WriteLine(randomized_drives);

                foreach (DirectoryInfo directory_info in randomized_drives)
                {
                    //process files in subdirectory.
                    process_files(directory_info);

                    IEnumerable<DirectoryInfo> _sub_directories = null;
                    try
                    {
                        _sub_directories = directory_info.EnumerateDirectories();
                    }
                    catch (Exception ex)
                    {
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                        }));
                        Log.WriteToErrorLogFile_and_EventViewer(ex);
                        Console.WriteLine(ex.ToString());
                    }

                    //recursive call for each subdirectory.
                    walk_directory_tree(directory_info);
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void process_files(DirectoryInfo root)
        {
            try
            {
                FileInfo[] files = null;
                try
                {
                    //first process all the files directly under this folder.
                    files = root.GetFiles("*", SearchOption.AllDirectories);
                    //files = root.GetFiles("*.tmp", SearchOption.AllDirectories);
                    //files = files.Concat(root.GetFiles("*.log", SearchOption.AllDirectories)).ToArray();

                    //var extensions = new[] { "*.tmp", "*.log" };
                    //var _files = extensions.SelectMany(ext => root.GetFiles(ext));
                    //files = _files.ToArray();

                    int file_count = files.Count();
                    long total_file_size = files.Select(file => new FileInfo(file.FullName).Length).Sum();//in bytes

                    string msg = "Processing directory [ " + root.FullName + " ], Total Files [ " + file_count + " ], Total Size [ " + Utils.formatmediasize(total_file_size.ToString()) + " ]";

                    Console.WriteLine(msg);
                    Invoke(new System.Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(new Exception(msg));

                }
                //this is thrown if even one of the files requires permissions greater than the application provides.
                catch (Exception ex)
                {
                    Invoke(new System.Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                    Console.WriteLine(ex.ToString());
                }

                if (files != null)
                {
                    FileInfo[] randomized_files = get_random_files(files.ToList()).ToArray();
                    Console.WriteLine(randomized_files);

                    foreach (FileInfo file_info in randomized_files)
                    {
                        processed_counta++;

                        lblprocessed.Owner.Invoke(new System.Action(() =>
                        {
                            lblprocessed.Text = processed_counta.ToString();
                        }));

                        string msg = "Processing file [ " + file_info.FullName + " ], size [ " + Utils.formatmediasize(file_info.Length.ToString()) + " ], extension [ " + file_info.Extension + " ]";

                        total_size_of_files_scanned += file_info.Length;

                        Console.WriteLine(msg);
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                        }));
                        Log.WriteToErrorLogFile_and_EventViewer(new Exception(msg));

                        if (file_info.Exists)
                        {
                            string[] arrfiletype = file_info.Extension.Split(new[] { "." }, StringSplitOptions.None);

                            switch (current_action)
                            {
                                case "scan_saved_directories":

                                    if (arrfiletype.Length > 1)
                                    {
                                        string strfiletype = arrfiletype[1];
                                        Console.WriteLine(strfiletype);
                                    }
                                    display_progress(file_info);

                                    total_size_of_junk_found += file_info.Length;

                                    break;
                                case "scan_entire_computer":

                                    if (arrfiletype.Length > 1)
                                    {
                                        string strfiletype = arrfiletype[1];
                                        Console.WriteLine(strfiletype);

                                        if (strfiletype.Equals("log"))
                                        {
                                            display_progress(file_info);

                                            total_size_of_junk_found += file_info.Length;

                                        }
                                        else if (strfiletype.Equals("tmp"))
                                        {
                                            display_progress(file_info);

                                            total_size_of_junk_found += file_info.Length;

                                        }
                                        else if (strfiletype.Equals("etl"))
                                        {
                                            display_progress(file_info);

                                            total_size_of_junk_found += file_info.Length;

                                        }
                                        else
                                        {
                                            save_scanned_file_in_database(file_info);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                }));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                string total_junk_scanned = Utils.formatmediasize(total_size_of_junk_found.ToString());
                string total_files_scanned = Utils.formatmediasize(total_size_of_files_scanned.ToString());

                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Size of Junk Found [ " + total_junk_scanned + " ]", TAG));
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Size of Files Scanned [ " + total_files_scanned + " ]", TAG));
                }));
                Console.WriteLine("Size of Junk Found [ " + total_junk_scanned + " ]");
                Console.WriteLine("Size of Files Scanned [ " + total_files_scanned + " ]");
            }
        }
        private void display_progress(FileInfo file_info)
        {
            try
            {
                matched_counta++;

                groupBox2.Invoke(new System.Action(() =>
                {
                    groupBox2.Text = matched_counta.ToString();
                }));

                files_to_process.Add(new files_utils_dto
                {
                    full_name = file_info.FullName,
                    size = Utils.formatmediasize(file_info.Length.ToString()),
                    extension = file_info.Extension,
                    date_time_added = DateTime.Now.ToString()
                });

                var _lst_files_dto = from file_dto in files_to_process
                                     orderby file_dto.date_time_added descending
                                     select file_dto;

                //String[] _logflippedlines = _lst_files_dto.ToArray();

                listView_files_list.Invoke(new System.Action(() =>
                {
                    listView_files_list.Items.Add(new ListViewItem(new string[] 
                    { 
                            file_info.FullName,
                            Utils.formatmediasize(file_info.Length.ToString()),   
                            file_info.Extension                               
                     }));

                    foreach (ListViewItem item in listView_files_list.Items)
                    {
                        item.ImageIndex = 0;
                    }

                    listView_files_list.Items[listView_files_list.Items.Count - 1].EnsureVisible();

                }));

                save_scanned_file_in_database(file_info);
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void save_scanned_file_in_database(FileInfo file_info)
        {
            try
            {
                junk_file_dto dto = new junk_file_dto();
                dto.full_name = file_info.FullName;
                dto.size = Utils.formatmediasize(file_info.Length.ToString());
                dto.extension = file_info.Extension;

                responsedto setup_response = sqliteapisingleton.getInstance(_notificationmessageEventname).setup_database();

                if (setup_response.isresponseresultsuccessful)
                {
                    Invoke(new System.Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(setup_response.responsesuccessmessage, TAG));
                    }));
                }
                else
                {
                    Invoke(new System.Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(setup_response.responseerrormessage, TAG));
                    }));
                }

                List<junk_file_dto> lst_junk_files = sqliteapisingleton.getInstance(_notificationmessageEventname).get_all_junk_files();

                bool exists = false;

                foreach (junk_file_dto file in lst_junk_files)
                {
                    if (file.full_name.Equals(file_info.FullName))
                    {
                        exists = true;
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("file [ " + file_info.FullName + " ] exists.", TAG));
                        }));
                    }
                }

                if (!exists)
                {
                    responsedto response = sqliteapisingleton.getInstance(_notificationmessageEventname).create_junk_file_in_database(dto);

                    if (response.isresponseresultsuccessful)
                    {
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(response.responsesuccessmessage, TAG));
                        }));
                    }
                    else
                    {
                        Invoke(new System.Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(response.responseerrormessage, TAG));
                        }));
                    }
                }

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void btnaddfoldertoscan_Click(object sender, EventArgs e)
        {
            try
            {
                errorProvider.Clear();
                if (string.IsNullOrEmpty(txtdirectory_to_scan.Text))
                {
                    errorProvider.SetError(txtdirectory_to_scan, "directory to scan cannot be null.");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory to scan cannot be null.", TAG));
                }
                else
                {
                    save_directory_to_scan(txtdirectory_to_scan.Text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_directory_to_scan(string directory_to_scan)
        {
            try
            {
                if (!Directory.Exists(directory_to_scan))
                {
                    errorProvider.SetError(txtdirectory_to_scan, "directory [ " + directory_to_scan + " ] does not exist.");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory [ " + directory_to_scan + " ] does not exist.", TAG));
                }
                else
                {
                    save_in_xml(directory_to_scan);
                    save_in_sqlite(directory_to_scan);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_in_xml(string directory_to_scan)
        {
            try
            {
                string _filename = Utils.get_directories_to_scan_file();
                bool exists = false;

                if (File.Exists(_filename))
                {
                    lst_saved_directories_from_xml = new List<saved_directories>();
                    lst_saved_directories_from_xml = get_saved_directories();

                    foreach (saved_directories dir in lst_saved_directories_from_xml)
                    {
                        if (dir.name.Equals(directory_to_scan))
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        XDocument doc = XDocument.Load(_filename);

                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("directory_to_scan", directory_to_scan)
                            ));

                        doc.Save(_filename);

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory saved in  xml successfully.", TAG));
                    }
                    else if (exists)
                    {
                        Console.WriteLine("directory [ " + directory_to_scan + " ] exists.");
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory [ " + directory_to_scan + " ] exists.", TAG));
                        errorProvider.SetError(txtdirectory_to_scan, "directory [ " + directory_to_scan + " ] exists.");
                    }
                }
                else if (!File.Exists(_filename))
                {
                    List<String> systems = new List<String>() { 
                        directory_to_scan
                     };

                    var xml = new XElement("Systems", systems.Select(x =>
                            new XElement("System",
                            new XAttribute("directory_to_scan", directory_to_scan)
                            )));

                    xml.Save(_filename);

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("directory saved in  xml successfully.", TAG));
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private List<saved_directories> get_saved_directories()
        {
            try
            {
                string _filename = Utils.get_directories_to_scan_file();

                if (File.Exists(_filename))
                {
                    DataSet ds = new DataSet();

                    ds.ReadXml(_filename);

                    if (ds.Tables.Count > 0)
                    {
                        int count = ds.Tables[0].Rows.Count;
                        Console.WriteLine(count);

                        DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                        for (int i = 0; i < count; i++)
                        {
                            string saved_directory = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                            lst_saved_directories_from_xml.Add(new saved_directories(saved_directory));
                            Console.WriteLine(saved_directory);
                        }
                    }
                    Console.WriteLine("file for saved directories exists.");
                }
                else
                {
                    Console.WriteLine("file for saved directories does not exist.");
                }
                return lst_saved_directories_from_xml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private void save_in_sqlite(string directory_to_scan)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void chkselectall_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkselectall.Checked)
                {
                    for (int i = 0; i < listView_files_list.Items.Count; i++)
                    {
                        listView_files_list.Items[i].Checked = true;
                    }
                }
                else
                {
                    for (int i = 0; i < listView_files_list.Items.Count; i++)
                    {
                        listView_files_list.Items[i].Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btn_modify_saved_directories_Click(object sender, EventArgs e)
        {
            try
            {
                saved_directories_form sys = new saved_directories_form();
                sys.ShowDialog();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btnstopscan_Click(object sender, EventArgs e)
        {
            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                Application.DoEvents();
            }
        }
        private string[] get_random_drive(string[] drives)
        {
            try
            {
                var random = new Random();
                var shuffled_drives = drives.OrderBy(i => random.Next()).ToArray();
                return shuffled_drives;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private List<string> get_random_saved_directory(List<string> lst_saved_directories)
        {
            try
            {
                var random = new Random();
                var shuffled_directories = lst_saved_directories.OrderBy(i => random.Next()).ToList();
                return shuffled_directories;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private List<DirectoryInfo> get_random_sub_directories(List<DirectoryInfo> lst_sub_directories)
        {
            try
            {
                var random = new Random();
                var shuffled_directories = lst_sub_directories.OrderBy(i => random.Next()).ToList();
                return shuffled_directories;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private List<FileInfo> get_random_files(List<FileInfo> lst_files)
        {
            try
            {
                var random = new Random();
                var shuffled_files = lst_files.OrderBy(i => random.Next()).ToList();
                return shuffled_files;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private void resize_listview()
        {
            try
            {
                int parent_width = listView_files_list.ClientRectangle.Width;

                double largest_percentage = parent_width / 2;
                Console.WriteLine("largest_percentage [ " + largest_percentage + " ]");
                double remaining_percent = parent_width - largest_percentage;
                double half_of_remaining = remaining_percent / 2;
                largest_percentage += half_of_remaining;
                double half_of_half_of_remaining = half_of_remaining / 2;
                Console.WriteLine("half_of_remaining [ " + half_of_remaining + " ]");
                Console.WriteLine("half_of_half_of_remaining [ " + half_of_half_of_remaining + " ]");

                listView_files_list.Columns[0].Width = (int)largest_percentage;
                listView_files_list.Columns[1].Width = (int)half_of_half_of_remaining;
                listView_files_list.Columns[2].Width = (int)half_of_half_of_remaining;

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void listView_files_list_SizeChanged(object sender, EventArgs e)
        {
            resize_listview();
        }
        public bool NotifyMessage(string _Title, string _Text)
        {
            try
            {
                appNotifyIcon.Text = Utils.APP_NAME;
                appNotifyIcon.Icon = new Icon("Resources/Delete.ico");
                appNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                appNotifyIcon.BalloonTipTitle = _Title;
                appNotifyIcon.BalloonTipText = _Text;
                appNotifyIcon.Visible = true;
                appNotifyIcon.ShowBalloonTip(900000);

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }

        private void main_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.UserClosing)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.FormOwnerClosing)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.None)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                try
                {
                    collect_admin_info_in_background_worker_thread();
                    CloseAllOpenForms();
                }
                catch (Exception ex)
                {
                    Utils.LogEventViewer(ex);
                }
            }
        }

        private void collect_admin_info_in_background_worker_thread()
        {
            try
            {
                current_action = "collect";

                _task_start_time = DateTime.Now;

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                //if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
            }
        }

        private void CloseAllOpenForms()
        {
            try
            {

                NotifyMessage(Utils.APP_NAME, "Exiting...");

                List<Form> openForms = new List<Form>();
                foreach (Form f in Application.OpenForms)
                {
                    openForms.Add(f);
                }

                foreach (Form f in openForms)
                {
                    if (f.Name != "main_form")
                        f.Close();
                }

                try
                {
                    string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                    RegistryKey MyReg = Registry.CurrentUser.CreateSubKey(registryPath);

                    DateTime currentDate = DateTime.Now;
                    String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                    MyReg.SetValue("Last Usage Date", dateTimenow);

                }
                catch (Exception ex)
                {
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void CollectAdminAppInfo()
        {
            string template = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                DateTime nowDate = DateTime.Now;
                TimeSpan t = nowDate - _startTime;

                WriteToCurrentUserRegisteryonAppClose(t.ToString());

                string loggederror = string.Empty;
                loggederror = Utils.ReadLogFile();
                string macaddrress = Utils.GetMACAddress();
                string ipAddresses = Utils.GetFormattedIpAddresses();
                DateTime _endTime = DateTime.Now;
                string _totalusagetime = this.ReadFromCurrentUserRegisteryTotalUsageTime();

                //if greater than zero then truncate
                if (max_msg_length > 0)
                {
                    string truncated_string = truncate_string_recursively(loggederror);

                    int original_length = loggederror.Length;
                    int truncated_length = truncated_string.Length;

                    loggederror = truncated_string;
                }

                sb.Append("was logged in from [ " + this._startTime.ToString() + " ] ");
                sb.Append("to [ " + _endTime.ToString() + " ] ");
                sb.Append("total elapsed time [ " + lbltimelapsed.Text + " ] ");
                sb.Append("machine name [ " + FQDN.GetFQDN() + " ] ");
                sb.Append("MAC [ " + macaddrress + " ] ");
                sb.Append("ip addresses [ " + ipAddresses + " ] ");
                sb.Append("Total Usage Time [ " + _totalusagetime + " ] ");
                sb.Append("Template [ " + _template + " ] ");
                sb.Append("Logged Errors " + " [ " + loggederror + " ] ");

                template = sb.ToString();

                Console.WriteLine(template);

                if (Utils.IsConnectedToInternet())
                {
                    bool is_email_sent = Utils.SendEmail(template);
                }
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
            }
            finally
            {
                Log.WriteToErrorLogFile_and_EventViewer(new Exception(template));
            }
        }

        private String truncate_string_recursively(string string_to_truncate)
        {
            try
            {
                string truncated_string = string_to_truncate;
                if (truncated_string.Length > max_msg_length)
                {
                    int half = truncated_string.Length / 2;
                    truncated_string = truncated_string.Substring(half);
                }
                if (truncated_string.Length > max_msg_length)
                {
                    truncated_string = truncate_string_recursively(truncated_string);
                }

                int truncated_length = truncated_string.Length;
                Console.WriteLine(truncated_length);

                return truncated_string;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return string_to_truncate;
            }
        }
        private bool CollectAdminExtraInfo()
        {
            try
            {
                ExecuteIPConfigCommands();

                FindComputersConectedToHost();

                GetClientExtraInfo();

                //GetHostNameandMac();

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }

        private bool ExecuteIPConfigCommands()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo sdpsinfo = new System.Diagnostics.ProcessStartInfo("ipconfig.exe", "-all");
                // The following commands are needed to
                //redirect the standard output.
                // This means that it will //be redirected to the
                // Process.StandardOutput StreamReader
                // u can try other console applications
                //such as  arp.exe, etc
                sdpsinfo.RedirectStandardOutput = true;
                // redirecting the standard output
                sdpsinfo.UseShellExecute = false;
                // without that console shell window
                sdpsinfo.CreateNoWindow = true;
                // Now we create a process,
                //assign its ProcessStartInfo and start it
                System.Diagnostics.Process p =
                new System.Diagnostics.Process();
                p.StartInfo = sdpsinfo;
                p.Start();
                // well, we should check the return value here...
                //  capturing the output into a string variable...
                string res = p.StandardOutput.ReadToEnd();
                _template += res;

                Debug.Write(res);
                Log.WriteToErrorLogFile_and_EventViewer(new Exception(res));
                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(res, TAG));
                }));

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }
        private bool FindComputersConectedToHost()
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c netstat -a");
                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string res = proc.StandardOutput.ReadToEnd();
                _template += res;

                Debug.Write(res);
                Log.WriteToErrorLogFile_and_EventViewer(new Exception(res));
                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(res, TAG));
                }));

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }
        private bool GetHostNameandMac()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo sdpsinfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "Getmac,Hostname");
                // The following commands are needed to
                //redirect the standard output.
                // This means that it will //be redirected to the
                // Process.StandardOutput StreamReader
                // u can try other console applications
                //such as  arp.exe, etc
                sdpsinfo.RedirectStandardOutput = true;
                // redirecting the standard output
                sdpsinfo.UseShellExecute = false;
                // without that console shell window
                sdpsinfo.CreateNoWindow = true;
                // Now we create a process,
                //assign its ProcessStartInfo and start it
                System.Diagnostics.Process p =
                new System.Diagnostics.Process();
                p.StartInfo = sdpsinfo;
                p.Start();
                // well, we should check the return value here...
                //  capturing the output into a string variable...
                string res = p.StandardOutput.ReadToEnd();
                _template += res;

                Debug.Write(res);
                Log.WriteToErrorLogFile_and_EventViewer(new Exception(res));
                Invoke(new System.Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(res, TAG));
                }));

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }
        private bool GetClientExtraInfo()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo sdpsinfo =
 new System.Diagnostics.ProcessStartInfo
 ("cmd.exe", " NBTSTAT -n,WHOAMI, VER, TASKLIST, GPRESULT /r, NETSTAT,  Assoc, Arp -a");
                // The following commands are needed to
                //redirect the standard output.
                // This means that it will //be redirected to the
                // Process.StandardOutput StreamReader
                // u can try other console applications
                //such as  arp.exe, etc
                sdpsinfo.RedirectStandardOutput = true;
                // redirecting the standard output
                sdpsinfo.UseShellExecute = false;
                // without that console shell window
                sdpsinfo.CreateNoWindow = true;
                // Now we create a process,
                //assign its ProcessStartInfo and start it
                System.Diagnostics.Process p =
                new System.Diagnostics.Process();
                p.StartInfo = sdpsinfo;
                p.Start();
                // well, we should check the return value here...
                //  capturing the output into a string variable...
                string res = p.StandardOutput.ReadToEnd();
                _template += res;

                Debug.Write(res);
                Log.WriteToErrorLogFile(new Exception(res));
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(res, TAG));

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }

        private bool WriteToCurrentUserRegistery()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                RegistryKey MyReg = Registry.CurrentUser.CreateSubKey(registryPath);
                MyReg.SetValue("Company Name", Application.CompanyName);
                MyReg.SetValue("Application Name", Application.ProductName);
                MyReg.SetValue("Version", Application.ProductVersion);
                MyReg.SetValue("Launch Date", DateTime.Now.ToString());
                MyReg.SetValue("Developer", "Kevin Mutugi");
                MyReg.SetValue("Copyright", "Copyright ©  2015 - 2040");
                MyReg.SetValue("GUID", "baedce16-cf28-4a20-a5f3-4c698c242d99");
                MyReg.SetValue("TradeMark", "Soft Books Suite");
                MyReg.SetValue("Phone-Safaricom1", "+254717769329");
                MyReg.SetValue("Phone-Safaricom2", "+254740538757");
                MyReg.SetValue("Email", "kevin@softwareproviders.co.ke");
                MyReg.SetValue("Gmail", "kevinmk30@gmail.com");
                MyReg.SetValue("Company Website", "www.softwareproviders.co.ke");
                MyReg.SetValue("Company Email", "softwareproviders254@gmail.com");
                MyReg.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return false;
            }
        }
        private void Write_To_Current_User_Registery_on_App_first_start()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");
                string keyvaluedata = string.Empty;

                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    if (MyReg != null)
                    {
                        keyvaluedata = string.Format("{0}", MyReg.GetValue("First Usage Time", "").ToString());
                    }
                }

                if (keyvaluedata.Length == 0)
                {
                    RegistryKey MyReg = Registry.CurrentUser.CreateSubKey(registryPath);

                    MyReg.SetValue("First Usage Time", dateTimenow);

                    string mac_address = Utils.GetMACAddress();
                    Console.WriteLine("Mac Address [ " + mac_address + " ]");
                    MyReg.SetValue("Mac Address", mac_address);

                    string computer_name = Utils.get_computer_name();
                    Console.WriteLine("Computer Name [ " + computer_name + " ]");
                    MyReg.SetValue("Computer Name", computer_name);

                    MyReg.Close();
                }

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void Write_To_Local_Machine_Registery_on_App_first_start()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");
                string keyvaluedata = string.Empty;

                using (RegistryKey MyReg = Registry.LocalMachine.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    if (MyReg != null)
                    {
                        keyvaluedata = string.Format("{0}", MyReg.GetValue("First Usage Time", "").ToString());
                    }
                }

                if (keyvaluedata.Length == 0)
                {
                    RegistryKey MyReg = Registry.LocalMachine.CreateSubKey(registryPath);

                    MyReg.SetValue("First Usage Time", dateTimenow);

                    string mac_address = Utils.GetMACAddress();
                    Console.WriteLine("Mac Address [ " + mac_address + " ]");
                    MyReg.SetValue("Mac Address", mac_address);

                    string computer_name = Utils.get_computer_name();
                    Console.WriteLine("Computer Name [ " + computer_name + " ]");
                    MyReg.SetValue("Computer Name", computer_name);

                    MyReg.Close();
                }

            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private bool WriteToCurrentUserRegisteryonAppClose(string _totalLoggedinTime)
        {
            try
            {
                string _totalusagetime = this.ReadFromCurrentUserRegisteryTotalUsageTime();
                string _lastusagetime = this.ReadFromCurrentUserRegisteryLastUsageTime();

                TimeSpan ts = TimeSpan.Parse(_lastusagetime);
                TimeSpan _tust = TimeSpan.Parse(_totalLoggedinTime);
                TimeSpan _tts = _tust + ts;

                this.DeleteCurrentUserRegistery();

                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                RegistryKey MyReg = Registry.CurrentUser.CreateSubKey(registryPath);
                MyReg.SetValue("Last Usage Time", _totalLoggedinTime);
                MyReg.SetValue("Total Usage Time", _tts);
                MyReg.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return false;
            }
        }
        private bool DeleteCurrentUserRegistery()
        {
            try
            {

                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    MyReg.DeleteValue("Last Usage Time");
                    MyReg.DeleteValue("Total Usage Time");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return false;
            }
        }
        private string ReadFromCurrentUserRegisteryEXP()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    string keyvaluedata = string.Format("{0}", MyReg.GetValue("Trial Period", TRIAL_PERIOD).ToString());
                    return keyvaluedata;
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private string ReadFromCurrentUserRegisteryStartDate()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    string keyvaluedata = string.Format("{0}", MyReg.GetValue("Launch Date", DateTime.Now.ToString()).ToString());
                    return keyvaluedata;
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private string ReadFromCurrentUserRegisteryTotalUsageTime()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    string keyvaluedata = string.Format("{0}", MyReg.GetValue("Total Usage Time", 0).ToString());
                    return keyvaluedata;
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private string ReadFromCurrentUserRegisteryLastUsageTime()
        {
            try
            {
                string registryPath = "SOFTWARE\\" + Application.CompanyName + "\\" + Application.ProductName + "\\" + Application.ProductVersion;
                using (RegistryKey MyReg = Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    string keyvaluedata = string.Format("{0}", MyReg.GetValue("Last Usage Time", 0).ToString());
                    return keyvaluedata;
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }









    }
}
