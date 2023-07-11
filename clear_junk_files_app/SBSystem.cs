using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace clear_junk_files_app
{
    public class SBSystem
    {
        public string Name { get; set; }
        public string Application { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string AttachDB { get; set; }
        public string Metadata { get; set; }
        public string Version { get; set; }
        public bool Default { get; set; }

        public SBSystem(string name, string app, string database, string server, string attach, string metadata, string ver, bool def)
        {
            this.Name = name;
            this.Application = app;
            this.Database = database;
            this.Server = server;
            this.AttachDB = attach;
            this.Metadata = metadata;
            this.Version = ver;
            this.Default = def;
        }
    }

    public class SBSystem_Exp
    {
        public string Name { get; set; }
        public string Application { get; set; }

        public SBSystem_Exp(string name, string app)
        {
            this.Name = name;
            this.Application = app;
        }
    }

    public class SBSystem_DTO
    {
        public string Name { get; set; }
        public string Application { get; set; }

        public SBSystem_DTO(string name, string app)
        {
            this.Name = name;
            this.Application = app;
        }
    }

    public class SBSystem_DCP_DTO
    {
        public string Name { get; set; }
        public string Application { get; set; }
        public string Database { get; set; }
        public bool Defaultsn { get; set; }
        public bool Defaultun { get; set; }
        public bool Defaultpd { get; set; }

        public SBSystem_DCP_DTO(string name, string app, string database, bool defaultsn, bool defaultun, bool defaultpd)
        {
            this.Name = name;
            this.Application = app;
            this.Database = database;
            this.Defaultsn = defaultsn;
            this.Defaultun = defaultun;
            this.Defaultpd = defaultpd;
        }
    }

    public class SB_Login
    {
        public string system { get; set; }
        public string serverName { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string IntegratedSecurity { get; set; }
        public string remember { get; set; }

        public SB_Login(string system, string serverName, string databaseName, string userName, string password, string IntegratedSecurity, string remember)
        {
            this.system = system;
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            this.password = password;
            this.IntegratedSecurity = IntegratedSecurity;
            this.remember = remember;
        }
    }

    public class SB_Server_Login
    {
        public string system { get; set; }
        public string serverName { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string IntegratedSecurity { get; set; }
        public string remember_username { get; set; }
        public string remember_password { get; set; }
        public string remember_servername { get; set; }

        public SB_Server_Login(string system, string serverName, string databaseName, string userName, string password, string IntegratedSecurity, string remember_username, string remember_password, string remember_servername)
        {
            this.system = system;
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            this.password = password;
            this.IntegratedSecurity = IntegratedSecurity;
            this.remember_username = remember_username;
            this.remember_password = remember_password;
            this.remember_servername = remember_servername;
        }
    }

    public class ServerDatabase
    {
        public SBSystem System;
        public double Size;
        public ServerDatabase(SBSystem system, double sz)
        {
            this.System = system;
            this.Size = sz;
        }
    }
    public class saved_directories
    {
        public string name { get; set; }
        public saved_directories(string _name)
        {
            this.name = _name; 
        }
    }





}
