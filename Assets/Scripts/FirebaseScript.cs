using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using Firebase.Extensions;
using TMPro;
using Google.MiniJSON;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

//database e y�klencek class
[Serializable]
public class Data
{   //databese y�klenecek de�i�kenler
    [HideInInspector]
    public int Point, Money;
    [HideInInspector]
    public string Nickname;

}

public class Leaderboard
{
    //Leaderboarddan �ekilecek class
    public List<string> NameList = new List<string>();
    public List<int> PointList = new List<int>();

}

public class FirebaseScript : MonoBehaviour
{


    public TMP_InputField EmailText;
    public TMP_InputField PasswordText;
    public TMP_InputField NicknameText;



    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference db;

    [SerializeField]
    private int point, money;
    [SerializeField]
    private string nickname;

    public Data Dataram;
    public Leaderboard Leaderram;

    public TMPro.TextMeshProUGUI[] NameList;
    public TMPro.TextMeshProUGUI[] PointList;




    //database ve kullan�c�lara eri�im
    private void Awake()
    {
        db = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    //istatistik toplama
    private void Start()
    {


        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {


            var app = Firebase.FirebaseApp.DefaultInstance;


        });
    }

    //kullan�c� kaydetme
    public void SignUp()
    {

        //email kaydedilir
        auth.CreateUserWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
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

        Dataram.Point = point;
        Dataram.Money = money;
        Dataram.Nickname = NicknameText.text;


        //veriler jsona �eviirilir y�klenir
        string json = JsonUtility.ToJson(Dataram);
        db.Child(user.UserId).SetRawJsonValueAsync(json);



    }



    //kullan�c� giri�i
    public void Login()
    {


        auth.SignInWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
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
                point = Dataram.Point;
                money = Dataram.Money;
                nickname = Dataram.Nickname;
            }
            else
            {//yeni kullan�c� giri�i oldu�unda veri olmad��� i�in 0 atan�r
                point = 0;
                money = 0;
                nickname = null;

            }
        });
    }





    //Firebaseden veri �ekmek biraz uzun s�r�yor o y�zden Ienumerator �a��r�yoruz
    public void Leaderboard()
    {
        StartCoroutine(Writeboard());

    }

    public void Refreshboard()
    {
        StartCoroutine(Refresh());

    }




    //kullan�c�y� leaderboarda kaydetme
    IEnumerator Writeboard()
    {
        //class al
        var data1 = db.Child("LeaderBoard").GetValueAsync();
        yield return new WaitUntil(predicate: () => data1.IsCompleted);
        string tson = data1.Result.GetRawJsonValue();
        Leaderram = JsonUtility.FromJson<Leaderboard>(tson);

        int number = 0;

        point = Convert.ToInt32(PasswordText.text);
        nickname = NicknameText.text;

        //mevcut puan� best 10 ki�iyle k�yasla
        while (number < 11)
        {
            if (Leaderram.PointList[number] < point)
            {
                //listeye ekle 
                Leaderram.PointList.Insert(number, point);
                Leaderram.NameList.Insert(number, nickname);


                //geriye d��eni sil
                Leaderram.PointList.RemoveAt(11);
                Leaderram.NameList.RemoveAt(11);
                
                //keydet
                string json = JsonUtility.ToJson(Leaderram);
                db.Child("LeaderBoard").SetRawJsonValueAsync(json);

                break;
            }
            number++;

        }
    }


    //best 10 ki�i ve puan� listeye yazd�r
    IEnumerator Refresh()
    {
        //class� al
        var data1 = db.Child("LeaderBoard").GetValueAsync();
        yield return new WaitUntil(predicate: () => data1.IsCompleted);
        string tson = data1.Result.GetRawJsonValue();
        Leaderram = JsonUtility.FromJson<Leaderboard>(tson);

        int number = 0;

        //teker teker yazd�r
        while (number < 10)
        {
            
            NameList[number].text = Leaderram.NameList[number];
            PointList[number].text = Leaderram.PointList[number].ToString();
            number++;

        }

    }
}