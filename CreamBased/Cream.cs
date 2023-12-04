using System;
using System.Collections.Generic;

namespace Cream
{
    [System.Serializable]
    public class CreamCompiler
    {
        public Dictionary<string, Action<CreamCompiler, String>> instructions;
        public Dictionary<string, string> variables;
        public Dictionary<string, int> shires;
        public Stack<int> returnStack;
        public string[] lines;
        public int pointer;
        public bool running = true;
        public string error = "";


        public CreamCompiler(
            string code,
            Dictionary<string, string> initialVariables = null,
            Dictionary<string, Action<CreamCompiler, String>> customInstructions = null
        )
        {
            CleanCode(code);
            Initialize();
            SetShires();
            SetVariables(initialVariables);
            SetInstructions(customInstructions);
        }

        private void CleanCode(string code)
        {
            while (code.Contains("  "))
            {
                code = code.Replace("  ", " ");
            }
            code = code.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            lines = code.Split(';');
        }

        private void SetShires()
        {
            shires = new Dictionary<string, int>();
            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = ParseLine(lines[i]);
                string instruction = line[0].ToLower().Trim();
                if (instruction.Equals("shire"))
                {
                    string shire = line[1].Trim();
                    shires.Add(shire, i);
                }
            }
        }

        private void Initialize()
        {
            returnStack = new Stack<int>();
            instructions = new Dictionary<string, Action<CreamCompiler, String>>();
            variables = new Dictionary<string, string>();
            pointer = 0;
        }

        private string[] ParseLine(string line)
        {
            return line.Split(' ');
        }

        private void SetVariables(Dictionary<string, string> initialVariables)
        {

            variables.Add("ward", "");
            if (initialVariables != null)
            {
                variables.AddRange<string, string>(initialVariables);
            }
        }

        private void SetInstructions(
            Dictionary<string, Action<CreamCompiler, String>> customInstructions
        )
        {
            instructions.Add("error", (m, s) => Console.WriteLine(s));
            
            instructions.Add("print", BuildInstructions.Print);
            instructions.Add("jump", BuildInstructions.Jump);
            instructions.Add("jumpto", BuildInstructions.JumpTo);
            instructions.Add("return", BuildInstructions.Return);

            instructions.Add("plant", BuildInstructions.Assign);
            instructions.Add("mint", BuildInstructions.AssignInt);
            instructions.Add("potate", BuildInstructions.AssignFloat);
            instructions.Add("chard", BuildInstructions.AssignChar);
            instructions.Add("ling", BuildInstructions.AssignString);
            
            instructions.Add("get", BuildInstructions.GetValue);

            instructions.Add("solve", BuildInstructions.Operation);

            instructions.Add("if", BuildInstructions.IfJumpTo);
            
            instructions.Add("end", BuildInstructions.End);

            if (customInstructions != null)
            {
                instructions.AddRange<string, Action<CreamCompiler, String>>(customInstructions);
            }
        }

        public void NextStep()
        {
            if (pointer >= lines.Length)
            {
                running = false;
                return;
            }

            string currentLine = lines[pointer];
            string[] line = ParseLine(currentLine);
            string instruction = line[0].ToLower().Trim();
            string parameters = currentLine.Substring(instruction.Length).Trim();

            if (instructions.ContainsKey(instruction))
            {
                try
                {
                    instructions[instruction].Invoke(this, parameters);
                }
                catch (System.Exception)
                {
                    error = $"System error at {pointer}";
                    return;
                }
            }
            else
            {
                pointer++;
            }
        }

        public string ApplyVariablesInString(string line)
        {
            foreach (var item in variables)
            {
                line = line.Replace("{" + item.Key + "}", item.Value);
            }

            return line;
        }

        public void Run()
        {
            while (running && error == "")
            {
                NextStep();
            }

            if (error != "")
            {
                instructions["error"].Invoke(this, "Error at: " + error);
            }
            else
            {
                instructions["print"].Invoke(this, "Successful runtime");
            }
        }

    }
}