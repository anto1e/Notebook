﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class MainPage : ContentPage
    {
        NotesCollection notCol = NotesCollection.getInstance();
        public MainPage()
        {
            InitializeComponent();
            notesList.BindingContext = notCol.Notes;
            SubscribeColChanging();
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            
        }
        private string currentPage = "All";
        private void SubscribeColChanging()     //Функция для изменения отображения кол-ва заметок на главной странице при изменении коллекции
        {
              MessagingCenter.Subscribe<Page>(
                this, // кто подписывается на сообщения
                "CollectionChanged!",   // название сообщения
                (sender) => {
                    int cntNotCol = notCol.Notes.Count;
                    int cntNotColPriv = notCol.Notes.Count(i => i.Allowers.Count() == 0 && i.Author == "123@mail.ru");
                    int cntNotColGroup = notCol.Notes.Count(i => i.Allowers.Count() != 0 || i.Author != "123@mail.ru");
                    if (cntNotCol < 100)
                        label_all.Text = "Все(" + cntNotCol.ToString() + ")";
                    else
                        label_all.Text = "Все>99";
                    if (cntNotColPriv < 100)
                        label_priv.Text = "Личные(" + cntNotColPriv.ToString() + ")";
                    else
                        label_priv.Text = "Личные>99";
                    if (cntNotColGroup < 100)
                        label_group.Text = "Групповые(" + cntNotColGroup.ToString() + ")";
                    else
                        label_group.Text = "Групповые>99";
                    if (currentPage == "Private")
                        notCol.fillPrivateTemp();
                    if (currentPage == "Group")
                        notCol.fillGroupTemp();
                });    // вызываемое действие

        }
        private async void addBtnClicked(object sender, EventArgs args)       //Обработка нажатия на кнопку "Добавить"
        {
            await Navigation.PushAsync(new AddPage());
        }
        private double previousScrollPosition = 0;          //Переменная для хранения значения предыдущей позиции
        private int scrollXCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем уберется тулбар
        private int scrollYCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем появится тулбар
        private void scrollHandler(object sender, Xamarin.Forms.ScrolledEventArgs e)        //Функция отображения/скрытия тулбара
        {
            if (Convert.ToInt16(e.ScrollY) > 10)
            {
                if (previousScrollPosition < e.ScrollY)
                {
                    if (scrollXCnt > 5)
                    {
                        //scrolled down
                        previousScrollPosition = e.ScrollY;
                        toolbar_layout.TranslateTo(0, 250, 150);
                    }
                    else
                    {
                        scrollXCnt++;
                        scrollYCnt = 0;
                    }
                }
                else
                {
                    if (scrollYCnt > 1)
                    {
                        if (Convert.ToInt16(e.ScrollY) == 0)
                            previousScrollPosition = 0;
                        previousScrollPosition = e.ScrollY;
                        toolbar_layout.TranslateTo(0, 0, 150);
                    }
                    else
                    {
                        scrollYCnt++;
                        scrollXCnt = 0;
                    }
                }
            }
        }
        private async void OnListViewItemSelected(object sender, EventArgs args) {                //Функция полученя заметки, на которую нажал пользователь
            var frame = (Frame)sender;
            frame.BackgroundColor = Color.FromHex("DCE1C6");
            Note SelectedItem = notCol.Notes.FirstOrDefault(itm => itm.Number == Convert.ToInt32(((TappedEventArgs)args).Parameter.ToString()));
            await Navigation.PushAsync(new ModifyPage(SelectedItem));
            frame.BackgroundColor = Color.FromHex("F9FFE2");
        }
        private async void PrivateNotesClicked(object sender, EventArgs e)      //Переход на страницу с личными заметками
        {
            notCol.fillPrivateTemp();
            underline_priv.TranslationX = 0;
            underline_priv.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "All")
            {
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 500;
                await Grid_messages.TranslateTo(-500, 0, 100);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "Private";
            }
            if (currentPage == "Group")
            {
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 500;
                await Grid_messages.TranslateTo(500, 0, 100);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(-500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "Private";
            }
        }
        private async void AllNotesClicked(object sender, EventArgs e)      //Переход на страницу со всеми заметками
        {
            underline_all.TranslationX = 0;
            underline_all.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 500;
                await Grid_messages.TranslateTo(500, 0, 100);
                notesList.BindingContext = notCol.Notes;
                await Grid_messages.TranslateTo(-500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "All";
            }
            if (currentPage == "Group")
            {
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 500;
                await Grid_messages.TranslateTo(500, 0, 100);
                notesList.BindingContext = notCol.Notes;
                await Grid_messages.TranslateTo(-500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "All";
            }
        }
        private async void GroupNotesClicked(object sender, EventArgs e)      //Переход на страницу с групповыми заметками
        {
            notCol.fillGroupTemp();
            underline_group.TranslationX = 0;
            underline_group.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 500;
                await Grid_messages.TranslateTo(-500, 0, 100);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "Group";
            }
            if (currentPage == "All")
            {
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 500;
                await Grid_messages.TranslateTo(-500, 0, 100);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(500, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 100);
                currentPage = "Group";
            }
        }
    }
}

