using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using Firebase.Extensions;

//database e yüklencek class
[Serializable]
public class Data
{   //databese yüklenecek deðiþkenler
    [HideInInspector]
    public int Data1, Data2, Data3, Data4;

}

public class FirebaseScript : MonoBehaviour
{
    public string Email;
    public string Password;

    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference db;

    public int Var5;
    public int Var6;
    public int Var7;
    public int Var8;
    public Data Dataram;






    //database ve kullanýcýlara eriþim
    private void Awake()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    //istatistik toplama
    private void Start()
    {


        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {


            var app = Firebase.FirebaseApp.DefaultInstance;


        });
    }

    //kullanýcý kaydetme
    public void SignUp()
    {

        //email kaydedilir
        auth.CreateUserWithEmailAndPasswordAsync(Email, Password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Birþeyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Sign up failed. Check your email and password.");
                return;
            }

            //kullanýcý Id si oluþturulur
            user = task.Result.User;
            Debug.Log("Sign up successful!");
            SaveData();
        });



    }

    //kullanýcý Id sine verileri kaydedilir
    public void SaveData()
    {

        Dataram.Data1 = Var5;
        Dataram.Data2 = Var6;
        Dataram.Data3 = Var7;
        Dataram.Data4 = Var8;

        //veriler jsona çeviirilir yüklenir
        string json = JsonUtility.ToJson(Dataram);
        db.Child(user.UserId).SetRawJsonValueAsync(json);
    }



    //kullanýcý giriþi
    public void Login()
    {


        auth.SignInWithEmailAndPasswordAsync(Email, Password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Birþeyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Login failed please try again");
                return;
            }

            //Id alýnýr
            user = task.Result.User;
            Debug.Log("Log in successful!");

            //Id deki veriler classa aktarýlýr
            var serverData = db.Child(user.UserId).GetValueAsync();
            string json = serverData.Result.GetRawJsonValue();
            Dataram = JsonUtility.FromJson<Data>(json);


            //class deðiþkenlere aktarýlýr
            if (Dataram != null)
            {
                Var5 = Dataram.Data1;
                Var6 = Dataram.Data2;
                Var7 = Dataram.Data3;
                Var8 = Dataram.Data4;
            }
            else
            {//yeni kullanýcý giriþi olduðunda veri olmadýðý için 0 atanýr
                Var5 = 0;
                Var6 = 0;
                Var7 = 0;
                Var8 = 0;
            }




        });
    }




}