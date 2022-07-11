using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MySqlConnector;

namespace FineNotes
{
    public class Session
    {
        readonly string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        NotesCollection notCol = NotesCollection.getInstance();
        private string email;
        private bool online;
        private bool modified;
        private static Session instance;
        public static Session getInstance()
        {
            if (instance == null)
                instance = new Session();
            return instance;
        }
        public Session()
        {
            email = "";
            online = false;
            modified = false;
        }
        public void getGroupElemsBd()
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string email_to_find = "'" + email + "'";
                string databaseTable = "GroupNotes";
                var query = "SELECT * FROM " + databaseTable + " WHERE Author = " + email_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Note note_temp;
                    while (reader.Read())
                    {
                        List<string> temp = new List<string>();
                        note_temp = new Note { Number = reader.GetInt32(0), Header = reader.GetString(2), Message = reader.GetString(3), Author = reader.GetString(1), Date = reader.GetString(4), Type = 1 };
                        notCol.Notes_group.Add(note_temp);
                    }
                }
                conn.Close();
                conn.Open();
                query = "SELECT GroupNotes.Number, GroupNotes.Author, GroupNotes.Header, GroupNotes.Message, GroupNotes.Date FROM " + databaseTable + " LEFT JOIN SharedUsers on GroupNotes.Number = SharedUsers.Number WHERE SharedUsers.Shared = " + email_to_find;
                cmd = new MySqlCommand(query, conn);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Note note_temp;
                    while (reader.Read())
                    {
                        note_temp = new Note { Number = reader.GetInt32(0), Header = reader.GetString(2), Message = reader.GetString(3), Author = reader.GetString(1), Date = reader.GetString(4), Type = 1 };
                        notCol.Notes_group.Add(note_temp);
                    }
                }
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
            }
        }
    
        public void getElemsFromDB()
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                notCol.ClearAll();
                string email_to_find = "'"+email+"'";
                string databaseTable = "Notes";
                var query = "SELECT * FROM " + databaseTable + " WHERE Author = "+email_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        List<string> temp = new List<string>();
                        notCol.addNote(reader.GetString(2),reader.GetString(3), reader.GetString(1), reader.GetString(4), 0);
                    }
                }
                conn.Close();
                getGroupElemsBd();
                notCol.JoinAllAndGroup();
                notCol.Save();
            }
            catch (Exception e)
            {
                conn.Close();
            }
        }
        public bool clearNotes()      //Удаление всех заметок по Id автора
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string email_to_find = "'" + email + "'";
                string databaseTable = "Notes";
                var query = "DELETE FROM " + databaseTable + " WHERE Author=" + email_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                conn.Close();
                return true;
              }
               catch (Exception exp)
            {
                conn.Close();
                return false;
            }
        }
        public bool insertAllNotes()    //Сохранение всех заметок пользователя в БД
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                bool first = false;
                string email_to_find = "" + email + "";
                string databaseTable = "Notes";
                var query = "INSERT INTO " + databaseTable + " VALUES ";
                notCol.Read();
                notCol.fillPrivateTemp(email);
                foreach (var elem in notCol.Notes_temp)
                {
                    if (elem.Type == 0)
                    {
                        if (!first)
                            first = true;
                        else
                        {
                            query += ",";
                        }
                        query += "(" + "'" + elem.Number.ToString() + "','"+elem.Author +"','" + elem.Header + "','" + elem.Message + "','" + elem.Date + "')";
                    }
                 }
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                conn.Close();
                return false;
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                }
            }
        }
        public bool Online
        {
            get { return online; }
            set
            {
                if (online != value)
                {
                    online = value;
                }
            }
        }
        public bool Modified
        {
            get { return modified; }
            set
            {
                if (modified != value)
                {
                    modified = value;
                }
            }
        }
        public void writeSession()         //Запись в файл информации о сессии
        {
                string filename = "Session.txt";
                if (String.IsNullOrEmpty(filename)) return;
            // перезаписываем файл
                string data = email + "/" + online + "/" + modified;
                File.WriteAllText(Path.Combine(folderPath, filename), data);
                // обновляем список файлов
        }
        public bool readSession()       //Чтение из файла информации о предыдущей сессии
        {
            string filename = "Session.txt";
            if (File.Exists(Path.Combine(folderPath, filename)))
            {
                string readText = File.ReadAllText(Path.Combine(folderPath, filename));
                string[] words = readText.Split('/');
                email = words[0];
                online = Convert.ToBoolean(words[1]);
                modified = Convert.ToBoolean(words[2]);
                return true;
            }
            else
                return false;
        }
    }
}

