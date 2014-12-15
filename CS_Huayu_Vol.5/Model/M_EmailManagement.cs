using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
   public class M_EmailManagement
    {
       public Int32 eId
       {
           get;
           set;
       }

       public string EmailSMTP
       {
           get;
           set;
       }

       public string EmailUserName
       {
           get;
           set;
       }

       public string EmailPwd
       {
           get;
           set;
       }

       public string FirstPickGoodEmail_Subject
       {
           get;
           set;
       }

       public string FirstPickGoodEmail_Body
       {
           get;
           set;
       }

       public string UnReleaseGoodEmail_Subject
       {
           get;
           set;
       }

       public string UnReleaseGoodEmail_Body
       {
           get;
           set;
       }

       public string RejectGoodEmail_Subject
       {
           get;
           set;
       }

       public string RejectGoodEmail_Body
       {
           get;
           set;
       }

       public string SendDialyReport_Subject
       {
           get;
           set;
       }

       public string SendDialyReport_Body
       {
           get;
           set;
       }

       public string mMemo
       {
           get;
           set;
       }
    }
}
