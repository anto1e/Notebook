using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class ModifyPage : ContentPage
    {
        List<string> users;
        Session session = Session.getInstance();
        NotesCollection notCol = NotesCollection.getInstance();
        private int number;
        private int type;
        private string author="";
        bool loaded = false;
        public ModifyPage(Note note)
        {
            InitializeComponent();
            type = note.Type;
            author = note.Author;
            number = note.Number;
            if (note.Author != session.Email || !session.Online)
                SidebarBtn.IsVisible = false;
            if (type==1 && !session.Online)
            {
                saveBtn.IsVisible = false;
                cancelBtn.IsVisible = false;
            }
            if (type==1 && session.Email!= note.Author)
            {
                cancelBtn.IsVisible = false;
            }
            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Padding = new Thickness(0, 35, 0, 0);
            }
            if (type == 1 && session.Online)
            {
                string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    string note_to_find = "'" + number * -1 + "'";
                    string databaseTable = "GroupNotes";
                    var query = "SELECT Author, Header, Message, Date FROM " + databaseTable + " WHERE Number = " + note_to_find;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            author = reader.GetString(0);
                            Note_header.Text = reader.GetString(1);
                            Note_msg.Text = reader.GetString(2);
                            Date_label.Text = reader.GetString(3);
                        }
                    }
                    loaded = true;
                    conn.Close();
                }
                catch (Exception e)
                {
                    conn.Close();
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Note_header.Text = note.Header;
                Note_msg.Text = note.Message;
                Date_label.Text = note.Date;
            }
                Email_label.Text = author;
        }
        private bool side_hided = true;
        private List<string> getSharedUsers()           //Получение списка расшареных пользователей из БД для данной заметки
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                List<string> temp = new List<string>();
                string note_to_find = "'" + number*-1 + "'";
                string databaseTable = "SharedUsers";
                var query = "SELECT Shared FROM " + databaseTable + " WHERE Number = " + note_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        temp.Add(reader.GetString(0));
                    }
                }
                conn.Close();
                return temp;
            }
            catch (Exception e)
            {
                conn.Close();
                DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                Console.WriteLine(e.Message);
                session.Online = false;
                return null;
            }
        }
        private async void showSidebarClicked(object sender, EventArgs e)   //Функция показа / скрытия сайдбара
        {
            users = getSharedUsers();
            var template = new DataTemplate(typeof(TextCell));
            template.SetValue(TextCell.TextColorProperty, Color.FromHex("FFEEB1"));
            usersList.ItemTemplate = template;
            template.SetBinding(TextCell.TextProperty, ".");
            if (side_hided)
            {
                usersList.ItemsSource = users;
                Sidebar.IsVisible = true;
                await Sidebar.TranslateTo(0, 0, 250);
                side_hided = false;
            }
            else
            {
                await Sidebar.TranslateTo(500, 0, 250);
                Sidebar.IsVisible = false;
                side_hided = true;
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e)       //Нажата кнопка домой
        {
            if (loaded)
            {
                var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
                if (note != null)
                {
                    note.Header = Note_header.Text;
                    note.Message = Note_msg.Text;
                    note.Date = DateTime.Now.ToString();
                    notCol.Save();
                    session.Modified = true;
                }
                //MessagingCenter.Send<Page>(this, "Group Changed!");
                MessagingCenter.Send<Page>(this, "CollectionChanged!");
            }
            await Navigation.PopAsync();
        }
        private bool hided = true;
        private void ShowBtnsClicked(object sender, EventArgs e) //Функция скрывания/показа кнопок удаления и изменения
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
            }
        }
        private async void NoteChangeClicked(object sender, EventArgs e)      //Функция сохранения изменений в заметке
        {
            if (type == 1)
            {
                string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    string databaseTable = "GroupNotes";
                    string note_to_change = "'" + number + "'";
                    var query = "UPDATE " + databaseTable + " SET Header = " + "'" + Note_header.Text.ToString() + "',Message = '" + Note_msg.Text.ToString() + "',Date = '" + DateTime.Now.ToString() + "' WHERE Number = '" + number*-1 + "'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();
                    session.Modified = true;
                    conn.Close();
                    MessagingCenter.Send<Page>(this, "Group Changed!");
                    MessagingCenter.Send<Page>(this, "CollectionChanged!");
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Console.WriteLine(ex.Message);
                    await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                    session.Online = false;
                    return;
                }
            }
            else
            {
                var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
                if (note != null)
                {
                    note.Header = Note_header.Text;
                    note.Message = Note_msg.Text;
                    note.Date = DateTime.Now.ToString();
                    notCol.Save();
                    session.Modified = true;
                }
                MessagingCenter.Send<Page>(this, "CollectionChanged!");
            }
        }
        private async void NoteDeleteClicked(object sender, EventArgs e)        //Функция удаления заметки
        {
            string action = await DisplayActionSheet("Вы действительно хотите удалить заметку?", "Отмена", "Удалить");
            if (action == "Удалить")
            {
                if (type == 1)
                {
                    if (author == session.Email)
                    {
                        string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                        MySqlConnection conn = new MySqlConnection(connStr);
                        try
                        {
                            conn.Open();
                            string databaseTable = "GroupNotes";
                            string note_to_change = "'" + number + "'";
                            string query = "DELETE FROM " + databaseTable + " WHERE Number = " + "'" + number * -1 + "'";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            var result = cmd.ExecuteScalar();
                            MessagingCenter.Send<Page>(this, "Group Changed!");
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            Console.WriteLine(ex.Message);
                            await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                            session.Online = false;
                            return;
                        }
                    }
                }
                else
                {
                    var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
                    notCol.Notes.Remove(note);
                    notCol.change_indexes();
                    notCol.Save();
                }

                session.Modified = true;
                MessagingCenter.Send<Page>(this, "CollectionChanged!");
                MessagingCenter.Send<Page>(this, "Show Toolbar!");
                await Navigation.PopAsync();
            }
        }
        void editor_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (DeviceInfo.Platform.Equals(DevicePlatform.iOS))
            {
                scrollView.ScrollToAsync(scrollView.ScrollX, scrollView.ScrollY, true);
            }
        }
        private async void SharedUserSelected(object sender, SelectedItemChangedEventArgs e)      //Функция удаления расшаренного пользователя
        {
            if (e.SelectedItem != null)
            {
                string user_to_del = "'"+e.SelectedItem.ToString()+"'";
                string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    string databaseTable = "SharedUsers";
                    var query = "DELETE FROM " + databaseTable + " WHERE Shared=" + user_to_del + " AND Number = " + "'"+number*-1+"'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    users.Remove(e.SelectedItem.ToString());
                    usersList.ItemsSource = null;
                    usersList.ItemsSource = users;
                    if (users.Count() == 0)
                    {
                        try
                        {
                            conn.Open();
                            query = "DELETE FROM GroupNotes WHERE Number=" + number * -1;
                            cmd = new MySqlCommand(query, conn);
                            var result1 = cmd.ExecuteScalar();
                            type = 0;
                            notCol.addNote(Note_header.Text, Note_msg.Text, session.Email, DateTime.Now.ToString(), type);
                            number = notCol.Notes.Count() + 1;
                            notCol.Save();
                            MessagingCenter.Send<Page>(this, "Group Changed!");
                            MessagingCenter.Send<Page>(this, "CollectionChanged!");
                        }
                        catch(Exception ex)
                        {
                            conn.Close();
                            Console.WriteLine(ex.Message);
                            await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                            session.Online = false;
                            return;
                        }
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                    session.Online = false;
                    return;
                }
            }
        }
        private async void AddSharedUser(object sender, EventArgs e)      //Функция добавления расшареного пользователя
        {
            string user_to_add = email_shared.Text.ToString();
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "SharedUsers";
                if (isUserExists(user_to_add))
                {
                    if (type == 0)
                    {
                        try
                        {
                            string databaseNotes = "GroupNotes";
                            var query1 = "INSERT INTO " + databaseNotes + "(Author,Header,Message,Date) VALUES ('" + session.Email + "','" + Note_header.Text.ToString() + "','" + Note_msg.Text + "','" + DateTime.Now.ToString() + "')";
                            MySqlCommand cmd1 = new MySqlCommand(query1, conn);
                            var result1 = cmd1.ExecuteScalar();
                            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
                            notCol.Notes.Remove(note);
                            session.Modified = true;
                            type = 1;
                            number = Convert.ToInt32(cmd1.LastInsertedId) * -1;
                            notCol.change_indexes();
                            notCol.Save();
                            MessagingCenter.Send<Page>(this, "Group Changed!");
                            MessagingCenter.Send<Page>(this, "CollectionChanged!");
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            Console.WriteLine(ex.Message);
                            await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                            session.Online = false;
                            return;
                        }
                    }
                        var query = "INSERT INTO " + databaseTable + " VALUES (" + "'" + number * -1 + "','" + user_to_add + "'" + ")";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        var result = cmd.ExecuteScalar();
                    var temp = result;
                        conn.Close();
                        users.Add(user_to_add);
                        usersList.ItemsSource = null;
                        usersList.ItemsSource = users;
                }
                else
                    await DisplayAlert("Ошибка", "Пользователь с таким Email не зарегистрирован", "ОK");
            }
            catch (MySqlException ex)
            {
                if (ex.ErrorCode.ToString() == "DuplicateKeyEntry")
                {
                    await DisplayAlert("Ошибка", "Пользователь уже добавлен", "ОK");
                }
                else
                {
                    await DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                    session.Online = false;
                }
                conn.Close();
                return;
            }
        }
        private bool isUserExists(string user_to_add)     //Функция проверки существования пользователя
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "Users";
                var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + "'"+user_to_add+"'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                conn.Close();
                email_shared.Text = "";
                if (result != null && user_to_add!=session.Email)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                conn.Close();
                Console.WriteLine(e.Message);
                DisplayAlert("Нет подключения", "Нет подключения к Базе Данных, вы теперь в офлайн режиме", "ОK");
                session.Online = false;
                return false;
            }
        }
    }
}   

