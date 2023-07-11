using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clear_junk_files_app
{
    public static class DBContract
    {
        public static String DATABASE_NAME = "junk_files_db";
        public static String SQLITE_DATABASE_NAME = "junk_files_db.sqlite3";

        public static String error = "error";
        public static String info = "info";
        public static String warn = "warn";

        public static String mssql = "mssql";
        public static String mysql = "mysql";
        public static String sqlite = "sqlite";
        public static String postgresql = "postgresql";

        public static String JUNK_FILES_SELECT_ALL_QUERY = "SELECT * FROM " + DBContract.junk_files_entity_table.TABLE_NAME;


        //junk files table
        public static class junk_files_entity_table
        {
            public static String TABLE_NAME = "tbl_junk_files";
            //Columns of the table
            public static String FILE_ID = "file_id";
            public static String FULL_NAME = "full_name";
            public static String SIZE = "size";
            public static String EXTENSION = "extension";
            public static String CREATED_DATE = "created_date";
        }




    }
}
