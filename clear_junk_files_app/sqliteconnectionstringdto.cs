/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 09/27/2018
 * Time: 07:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace clear_junk_files_app
{
    public class sqliteconnectionstringdto : connectionstringdto
    {
        public string sqlite_database_path { get; set; }
        public string sqlite_version { get; set; }
        public string sqlite_pooling { get; set; }
        public string sqlite_fail_if_missing { get; set; }
        public string sqlite_db_extension { get; set; }
    }
}
