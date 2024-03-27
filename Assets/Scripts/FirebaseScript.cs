using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using Firebase.Extensions;

//database e y�klencek class
[Serializable]
public class Data
{   //databese y�klenecek de�i�kenler
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






    //database ve kullan�c�lara eri�im
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

    //kullan�c� kaydetme
    public void SignUp()
    {

        //email kaydedilir
        auth.CreateUserWithEmailAndPasswordAsync(Email, Password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Bir�eyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Sign up failed. Check your email and password.");
                return;
            }

            //kullan�c� Id si olu�turulur
            user = task.Result.User;
            Debug.Log("Sign up successful!");
            SaveData();
        });



    }

    //kullan�c� Id sine verileri kaydedilir
    public void SaveData()
    {

        Dataram.Data1 = Var5;
        Dataram.Data2 = Var6;
        Dataram.Data3 = Var7;
        Dataram.Data4 = Var8;

        //veriler jsona �eviirilir y�klenir
        string json = JsonUtility.ToJson(Dataram);
        db.Child(user.UserId).SetRawJsonValueAsync(json);
    }



    //kullan�c� giri�i
    public void Login()
    {


        auth.SignInWithEmailAndPasswordAsync(Email, Password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {

                Debug.Log("Bir�eyler ters gitti :(");
                return;
            }
            if (task.IsFaulted)
            {

                Debug.Log("Login failed please try again");
                return;
            }

            //Id al�n�r
            user = task.Result.User;
            Debug.Log("Log in successful!");

            //Id deki veriler classa aktar�l�r
            var serverData = db.Child(user.UserId).GetValueAsync();
            string json = serverData.Result.GetRawJsonValue();
            Dataram = JsonUtility.FromJson<Data>(json);


            //class de�i�kenlere aktar�l�r
            if (Dataram != null)
            {
                Var5 = Dataram.Data1;
                Var6 = Dataram.Data2;
                Var7 = Dataram.Data3;
                Var8 = Dataram.Data4;
            }
            else
            {//yeni kullan�c� giri�i oldu�unda veri olmad��� i�in 0 atan�r
                Var5 = 0;
                Var6 = 0;
                Var7 = 0;
                Var8 = 0;
            }




        });
    }




}