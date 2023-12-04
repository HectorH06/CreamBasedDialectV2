using System;

namespace Cream
{
    public static class BuildInstructions
    {

        public static void IfJumpTo(CreamCompiler machine, string line)
        {
            var operators = line.Split(new[] { "jumpto" }, StringSplitOptions.None);
            var conditional = operators[0].Trim();
            conditional = machine.ApplyVariablesInString(conditional);

            bool result = false;
            try
            {
                conditional = Comparators.Evaluator(conditional);
                conditional = Logic.Evaluator(conditional);
                result = Logic.EvaluatorFinal(conditional);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
                machine.error = $"Conditional error at line {machine.pointer}";
                return;
            }

            if (result)
            {
                machine.returnStack.Push(machine.pointer);
                machine.pointer += int.Parse(operators[1], Utils.usNumberFormat) + 1;
                return;
            }
            machine.pointer++;
        }

        public static void Return(CreamCompiler machine, string line)
        {
            if (machine.returnStack.Count == 0)
            {
                machine.pointer++;
                return;
            }

            machine.pointer = machine.returnStack.Pop() + 1;
        }

        public static void End(CreamCompiler machine, string line)
        {
            machine.running = false;
            return;
        }

        public static void Jump(CreamCompiler machine, string line)
        {
            machine.returnStack.Push(machine.pointer);

            var sign = "";
            if (line[0] == '+' || line[0] == '-')
            {
                sign = line[0].ToString();
                line = line.Substring(1).Trim();
            }

            int i = 0;
            if (!Int32.TryParse(line, out i))
            {
                machine.error = $"Jump parameter must to be integer number at {machine.pointer}";
                return;
            }
            if (i > machine.lines.Length || i <= 0)
            {
                machine.error = $"index is outside of the range at {machine.pointer}";
                return;
            }

            if (sign == "")
            {
                machine.pointer = i;
            }
            else if (sign == "+")
            {
                machine.pointer = i + machine.pointer + 1;
            }
            else if (sign == "-")
            {
                machine.pointer = machine.pointer - i;
            }
        }

        #region Assign
        public static void Assign(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim().Replace(" ", "");

            var data = line.Replace($"{name} ", "");

            data = machine.ApplyVariablesInString(data);

            machine.pointer++;

            if (machine.variables.ContainsKey(name))
            {
                machine.variables[name] = data;
                return;
            }

            machine.variables.Add(name, data);
        }

        public static void AssignInt(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim().Replace(" ", "");

            if (!int.TryParse(v[1], out int data))
            {
                throw new ArgumentException("El valor proporcionado no es un número entero válido.");
            }

            machine.pointer++;

            if (machine.variables.ContainsKey(name))
            {
                machine.variables[name] = data.ToString();
                return;
            }

            machine.variables.Add(name, data.ToString());
        }

        public static void AssignFloat(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim().Replace(" ", "");

            if (!float.TryParse(v[1], out float data))
            {
                throw new ArgumentException("El valor proporcionado no es un número flotante válido.");
            }

            machine.pointer++;

            if (machine.variables.ContainsKey(name))
            {
                machine.variables[name] = data.ToString();
                return;
            }

            machine.variables.Add(name, data.ToString());
        }

        public static void AssignChar(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim().Replace(" ", "");

            if (!char.TryParse(v[1], out char data))
            {
                throw new ArgumentException("El valor proporcionado no es un carácter válido.");
            }

            machine.pointer++;

            if (machine.variables.ContainsKey(name))
            {
                machine.variables[name] = data.ToString();
                return;
            }

            machine.variables.Add(name, data.ToString());
        }

        public static void AssignString(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim().Replace(" ", "");

            var data = line.Replace($"{name} ", "");

            machine.pointer++;

            if (machine.variables.ContainsKey(name))
            {
                machine.variables[name] = data;
                return;
            }

            machine.variables.Add(name, data);
        }

        #endregion
        public static void GetValue(CreamCompiler machine, string line)
        {
            var v = line.Split(' ');
            var name = v[0].Trim();
            var id1 = int.Parse(v[1].Trim(), Utils.usNumberFormat);
            var id2 = v.Length == 3 ? int.Parse(v[2].Trim(), Utils.usNumberFormat) : -1;

            machine.pointer++;

            if (id2 == -1)
            {
                machine.variables["ward"] = machine.variables[name].Substring(id1);
                return;
            }
            machine.variables["ward"] = machine.variables[name].Substring(id1, id2);
        }

        public static void Print(CreamCompiler machine, string line)
        {
            line = machine.ApplyVariablesInString(line);
            Console.WriteLine(line);
            machine.pointer++;
        }

        public static void JumpTo(CreamCompiler machine, string shireToJump)
        {
            machine.returnStack.Push(machine.pointer);
            if (machine.shires.ContainsKey(shireToJump))
            {
                machine.pointer = machine.shires[shireToJump];
            }
            else
            {
                machine.error = $"Jump shire {shireToJump} in line: {machine.pointer} does not exist";
            }
            machine.pointer++;
        }

        public static void Operation(CreamCompiler machine, string line)
        {
            line = machine.ApplyVariablesInString(line);
            try
            {
                line = Aritmetics.Evaluator(line).ToString();
            }
            catch
            {
                machine.error = $"Syntax in line {machine.pointer}";
                return;
            }
            if (float.TryParse(line, out float number) && number > 999999999999999)
            {
                line = "." + line;
            }
            machine.variables["ward"] = line;
            machine.pointer++;
        }
    }
}