using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    class TestManager : QuizManager
    {
        private DoublelineListStruct[] questions;
        private int questioncount;
        public TestManager(Activity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, ImageView image, bool faststart)
            : base(activity, words, edit, question, answerbutton, image, faststart)
        {
            questions = base.GetWordList();
            ShowDescription();
        }

        private void ShowDescription()
        {
            questiontext.Text = $"{questions.Length}問のテストを開始します。" + System.Environment.NewLine +
                "よろしいですか？";
            base.button.Text = "開始";
            button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            GiveQuestion(questioncount);
            button.Click -= Button_Click;
            if (questioncount + 1 >= questions.Length)
            {
                button.Click -= CheckAnswer;
                button.Click += CheckAnswerLast;
            }
            else
            {
                button.Click += CheckAnswer;
            }
        }

        public override void CheckAnswer(object sender, EventArgs e)
        {
            base.CheckAnswer(sender, e);
            //counter.Text = $"{questioncount}/{questions.Length}";
            button.Click -= base.NextQuestion;
            button.Click -= CheckAnswer;
            button.Click += Button_Click;
            questioncount++;
        }

        public void CheckAnswerLast(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            button.Click -= CheckAnswerLast;
            base.CheckAnswer(sender, e);
            button.Click -= base.NextQuestion;
            button.Click += ShowResult;
            button.Text = "結果を見る";
        }

        private void ShowResult(object sender, EventArgs e)
        {
            questiontext.Text = $"正解:{base.CorrectCount}"
                + System.Environment.NewLine
                + $"不正解:{questions.Length - CorrectCount}"
                + System.Environment.NewLine
                + $"正解率:{Math.Round((double)CorrectCount / (double)questions.Length, 3) * 100}%";
            button.Click -= ShowResult;
            button.Text = "再挑戦する";
            button.Click += (sende, a) => { ShowDescription(); };
        }
    }
}
