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

        //05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees.Where(x => x.Department.Name.Equals("Research and Development")).Select(x => new { x.FirstName, x.LastName, x.Salary, depName = x.Department.Name }).ToList().OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName);

            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.depName} - ${item.Salary:F2}");
            }
            return sb.ToString().TrimEnd();

        }

        //06
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var newAddress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAddress);
            context.SaveChanges();

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            nakov.AddressId = newAddress.AddressId;

            context.SaveChanges();

            var employeesAddresses = context.Employees.Select(x => new { x.Address.AddressId, x.Address.AddressText }).ToList().OrderByDescending(x => x.AddressId).Take(10);
            foreach (var item in employeesAddresses)
            {
                sb.AppendLine($"{item.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }
        //07

        //08

        //09

        //10

        //11

        //12

        //13

        //14

        //15

    }
}
