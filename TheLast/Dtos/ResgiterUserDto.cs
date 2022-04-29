using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// 用户注册DTO
    /// </summary>
    public class ResgiterUserDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string account;
        public string Account
        {
            get { return account; }
            set { SetProperty(ref account, value); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }
        private string roleName;
        public string RoleName
        {
            get { return roleName; }
            set { SetProperty(ref roleName, value); }
        }
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }
        private string newPassWord;
        public string NewPassWord
        {
            get { return newPassWord; }
            set { SetProperty(ref newPassWord, value); }
        }
    }
}
