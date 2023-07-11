using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clear_junk_files_app
{
    /// <summary>
    /// Description of utilzsingleton.
    /// </summary>
    public sealed class utilzsingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property. 
        private static utilzsingleton singleInstance;

        public static utilzsingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new utilzsingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public string TAG;

        private utilzsingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            TAG = this.GetType().Name;
            _notificationmessageEventname = notificationmessageEventname;
        }

        private utilzsingleton()
        {

        }


        public string getappsettinggivenkey(string key = "", string defaultvalue = "")
        {
            try
            {

                string configvalue = "";

                configvalue = System.Configuration.ConfigurationManager.AppSettings[key];

                if (configvalue == null || String.IsNullOrEmpty(configvalue))
                {
                    return defaultvalue;
                }
                else
                {
                    return configvalue;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, this.TAG));
                return defaultvalue;
            }
        }

        public junk_file_dto buildweightdtogivendatatable(DataTable dt, int _index)
        {
            junk_file_dto _weight_record_dto = new junk_file_dto();
            _weight_record_dto.file_id = Convert.ToString(dt.Rows[_index][DBContract.junk_files_entity_table.FILE_ID]);
            _weight_record_dto.full_name = Convert.ToString(dt.Rows[_index][DBContract.junk_files_entity_table.FULL_NAME]);
            _weight_record_dto.size = Convert.ToString(dt.Rows[_index][DBContract.junk_files_entity_table.SIZE]);
            _weight_record_dto.extension = Convert.ToString(dt.Rows[_index][DBContract.junk_files_entity_table.EXTENSION]);
            _weight_record_dto.created_date = Convert.ToString(dt.Rows[_index][DBContract.junk_files_entity_table.CREATED_DATE]);

            return _weight_record_dto;
        }




    }
}
