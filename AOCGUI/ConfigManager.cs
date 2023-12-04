using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Collections.ObjectModel;

namespace AOCGUI;

public static class ConfigManager
{
    private static object _lock = new object();
    //root path
    private static string? _rootPath = null;
    public static string RootPath { get{
        lock(_lock){
            if(_rootPath == null){
                _rootPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            return _rootPath!;
        }
    } set{
        lock(_lock){
            _rootPath = value;
        }
    }
    }

    //config file path
    private const string _configPath = "config.db";
    public static string ConfigPath=>System.IO.Path.Combine(RootPath, _configPath);
    //config file connection
    private static SQLiteConnection? _configConnection = null;

    //queries
    private const string _MakeTableQuery = @"CREATE TABLE IF NOT EXISTS [@Table] (
        [key] TEXT NOT NULL PRIMARY KEY,
        [value] TEXT NOT NULL
    );";



    //general variables
    private static bool _finishedStartup = false;
    private static readonly ReadOnlyCollection<string> _tableNames = new string[]{
        "config",
        "Tests"
    }.AsReadOnly();

    //init
    public static void Initialize(){
        Console.WriteLine("Initializing ConfigManager");
        lock(_lock){
            if(_finishedStartup)
                return;
            //create root directory if it doesn't exist
            if(!System.IO.Directory.Exists(RootPath)){
                System.IO.Directory.CreateDirectory(RootPath);
            }
            
            //create config file if it doesn't exist
            if(!System.IO.File.Exists(ConfigPath)){
                SQLiteConnection.CreateFile(ConfigPath);
            }
            //open connection
            _configConnection = new SQLiteConnection($"Data Source={ConfigPath};Version=3;");
            _configConnection.Open();
            //create tables if they don't exist
            using var cmd = new SQLiteCommand(_configConnection);
                
                //create config table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS [config] (
        [key] TEXT NOT NULL PRIMARY KEY,
        [value] TEXT NOT NULL
    );";
                cmd.ExecuteNonQuery();

                //create tests table
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS [Tests] (
        [key] INTEGER NOT NULL PRIMARY KEY,
        [dayID] INTEGER,
        [test_input] TEXT,
        [test_output] TEXT
    );";
                cmd.ExecuteNonQuery();
            _finishedStartup = true;
    }
    Console.WriteLine("ConfigManager initialized");
    }

    public static void Shutdown(){
        lock(_lock){
            if(!_finishedStartup)
                return;
            _configConnection?.Close();
            _configConnection?.Dispose();
            _configConnection = null;
            _finishedStartup = false;
        }
    }

    public static void Reset(){
        lock(_lock){
            Shutdown();
            System.IO.File.Delete(ConfigPath);
            Initialize();
        }
    }

    //helper functions
    private static bool _IsInitialized(){
        lock(_lock){
            return _finishedStartup;
        }
    }
    private static bool _IsTable(string table){
            return _tableNames.Contains(table);
    }
    
    private static void _QueryGuard(string table = "config"){
            if(!_IsInitialized())
                throw new Exception("ConfigManager not initialized");
            if(!_IsTable(table))
                throw new Exception($"Table {table} does not exist");
    }
    //DB functions
    private static string? Get(string key, string table = "config"){
            _QueryGuard(table);
            using var cmd = new SQLiteCommand(_configConnection);
            cmd.CommandText = $"SELECT [value] FROM [{table}] WHERE [key] = @Key;";
            cmd.Parameters.AddWithValue("@Key", key);
            using var reader = cmd.ExecuteReader();
            if(reader.Read()){
                return reader.GetString(0);
            }
            return null;
    }
    private static void Set(string key, string value, string table = "config"){
            _QueryGuard(table);
            using var cmd = new SQLiteCommand(_configConnection);
            cmd.CommandText = $"INSERT OR REPLACE INTO [{table}] ([key], [value]) VALUES (@Key, @Value);";
            cmd.Parameters.AddWithValue("@Key", key);
            cmd.Parameters.AddWithValue("@Value", value);
            cmd.ExecuteNonQuery();
    }
    private static void Delete(string key, string table = "config"){
            _QueryGuard(table);
            using var cmd = new SQLiteCommand(_configConnection);
            cmd.CommandText = $"DELETE FROM [{table}] WHERE [key] = @Key;";
            cmd.Parameters.AddWithValue("@Key", key);
            cmd.ExecuteNonQuery();
    }

    //config functions
    public static string GetConfig(string key){
        return Get(key, "config") ?? "";
    }
    public static void SetConfig(string key, string value){
        Set(key, value, "config");
    }
    public static void DeleteConfig(string key){
        Delete(key, "config");
    }
    public static void DeleteAllConfig(){
            _QueryGuard("config");
            using var cmd = new SQLiteCommand(_configConnection);
            cmd.CommandText = $"DELETE FROM [config];";
            cmd.ExecuteNonQuery();
    }

    //day functions
    public static Data.Test[] GetDayTests(int day){
        _QueryGuard("Tests");
        using var cmd = new SQLiteCommand(_configConnection);
        cmd.CommandText = $"SELECT [test_input], [test_output] FROM [Tests] WHERE [dayID] = @Day;";
        cmd.Parameters.AddWithValue("@Day", day);
        using var reader = cmd.ExecuteReader();
        var tests = new List<Data.Test>();
        while(reader.Read()){
            tests.Add(new Data.Test(reader.GetString(0), reader.GetString(1)));
        }
        return tests.ToArray();
    }
    public static void SetDayTests(int day, AOCGUI.Data.Test[] tests){
        _QueryGuard("Tests");
        using var cmd = new SQLiteCommand(_configConnection);
        cmd.CommandText = $"DELETE FROM [Tests] WHERE [dayID] = @Day;";
        cmd.Parameters.AddWithValue("@Day", day);
        cmd.ExecuteNonQuery();
        cmd.CommandText = $"INSERT INTO [Tests] ([dayID], [test_input], [test_output]) VALUES (@Day, @Input, @Output);";
        cmd.Parameters.AddWithValue("@Input", "");
        cmd.Parameters.AddWithValue("@Output", "");
        foreach(var test in tests){
            cmd.Parameters["@Input"].Value = test.Input;
            cmd.Parameters["@Output"].Value = test.ExpectedOutput;
            cmd.ExecuteNonQuery();
        }
    }
}
