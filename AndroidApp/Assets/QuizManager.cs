using Android.App;
using Android.Views.Animations;
using Android.Widget;
using System;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    internal class QuizManager
    {
        public readonly Activity currentactivity;
        private readonly DoublelineListStruct[] wordlist;
        private int currentquestionindex;
        public readonly TextView questiontext;
        public readonly Button button;
        private readonly ImageView marubatsu;
        private readonly Animation animation;
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
            if (textfield.Text.Replace(" ", "") == wordlist[currentquestionindex].Description)
            {
                marubatsu.SetImageResource(Resource.Drawable.maru);
                CorrectCount++;
                OnCorrect?.Invoke(currentquestionindex, QuestionResult.Correct);
                if (!Preferences.Get("test_and _quiz_enableanim", true))
                {
                    questiontext.Text = "正解";
                }
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.batsu);
                questiontext.Text = wordlist[currentquestionindex].Description;
                questiontext.SetTextColor(Android.Graphics.Color.Red);
                OnMiss?.Invoke(currentquestionindex, QuestionResult.Miss);
            }

            if (Preferences.Get("test_and _quiz_enableanim", true))
            {
                marubatsu.StartAnimation(animation);
            }

            button.Text = currentactivity.GetString(Resource.String.quiz_next);
            button.Click -= CheckAnswer;
            button.Click += NextQuestion;
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