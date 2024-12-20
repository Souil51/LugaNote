using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DataBinding.Tests
{
    public class Player : INotifyPropertyChanged
    {
        public Player(int maxHealth, string name)
        {
            Health = maxHealth;
            Name = name;
        }

        public Player(int maxHealth)
        {
            Health = maxHealth;
        }

        private int _health;
        public int Health
        {
            get => _health;
            private set
            {
                if (value < 0)
                    _health = 0;
                else
                    _health = value;

                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private Player _friend;
        public Player Friend
        {
            get => _friend;
            private set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public void GetDamaged(int amount)
        {
            Health -= amount;

            if (Health == 0)
            {
                //Mort
            }
        }

        public void SetFriend(Player friend)
        {
            this.Friend = friend;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}