using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    public enum SalaryType
    {
        Hourly = 1,
        Daily = 2,
    };

    class Employee
    {
        public int Id { get; set; }
        public SalaryType SalaryType { get; set; }
        public int SalaryRate { get; set; }
        public int WorkingHours { get; set; }
        public bool IsFired { get; set; }

        public Employee(int Id, SalaryType SalaryType, int SalaryRate, int WorkingHours, bool IsFired)
        {
            this.Id = Id;
            this.SalaryType = SalaryType;
            this.SalaryRate = SalaryRate;
            this.WorkingHours = WorkingHours;
            this.IsFired = IsFired;
        }

        public bool IsSalaryTypeHourly()
        {
            return this.SalaryType == SalaryType.Hourly;
        }

        public bool IsSalaryTypeDaily()
        {
            return this.SalaryType == SalaryType.Daily;
        }
    }

    class EmployManager
    {
        public const int HoursInWorkingDay = 8;

        private int workingDays;
        private List<Employee> employees;

        public EmployManager(int workingDays, List<Employee> employees)
        {
            this.workingDays = workingDays;
            this.employees = employees;
        }

        public float CalculateAllEmployeesSallary()
        {
            float result = 0;
            foreach (Employee employee in this.employees)
            {
                result += CalculateEmployeeSallary(employee);
            }

            return result;
        }

        public float CalculateEmployeeSallary(Employee empoyee)
        {
            float salary;
            if (empoyee.IsSalaryTypeHourly())
            {
                salary = CalculateHourlySallary(empoyee);
            }
            else
            {
                salary = CalculateDailySallary(empoyee);
            }

            if (empoyee.IsFired)
            {
                salary += salary / 2;
            }

            return salary;
        }

        private float CalculateHourlySallary(Employee empoyee)
        {
            float salary;

            var workingHoursInMonth = HoursInWorkingDay * this.workingDays;
            var hasOvertime = empoyee.WorkingHours > workingHoursInMonth;
            if (hasOvertime)
            {
                salary = (empoyee.SalaryRate * workingHoursInMonth) +
                    ((empoyee.WorkingHours - workingHoursInMonth) * 2 * empoyee.SalaryRate);
            }
            else
            {
                salary = empoyee.SalaryRate * empoyee.WorkingHours;
            }

            return salary;
        }

        private float CalculateDailySallary(Employee empoyee)
        {
            var days = (float)Math.Floor((decimal)(empoyee.WorkingHours / HoursInWorkingDay));

            return days * empoyee.SalaryRate;
        }
    }

    class Parser
    {
        private String[] Separator = new String[] { "-1" };

        public List<Employee> ParseEmployees(string input)
        {
            var employes = new List<Employee>();

            var a = input.Split(Separator, StringSplitOptions.None)[0];
            var employeesInput = input.Split(Separator, StringSplitOptions.None)[0].Trim();
            if (employeesInput == "")
            {
                return employes;
            }

            var numbers = employeesInput.Split(' ').Select(x => int.Parse(x)).ToArray();

            for (int i = 0; i < numbers.Length; i += 3)
            {
                var employId = numbers[i];
                var salaryType = numbers[i + 1] == 1 ? SalaryType.Hourly : SalaryType.Daily;
                var salaryRate = numbers[i + 2];

                var employee = new Employee(
                    employId,
                    salaryType,
                    salaryRate,
                    this.ParseEmployWorkingHours(employId, input),
                    this.ParseIsEmployFired(employId, input)
                );

                employes.Add(employee);
            }

            return employes;
        }

        public int ParseWorkingDays(string input)
        {
            return Int32.Parse(input.Split(Separator, StringSplitOptions.None)[1]);
        }

        private int ParseEmployWorkingHours(int employId, string input)
        {
            int result = 0;

            input = input.Split(Separator, StringSplitOptions.None)[2].Trim();
            if (input == "")
            {
                return 0;
            }

            var numbers = input.Split(' ').Select(x => int.Parse(x)).ToArray();
            for (int i = 0; i < numbers.Length; i += 2)
            {
                if (numbers[i] == employId)
                {
                    result += numbers[i + 1];
                }
            }

            return result;
        }

        private bool ParseIsEmployFired(int employId, string input)
        {
            input = input.Split(Separator, StringSplitOptions.None)[3].Trim();
            if (input == "")
            {
                return false;
            }

            var numbers = input.Split(' ').Select(x => int.Parse(x)).ToArray();

            foreach (var firedEmployId in numbers)
            {
                if (firedEmployId == employId)
                {
                    return true;
                }
            }

            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var input = Console.ReadLine();
            var input = "1 1 15 2 1 17 3 2 150 -1 21 -1 1 144 2 184 3 163 -1 2 -1";
            var parser = new Parser();

            var workingDays = parser.ParseWorkingDays(input);
            var empoyees = parser.ParseEmployees(input);

            var employManager = new EmployManager(workingDays, empoyees);

            float result = employManager.CalculateAllEmployeesSallary();

            Console.Write(result.ToString("#.00"));
            Console.ReadLine();
        }
    }
}
