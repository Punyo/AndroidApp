using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class QuizManager
    {
        public readonly Activity currentactivity;
        private List<DoublelineListStruct> wordlist;
        private int[] priorities;
        private int currentquestionindex;
        public readonly TextView questiontext;
        public readonly Button button;
        private readonly int maxpriority = 3;
        private readonly int minpriority = -3;
        private ImageView marubatsu;
        private Animation animation;
        private Random random;
        public readonly EditText textfield;

        public QuizManager(Activity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, ImageView image, bool faststart)
        {
            currentactivity = activity;
            wordlist = words.ToList();
            questiontext = question;
            textfield = edit;
            button = answerbutton;
            animation = AnimationUtils.LoadAnimation(activity, Resource.Animation.marubatsuanim);
            animation.AnimationEnd += AnimationEnd;
            marubatsu = image;
            priorities = new int[words.Length];
            random = new Random();
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
            currentquestionindex = random.Next(0, wordlist.Count);
            if (random.Next(0, (maxpriority - priorities[currentquestionindex]) * 2) != 0)
            {
                GiveQuestion();
            }
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

            if (!Preferences.Get("test_and _quiz_enableanim", true))
            {
                questiontext.Text = "正解";
                AnimationEnd(null, null);
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.maru);
            }
            DecreasePriority(currentquestionindex);
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
            //OnMiss?.Invoke(currentquestionindex, QuestionResult.Miss);
            if (!Preferences.Get("test_and _quiz_enableanim", true))
            {
                AnimationEnd(null, null);
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.batsu);
            }
            IncreasePriority(currentquestionindex);
        }

        public void NextQuestion(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            GiveQuestion();
            button.Click += CheckAnswer;
            button.Click -= NextQuestion;
            textfield.Text = string.Empty;
        }

        private void IncreasePriority(int questionindex)
        {

            if (priorities[questionindex] < maxpriority)
            {
                priorities[questionindex]++;
            }
        }

        private void DecreasePriority(int questionindex)
        {
            if (priorities[questionindex] > minpriority)
            {
                priorities[questionindex]--;
            }
        }
    }
}