using UnityEngine;
using System.Collections.Generic;

public class RPN {
    public static int eval(string exp, Dictionary<string, int> vars){
        Stack<int> nums = new Stack<int>();
        string[] tokens = exp.Split(' ');
        foreach (var token in tokens) {
            bool success = int.TryParse(token, out int num);
            if (success){
                nums.Push(num);
            }
            else{
                if (vars.ContainsKey(token)) {
                    nums.Push(vars[token]);

                }
                else{
                    switch(token){
                        case "+": {
                            int a = nums.Pop();
                            int b = nums.Pop();
                            nums.Push(b + a);
                            break;
                        }
                        case "-": {
                            int a = nums.Pop();
                            int b = nums.Pop();
                            nums.Push(b - a);
                            break;
                        }
                        case "*": {
                            int a = nums.Pop();
                            int b = nums.Pop();
                            nums.Push(b * a);
                            break;
                        }
                        case "/": {
                            int a = nums.Pop();
                            int b = nums.Pop();
                            nums.Push(b / a);
                            break;
                        }
                        case "%": {
                            int a = nums.Pop();
                            int b = nums.Pop();
                            nums.Push(b % a);
                            break;
                        }
                        default:
                            Debug.Log("RPN Parse Error");
                            return 0;
                    }
                }
            }

        }

        return nums.Pop();
    }
}
