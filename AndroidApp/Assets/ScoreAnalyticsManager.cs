﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Microcharts;
using System.Linq;
using Microcharts.Droid;

namespace AndroidApp.Assets
{
    class ScoreAnalyticsManager
    {
        private ChartView chartview;
        private LineChart linechart;
        private ChartEntry[] entries;
        private TextView title;
        private Button next;
        private Button previous;
        private Activity a;
        private int id;
        private const int maxresults = 10;
        private int currentresultindex;
        private List<TestResultStruct[]> resultsarray;
        public ScoreAnalyticsManager(Activity activity, int recyclerviewid, TextView titletext, ChartView chart, Button nextbutton, Button previousbutton, TestResultStruct[] results)
        {
            linechart = new LineChart();
            chartview = chart;
            title = titletext;
            a = activity;
            id = recyclerviewid;
            next = nextbutton;
            previous = previousbutton;
            resultsarray = new List<TestResultStruct[]>();
            if (results == null)
            {
                title.Text = "仮";
            }
            else
            {
                previous.Visibility = Android.Views.ViewStates.Invisible;
                if (results.Length > maxresults)
                {
                    List<TestResultStruct> resultspermax = new List<TestResultStruct>();
                    int indexcount = 0;
                    for (int a = 0; a < results.Length / maxresults; a++)
                    {
                        for (int i = 0 + maxresults * a; i < maxresults * (a + 1); i++)
                        {
                            resultspermax.Add(results[i]);
                        }
                        resultsarray.Add(resultspermax.ToArray());
                        resultspermax.Clear();
                        indexcount++;
                    }
                    for (int i = indexcount * maxresults; i < results.Length; i++)
                    {
                        resultspermax.Add(results[i]);
                    }
                    resultsarray.Add(resultspermax.ToArray());
                    previous.Click += Previous_Click;
                    next.Click += Next_Click;
                }
                else
                {
                    resultsarray.Add(results);
                    next.Visibility = Android.Views.ViewStates.Gone;
                    previous.Visibility = Android.Views.ViewStates.Gone;
                }
                ShowResults(resultsarray[0], true);
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            currentresultindex++;
            ShowResults(resultsarray[currentresultindex], false);
            if (resultsarray.Count - 1 == currentresultindex)
            {
                next.Visibility = Android.Views.ViewStates.Invisible;
            }
            previous.Visibility = Android.Views.ViewStates.Visible;
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            currentresultindex--;
            ShowResults(resultsarray[currentresultindex], false);
            if (0 == currentresultindex)
            {
                previous.Visibility = Android.Views.ViewStates.Invisible;
            }
            next.Visibility = Android.Views.ViewStates.Visible;
        }

        private void ShowResults(TestResultStruct[] testresults, bool updateaccuratelylist)
        {
            linechart.MaxValue = 100;
            entries = Array.Empty<ChartEntry>();
            TestQuestionStruct[] questionStructs = new TestQuestionStruct[0];
            foreach (var item in testresults)
            {
                Array.Resize(ref entries, entries.Length + 1);
                entries[entries.Length - 1] = new ChartEntry((float)item.CorrectPercent)
                {
                    Label = $"{item.TestTime.Year}/{item.TestTime.Month}/{item.TestTime.Day} {item.TestTime.Hour}:{item.TestTime.Minute}",
                    ValueLabel = item.CorrectPercent.ToString(),
                    Color = SkiaSharp.SKColor.FromHsv(260 + (float)item.CorrectPercent, 100, 100)
                };
                for (int i = 0; i < item.Questions.Length; i++)
                {
                    Array.Resize(ref questionStructs, questionStructs.Length + 1);
                    questionStructs[questionStructs.Length - 1] = item.Questions[i];
                }
            }
            DoublelineListStruct[] Questionsvarious = new DoublelineListStruct[0];
            TestQuestionStruct[][] Questionsforaccurately = new TestQuestionStruct[0][];
            for (int i = 0; i < questionStructs.Length; i++)
            {
                if (Array.IndexOf(Questionsvarious, questionStructs[i].Question) == -1)
                {
                    Array.Resize(ref Questionsvarious, Questionsvarious.Length + 1);
                    Questionsvarious[Questionsvarious.Length - 1] = questionStructs[i].Question;
                }
            }
            if (Questionsvarious.Length > 1)
            {
                for (int i = 0; i < Questionsvarious.Length; i++)
                {
                    Array.Resize(ref Questionsforaccurately, Questionsforaccurately.Length + 1);
                    Questionsforaccurately[Questionsforaccurately.Length - 1] = Array.FindAll(questionStructs, x => x.Question.Title == Questionsvarious[i].Title && x.Question.Description == Questionsvarious[i].Description);
                }
            }
            else
            {
                Array.Resize(ref Questionsforaccurately, 1);
                Questionsforaccurately[0] = questionStructs;
            }
            Accurately[] recycler = new Accurately[0];
            double[] ak = new double[0];
            for (int i = 0; i < Questionsforaccurately.Length; i++)
            {
                Array.Resize(ref recycler, recycler.Length + 1);
                double accurate_double = GetAccurate(Questionsforaccurately[i]);
                recycler[recycler.Length - 1].Content.Title = $"{Questionsforaccurately[i][0].Question.Title}(解答:{Questionsforaccurately[i][0].Question.Description})";
                recycler[recycler.Length - 1].Content.Description = $"正解率:{accurate_double}%";
                recycler[recycler.Length - 1].Accurate = accurate_double;
            }
            Array.Sort(recycler);
            if (updateaccuratelylist)
            {
                RecyclerViewComponents.CreateDoublelineList(GetDoublelineListStruct(recycler), a, id);
            }
            linechart.Entries = entries;
            chartview.Chart = linechart;  
            title.Text = testresults[0].Genrename;
        }
        private static double GetAccurate(TestQuestionStruct[] questions)
        {
            int correctcount = 0;
            correctcount = Array.FindAll(questions, x => x.Result == QuestionResult.Correct).Length;
            return Math.Round((double)correctcount / (double)questions.Length, 3) * 100;
        }
        private static DoublelineListStruct[] GetDoublelineListStruct(Accurately[] a)
        {
            DoublelineListStruct[] returnvalue = new DoublelineListStruct[0];
            for (int i = 0; i < a.Length; i++)
            {
                Array.Resize(ref returnvalue, returnvalue.Length + 1);
                returnvalue[i] = a[i].Content;
            }
            return returnvalue;
        }
        private struct Accurately : IComparable
        {
            public DoublelineListStruct Content;
            public double Accurate;

            public int CompareTo(object obj)
            {
                if (obj != null)
                {
                    Accurately comp = (Accurately)obj;
                    return comp.Accurate.CompareTo(Accurate);
                }
                return 1;
            }
        }
    }
}