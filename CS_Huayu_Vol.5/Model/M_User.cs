using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
 public   class M_User
    {
       public  M_User()
       {
       }

       private int _userID;

       public int UserID
       {
           get { return _userID; }
           set { _userID = value; }
       }
       private string _userName;

       public string UserName
       {
           get { return _userName; }
           set { _userName = value; }
       }
       private string _userPassword;

       public string UserPassword
       {
           get { return _userPassword; }
           set { _userPassword = value; }
       }

       private int _authority;

       public int Authority
       {
           get { return _authority; }
           set { _authority = value; }
       }

       private int _comment;
       public int Comment
       {
           get { return _comment; }
           set { _comment = value; }
       }
       private string _company;

       public string Company
       {
           get { return _company; }
           set { _company = value; }
       }

       public string CompanyFullName
       {
           get;
           set;
       }

       public string LinkPerson
       {
           get;
           set;
       }

       public string IdentityCode
       {
           get;
           set;
       }

       public string LinkTel
       {
           get;
           set;
       }

       public string CompanyPhone
       {
           get;
           set;
       }

       public string CompanyAddr
       {
           get;
           set;
       }

       public string LinkMail
       {
           get;
           set;
       }

       public Int32 iSendFirstPickGoodsEmail
       {
           get;
           set;
       }

       public Int32 iSendUnReleaseGoodsEmail
       {
           get;
           set;
       }

       public Int32 iSendRejectGoodsEmail
       {
           get;
           set;
       }
    }
}
