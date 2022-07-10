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
        public int getIdFromDB(string email_find)       //Поиск Id автора по Email
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "Users";
                string email_str = "'" + email_find + "'";
                var query = "SELECT Id FROM " + databaseTable + " WHERE Email=" + email_str;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                conn.Close();
                return Convert.ToInt32(result);
            }
            catch (Exception e)
            {
                conn.Close();
                return -1;
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
                int id_to_find = getIdFromDB(email);
                string databaseTable = "Notes";
                string Id = "'" + id_to_find.ToString() + "'";
                var query = "SELECT Number,Header,Message,Date FROM " + databaseTable + " WHERE id_author = "+id_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Note note_temp;
                    while (reader.Read())
                    {
                        List<string> temp = new List<string>();
                        note_temp = new Note { Number = reader.GetInt32(0), Header = reader.GetString(1), Message = reader.GetString(2), Author = email, Date = reader.GetString(3), Allowers = temp };
                        notCol.Notes.Add(note_temp);
                    }
                }
                notCol.Save();
                conn.Close();
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
                int Id_to_del = getIdFromDB(email);
                string databaseTable = "Notes";
                string Id = "'"+Id_to_del.ToString()+"'";
                var query = "DELETE FROM " + databaseTable + " WHERE id_author=" + Id;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                conn.Close();
                return true;
              }
               catch (Exception e)
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
                int id_to_insert = getIdFromDB(email);
                string databaseTable = "Notes";
                string Id = "'" + id_to_insert.ToString() + "'";
                var query = "INSERT INTO " + databaseTable + " VALUES ";
                notCol.Read();
                notCol.fillPrivateTemp(email);
                foreach (var elem in notCol.Notes_temp)
                {
                    if (!first)
                        first = true;
                    else
                    {
                        query += ",";
                    }
                    query += "(" + "'" + elem.Number.ToString() + "','" + id_to_insert.ToString() + "','" + elem.Header + "','" + elem.Message + "','" + elem.Date + "')";
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

