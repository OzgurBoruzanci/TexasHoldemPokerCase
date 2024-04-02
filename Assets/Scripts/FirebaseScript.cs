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

//database e yüklencek class
[Serializable]
public class Data
{   //databese yüklenecek deðiþkenler
    [HideInInspector]
    public int Point, Money;
    [HideInInspector]
    public string Nickname;

}

public class Leaderboard
{
    //Leaderboarddan çekilecek class
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




    //database ve kullanýcýlara eriþim
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

    //kullanýcý kaydetme
    public void SignUp()
    {

        //email kaydedilir
        auth.CreateUserWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
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

        Dataram.Point = point;
        Dataram.Money = money;
        Dataram.Nickname = NicknameText.text;


        //veriler jsona çeviirilir yüklenir
        string json = JsonUtility.ToJson(Dataram);
        db.Child(user.UserId).SetRawJsonValueAsync(json);



    }



    //kullanýcý giriþi
    public void Login()
    {


        auth.SignInWithEmailAndPasswordAsync(EmailText.text, PasswordText.text).ContinueWith(task =>
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
                point = Dataram.Point;
                money = Dataram.Money;
                nickname = Dataram.Nickname;
            }
            else
            {//yeni kullanýcý giriþi olduðunda veri olmadýðý için 0 atanýr
                point = 0;
                money = 0;
                nickname = null;

            }
        });
    }





    //Firebaseden veri çekmek biraz uzun sürüyor o yüzden Ienumerator çaðýrýyoruz
    public void Leaderboard()
    {
        StartCoroutine(Writeboard());

    }

    public void Refreshboard()
    {
        StartCoroutine(Refresh());

    }




    //kullanýcýyý leaderboarda kaydetme
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

        //mevcut puaný best 10 kiþiyle kýyasla
        while (number < 11)
        {
            if (Leaderram.PointList[number] < point)
            {
                //listeye ekle 
                Leaderram.PointList.Insert(number, point);
                Leaderram.NameList.Insert(number, nickname);


                //geriye düþeni sil
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


    //best 10 kiþi ve puaný listeye yazdýr
    IEnumerator Refresh()
    {
        //classý al
        var data1 = db.Child("LeaderBoard").GetValueAsync();
        yield return new WaitUntil(predicate: () => data1.IsCompleted);
        string tson = data1.Result.GetRawJsonValue();
        Leaderram = JsonUtility.FromJson<Leaderboard>(tson);

        int number = 0;

        //teker teker yazdýr
        while (number < 10)
        {
            
            NameList[number].text = Leaderram.NameList[number];
            PointList[number].text = Leaderram.PointList[number].ToString();
            number++;

        }

    }
}