using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using UserReg.Models;

namespace UserReg.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserClass uc)
        {
            try
            {
                byte[] bytes = null;
                if (uc.Uimg != null && uc.Uimg.ContentLength > 0)
                {
                    using (Stream fs = uc.Uimg.InputStream)
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                    }

                    string key = "sblw-3hn8-sqoy19";
                    byte[] encryptedBytes = Encrypt(bytes, key);

                    string maincon = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
                    using (SqlConnection sqlcon = new SqlConnection(maincon))
                    {
                        using (SqlCommand sqlcomm = new SqlCommand("InsertUserDetails", sqlcon))
                        {
                            sqlcomm.CommandType = CommandType.StoredProcedure;
                            sqlcomm.Parameters.AddWithValue("@Uname", uc.Uname);
                            sqlcomm.Parameters.AddWithValue("@Uemail", uc.Uemail);
                            sqlcomm.Parameters.AddWithValue("@Upwd", uc.Upwd);
                            sqlcomm.Parameters.AddWithValue("@Gender", uc.Gender);
                            sqlcomm.Parameters.AddWithValue("@Uimg", encryptedBytes);
                            sqlcon.Open();
                            sqlcomm.ExecuteNonQuery();
                        }
                    }

                    byte[] decryptedBytes = Decrypt(encryptedBytes, key);
                    ViewBag.Image = ViewImage(decryptedBytes);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        private string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);
            return "data:image/png;base64," + base64String;
        }

        public byte[] Encrypt(byte[] data, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedData, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[16];
                Array.Copy(encryptedData, iv, 16);
                aesAlg.IV = iv;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(encryptedData, 16, encryptedData.Length - 16);
                        csDecrypt.FlushFinalBlock();
                    }
                    return msDecrypt.ToArray();
                }
            }
        }
    }
}













/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserReg.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UserReg.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UserClass uc)
        {
            try
            {
                Byte[] bytes = null;
                if (uc.Uimg.FileName != null)
                {
                    Stream fs = uc.Uimg.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    bytes = br.ReadBytes((Int32)fs.Length);

                    string ggg = Encrypt(bytes.ToString(), "sblw-3hn8-sqoy19");

                    string maincon = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
                    SqlConnection sqlcon = new SqlConnection(maincon);
                    SqlCommand sqlcomm = new SqlCommand("InsertUserDetails", sqlcon);
                    sqlcomm.CommandType = CommandType.StoredProcedure;
                    sqlcomm.Parameters.AddWithValue("@Uname", uc.Uname);
                    sqlcomm.Parameters.AddWithValue("@Uemail", uc.Uemail);
                    sqlcomm.Parameters.AddWithValue("@Upwd", uc.Upwd);
                    sqlcomm.Parameters.AddWithValue("@Gender", uc.Gender);
                    sqlcomm.Parameters.AddWithValue("@Uimg", ggg);
                    sqlcon.Open();
                    sqlcomm.ExecuteNonQuery();
                    sqlcon.Close();

                    byte[] fsd = Encoding.ASCII.GetBytes(ggg);
                    
                    byte[]  gg = Decrypt(fsd, "sblw-3hn8-sqoy19");


                    ViewBag.Image = ViewImage(bytes);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
        private string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            return "data:image/png;base64," + base64String;
        }



       

        public string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        *//*public string shortImg(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }*/




/*public ActionResult Index(UserClass uc, HttpPostedFileBase file)
{
    string maincon = ConfigurationManager.ConnectionStrings["mycon"].ConnectionString;
    SqlConnection sqlcon = new SqlConnection(maincon);
    SqlCommand sqlcomm = new SqlCommand("InsertUserDetails", sqlcon);
    sqlcon.Open();
    sqlcomm.CommandType = CommandType.StoredProcedure;
    sqlcomm.Parameters.AddWithValue("@Uname", uc.Uname);
    sqlcomm.Parameters.AddWithValue("@Uemail", uc.Uemail);
    sqlcomm.Parameters.AddWithValue("@Upwd", uc.Upwd);
    sqlcomm.Parameters.AddWithValue("@Gender", uc.Gender);

    if(file!=null && file.ContentLength>0)
    {
        string filename = Path.GetFileName(file.FileName);
        string imgpath = Path.Combine(Server.MapPath("~/User-Images/"), filename);
        file.SaveAs(imgpath);
    }
    sqlcomm.Parameters.AddWithValue("@Uimg", "~/User-Images/"+file.FileName);
    sqlcomm.ExecuteNonQuery();
    sqlcon.Close();
    ViewData["Message"] = "User Record " + uc.Uname + " Is save Successfully ! ";

    return View();
}*//*
}
}*/