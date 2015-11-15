using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetrologyTwo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct InfoAboutVariables
        {
            public string TitleOfVariable;
            public int AmountVariableInCode;
            public InfoAboutVariables(string StringOfName, int AmountOfMeetVariable)
            {
                TitleOfVariable = StringOfName;
                AmountVariableInCode = AmountOfMeetVariable;
            }
        }

      

        void DeleteCommentsInCode(string NewDataOfMainFileWithoutComments)
        {
            bool FlagOfStringComment = false;

            for (int i = 0; i < TextBoxOfMainText.Text.Length; i++)
            {
                if ((TextBoxOfMainText.Text[i] == '/') && (TextBoxOfMainText.Text[i + 1] == '/'))
                    while (TextBoxOfMainText.Text[i] != '\r')
                    {
                        i++;
                    }

                if ((TextBoxOfMainText.Text[i] == '/') && (TextBoxOfMainText.Text[i + 1] == '*'))
                {
                    while (FlagOfStringComment == false)
                    {
                        if ((TextBoxOfMainText.Text[i] == '*') && (TextBoxOfMainText.Text[i + 1] == '/'))
                            FlagOfStringComment = true;
                        i++;
                    }
                    i += 2;
                    FlagOfStringComment = false;
                }
                if (i >= TextBoxOfMainText.Text.Length)
                    NewDataOfMainFileWithoutComments += "";
                else
                    NewDataOfMainFileWithoutComments += Convert.ToString(TextBoxOfMainText.Text[i]);
            }
            TextBoxOfMainText.Text = NewDataOfMainFileWithoutComments;
        }

        List<InfoAboutVariables> DoFindVariablesInLine(ref List<InfoAboutVariables> NamesOfVariablesOurCode, string[] ContainsOfLineWithCode)
        {
            for(int i = 0; i< ContainsOfLineWithCode.Length;i++)
            {
                if((ContainsOfLineWithCode[i].Contains("=") == true) || (ContainsOfLineWithCode[i].Contains(";") == true))
                {
                    if (ContainsOfLineWithCode[i].Contains(";") == true)
                    {

                        ContainsOfLineWithCode[i] = ContainsOfLineWithCode[i].Replace(";", "");
                        NamesOfVariablesOurCode.Add(new InfoAboutVariables(ContainsOfLineWithCode[i], 0));
                    }
                    else
                    {
                        NamesOfVariablesOurCode.Add(new InfoAboutVariables(ContainsOfLineWithCode[i - 1], 0));
                        break;
                    }
                }
            }
            return NamesOfVariablesOurCode;
        }
        List<InfoAboutVariables> DoFindAmountVariables(ref List<InfoAboutVariables> OurVariablesInCode, string OurAnalyzingCode)
        {
            string[] ContainsOfLineWithCode = {};
            string StringOfVAR = "var", StringOfCONST = "const", LineWithCode = "";
         
            for (int i = 0; i < TextBoxOfMainText.Lines.Length; i++)
            {
                LineWithCode = TextBoxOfMainText.Lines[i];
               
                if ((LineWithCode.Contains(StringOfVAR) == true)|| (LineWithCode.Contains(StringOfCONST) == true))
                {
                   do
                   {
                        LineWithCode = TextBoxOfMainText.Lines[i];
                        ContainsOfLineWithCode = LineWithCode.Split(' ');
                        OurVariablesInCode = DoFindVariablesInLine(ref OurVariablesInCode, ContainsOfLineWithCode);
                        i++;
                        
                   } while (LineWithCode.Contains(';') != true) ;

                   i--;   
                }
            }
            return OurVariablesInCode;
        }

        List<InfoAboutVariables> DoFindAmountOfMeetOfVariables(ref List<InfoAboutVariables> AmountMeetOfVariables, string OurAnalyzingCode)
        {
            string LineWithCode = "";
            InfoAboutVariables[] MyArrayOfCopyMyList = AmountMeetOfVariables.ToArray();

            for (int i = 0; i < TextBoxOfMainText.Lines.Length; i++)
            {
                LineWithCode = TextBoxOfMainText.Lines[i];

                for (int j = 0;j < AmountMeetOfVariables.Count; j++)
                {
                    if (LineWithCode.Contains(AmountMeetOfVariables[j].TitleOfVariable) == true)
                        MyArrayOfCopyMyList[j].AmountVariableInCode++;
                }
            }
      
            AmountMeetOfVariables = MyArrayOfCopyMyList.ToList<InfoAboutVariables>();

            return AmountMeetOfVariables.Distinct().ToList();
        }

        void DoOutputInfomationAboutResult(List<InfoAboutVariables>  ResultVariables)
        {
            for(int i = 0;i < ResultVariables.Count;i++)
            {
                TextBoxOfResultText.Text += ResultVariables[i].TitleOfVariable + "-" + Convert.ToString(ResultVariables[i].AmountVariableInCode - 1) + Environment.NewLine; 
            }
        }
        void DoMainTask()
        {
            string NewDataOfMainFileWithoutComments = "";
         
            List<InfoAboutVariables> ResultVariables = new List<InfoAboutVariables>();

            DeleteCommentsInCode(NewDataOfMainFileWithoutComments);

            ResultVariables = DoFindAmountVariables(ref ResultVariables, NewDataOfMainFileWithoutComments);

            ResultVariables = DoFindAmountOfMeetOfVariables(ref ResultVariables, NewDataOfMainFileWithoutComments);

            DoOutputInfomationAboutResult(ResultVariables);
        }
        private void ButtonOfProccedTask_Click(object sender, EventArgs e)
        {
            DoMainTask();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName, Encoding.Default);

                TextBoxOfMainText.Text = sr.ReadToEnd();
                sr.Close();

                if (TextBoxOfMainText.Text != "")
                    ButtonOfProccedTask.Enabled = true;
            }
        }
    }
}
