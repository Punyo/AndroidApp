using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace AndroidApp.Assets
{
    class QuizManager
    {
        private Activity currentactivity;
        private DoublelineListStruct[] wordlist;
        private DoublelineListStruct currentquestion;
        private TextView questiontext;
        private Button button;
        private ImageView marubatsu;
        private Animation animation;
        private EditText textfield;

        public QuizManager(Activity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, ImageView image)
        {
            currentactivity = activity;
            wordlist = words;
            questiontext = question;
            textfield = edit;
            button = answerbutton;
            answerbutton.Click += CheckAnswer;
            animation = AnimationUtils.LoadAnimation(activity, Resource.Animation.marubatsuanim);
            animation.AnimationEnd += AnimationEnd;
            marubatsu = image;
            GiveQuestion();
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

        private void GiveQuestion()
        {
            Random r = new Random();
            currentquestion = wordlist[r.Next(0, wordlist.Length)];
            questiontext.Text = currentquestion.Title;
            button.Text = currentactivity.GetString(Resource.String.quiz_checkanswer);
        }

        private void CheckAnswer(object sender, EventArgs e)
        {
            if (textfield.Text.Replace(" ", "") == currentquestion.Description)
            {
                marubatsu.SetImageResource(Resource.Drawable.maru);
            }
            else
            {
                marubatsu.SetImageResource(Resource.Drawable.batsu);
                questiontext.Text = currentquestion.Description;
                questiontext.SetTextColor(Android.Graphics.Color.Red);
            }
            marubatsu.StartAnimation(animation);
            button.Text = currentactivity.GetString(Resource.String.quiz_next);
            button.Click -= CheckAnswer;
            button.Click += NextQuestion;
        }

        private void NextQuestion(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            GiveQuestion();
            button.Click += CheckAnswer;
            button.Click -= NextQuestion;
        }
    }
}