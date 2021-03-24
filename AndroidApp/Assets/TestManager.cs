using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Android.Widget;

namespace AndroidApp.Assets
{
    class TestManager : QuizManager
    {
        private MainActivity mainactivity;
        private DoublelineListStruct[] questions;
        private TestQuestionStruct[] questionStruct;
        private TestResultStruct[] pastresults;
        private int questioncount;
        private int lastcorrect;
        private QuestionResult[] results;
        private string genre_name;
        private string datapath;
        private EventHandler a;
        private int id;
        public TestManager(MainActivity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, ImageView image, string scoredatapath, bool faststart, string genrename,int genreid)
            : base(activity, words, edit, question, answerbutton, image, faststart)
        {
            mainactivity = activity;
            questions = base.GetWordList();
            datapath = scoredatapath;
            base.OnCorrect += SetResult;
            base.OnMiss += SetResult;
            genre_name = genrename;
            id = genreid;
            questionStruct = new TestQuestionStruct[questions.Length];
            results = new QuestionResult[questions.Length];
            for (int i = 0; i < questions.Length; i++)
            {
                questionStruct[i].Question = questions[i];
            }
            a = (s, ev) => { ShowDescription(); };
            ShowDescription();
            NewMethod();
        }

        private async Task NewMethod()
        {
            string[] pastresult = await FileIO.ReadFileLineAsync(datapath);
            pastresults = new TestResultStruct[0];
            foreach (var item in pastresult)
            {
                Array.Resize(ref pastresults, pastresults.Length + 1);
                pastresults[pastresults.Length - 1] = JsonSerializer.Deserialize<TestResultStruct>(item);
            }
        }

        private void SetResult(int index, QuestionResult result)
        {
            results[index] = result;
        }

        private void ShowDescription()
        {
            questiontext.Text = $"{questions.Length}問のテストを開始します。" + System.Environment.NewLine +
                "よろしいですか？";
            base.button.Text = "開始";
            questioncount = 0;
            button.Click -= a;
            button.Click += Button_Click;
            ResetCorrectCount();
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
            lastcorrect = CorrectCount;
            base.CheckAnswer(sender, e);
            button.Click -= base.NextQuestion;
            button.Click -= CheckAnswer;
            button.Click += Button_Click;
            base.textfield.Text = string.Empty;
            questioncount++;
        }

        public void CheckAnswerLast(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            button.Click -= CheckAnswerLast;
            lastcorrect = CorrectCount;
            base.CheckAnswer(sender, e);
            button.Click -= base.NextQuestion;
            button.Click += ShowResult;
            button.Text = "結果を見る";
        }

        private async void ShowResult(object sender, EventArgs e)
        {
            double correctpercent = Math.Round((double)CorrectCount / (double)questions.Length, 3) * 100;
            questiontext.Text = $"正解:{base.CorrectCount}"
                + System.Environment.NewLine
                + $"不正解:{questions.Length - CorrectCount}"
                + System.Environment.NewLine
                + $"正解率:{correctpercent}%";
            button.Click -= ShowResult;
            button.Text = "再挑戦する";
            for (int i = 0; i < results.Length; i++)
            {
                questionStruct[i].Result = results[i];
            }
            TestResultStruct testResult = new TestResultStruct();
            testResult.CorrectPercent = correctpercent;
            testResult.Questions = questionStruct;
            testResult.TestTime = DateTime.Now;
            testResult.Genrename = genre_name;
            string content = JsonSerializer.Serialize(testResult) + System.Environment.NewLine;
            Array.Resize(ref pastresults, pastresults.Length + 1);
            pastresults[pastresults.Length - 1] = JsonSerializer.Deserialize<TestResultStruct>(content);
            await FileIO.WriteFileAsync(datapath, content, FileMode.Append);
            mainactivity.SetTestResults(pastresults, id);
            button.Click += a;
        }
    }
    [Serializable]
    public struct TestResultStruct
    {
        public double CorrectPercent { set; get; }
        public DateTime TestTime { set; get; }
        public TestQuestionStruct[] Questions { set; get; }
        public string Genrename { set; get; }
    }
    [Serializable]
    public struct TestQuestionStruct
    {
        public DoublelineListStruct Question { set; get; }
        public QuestionResult Result { set; get; }
    }
}