﻿using System;

using Android.App;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class QuizManager
    {
        public readonly Activity currentactivity;
        private DoublelineListStruct[] wordlist;
        private int currentquestionindex;
        public readonly TextView questiontext;
        public readonly Button button;
        private ImageView marubatsu;
        private Animation animation;
        public readonly EditText textfield;

        public delegate void QuestionInfo(int index, QuestionResult result);
        public event QuestionInfo OnCorrect;
        public event QuestionInfo OnMiss;
        public int CorrectCount { private set; get; }

        public QuizManager(Activity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, ImageView image, bool faststart)
        {
            currentactivity = activity;
            wordlist = words;
            questiontext = question;
            textfield = edit;
            button = answerbutton;
            animation = AnimationUtils.LoadAnimation(activity, Resource.Animation.marubatsuanim);
            animation.AnimationEnd += AnimationEnd;
            marubatsu = image;
            if (faststart)
            {
                GiveQuestion();
                answerbutton.Click += CheckAnswer;
            }
        }

        private async void AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            button.Clickable = false;
            textfield.Clickable = false;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1));
            marubatsu.SetImageResource(Resource.Drawable.empty);
            button.Clickable = true;
            textfield.Clickable = true;
        }

        public virtual void GiveQuestion()
        {
            Random r = new Random();
            currentquestionindex = r.Next(0, wordlist.Length);
            questiontext.Text = wordlist[currentquestionindex].Title;
            button.Text = currentactivity.GetString(Resource.String.quiz_checkanswer);
        }

        public virtual void GiveQuestion(int questionindex)
        {
            currentquestionindex = questionindex;
            questiontext.Text = wordlist[currentquestionindex].Title;
            button.Text = currentactivity.GetString(Resource.String.quiz_checkanswer);
        }

        public virtual void CheckAnswer(object sender, EventArgs e)
        {
            if (!wordlist[currentquestionindex].Description.Contains("・"))
            {
                if (textfield.Text.Replace(" ", "") == wordlist[currentquestionindex].Description)
                {
                    Correct();
                }
                else
                {
                    Miss();
                }
            }
            else
            {
                string[] ansarray = wordlist[currentquestionindex].Description.Split("・");
                string[] player_ans = textfield.Text.Split("・");
                Array.Sort(ansarray);
                Array.Sort(player_ans);
                foreach (var item in ansarray)
                {
                    if (Array.BinarySearch(player_ans, item) < 0)
                    {
                        Miss();
                        End();
                        return;
                    }
                }
                foreach (var item in player_ans)
                {
                    if (Array.BinarySearch(ansarray, item) < 0)
                    {
                        Miss();
                        End();
                        return;
                    }
                }
                Correct();
            }       
            End();
        }

        private void Correct()
        {
            CorrectCount++;
            OnCorrect?.Invoke(currentquestionindex, QuestionResult.Correct);
            if (!Preferences.Get("test_and _quiz_enableanim", true))
            {

                questiontext.Text = "正解";
                AnimationEnd(null, null);
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.maru);
            }
        }

        private void End()
        {
            if (Preferences.Get("test_and _quiz_enableanim", true))
            {
                marubatsu.StartAnimation(animation);
            }

            button.Text = currentactivity.GetString(Resource.String.quiz_next);
            button.Click -= CheckAnswer;
            button.Click += NextQuestion;
        }

        private void Miss()
        {
            questiontext.Text = wordlist[currentquestionindex].Description;
            questiontext.SetTextColor(Android.Graphics.Color.Red);
            OnMiss?.Invoke(currentquestionindex, QuestionResult.Miss);
            if (!Preferences.Get("test_and _quiz_enableanim", true))
            {
                AnimationEnd(null, null);
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.batsu);
            }
        }

        public void NextQuestion(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            GiveQuestion();
            button.Click += CheckAnswer;
            button.Click -= NextQuestion;
            textfield.Text = string.Empty;
        }

        public DoublelineListStruct[] GetWordList()
        {
            return wordlist;
        }

        public void ResetCorrectCount()
        {
            CorrectCount = 0;
        }
    }
    [Serializable]
    public enum QuestionResult
    {
        Correct,
        Miss
    }
}