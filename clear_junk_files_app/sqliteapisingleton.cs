using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace clear_junk_files_app
{
    public sealed class sqliteapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static sqliteapisingleton singleInstance;

        public static sqliteapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new sqliteapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private string CONNECTION_STRING = @"Data Source=junk_files_db.sqlite3;Pooling=true;FailIfMissing=false";
        private const string db_name = "junk_files_db";//DBContract.DATABASE_NAME;
        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private event EventHandler<notificationmessageEventArgs> _databaseutilsnotificationeventname;
        private string TAG;

        // Holds our connection with the database
        SQLiteConnection m_dbConnection;

        private sqliteapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
            try
            {
                TAG = this.GetType().Name;
                createdatabaseonfirstload();
                createtablesonfirstload();
                createconnectionstring();
                setconnectionstring();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private sqliteapisingleton()
        {

        }
        public responsedto setup_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                responsedto _db_responsedto = createdatabaseonfirstload();
                responsedto _table_responsedto = createtablesonfirstload();
                createconnectionstring();
                setconnectionstring();

                if (_db_responsedto.responsesuccessmessage != null && _db_responsedto.isresponseresultsuccessful)
                {
                    _responsedto.responsesuccessmessage += (Environment.NewLine + _db_responsedto.responsesuccessmessage);
                }
                else if (_db_responsedto.responseerrormessage != null)
                {
                    _responsedto.responseerrormessage += (Environment.NewLine + _db_responsedto.responseerrormessage);
                }

                if (_table_responsedto.responsesuccessmessage != null && _table_responsedto.isresponseresultsuccessful)
                {
                    _responsedto.responsesuccessmessage += (Environment.NewLine + _table_responsedto.responsesuccessmessage);
                }
                else if (_table_responsedto.responseerrormessage != null)
                {
                    _responsedto.responseerrormessage += (Environment.NewLine + _table_responsedto.responseerrormessage);
                }

            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage += ex.ToString();
            }

            return _responsedto;
        }
        private void createconnectionstring()
        {
            try
            {
                CONNECTION_STRING = setconnectionstring();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private string setconnectionstring()
        {
            try
            {
                sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

                _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
                _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
                _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
                _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

                CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                return CONNECTION_STRING;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return "";
            }
        }

        string buildconnectionstringfromobject(sqliteconnectionstringdto _connectionstringdto)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

            string plain_dbname = _connectionstringdto.database;
            string database_version = _connectionstringdto.sqlite_version;
            string db_extension = _connectionstringdto.sqlite_db_extension;
            string dbname = plain_dbname + "." + db_extension + database_version;

            if (!Directory.Exists(database_dir))
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] does not exist.", TAG));
            }
            else
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + _connectionstringdto.sqlite_database_path + " ] exist.", TAG));
            }

            string full_database_name_with_path = Path.Combine(database_dir, dbname);
            string _secure_path_name_response = dbname;

            if (!File.Exists(full_database_name_with_path))
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] does not exist.", TAG));
            }
            else
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] exist.", TAG));
            }

            string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
            "Version=" + _connectionstringdto.sqlite_version + ";" +
            "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
            "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

            return CONNECTION_STRING;
        }

        // Creates a connection with our database file.
        public void connectToDatabase()
        {
            try
            {
                CONNECTION_STRING = setconnectionstring();
                m_dbConnection = new SQLiteConnection(CONNECTION_STRING);
                m_dbConnection.Open();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private responsedto createdatabaseonfirstload()
        {
            sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

            _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
            _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
            _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
            _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

            responsedto _responsedto = createdatabasegivenname(_connectionstringdto);
            return _responsedto;
        }
        private responsedto createtablesonfirstload()
        {
            sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

            _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
            _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
            _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
            _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

            responsedto _responsedto = createtables(_connectionstringdto);
            return _responsedto;
        }
        public responsedto createdatabasegivenname(sqliteconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string base_dir = AppDomain.CurrentDomain.BaseDirectory;
                string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

                string new_database_name = _connectionstringdto.new_database_name;
                string database_version = _connectionstringdto.sqlite_version;
                string db_extension = _connectionstringdto.sqlite_db_extension;
                string dbname = new_database_name + "." + db_extension + database_version;

                if (!Directory.Exists(database_dir))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + database_dir + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating path...";

                    Directory.CreateDirectory(database_dir);

                    _responsedto.responsesuccessmessage += "\ncreated sqlite datastore path with name [ " + database_dir + " ].";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + _connectionstringdto.sqlite_database_path + " ] exist.";
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + _secure_path_name_response + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating database...";

                    SQLiteConnection.CreateFile(full_database_name_with_path);

                    _responsedto.responsesuccessmessage += "\nsuccessfully created database [ " + _secure_path_name_response + " ] in sqlite.";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + _secure_path_name_response + " ] exist.";
                }

                _responsedto.isresponseresultsuccessful = true;
                return _responsedto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto createdatabase()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string _default_db_path = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_database_path", @"\databases\");
                string dbname = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_database", "ntharenedb");
                string database_version = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_version", "3");
                string db_extension = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_db_extension", "sqlite");
                dbname = dbname + "." + db_extension + database_version;

                string base_dir = AppDomain.CurrentDomain.BaseDirectory;

                string database_dir = Path.Combine(base_dir, _default_db_path);


                if (!Directory.Exists(database_dir))
                {
                    Directory.CreateDirectory(database_dir);
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    SQLiteConnection.CreateFile(full_database_name_with_path);
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "successfully created database [ " + _secure_path_name_response + " ] in sqlite.";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite datastore with name [ " + _secure_path_name_response + " ] exists.";
                    return _responsedto;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto createtables(sqliteconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            responsedto _innerresponsedto = new responsedto();
            try
            {

                _connectionstringdto.database = _connectionstringdto.new_database_name;
                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                bool does_table_exist_in_db = checkiftableexists(CONNECTION_STRING, DBContract.junk_files_entity_table.TABLE_NAME);

                //Create the table 
                string SQL_CREATE_CROPS_TABLE = " CREATE TABLE IF NOT EXISTS " + DBContract.junk_files_entity_table.TABLE_NAME + " (" +
                      DBContract.junk_files_entity_table.FILE_ID + " INTEGER PRIMARY KEY, " +
                      DBContract.junk_files_entity_table.FULL_NAME + " TEXT, " +
                      DBContract.junk_files_entity_table.SIZE + " TEXT, " +
                      DBContract.junk_files_entity_table.EXTENSION + " TEXT, " +
                      DBContract.junk_files_entity_table.CREATED_DATE + " TEXT " +
                       " ); ";

                _innerresponsedto = createtable(SQL_CREATE_CROPS_TABLE, CONNECTION_STRING);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                string successmsg = "successfully created tables in database [ " + _connectionstringdto.database + " ] - server [ " + DBContract.sqlite + " ].";
                int msg_length = successmsg.Length;
                msg_length = msg_length + 1;
                int stars_printed = 0;
                string str_stars = "";
                string str_newline = Environment.NewLine;

                while (stars_printed != msg_length)
                {
                    str_stars += "*";
                    ++stars_printed;
                }

                _responsedto.responsesuccessmessage += str_newline;
                _responsedto.responsesuccessmessage += str_stars;
                _responsedto.responsesuccessmessage += str_newline;
                _responsedto.responsesuccessmessage += successmsg;
                _responsedto.responsesuccessmessage += str_newline;
                _responsedto.responsesuccessmessage += str_newline;
                _responsedto.responsesuccessmessage += str_stars;

                _responsedto.isresponseresultsuccessful = true;
                return _responsedto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage += Environment.NewLine + ex.Message;
                return _responsedto;
            }
        }

        bool checkiftableexists(string CONNECTION_STRING, string table_name)
        {
            try
            {
                string query = "SELECT EXISTS (SELECT name FROM sqlite_master WHERE type='table' AND name = '" + table_name + "')";

                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        //execute the query  
                        int _rows_affected = cmd.ExecuteNonQuery();
                        Console.WriteLine("_rows_affected [ " + _rows_affected + " ]");

                        var da = new SQLiteDataAdapter(cmd);
                        var dt = new DataTable();
                        da.Fill(dt);
                        da.Dispose();

                        int _rows_count = dt.Rows.Count;
                        if (_rows_count > 0) return true;
                        else return false;
                    }
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                return false;
            }
        }

        public responsedto createtable(string query, string CONNECTION_STRING)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;
                        string[] _conn_arr = CONNECTION_STRING.Split(new char[] { ';' });
                        _conn_arr.SetValue("", _conn_arr.Length - 1);
                        _conn_arr.SetValue("", _conn_arr.Length - 2);
                        string _sanitized_conn_arr = _conn_arr[0] + ";" + _conn_arr[1];
                        _responsedto.responsesuccessmessage += "successfully executed query [ " + query + " ] against connection [ " + _sanitized_conn_arr + " ]." + Environment.NewLine;
                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage += Environment.NewLine + "error executing query [ " + query + " ].";
                _responsedto.responseerrormessage += Environment.NewLine + Environment.NewLine + ex.Message + Environment.NewLine;
                return _responsedto;
            }
        }

        public responsedto checksqliteconnectionstate()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                sqliteconnectionstringdto _connectionstringdto = getsqliteconnectionstringdto();

                _responsedto = checkconnectionasadmin(_connectionstringdto, _databaseutilsnotificationeventname);

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public sqliteconnectionstringdto getsqliteconnectionstringdto()
        {
            try
            {
                sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

                _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
                _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
                _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
                _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        public responsedto checkconnectionasadmin(sqliteconnectionstringdto _connectionstringdto, EventHandler<notificationmessageEventArgs> databaseutilsnotificationeventname)
        {
            _databaseutilsnotificationeventname = databaseutilsnotificationeventname;
            responsedto _responsedto = new responsedto();
            try
            {
                string base_dir = Environment.CurrentDirectory;
                string database_dir = base_dir + _connectionstringdto.sqlite_database_path;
                string dbname = _connectionstringdto.database;
                string database_version = _connectionstringdto.sqlite_version;
                string db_extension = _connectionstringdto.sqlite_db_extension;
                dbname = dbname + "." + db_extension + database_version;

                if (!Directory.Exists(database_dir))
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite datastore path with name [ " + database_dir + " ] does not exist.";
                    return _responsedto;
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + _connectionstringdto.sqlite_database_path + " ] exist.", TAG));

                    //_databaseutilsnotificationeventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + _connectionstringdto.sqlite_database_path + " ] exist.", TAG));
                }

                string full_database_name_with_path = database_dir + dbname;
                string _secure_path_name_response = _connectionstringdto.sqlite_database_path + dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite database with name [ " + _secure_path_name_response + " ] does not exist.";
                    return _responsedto;
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] exist.", TAG));

                    //_databaseutilsnotificationeventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] exist.", TAG));
                }

                string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
                "Version=" + _connectionstringdto.sqlite_version + ";" +
                "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
                "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

                string query = DBContract.JUNK_FILES_SELECT_ALL_QUERY;

                int count = getrecordscountgiventable(DBContract.junk_files_entity_table.TABLE_NAME, CONNECTION_STRING);

                if (count != -1)
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "connection to sqlite successfull. crops count [ " + count + " ].";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "connection to sqlite failed.";
                    return _responsedto;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public bool isdbconnectionalive(string CONNECTION_STRING)
        {
            var con = new SQLiteConnection(CONNECTION_STRING);
            con.Open();
            return true;
        }

        public bool isdbconnectionalive()
        {
            try
            {
                //setup the connection to the database
                var con = new SQLiteConnection(CONNECTION_STRING);
                con.Open();
                return true;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return false;
            }

        }

        public int getrecordscountgiventable(string tablename, string CONNECTION_STRING)
        {
            string query = "SELECT * FROM " + tablename;
            DataTable dt = getallrecordsglobal(query, CONNECTION_STRING);
            if (dt != null)
                return dt.Rows.Count;
            else
                return -1;
        }

        public DataTable getallrecordsglobal(string query)
        {
            if (!isdbconnectionalive()) return null;

            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        public DataTable getallrecordsglobal(string query, string CONNECTION_STRING)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }
        public DataTable getallrecordsglobal()
        {
            DataTable dt = getallrecordsglobal(DBContract.JUNK_FILES_SELECT_ALL_QUERY);
            return dt;
        }

        public List<junk_file_dto> get_all_junk_files()
        {
            List<junk_file_dto> lst_junk_files = new List<junk_file_dto>();
            DataTable dt = getallrecordsglobal();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                junk_file_dto junk_file_dto = utilzsingleton.getInstance(_notificationmessageEventname).buildweightdtogivendatatable(dt, i);
                lst_junk_files.Add(junk_file_dto);
            }
            return lst_junk_files;
        }

        public int insertgeneric(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;
            //setup the connection to the database
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }

        public int insertgeneric(string query, Dictionary<string, object> args, string CONNECTION_STRING)
        {
            int numberOfRowsAffected;
            if (CONNECTION_STRING == null)
            {
                numberOfRowsAffected = insertgeneric(query, args);
                return numberOfRowsAffected;
            }
            else if (String.IsNullOrEmpty(CONNECTION_STRING))
            {
                numberOfRowsAffected = insertgeneric(query, args);
                return numberOfRowsAffected;
            }
            else
            {

                //setup the connection to the database
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        //set the arguments given in the query
                        foreach (var pair in args)
                        {
                            cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        //execute the query and get the number of row affected
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                    return numberOfRowsAffected;
                }
            }
        }

        public responsedto create_junk_file_in_database(junk_file_dto junk_file_dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string query = "INSERT INTO " +
                DBContract.junk_files_entity_table.TABLE_NAME +
                " ( " +
                DBContract.junk_files_entity_table.FULL_NAME + ", " +
                DBContract.junk_files_entity_table.SIZE + ", " +
                DBContract.junk_files_entity_table.EXTENSION + ", " +
                DBContract.junk_files_entity_table.CREATED_DATE +
                " ) VALUES (@full_name, @size, @extension, @created_date)";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
			    {
				    {"@full_name", junk_file_dto.full_name},
				    {"@size", junk_file_dto.size},
                    {"@extension", junk_file_dto.extension},
				    {"@created_date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")}
                    
			    };

                int numberOfRowsAffected = insertgeneric(query, args, CONNECTION_STRING);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "Record creation failed in " + DBContract.sqlite + ".";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.sqlite + ".";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }










    }
}
