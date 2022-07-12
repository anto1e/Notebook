using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MySqlConnector;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace FineNotes
{
    public partial class LoginPage : ContentPage
    {
        Session session = Session.getInstance();
        public LoginPage()
        {
            InitializeComponent();
        }
        private bool hided = true;
        private void ShowBtnsClicked(object sender, EventArgs e)    //Функция показа кнопок
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
                Email_entry.Text = "";
                Password_entry.Text = "";
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
                Email_entry_reg.Text = "";
                Password_entry_reg.Text = "";
                RePassword_entry_reg.Text = "";
            }
        }
        private string hashing(string password)     //Хэширование паролея
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        private bool isReg = false;
        private async void RegSignBtnClicked(object sender, EventArgs e)        //Нажата кнопка смены Входа/Регистрации
        {
            if (!isReg)
            {
                await SignInLayout.TranslateTo(-500, 0, 200);
                SignInLayout.IsVisible = false;
                RegisterLayout.IsVisible = true;
                await RegisterLayout.TranslateTo(0, 0, 200);
                isReg = true;
                SignBtn.IsVisible = false;
                RegBtn.IsVisible = true;
            }
            else
            {
                await RegisterLayout.TranslateTo(500, 0, 200);
                RegisterLayout.IsVisible = false;
                SignInLayout.IsVisible = true;
                await SignInLayout.TranslateTo(0, 0, 200);
                isReg = false;
                RegBtn.IsVisible = false;
                SignBtn.IsVisible = true;
            }
        }
        bool IsValidEmail(string email) //Проверка email
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        private async void OfflineClicked (object sender, EventArgs e)          //Нажата кнопку Оффлайн режима
        {
            if (session.readSession())
            {
                session.Online = false;
                await Navigation.PushAsync(new MainPage());
            }
            else
                await DisplayAlert("Нет данных", "Нет данных о прошлом входе", "ОK");
        }
        private async void LoginButton_Clicked(object sender, EventArgs e)            //Нажата кнопка входа
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "Users";
                string email="";
                string email_ses = "";
                if (!isReg)
                {
                    email = Email_entry.Text;
                    email_ses = "\"" + email + "\"";
                    if (!IsValidEmail(email))
                    {
                        conn.Close();
                        error_label.Text = "Неверный email!";
                        return;
                    }
                    else
                    {
                        string password = "'" + Password_entry.Text + "'";
                        //var query = "SELECT Email FROM Users";
                        string pass_hashed = "'" + hashing(password) + "'";
                        var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + "'" + email + "'" + " AND Password=" + pass_hashed;
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        var result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            conn.Close();
                            error_label.Text = "Неверный ввод!";
                            return;
                        }
                        else
                        {
                            Email_entry.Text = "";
                            Password_entry.Text = "";
                            error_label.Text = "";
                        }
                    }
                }
                else
                {
                    email = Email_entry_reg.Text;
                    email_ses = "\"" + email + "\"";
                    if (!IsValidEmail(email))
                    {
                        conn.Close();
                        error_label_reg.Text = "Неверный email!";
                        return;
                    }
                    else
                    {
                        string password = "'" + Password_entry_reg.Text + "'";
                        string repass = "'" + RePassword_entry_reg.Text + "'";
                        if (password != repass)
                        {
                            conn.Close();
                            error_label_reg.Text = "Пароли не совпадают!";
                            return;
                        }
                        else
                        {
                            var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + "'" + email + "'";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                conn.Close();
                                error_label_reg.Text = "Пользователь уже зарегистрирован!";
                                return;
                            }
                            else
                            {
                                string pass_hashed = "'" + hashing(password) + "'";
                                query = "INSERT INTO " + databaseTable + "(Email,Password) VALUES (" + "\"" + email + "\"" + "," + pass_hashed + ")";
                                cmd = new MySqlCommand(query, conn);
                                cmd.ExecuteNonQuery();
                                error_label_reg.Text = "";
                                Email_entry_reg.Text = "";
                                Password_entry_reg.Text = "";
                                RePassword_entry_reg.Text = "";

                            }
                        }
                    }
                }
                session.readSession();
                if (session.Email != email)
                {
                    if (session.Email != "")
                    {
                        session.clearNotes();
                        session.insertAllNotes();
                    }
                    session.Email = email;
                }
                else
                {
                    session.Email = email;
                    if (session.Modified && !session.Online)
                    {
                        session.clearNotes();
                        session.insertAllNotes();
                    }

                }
                session.getElemsFromDB();
                session.Online = true;
                session.Modified = false;
                session.writeSession();
                conn.Close();
                await Navigation.PushAsync(new MainPage());
            }
            catch (Exception ex)
            {
                conn.Close();
                Console.WriteLine(ex.Message);
                await DisplayAlert("Нет подключения", "Не удалось подключиться к базе данных", "ОK");
            }
            conn.Close();
        }
    }
}

