using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Views.Animations;
using Android.Widget;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class TestManager
    {
        private MainActivity mainactivity;
        private DoublelineListStruct[] questions;
        private TestQuestionStruct[] questionStruct;
        private TestResultStruct[] pastresults;
        private Spinner spinner;
        private int questioncount;
        public readonly TextView questiontext;
        public readonly Button button;
        private QuestionResult[] results;
        private string genre_name;
        private string datapath;
        private EventHandler a;
        private int id;
        private ImageView marubatsu;
        private Animation animation;
        public readonly EditText textfield;
        private int currentquestionindex;
        private Random randomquestion;

        public int CorrectCount { private set; get; }

        private readonly string modepreferencekey = "test_mode";

        public delegate void QuestionInfo(int index, QuestionResult result);
        public event QuestionInfo OnCorrect;
        public event QuestionInfo OnMiss;
        public TestManager(MainActivity activity, DoublelineListStruct[] words, EditText edit, TextView question, Button answerbutton, Spinner spin, ImageView image, string scoredatapath, bool faststart, string genrename, int genreid)
        {
            mainactivity = activity;
            questions = words;
            datapath = scoredatapath;
            OnCorrect += SetResult;
            OnMiss += SetResult;
            genre_name = genrename;
            questiontext = question;
            textfield = edit;
            spinner = spin;
            button = answerbutton;
            id = genreid;
            animation = AnimationUtils.LoadAnimation(activity, Resource.Animation.marubatsuanim);
            animation.AnimationEnd += AnimationEnd;
            marubatsu = image;
            questionStruct = new TestQuestionStruct[questions.Length];
            results = new QuestionResult[questions.Length];
            ArrayAdapter adap = ArrayAdapter.CreateFromResource(activity, Resource.Array.test_prompt_options, Android.Resource.Layout.SimpleSpinnerItem);
            adap.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adap;
            if (Preferences.ContainsKey(modepreferencekey))
            {
                spinner.SetSelection(Preferences.Get(modepreferencekey, 0));
            }
            for (int i = 0; i < questions.Length; i++)
            {
                questionStruct[i].Question = questions[i];
            }
            a = (s, ev) => { ShowDescription(); };
            ShowDescription();
            NewMethod();
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
            spinner.Visibility = Android.Views.ViewStates.Visible;
            questiontext.Text = $"{questions.Length}問のテストを開始します。" + System.Environment.NewLine +
                "よろしいですか？";
            button.Text = "開始";
            questioncount = 0;
            button.Click -= a;
            button.Click += InitialBeforeStart;
            CorrectCount = 0;
        }

        private void InitialBeforeStart(object sender, EventArgs e)
        {
            spinner.Visibility = Android.Views.ViewStates.Gone;
            Preferences.Set(modepreferencekey, (int)spinner.SelectedItemId);
            Button_Click(null, null);
            button.Click -= InitialBeforeStart;
            //button.Click += Button_Click;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            if (spinner.SelectedItemId == 0)
            {
                GiveQuestion(questioncount);
            }
            else
            {
                GiveQuestionRandom();
            }
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

        public void CheckAnswer(object sender, EventArgs e)
        {
            //lastcorrect = CorrectCount;
            button.Click -= Button_Click;
            button.Click -= CheckAnswer;
            if (!questions[currentquestionindex].Description.Contains("・"))
            {
                if (textfield.Text.Replace(" ", "") == questions[currentquestionindex].Description)
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
                string[] ansarray = questions[currentquestionindex].Description.Split("・");
                string[] player_ans = textfield.Text.Split("・");
                Array.Sort(ansarray);
                Array.Sort(player_ans);
                foreach (var item in ansarray)
                {
                    if (Array.BinarySearch(player_ans, item) < 0)
                    {
                        Miss();
                        End();
                        questioncount++;
                        return;
                    }
                }
                foreach (var item in player_ans)
                {
                    if (Array.BinarySearch(ansarray, item) < 0)
                    {
                        Miss();
                        End();
                        questioncount++;
                        return;
                    }
                }
                Correct();
            }
            End();
            textfield.Text = string.Empty;

            questioncount++;
        }

        public void CheckAnswerLast(object sender, EventArgs e)
        {
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            button.Click -= CheckAnswerLast;
            //lastcorrect = CorrectCount;
            CheckAnswer(sender, e);
            button.Click -= Button_Click;
            button.Click += ShowResult;
            button.Text = "結果を見る";
        }

        private async void ShowResult(object sender, EventArgs e)
        {
            double correctpercent = Math.Round((double)CorrectCount / (double)questions.Length, 3) * 100;
            questiontext.SetTextColor(Android.Graphics.Color.Black);
            questiontext.Text = $"正解:{CorrectCount}"
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

        //public void NextQuestion(object sender, EventArgs e)
        //{
        //    questiontext.SetTextColor(Android.Graphics.Color.Black);
        //    GiveQuestion();
        //    button.Click += CheckAnswer;
        //    button.Click -= NextQuestion;
        //    textfield.Text = string.Empty;
        //}

        public virtual void GiveQuestionRandom()
        {
            if (randomquestion == null)
            {
                randomquestion = new Random();
            }
            currentquestionindex = randomquestion.Next(0, questions.Length);
            if (results[currentquestionindex] != QuestionResult.None)
            {
                GiveQuestionRandom();
            }
            questiontext.Text = questions[currentquestionindex].Title;
            button.Text = mainactivity.GetString(Resource.String.quiz_checkanswer);
        }

        public virtual void GiveQuestion(int questionindex)
        {
            currentquestionindex = questionindex;
            questiontext.Text = questions[currentquestionindex].Title;
            button.Text = mainactivity.GetString(Resource.String.quiz_checkanswer);
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

            button.Text = mainactivity.GetString(Resource.String.quiz_next);
            button.Click -= CheckAnswer;
            button.Click += Button_Click;
        }

        private void Miss()
        {
            questiontext.Text = questions[currentquestionindex].Description;
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
    [Serializable]
    public enum QuestionResult
    {
        None,
        Correct,
        Miss
    }
}