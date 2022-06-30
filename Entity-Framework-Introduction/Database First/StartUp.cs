using System;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
               
        }

        //03
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.EmployeeId,
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.JobTitle,
                x.Salary
            }).ToList();
            var sb = new StringBuilder();

            foreach (var item in employees.OrderBy(x => x.EmployeeId))
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} {item.MiddleName} {item.JobTitle} {item.Salary:F2}");
            }

            return sb.ToString().TrimEnd();

        }

        //04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {

            var sb = new StringBuilder();
            var employees = context.Employees.Where(x => x.Salary > 50_000).Select(x => new { x.FirstName, x.Salary }).ToList();
            foreach (var item in employees.OrderBy(x => x.FirstName))
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

    }
}
